using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Osu.Scoring;
using OsuApiHelper;
using osuAT.Game.API;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using osuTK.Graphics;

namespace osuAT.Game.Types
{

    public enum ProcessResult
    {
        FailedScore = -1,
        AlreadySaved = -2,
        BetterScoreSaved = 2,
        WorseScoreSaved = -3,
        UnrankedMap = -4,
        Okay = 1
    }

    public static class ApiScoreProcessor
    {

        public static double ApiReqs = 0;

        /// <summary>
        /// Checks if the quality of the score given meets the neccessary requirements for it to be saved.
        /// </summary>
        /// <param name="deleteIfBetter">If a worse score than the score given is set on the same map and
        /// this parameter is true, the worse score will be deleted from the SaveStorage to create an opening for
        /// the better score.</param>
        /// <returns></returns>
        public static ProcessResult CheckScoreValidity(OsuPlay osuScore,bool deleteIfBetter = false)
        {
            if (osuScore.Rank == "F")
            {
                Console.WriteLine("Score is F ranked.");
                return ProcessResult.FailedScore;
            }
            foreach (Score savedscore in SaveStorage.SaveData.Scores.Values)
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
                        Console.WriteLine("New top score detected!");
                        if (deleteIfBetter)
                        {
                            Console.Write(" Overwriting previous.");
                            SaveStorage.RemoveScore(savedscore.ID);
                        }
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
            // return if the map is not ranked/approved
            if (!(osuMap.Status == BeatmapStatus.Ranked || osuMap.Status == BeatmapStatus.Approved))
                return ProcessResult.UnrankedMap;

            return ProcessResult.Okay;
        }
        public static string GetMapFolder(OsuBeatmap osuMap) {

            string mapFolder = Directory.GetDirectories(SaveStorage.ConcateOsuPath(@"Songs\")).Where((folder) =>
            {
                return (folder.StartsWith(SaveStorage.ConcateOsuPath(@"Songs\" + osuMap.BeatmapSetID + ' ')));
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
                Console.WriteLine(filemapid, osuMap.BeatmapID);
                return (filemapid == osuMap.BeatmapID);
            }).ElementAt(0);

            return osuFile;
        }
        public static Score ConvertToScore(OsuPlay osuScore, OsuBeatmap osuMap)
        {

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
                    StarRating = (double)osuMap.Starrating
                },
                Combo = (int)osuScore.MaxCombo,
                AccuracyStats = new AccStat(
                    (int)osuScore.C300,
                    (int)osuScore.C100,
                    (int)osuScore.C50,
                    (int)osuScore.CMiss),
                TotalScore = osuScore.Score,
                ModsString = osuScore.Mods.ToString().Split(", ").ToList(),
                Grade = osuScore.Rank
            };
        }

        /// <summary>
        /// Saves the given play to SaveStorage by using data from osuScore and mapRet() to create a <see cref="Score"/>.
        /// </summary>
        /// <param name="osuScore">A score of type<see cref="OsuPlay"/>.</param>
        /// <param name="mapRet">A delegate that returns a <see cref="OsuBeatmap"/>. It's structured this way to make sure
        /// the least API requests are made as possible.</param>
        public static ProcessResult SaveToStorageIfValid(OsuPlay osuScore, BeatmapReturner mapRet)
        {
            Console.WriteLine($"{ApiReqs} osu!api v1 requests have been sent.");

            ProcessResult scoreResult = CheckScoreValidity(osuScore, true);
            Console.WriteLine(scoreResult);
            if (scoreResult < ProcessResult.Okay) return scoreResult;

            ApiReqs += 1;
            var osuMap = mapRet();
            ProcessResult mapResult = CheckMapValidity(osuMap);
            Console.WriteLine(mapResult);
            if (mapResult < ProcessResult.Okay) return mapResult;

            // Add to SaveStorage
            Score score = ConvertToScore(osuScore, osuMap);
            score.Register();
            SaveStorage.AddScore(score);
            SaveStorage.Save();
            return scoreResult;
        }

        public delegate OsuBeatmap BeatmapReturner();
    }
}
