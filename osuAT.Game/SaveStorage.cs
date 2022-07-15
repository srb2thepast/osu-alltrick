﻿using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using osu.Framework.Platform; // Reminder: consider using osu.Framework.Platform.Storage for safe file writing.
using osuAT.Game.Types;
using osuAT.Game.Skills;

namespace osuAT.Game
{
    public class CSaveData {
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
        /// The player the save data is attached to.
        /// </summary>
        [JsonProperty("playerid")]
        public int PlayerID { get; set; }

        /// <summary>
        /// The player's APIKey.
        /// </summary>
        [JsonProperty("APIKey")]
        public string APIKey { get; set; } // reminder to make a seperate class named APIManager that will manage all API requests with private variables.

        /// <summary>
        /// The overall cached PP for each skill.
        /// </summary>
        /// <remarks> The dictonary follows a format of (<see cref="ISkill.Identifier"/> : SkillPP).</remarks>
        [JsonProperty("alltrickPP")]
        public Dictionary<string, double> TotalSkillPP { get; set;}

        /// <summary>
        /// The cached top-scores for each skill.
        /// </summary>
        /// <remarks> The dictonary follows a format of
        /// (<see cref="ISkill.Identifier"/> : List( Tuple(<see cref="Score.ID"/>, ScorePP)) ).
        /// </remarks>
        [JsonProperty("alltrickTop")]
        public Dictionary<string, List<Tuple<Guid, double>>> AlltrickTop { get; set; }

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

        private static Dictionary<string, List<Tuple<Guid, double>>> getDefaultTop()
        {
            Dictionary<string, List<Tuple<Guid, double>>> dict = new Dictionary<string, List<Tuple<Guid, double>>>();
            foreach (ISkill skill in Skill.SkillList)
            {
                dict.Add(skill.Identifier, new List<Tuple<Guid, double>>((150)));
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

            int i = 0;
            foreach (Score score in SaveData.Scores.Values) {
                score.Register(index: i,setGUID: false,calcPP: false);
                i += 1;
            }

            /// For every skill in <see cref="Skill.SkillList"/>,
            /// check if it exists in SaveData.SkillVersions.
            /// if the skill is not found in SaveData.SkillVersions, yet it exists
            /// in the Skill.SkillList, that means it was a
            /// skill added as of the newest update, so:
            ///     Add new dictonary entry "skill.Identifier" to:
            ///         a. SaveData.SkillVersions
            ///         b. SaveData.AlltrickTop
            ///         c. Every score in SaveData.Scores
            ///         d. SaveData.TotalSkillPP
            ///         
            ///     1a. Set SaveData.SkillVersion[skill.Identifier] to skill.Version
            ///     
            ///     1b. Create a tempDict of type Dictionary<string, double>
            ///     2b. Loop through every score in SaveData.Scores
            ///     3bc. Calc the SkillPP of each score for the skill
            ///     4bc. Set score.alltrickpp[skill.Identifier] to the skillPP 
            ///     5b. Add (score.ID,score.alltrickPP[skill.Identifier]) to the tempDict
            ///     6b. Set SaveData.AlltrickTop[skill.Identifier] to tempDict
            ///
            ///     1d. Set SaveData.TotalSkillPP to
            ///     <see cref="Skill.CalcWeighted">(SaveData.AlltrickTop[skill.Identifier]

            foreach (KeyValuePair<string, string> skillVer in SaveData.SkillVersions) {

                /// check if the skill exists in <see cref="Skill.SkillList"/>, if it doesn't
                /// then that means it was a deleted/removed skill, so:
                ///     1. Dont do any of the stuff below, just "return;".
                ISkill skillInstance = Skill.GetSkillByID(skillVer.Key);
                if (skillInstance == null) {return; }

                string skillID = skillInstance.Identifier;
                /// check if the skill's version is different from the one in
                /// <see cref="Skill.SkillList"/>.
                if (!(skillVer.Value == skillInstance.Version))
                {
                    Console.WriteLine("---------------Score Recalc Updated----------------");
                    Console.WriteLine("The skill " + skillInstance.Name + "has been updated. Recalculating scores..");
                    List<Score> scorelist = GetTrickTopScoreList(skillInstance);
                    ///     2. Get the IDs of each score in that list
                    ///     3. Use those IDs to get each score from SaveData.Scores
                    ///     4. Create a tempList of type List<Score>
                    ///     5. Set each score[skill.Identifer] = skill.Calc(score)
                    foreach (Score score in scorelist) {
                        score.AlltrickPP[skillID] = skillInstance.SkillCalc(score);
                    }
                    //SaveData.AlltrickTop[skillID] = scorelist.val ;
                    ///     6. Save each score to tempList
                    ///     7. Wipe SaveData.AlltrickTop[skill.Identifer].Value
                    ///     8. Set SaveData.AlltrickTop[Skill.Identifer].Value to tempList
                    Console.WriteLine("---------------Score Recalc Update Complete----------------");

                }

            }
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
            foreach (Score score in SaveData.Scores.Values) {
                Console.WriteLine(score.ID);
                Console.WriteLine(score.AlltrickPP["flowaim"]);
            } 

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
                    "could not be found in the SaveData. Maybe you already deleted it?");
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

            Console.WriteLine("------------------------------new------------------------------------");

            foreach (var sitem in SaveData.Scores.Values)
            {
                Console.WriteLine(sitem.ID);
            }
            Console.WriteLine(score.ID.ToString());
            Dictionary<Guid, Score> tempdict = new Dictionary<Guid, Score>(SaveData.Scores);
            tempdict.Add(scoreclone.ID, scoreclone);
            SaveData.Scores = tempdict;



            Console.WriteLine(SaveData.Scores[score.ID].ToString());
            Console.WriteLine("------------------------------------------------------------------");


            foreach (var sitem in SaveData.Scores.Values) {
                Console.WriteLine(sitem.ID);
            }

            // add the score to each skill's alltricktop that it's high enough for.
            foreach (KeyValuePair<string, double> scoreSkillPP in score.AlltrickPP)
            {
                var SkillList = SaveData.AlltrickTop[scoreSkillPP.Key];
                if (SkillList.Count < 150)
                {
                    SkillList.Add(new Tuple<Guid, double>(score.ID, scoreSkillPP.Value));
                    SaveData.TotalSkillPP[scoreSkillPP.Key] = (Skill.CalcWeighted(SkillList));
                }
                else {
                    if (scoreSkillPP.Value > SkillList[149].Item2)
                    {
                        SkillList.Add(new Tuple<Guid, double>(score.ID, scoreSkillPP.Value));
                        SaveData.TotalSkillPP[scoreSkillPP.Key] = (Skill.CalcWeighted(SkillList));
                    }
                }
                SkillList.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            }
        }

        /// <summary>
        /// Returns a list of scores based on a SkillTop list contained in a <see cref="CSaveData.AlltrickTop"/>.
        /// </summary>
        /// <returns>A list of each <see cref="Score"/> in order.</returns>
        public static List<Score> GetTrickTopScoreList(ISkill skill) {
            List<Tuple<Guid, double>> list = SaveData.AlltrickTop[skill.Identifier];
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
