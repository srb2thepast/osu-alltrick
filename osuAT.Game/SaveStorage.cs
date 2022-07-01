using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using osuAT.Game.Types.Score;
using osuAT.Game.Types;
using osuAT.Game.Skills;

namespace osuAT.Game
{
    public interface ISaveData {
        /// <summary>
        /// The save data version.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// The player the save data is attached to.
        /// </summary>
        [JsonProperty("player")]
        public int PlayerName { get; set; }

        /// <summary>
        /// The player's APIKey.
        /// </summary>
        [JsonProperty("APIKey")]
        protected string APIKey { get; set; }

        /// <summary>
        /// The overall cached PP for each skill.
        /// </summary>
        [JsonProperty("alltrickPP")]
        public SkillPPTotals TotalSkillPP { get; set;}

        /// <summary>
        /// A list of every score that osu!alltrick was able to process.
        /// </summary>
        [JsonProperty("scores")]
        public List<Score> Scores { get; set; }
    }

    public class SaveStorage
    {
        private class ShortenedModInfo {

        }
        public static FileInfo SaveToFile(string filename,string text) {
            
            File.WriteAllText(filename, text);
            return new FileInfo(filename);
        }
        public static void SaveScore(Score score) {
            var savedata = "savedata\\" + score.Ruleset.Name.ToLower() + "\\data.json";
            

        }

        public static string ScoreToJson(Score score) {
            return JsonConvert.SerializeObject(score);
        }




        /*
        public static List<(IScore, string)> GetScores()
        {
            List<(IScore, string)> scores = new List<(IScore, string)>();

            if (!checkMainDirectoryExistance())
                return scores;

            var directories = Directory.GetDirectories("Levels/");

            if (directories.Length == 0)
                return scores;

            foreach (var dir in directories)
            {
                var file = $"{dir}/level";

                if (!File.Exists(file))
                    continue;

                var save = $"{dir}/level";
                var contents = File.ReadAllText(save);
                if (dir == "standard") {
                    OsuScore score = JsonConvert.DeserializeObject<OsuScore>(contents)!;
                }

                using (StreamReader sr = File.OpenText(file))
                {
                    var text = sr.ReadLine();
                    sr.Close();

                    var level = JsonConvert.DeserializeObject<IScore>(text);
                    var name = file.Substring(0, dir.Substring(7).Length);

                    scores.Add((level, name));
                }
            }

            return scores;
        }
        
        public static void CreateLevel(string name, Level level)
        {
            if (!LevelExists(name))
                CreateLevelDirectory(name);

            string jsonResult = JsonConvert.SerializeObject(level);

            using (StreamWriter sw = File.CreateText($"Levels/{name}/level"))
            {
                sw.WriteLine(jsonResult.ToString());
                sw.Close();
            }
        }

        public static bool LevelExists(string name)
        {
            if (!checkMainDirectoryExistance())
                return false;

            var directories = Directory.GetDirectories("Levels");

            if (directories.Length == 0)
                return false;

            foreach (var dir in directories)
            {
                if (dir.Substring(7).ToLower() == name.ToLower())
                    return true;
            }

            return false;
        }

        private static bool checkMainDirectoryExistance()
        {
            if (!Directory.Exists("Levels"))
            {
                Directory.CreateDirectory("Levels");
                return false;
            }

            return true;
        }*/
    }
}
