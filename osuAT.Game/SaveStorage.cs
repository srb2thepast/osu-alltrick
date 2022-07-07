using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using osu.Framework.Platform; // Reminder: consider using osu.Framework.Platform.Storage for safe file writing.
using osuAT.Game.Types;
using osuAT.Game.Skill;

namespace osuAT.Game
{
    public class CSaveData {
        /// <summary>
        /// The save data version.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

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
        [JsonProperty("alltrickPP")]
        public SkillPPTotals TotalSkillPP { get; set;}

        public SkillTopScores AlltrickTop { get; set; }

        /// <summary>
        /// A list of every score that osu!alltrick was able to process.
        /// </summary>
        [JsonProperty("scores")]
        public List<Score> Scores { get; set; }
    }

    public class SaveStorage
    {

        public static CSaveData SaveData;
        public static string SaveFile = "savedata\\data.json";

        static SaveStorage() {

            if (!(CheckSaveExists())) {
                SaveData = new CSaveData {
                    Version = "osu!AT save data v1",
                    PlayerID = -1,
                    APIKey = "null",
                    TotalSkillPP = new SkillPPTotals(),
                    Scores = new List<Score>()
                };
                Save();
                return;
            }


            SaveData = JsonConvert.DeserializeObject<CSaveData>(Read());

            int i = 0;
            foreach (Score score in SaveData.Scores) {
                score.Register(index: i);
                i += 1;
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
            return Base64Decode(File.ReadAllText(SaveFile));
        }

        public static FileInfo Save() {
            string encodedtext = Base64Encode(JsonConvert.SerializeObject(SaveData));
            File.WriteAllText(SaveFile, encodedtext);
            return new FileInfo(SaveFile);
        }

        public static void DeleteScore(int location) {
            SaveData.Scores[location] = new Score()
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
                IndexPosition = SaveData.Scores[location].IndexPosition,
                DateCreated = SaveData.Scores[location].DateCreated
            };
            SaveData.Scores[location].Register();
            Save(); 
        }

        public static (FileInfo,int) SaveScore(Score score) {
            CheckSaveExists();
            
            SaveData.Scores.Add(score);
            score.IndexPosition = SaveData.Scores.Count - 1;
            return (Save(), SaveData.Scores.Count);

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
