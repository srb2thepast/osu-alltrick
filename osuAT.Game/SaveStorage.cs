using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using osu.Framework.Platform; // Reminder: consider using osu.Framework.Platform.Storage for safe file writing.
using OsuApiHelper;
using osuAT.Game.Types;
using osuAT.Game.Skills;

namespace osuAT.Game
{
    public class CSaveData
    {
        /// <summary>
        /// The path of the player's osu! folder.
        /// </summary>
        [JsonProperty("osupath")]
        public string OsuPath { get; set; } = @"C:\Users\alexh\AppData\Local\osu!";

        /// <summary>
        /// The save data version.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; } = "osu!AT save data v1";

        /// <summary>
        /// The versions of each skill the last time this savedata was saved.
        /// </summary>
        /// <remarks> The dictonary follows a format of (<see cref="ISkill.Identifier"/> : <see cref="ISkill.Version"/>).</remarks>
        [JsonProperty("skillversion")]
        public Dictionary<string, string> SkillVersions { get; set; }

        /// <summary>
        /// The username of player the save data is attached to.
        /// </summary>
        [JsonProperty("username")]
        public string PlayerUsername { get; set; }

        /// <summary>
        /// The player the save data is attached to.
        /// </summary>
        [JsonProperty("playerid")]
        public int PlayerID { get; set; }


        /// <summary>
        /// The player's APIKey.
        /// </summary>
        [JsonProperty("APIKey")]
        public string APIKey { get; set; }

        /// <summary>
        /// The overall cached PP for each skill.
        /// </summary>
        /// <remarks> The dictonary follows a format of (<see cref="ISkill.Identifier"/> : SkillPP).</remarks>
        [JsonProperty("alltrickPP")]
        public Dictionary<string, double> TotalSkillPP { get; set; }

        /// <summary>
        /// The cached top-scores for each skill.
        /// </summary>
        /// <remarks> The dictonary follows a format of
        /// (<see cref="ISkill.Identifier"/> : Dict(Ruleset.Name (or "overall") : List( Tuple(<see cref="Score.ID"/>, ScorePP))) ).
        /// </remarks>
        // what if it was seperated this by ruleset so ruleset based leaderboards are possible?
        // then you could become an all-trick in every ruleset and it could glow on the SkillInfo
        // board. (update: i just did that.)
        [JsonProperty("alltrickTop")]
        public Dictionary<string, Dictionary<string,List<Tuple<Guid, double>>>> AlltrickTop { get; set; }

        /// <summary>
        /// A list of every score that osu!alltrick was able to process.
        /// </summary>
        /// <remarks> The dictonary follows a format of (<see cref="Score.ID"/> : <see cref="Score"/>).</remarks>
        [JsonProperty("scores")]
        public Dictionary<Guid, Score> Scores { get; set; }
    }

    public static class SaveStorage
    {
        public static NativeStorage GameStorage = new NativeStorage("savedata");
        public static CSaveData SaveData;
        public static string SaveFile = "savedata\\data.json";
        public static bool IsSaving = false;

        private static Dictionary<string, double> getDefaultTotal()
        {
            Dictionary<string, double>  dict = new Dictionary<string, double>();
            foreach (ISkill skill in Skill.SkillList) {
                dict.Add(skill.Identifier,0);
            }
            return dict;
        }

        private static Dictionary<string, Dictionary<string, List<Tuple<Guid, double>>>> getDefaultTop()
        {
            Dictionary<string, Dictionary<string, List<Tuple<Guid, double>>>> dict = new Dictionary<string, Dictionary<string, List<Tuple<Guid, double>>>>();
            foreach (ISkill skill in Skill.SkillList)
            {
                // add skils
                dict.Add(skill.Identifier, new Dictionary<string, List<Tuple<Guid, double>>>());

                // add all modes to each skill, regardless of whether it can support it.
                dict[skill.Identifier].Add("overall", new List<Tuple<Guid, double>>());
                dict[skill.Identifier].Add("osu", new List<Tuple<Guid, double>>()); 
                dict[skill.Identifier].Add("mania", new List<Tuple<Guid, double>>());
                dict[skill.Identifier].Add("catch", new List<Tuple<Guid, double>>());
                dict[skill.Identifier].Add("taiko", new List<Tuple<Guid, double>>());
            }
            return dict;
        }

        private static Dictionary<string, string> getDefaultSkillVer() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (ISkill skill in Skill.SkillList) {
                dict.Add(skill.Identifier, skill.Version);
            }
            return dict;
        }

        static SaveStorage() {

            if (!(CheckSaveExists())) {
                SaveData = new CSaveData {
                    PlayerID = -1,
                    APIKey = "null",
                    TotalSkillPP = getDefaultTotal(),
                    AlltrickTop = getDefaultTop(),
                    SkillVersions = getDefaultSkillVer(),
                    Scores = new Dictionary<Guid, Score>()
                };
                Save();
                return;
            }


            SaveData = JsonConvert.DeserializeObject<CSaveData>(Read());
            OsuApiKey.Key = SaveData.APIKey;
            int i = 0;
            foreach (Score score in SaveData.Scores.Values) {
                score.Register(index: i,setGUID: false,calcPP: false);
                i += 1;
            }

            // these two loops can be easily turned into one but im too tired to fix them rn

            /// Check the verions in the SaveFile for:
            ///     1. If it doesnt exist in the savefile
            foreach (ISkill skill in Skill.SkillList) {

                /// if the skill from SkillList is not found in SaveData.SkillVersions, the
                /// skill was added as of the newest update.

                if (!(SaveData.SkillVersions.ContainsKey(skill.Identifier)))
                {
                    IsSaving = true;

                    Console.WriteLine("---------------New Skill Addition Begun----------------");
                    Console.WriteLine("The skill " + skill.Name + " has been added to the game.");

                    SaveData.TotalSkillPP.Add(skill.Identifier, 0);
                    Console.WriteLine("added to total");

                    /// Add new dictonary entry "skill.Identifier" to:

                    /// a. SaveData.SkillVersions
                    Console.WriteLine("added to versions");

                    SaveData.SkillVersions.Add(skill.Identifier, skill.Version);

                    /// b. SaveData.AlltrickTop
                    Dictionary<string,List<Tuple<Guid, double>>> tuplList = new Dictionary<string, List<Tuple<Guid, double>>>
                    {
                        { "overall",new List<Tuple<Guid, double>>()},
                        { "osu",new List<Tuple<Guid, double>>()},
                        { "mania",new List<Tuple<Guid, double>>()},
                        { "catch",new List<Tuple<Guid, double>>()},
                        { "taiko",new List<Tuple<Guid, double>>()}
                    };
                    Console.WriteLine("adding to top (0/2)");
                    // Loop through every score in SaveData.Scores
                    foreach (Score score in SaveData.Scores.Values)
                    {
                        // Calc the SkillPP of each score for the new skill if the skill supports the score's ruleset
                        if (skill.SupportedRulesets.Contains(score.ScoreRuleset)) {
                            score.AlltrickPP.Add(skill.Identifier, skill.SkillCalc(score));
                            Tuple<Guid, double> newtupl = new Tuple<Guid, double>(score.ID, score.AlltrickPP[skill.Identifier]);
                            tuplList["overall"].Add(newtupl);
                            tuplList[score.ScoreRuleset.Name].Add(newtupl);
                        }
                    }
                    Console.WriteLine("adding to top (1/2)");
                    SaveData.AlltrickTop.Add(skill.Identifier, new Dictionary<string, List<Tuple<Guid, double>>>());

                    // sort and set each ruleset tuple
                    foreach (KeyValuePair<string, List<Tuple<Guid, double>>> tupl in tuplList) {
                        tupl.Value.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                        SaveData.AlltrickTop[skill.Identifier].Add(tupl.Key, tupl.Value);
                    }
                    Console.WriteLine("added to top (2/2)");

                    /// c. SaveData.TotalSkillPP
                    SaveData.TotalSkillPP[skill.Identifier] = Skill.CalcWeighted(tuplList["overall"]);
                    Console.WriteLine("added to total");

                    Console.WriteLine("---------------New Skill Addition Complete----------------");
                }
            }

            /// Check the verions in the SaveFile for:
            ///     1. If it's different than the code (recalcs scores)
            ///     2. If it doesnt exist in the code (does nothing)
            foreach (KeyValuePair<string, string> skillVer in SaveData.SkillVersions) {

                /// check if the skill exists in <see cref="Skill.SkillList"/>, if it doesn't
                /// then that means it was a deleted/removed skill.
                ISkill skillInstance = Skill.GetSkillByID(skillVer.Key);
                if (skillInstance == null) {return; }

                string skillID = skillInstance.Identifier;
                /// check if the skill's version is different from the one in
                /// <see cref="Skill.SkillList"/>.
                if (!(skillVer.Value == skillInstance.Version))
                {
                    IsSaving = true;

                    Console.WriteLine("---------------Score Recalc Begun----------------");
                    Console.WriteLine("The skill " + skillInstance.Name + " has been updated. Recalculating scores..");

                    List<Score> scorelist = GetTrickTopScoreList(skillInstance);
                    Dictionary<string, List<Tuple<Guid, double>>> tuplList = new Dictionary<string, List<Tuple<Guid, double>>>
                    {
                        { "overall",new List<Tuple<Guid, double>>()},
                        { "mania",new List<Tuple<Guid, double>>()},
                        { "mania",new List<Tuple<Guid, double>>()},
                        { "catch",new List<Tuple<Guid, double>>()},
                        { "taiko",new List<Tuple<Guid, double>>()}
                    };
                    foreach (Score score in scorelist)
                    {
                        // check if the skill supports the score's ruleset
                        if (skillInstance.SupportedRulesets.Contains(score.ScoreRuleset))
                        {
                            score.AlltrickPP[skillID] = skillInstance.SkillCalc(score);
                            Tuple<Guid, double> newTuple = new Tuple<Guid, double>(score.ID, score.AlltrickPP[skillID]);

                            tuplList["overall"].Add(newTuple);
                            tuplList[score.ScoreRuleset.Name].Add(newTuple);
                            Console.WriteLine(score.ID.ToString()+ " | NewPP: " + score.AlltrickPP[skillID].ToString());
                        }
                    }
                    // sort and set each ruleset tuple
                    foreach (KeyValuePair<string, List<Tuple<Guid, double>>> tupl in tuplList)
                    {
                        tupl.Value.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                        SaveData.AlltrickTop[skillID][tupl.Key] = tupl.Value;
                    }

                    SaveData.SkillVersions[skillID] = skillInstance.Version;
                    Console.WriteLine("---------------Score Recalc Complete----------------");

                }

            }
            Save();
        }

        


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
            
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Read()
        {
            if (!CheckSaveExists()) return null; 

            string decodedtext = "";
            foreach (char chara in File.ReadAllText(SaveFile))
            {
                decodedtext += (char)((chara) + 20);
            }
            return decodedtext;
        }

        public static FileInfo Save()
        {
            IsSaving = true;
            CheckSaveExists();
            string encodedtext = "";
            foreach (char chara in JsonConvert.SerializeObject(SaveData))
            {
                var newchar = (char)(chara - 20);
                encodedtext += newchar;
            }
            File.WriteAllText(SaveFile, encodedtext);
            IsSaving = false;
            return new FileInfo(SaveFile);
        } 

        public static void DeleteScore(Guid scoreID) {
            if (!(SaveData.Scores[scoreID] == null)) {
                Console.WriteLine(
                    "The score with the Guid " + scoreID.ToString() +
                    " could not be found in the SaveData. Maybe you already deleted it?");
                return;
            }
            SaveData.Scores[scoreID] = new Score()
            {
                ScoreRuleset = RulesetStore.Osu,
                IsLazer = false,
                OsuID = -1,
                BeatmapInfo = new Beatmap
                {
                    MapID = -1,
                    MapsetID = -1,
                    SongArtist = "deleted",
                    SongName = "deleted",
                    DifficultyName = "deleted",
                    MapsetCreator = "deleted",
                    StarRating = 0,
                    MaxCombo = 0
                },
                Grade = "deleted",
                Accuracy = 0,
                AccuracyStats = new AccStat(0, 0, 0, 0),
                Combo = 0,
                TotalScore = 0,
                Mods = new List<ModInfo>(),
                IndexPosition = SaveData.Scores[scoreID].IndexPosition,
                DateCreated = SaveData.Scores[scoreID].DateCreated
            };
            SaveData.Scores[scoreID].Register(setGUID: false);
            Save(); 
        }

        public static void AddScore(Score score) {
            CheckSaveExists();

            // So that there arent duplicate references of the same score in the savedata.
            Score scoreclone = score.Clone();
            Console.WriteLine(scoreclone.ID);

            Dictionary<Guid, Score> tempdict = new Dictionary<Guid, Score>(SaveData.Scores)
            {
                { scoreclone.ID, scoreclone }
            };
            SaveData.Scores = tempdict;
            addToSkillTops(score);
            OnScoreAdded?.Invoke(score);

        }

        public delegate void ScoreAddedHandler(Score score);
        public static event ScoreAddedHandler OnScoreAdded;

        /// <summary>
        /// Adds the score to every Skill's AlltrickTop([score.ScoreRuleset] and "overall"]) if it's high enough
        /// and resorts those AlltrickTop sections from greatest to least.
        /// </summary>

        private static void addToSkillTops(Score score)
        {
            var modeList = new List<string> { "overall", score.ScoreRuleset.Name };
            foreach (KeyValuePair<string, double> scoreSkillPP in score.AlltrickPP)
            {
                foreach (var mode in modeList)
                {
                    var SkillList = SaveData.AlltrickTop[scoreSkillPP.Key][mode];
                    if (SkillList.Count < 100)
                    {
                        SkillList.Add(new Tuple<Guid, double>(score.ID, scoreSkillPP.Value));
                        SaveData.TotalSkillPP[scoreSkillPP.Key] = (Skill.CalcWeighted(SkillList));
                    }
                    else
                    {
                        if (scoreSkillPP.Value > SkillList[99].Item2)
                        {
                            SkillList.Add(new Tuple<Guid, double>(score.ID, scoreSkillPP.Value));
                            SaveData.TotalSkillPP[scoreSkillPP.Key] = (Skill.CalcWeighted(SkillList));
                        }
                    }
                    SkillList.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                }
            }
            
        }

        /// <summary>
        /// Returns a list of scores based on a SkillTop list contained in a <see cref="CSaveData.AlltrickTop"/>.
        /// </summary>
        /// <returns>A list of each <see cref="Score"/> in order.</returns>
        public static List<Score> GetTrickTopScoreList(ISkill skill, string ruleset = "overall") {
            List<Tuple<Guid, double>> list = SaveData.AlltrickTop[skill.Identifier][ruleset];
            List<Score> scorelist = new List<Score>();  
            foreach (Tuple<Guid, double> pair in list)
            {

                Score score = SaveData.Scores[pair.Item1];
                scorelist.Add(score);

                /* debug
                Console.Write(pair.Item1);
                Console.Write(score.AlltrickPP[skill.Identifier]);
                Console.Write(" ");
                Console.Write(pair.Item2);
                Console.Write(" ");
                Console.WriteLine(scorelist[0].AlltrickPP[skill.Identifier]);
                if (score == null) { for (int i = 1; i < 3; i++) { Console.Write(" [NOT FOUND]"); } }
                */





            }
            return scorelist;
        }
            
        public static bool CheckSaveExists() {
            if (!Directory.Exists("savedata")) {
                Directory.CreateDirectory("savedata");
                return false;
            }
            if (!File.Exists("savedata\\data.json")) {
                return false;
            }

            return true;
        } 
        
    }
}
