using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NUnit.Framework;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.IO.Network;
using osu.Game.Localisation;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Osu.Scoring;
using osu.Game.Screens;
using OsuApiHelper;
using osuAT.Game.API;
using osuAT.Game.Types;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using osuTK.Graphics;

namespace osuAT.Game
{
    public enum ProcessResult
    {
        IDInvalid = 0,
        FailedScore = -1,
        AlreadySaved = -2,
        BetterScoreSaved = 2,
        WorseScoreSaved = -3,
        UnrankedMap = -4,
        MapNotDownloaded = -5,
        RateLimited = -6,
        Okay = 1
    }

    // [!] add difficulty recalculation based on mods (ex. DT 6* -. 8*)
    public static class ApiScoreProcessor
    {
        public static double ApiReqs = 0;

        /// <summary>
        /// Checks if the quality of the score given meets the neccessary requirements for it to be saved.
        /// </summary>
        /// <param name="deleteIfBetter">If a worse score than the score given is set on the same map and
        /// this parameter is true, the worse score will be deleted from the SaveStorage to create an opening for
        /// the better score.</param>
        public static ProcessResult CheckScoreValidity(OsuPlay osuScore, bool deleteIfBetter = false)
        {
            if (osuScore.ScoreID == null)
            {
                return ProcessResult.IDInvalid;
            }
            if (osuScore.Rank == "F")
            {
                Console.WriteLine("Score is F ranked.");
                return ProcessResult.FailedScore;
            }

            foreach (var savedscore in SaveStorage.SaveData.Scores.Values)
            {
                // check if the score has already been saved.
                if (savedscore.OsuID.ToString() == osuScore.ScoreID)
                {
                    Console.WriteLine("Score " + savedscore.OsuID.ToString() + " (osu! id) has already been imported.");
                    return ProcessResult.AlreadySaved;
                }

                // check if a score on that diff has already been set,
                if (savedscore.BeatmapInfo.MapID.ToString() == osuScore.MapID)
                {
                    // check if the just score set is higher than the one currently savd.
                    if (savedscore.TotalScore < osuScore.Score)
                    {
                        Console.Write("New top score detected!");
                        if (deleteIfBetter)
                        {
                            Console.WriteLine(" Overwriting previous.");
                            SaveStorage.RemoveScore(savedscore.ID);
                        }
                        Console.WriteLine("");
                        return ProcessResult.WorseScoreSaved;
                    }
                    else
                    {
                        Console.WriteLine("A better score has already been saved.");
                        return ProcessResult.BetterScoreSaved;
                    }
                }
            }
            return ProcessResult.Okay;
        }

        /// <summary>
        /// Checks if the quality of the map given meets the neccessary requirements for a score set on it to be saved.
        /// </summary>
        public static ProcessResult CheckMapValidity(OsuBeatmap osuMap)
        {
            if (ApiReqs > 30)
            {
                return ProcessResult.RateLimited;
            }
            // return if the map is not ranked/approved
            if (!(osuMap.Status == BeatmapStatus.Ranked || osuMap.Status == BeatmapStatus.Approved))
                return ProcessResult.UnrankedMap;
            if (GetMapFolder(osuMap) == default)
                return ProcessResult.MapNotDownloaded;
            return ProcessResult.Okay;
        }

        public static string GetResultMessages(ProcessResult result, OsuPlay score)
        {
            string scorestr = "[" + score.ScoreID + "]";
            switch (result)
            {
                case ProcessResult.AlreadySaved:
                    return "Score" + scorestr + " has already been saved.";

                case ProcessResult.BetterScoreSaved:
                    return "A higher score on " + scorestr + " has already been saved.";

                case ProcessResult.FailedScore:
                    return "Score " + scorestr + "is a failed score!";

                case ProcessResult.UnrankedMap:
                    return "The map on " + scorestr + " is unranked!";

                case ProcessResult.MapNotDownloaded:
                    return "! - The beatmap " + score.MapID + " isn't downloaded!";
            }
            return "Imported " + scorestr + "successfully!";
        }

        public static string GetMapFolder(OsuBeatmap osuMap)
        {
            Console.WriteLine(osuMap.BeatmapSetID + " | " + osuMap.BeatmapID);

            if (!SaveStorage.OsuPathIsValid())
                return default;

            var mapList = Directory.GetDirectories(SaveStorage.ConcateOsuPath(@"Songs\")).Where((folder) =>
            {
                return folder.StartsWith(SaveStorage.ConcateOsuPath(@"Songs\" + osuMap.BeatmapSetID + ' '));
            });

            if (!mapList.Any()) return default;

            var mapFolder = mapList.ElementAt(0);

            var osuFile = Directory.GetFiles(mapFolder, "*.osu").Where((file) =>
            {
                Console.WriteLine(file);
                var text = File.ReadAllLines(file);
                string filemapid = default;
                int format = int.Parse(text[0].Split("osu file format v")[1]);
                if (format > 9)
                {
                    // Directly compare BeatmapID
                    foreach (var line in text)
                    {
                        if (line.StartsWith("BeatmapID:")) filemapid = line.Split("BeatmapID:")[1];
                        if (line == "[Difficulty]") break;
                    }
                    Console.WriteLine(filemapid + "|" + osuMap.BeatmapID);
                    return filemapid == osuMap.BeatmapID;
                }

                // Instead compare difficulty name for versions at and below 9 (HACK)
                int matches = 0;
                foreach (var line in text)
                {
                    if (line.StartsWith("Version:") && line.Split("Version:")[1] == osuMap.DifficultyName) matches += 1;
                    if (line == "[Difficulty]") break;
                }
                return matches == 1;
            }).ElementAt(0);

            return osuFile;
        }

        public static Score ConvertToScore(OsuPlay osuScore, OsuApiBeatmap osuMap)
        {
            List<string> modString = osuScore.Mods.ToString().Split(", ").ToList();
            modString.ForEach(n => Console.WriteLine(n));
            // Prevent nightcore and doubletime from stacking
            if (modString.Contains("DoubleTime") && modString.Contains("Nightcore"))
            {
                modString.Remove("DoubleTime");
            }

            // Bind original score and map together
            osuScore.Beatmap = osuMap;

            return new Score
            {
                ScoreRuleset = RulesetStore.Osu,
                OsuID = long.Parse(osuScore.ScoreID),
                BeatmapInfo = new Beatmap
                {
                    MapID = int.Parse(osuMap.BeatmapID),
                    MapsetID = int.Parse(osuMap.BeatmapSetID),
                    SongArtist = osuMap.Artist,
                    SongName = osuMap.Title,
                    DifficultyName = osuMap.DifficultyName,
                    MapsetCreator = osuMap.Mapper,
                    FolderLocation = GetMapFolder(osuMap).Remove(0, SaveStorage.SaveData.OsuPath.Length + 1),
                    MaxCombo = (int)osuMap.MaxCombo,
                    StarRating = (double)osuMap.Starrating,
                    OnlineMD5Hash = osuMap.FileMd5
                },
                Combo = (int)osuScore.MaxCombo,
                AccuracyStats = new AccStat(
                    (int)osuScore.C300,
                    (int)osuScore.C100,
                    (int)osuScore.C50,
                    (int)osuScore.CMiss),
                TotalScore = osuScore.Score,
                ModsString = modString,
                Grade = osuScore.Rank
            };
        }

        /// <summary>
        /// Adds the given play to SaveStorage by using data from osuScore and mapRet() to create a <see cref="Score"/>.
        /// </summary>
        /// <param name="osuScore">A score of type<see cref="OsuPlay"/>.</param>
        /// <param name="mapRet">A delegate that returns a <see cref="OsuBeatmap"/> incase the score is already invalid. </param>
        public static async Task<ProcessResult> AddToStorageIfValid(OsuPlay osuScore, TaskBeatmapReturner mapRet)
        {
            Console.WriteLine($"{ApiReqs} osu!api v1 requests have been sent.");

            var scoreResult = CheckScoreValidity(osuScore, true);
            Console.WriteLine(scoreResult);
            if (scoreResult < ProcessResult.Okay) return scoreResult;

            var osuMap = await mapRet();
            var mapResult = CheckMapValidity(osuMap);
            Console.WriteLine(mapResult);
            if (mapResult < ProcessResult.Okay) return mapResult;

            // Add to SaveStorage
            Console.WriteLine("Converting");
            var score = ConvertToScore(osuScore, osuMap);
            Console.WriteLine("Registering");
            await score.Register(async: true);
            Console.WriteLine("Adding to storage");
            SaveStorage.AddScore(score);
            return scoreResult;
        }

        /// <summary>
        /// Saves the given play to SaveStorage by using data from osuScore and mapRet() to create a <see cref="Score"/>.
        /// </summary>
        /// <param name="osuScore">A score of type<see cref="OsuPlay"/>.</param>
        /// <param name="mapRet">A delegate that returns a <see cref="OsuBeatmap"/> incase the score is already invalid. </param>
        public static async Task<ProcessResult> SaveToStorageIfValid(OsuPlay osuScore, TaskBeatmapReturner mapRet)
        {
            ProcessResult result = await AddToStorageIfValid(osuScore, mapRet);
            if (result > ProcessResult.Okay)
                SaveStorage.Save();
            return result;
        }

        public delegate OsuApiBeatmap BeatmapReturner();

        public delegate Task<OsuApiBeatmap> TaskBeatmapReturner();

        public static async Task<List<OsuPlay>> OsuGetBeatmapScores(string beatmapid, string username, OsuMode mode = OsuMode.Standard, int limit = 1, bool generateBeatmaps = false)
        {
            ApiReqs++;
            string[] link_split = new string[11]
            {
                "https://osu.ppy.sh/api/",
                "get_scores?k=",
                OsuApiKey.Key,
                "&b=",
                beatmapid,
                "&u=",
                username,
                "&m=",
                ((int)mode).ToString(),
                "&limit=",
                limit.ToString(),
            };
            string link = string.Concat(link_split);
            var request = new NullableJsonWebRequest<List<OsuPlay>>(link);
            Console.WriteLine(link);
            await request.PerformAsync();
            var data = request.ResponseObject;
            if (data?.Count > 0)
            {
                data.ForEach((Action<OsuPlay>)(async (OsuPlay play) =>
                {
                    play.Mode = mode;
                    if (generateBeatmaps)
                    {
                        play.Beatmap = await ApiScoreProcessor.OsuGetBeatmap(play.MapID, play.Mods, mode);
                    }
                }));
            }
            if (data == null || data.Count == 0)
            {
                return null;
            }
            return data;
        }

        public static async Task<OsuApiBeatmap> OsuGetBeatmap(string id, OsuMods mods = OsuMods.None, OsuMode mode = OsuMode.Standard)
        {
            ApiReqs++;
            int num = (int)mode;
            string[] link_split = new string[9]
            {
                "https://osu.ppy.sh/api/",
                "get_beatmaps?k=",
                OsuApiKey.Key,
                "&b=",
                id,
                "&m=",
                num.ToString(),
                "&a=1&mods=",
                ((int)mods.ModParser(forApi: true)).ToString()
            };
            string link = string.Concat(link_split);
            var request = new NullableJsonWebRequest<List<OsuApiBeatmap>>(link);
            await request.PerformAsync();
            var data = request.ResponseObject;

            if (data != null && data.Count > 0)
            {
                data[0].Mode = mode;
                data[0].Mods = mods;
                data[0].MapStats = new MapStats(data[0], mods);
            }

            if (data == null || data.Count <= 0)
            {
                return null;
            }

            return data[0];
        }

        [Obsolete("Use the async method OsuGetBeatmapScores instead.")]
        public static List<OsuPlay> OsuApiGetBeatmapScores(string beatmapid, string username, OsuMode mode = OsuMode.Standard, int limit = 1, bool generateBeatmaps = false)
        {
            ApiReqs++;
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
                data.ForEach((Action<OsuPlay>)delegate (OsuPlay play)
                {
                    play.Mode = mode;
                    if (generateBeatmaps)
                    {
                        play.Beatmap = ApiScoreProcessor.OsuApiGetBeatmapWithMD5(play.MapID, play.Mods, mode);
                    }
                });
            }

            if (data == null || data.Count <= 0)
            {
                return null;
            }

            return data;
        }

        [Obsolete("Use the async method OsuGetBeatmap instead.")]
        public static OsuApiBeatmap OsuApiGetBeatmapWithMD5(string id, OsuMods mods = OsuMods.None, OsuMode mode = OsuMode.Standard)
        {
            ApiReqs++;
            string[] obj = new string[9]
            {
                "https://osu.ppy.sh/api/",
                "get_beatmaps?k=",
                OsuApiKey.Key,
                "&b=",
                id,
                "&m=",
                null,
                null,
                null
            };
            int num = (int)mode;
            obj[6] = num.ToString();
            obj[7] = "&a=1&mods=";
            obj[8] = ((int)mods.ModParser(forApi: true)).ToString();
            List<OsuApiBeatmap> data = APIHelper<List<OsuApiBeatmap>>.GetData(string.Concat(obj), useCache: true);
            if (data != null && data.Count > 0)
            {
                data[0].Mode = mode;
                data[0].Mods = mods;
                data[0].MapStats = new MapStats(data[0], mods);
            }

            if (data == null || data.Count <= 0)
            {
                return null;
            }

            return data[0];
        }

        public static bool ApiKeyValid = false;

        public static bool UpdateKeyValid()
        {
            ApiKeyValid = OsuApi.IsKeyValid();
            return ApiKeyValid;
        }
    }
}
