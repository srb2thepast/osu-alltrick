using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using osu.Framework;
using osuAT.Game.Types;
using osuAT.Game.Objects.LazerAssets;
using OsuApiHelper;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;

namespace osuAT.Game
{


    public class ScoreImporter
    {
        // looks like timers dont go to well with hot reload in o!f. alternative is needed
        public static int TickDelay = 1500;
        private static Timer scoreSetTimer = new Timer(TickDelay);
        private static double apireqs = 0;
        private static StructuredOsuMemoryReader osuReader;
        private static OsuMemoryStatus lastScreen = OsuMemoryStatus.Playing;


        public static void Init()
        {
            Console.WriteLine("initalised scoreimpoter");
            osuReader = StructuredOsuMemoryReader.Instance.GetInstanceForWindowTitleHint("");
            scoreSetTimer.Interval = TickDelay;
            scoreSetTimer.Elapsed += scoreSetTimer_Elapsed;
            scoreSetTimer.Enabled = true;
            scoreSetTimer.AutoReset = true;
            scoreSetTimer.Start();
        }

        public static List<OsuPlay> OsuApiGetScores(string beatmapid, string username, OsuMode mode = OsuMode.Standard, int limit = 1, bool generateBeatmaps = false)
        {
            string[] obj = new string[11]
            {
                "https://osu.ppy.sh/api/",
                "get_scores?k=",
                OsuApiKey.Key,
                "&b=",
                beatmapid,
                "&u=",
                username,
                "&m=",
                (((int)mode).ToString()),
                "&limit=",  
                limit.ToString(),
            };
            List<OsuPlay> data = APIHelper<List<OsuPlay>>.GetData(string.Concat(obj));
            if (data != null && data.Count > 0)
            {
                data.ForEach(delegate (OsuPlay play)
                {
                    play.Mode = mode;
                    if (generateBeatmaps)
                    {
                        play.Beatmap = OsuApi.GetBeatmap(play.MapID, play.Mods, mode);
                    }
                });
            }

            if (data == null || data.Count <= 0)
            {
                return null;
            }

            return data;
        }
            

        private static async void scoreSetTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!(OsuApi.IsKeyValid())) return;
            if (apireqs >= 30) return;
            try
            {
                GeneralData gameData = new GeneralData();
                osuReader.TryRead(gameData);
                Console.WriteLine("last: " + lastScreen + " | current: " + gameData.OsuStatus);
                var cachedscreen = gameData.OsuStatus;
                // if the play went from playing to the results screen, continue, otherwise, return.
                if (!(lastScreen == OsuMemoryStatus.Playing && gameData.OsuStatus == OsuMemoryStatus.ResultsScreen || (lastScreen == OsuMemoryStatus.Playing && gameData.OsuStatus == OsuMemoryStatus.MultiplayerResultsscreen)))
                {
                    lastScreen = gameData.OsuStatus;
                    return;
                }
                lastScreen = gameData.OsuStatus;
                apireqs -= TickDelay / 100;
                await Task.Delay(2000); // wait a bit incase osu!servers are slow
                var osuScore = OsuApi.GetUserRecent("srb2thepast")[0];
                apireqs += 1;
                Console.WriteLine(osuScore.Rank + " Rank");

                // return if score was an F rank
                if (osuScore.Rank == "F")
                    return;

                var osuMap = OsuApi.GetBeatmap(osuScore.MapID, osuScore.Mods, osuScore.Mode);
                apireqs += 1;

                Console.WriteLine(osuMap.BeatmapSetID + " is " + osuMap.Status);
                // return if the map is not ranked/approved
                if (!(osuMap.Status == BeatmapStatus.Ranked || osuMap.Status == BeatmapStatus.Approved))
                    return;
                var mapScore = OsuApiGetScores(osuScore.MapID, "srb2thepast")[0];
                apireqs += 1;

                Console.WriteLine("Locally set ID: " + osuScore.ScoreID, "Beatmap top ID: " + mapScore.ScoreID);
                // if the score set was not the user's best score on the map, return.
                if (false) // (mapScore?.ScoreID != osuScore.ScoreID)
                    return;

                // already saved check + new top score check
                foreach (Score savedscore in SaveStorage.SaveData.Scores.Values)
                {
                    // check if the score has already been saved, if so return.
                    if (savedscore.OsuID.ToString() == mapScore.ScoreID)
                    {
                        Console.WriteLine("Score " + savedscore.OsuID.ToString() + " (osu! id) has already been imported.");
                        lastScreen = (OsuMemoryStatus)gameData.RawStatus;
                        // return; [!]
                    }
                    // check if a score on that diff has already been set, 
                    if (savedscore.BeatmapInfo.MapID == int.Parse(osuMap.BeatmapID)) {
                        // check if the just score set is higher than the one currently savd. 
                        if (savedscore.TotalScore < osuScore.score) {
                            // if so delete it in preperation for replacement.
                            Console.WriteLine("New top score detected! Overwriting previous.");
                            SaveStorage.RemoveScore(savedscore.ID);
                        } else { // if it's not the best, return.
                            return;
                        }
                    }
                }

                string mapFolder = Directory.GetDirectories(SaveStorage.SaveData.OsuPath + "\\Songs\\").Where((folder) =>
                {
                    return (folder.StartsWith(SaveStorage.SaveData.OsuPath + "\\Songs\\" + osuMap.BeatmapSetID + ' '));
                }).ElementAt(0);
                string osuFile = Directory.GetFiles(mapFolder, "*.osu").Where((file) =>
                {
                    Console.WriteLine(file);
                    string[] text = File.ReadAllLines(file);
                    string filemapid = default;

                    foreach (string line in text)
                    {
                        if (line.StartsWith("BeatmapID:")) filemapid = line.Split("BeatmapID:")[1];
                        if (line == "[Difficulty]") break;
                    }

                    System.Console.WriteLine(filemapid, osuMap.BeatmapID);
                    return (filemapid == osuMap.BeatmapID);
                }).ElementAt(0);
                Console.WriteLine(mapFolder);
;
                AccStat accstats = new AccStat((int)osuScore.C300, (int)osuScore.C100, (int)osuScore.C50, (int)osuScore.CMiss);
                Score score = new Score
                {
                    ScoreRuleset = RulesetStore.Osu,
                    IsLazer = false,
                    OsuID = long.Parse(mapScore.ScoreID),
                    BeatmapInfo = new Beatmap
                    {
                        MapID = int.Parse(osuMap.BeatmapID),
                        MapsetID = int.Parse(osuMap.BeatmapSetID),
                        SongArtist = osuMap.Artist,
                        SongName = osuMap.Title,
                        DifficultyName = osuMap.DifficultyName,
                        MapsetCreator = osuMap.Mapper,
                        FolderLocation = osuFile.Remove(0, SaveStorage.SaveData.OsuPath.Length + 1),
                        MaxCombo = (int)osuMap.MaxCombo,
                        StarRating = (double)osuMap.Starrating
                    },
                    Accuracy = accstats.CalcAcc(),
                    Combo = (int)osuScore.MaxCombo,
                    AccuracyStats = accstats,
                    TotalScore = osuScore.Score,
                    ModsString = osuScore.Mods.ToString().Split(", ").ToList(),
                    Grade = osuScore.Rank


                };
                score.Register();
                SaveStorage.AddScore(score);
                SaveStorage.Save();


                lastScreen = cachedscreen;
            } catch(Exception err) {
                // remove this section when scoreimporter is converted to osu!of's schedule
                File.WriteAllText("errlog_scoreimp.txt", err.StackTrace + "\n ----------- ERROR MESSAGE: \n ----------- " + err.Message);
                lastScreen = OsuMemoryStatus.Unknown;
            }
            finally { }

        }


        public void ImportScore() {
        }
    }
}
