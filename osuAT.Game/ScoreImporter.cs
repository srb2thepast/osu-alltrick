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
using osuAT.Game.Types.BeatmapParsers;
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
                OsuMemoryStatus curStatus = (OsuMemoryStatus)gameData.RawStatus;
                osuReader.TryRead(gameData);
                Console.WriteLine(lastScreen); ;
                Console.WriteLine(gameData.OsuStatus);

                // check if the play went from playing to the results screen
                if (lastScreen == OsuMemoryStatus.Playing && curStatus == OsuMemoryStatus.ResultsScreen || (lastScreen == OsuMemoryStatus.Playing && curStatus == OsuMemoryStatus.MultiplayerResultsscreen))
                {
                    apireqs -= TickDelay / 100;
                    Task.Delay(2000); // wait a bit incase osu!servers are slow
                    var osuScore = OsuApi.GetUserRecent("srb2thepast")[0];
                    apireqs += 1;
                    Console.WriteLine(osuScore.Rank);

                    // check if score was an F rank
                    if (osuScore.Rank == "F") return;
                    var osuMap = OsuApi.GetBeatmap(osuScore.MapID, osuScore.Mods, osuScore.Mode);
                    apireqs += 1;
                    Console.WriteLine(osuMap.BeatmapSetID);
                    Console.WriteLine(osuMap.Status);

                    // check if the map is ranked/approved
                    if (osuMap.Status == BeatmapStatus.Ranked || osuMap.Status == BeatmapStatus.Approved)
                    {
                        var mapScore = OsuApiGetScores(osuScore.MapID, "srb2thepast")[0];
                        apireqs += 1;
                        Console.WriteLine(mapScore);

                        // check if the score set was the user's best score on the map before continuing
                        if (true)// (mapScore?.ScoreID == osuScore.ScoreID)
                        {

                            // check if the score has already been saved
                            foreach (Score savedscore in SaveStorage.SaveData.Scores.Values)
                            {
                                if (savedscore.OsuID.ToString() == mapScore.ScoreID)
                                {
                                    lastScreen = (OsuMemoryStatus)gameData.RawStatus;
                                    return;
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
                                    FolderName = osuFile.Remove(0, SaveStorage.SaveData.OsuPath.Length + 1),
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
                        }
                    }

                }


                lastScreen = (OsuMemoryStatus)gameData.RawStatus;
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
