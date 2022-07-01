using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Testing;
using osuAT.Game.Skills;

#nullable enable

namespace osuAT.Game.Types.Score
{

    public class AccStat
    {
        [JsonProperty("count_300s")]
        public int Count300 { get; set; }

        [JsonProperty("count_100s")]
        public int Count100 { get; set; }

        [JsonProperty("count_50s")]
        public int Count50 { get; set; }

        [JsonProperty("count_miss")]
        public int CountMiss { get; set; }

        public AccStat(int count300, int count100, int count50, int countMiss)
        {
            Count300 = count300;
            Count100 = count100;
            Count50 = count50;
            CountMiss = countMiss;
        }

    }

    public class Score
    {
        /// <summary>
        /// The score's ID based on osu!web. It can be null depending on how the score is imported.
        /// </summary>
        [JsonProperty("scoreid")]
        public int? ID { get; set; }

        /// <summary>
        /// Whether or not the score was set in osu!lazer.
        /// </summary
        [JsonProperty("islazer")]
        public bool IsLazer { get; set; }

        /// <summary>
        /// The ID of the beatmap this score was set on.
        /// </summary
        [JsonProperty("beatmap")]
        public uint BeatmapID { get; set; }

        /// <summary>
        /// The ID of the beatmapset this score was set on.
        /// </summary
        [JsonProperty("beatmapset")]
        public uint BeatmapsetID { get; set; }

        /// <summary>
        /// The location of the beatmap on the player's computer. There is a very high chance of it being null.
        /// </summary
        [JsonIgnore]
        public string? BeatmapLocation { get; set; }

        /// <summary>
        /// The date this score was created **in osu!alltrick**.
        /// </summary
        [JsonProperty("created_at")]
        public DateTime DateCreated { get; set; }


        /// <summary>
        /// The accuracy the player got.
        /// </summary
        [JsonProperty("accuracy")]
        public double Accuracy { get; set; }

        /// <summary>
        /// The Total Score the player got.
        /// </summary
        [JsonProperty("totalscore")]
        public long TotalScore { get; set; }

        /// <summary>
        /// The details on how many misses, 300s, 100s, and 50s the player got.
        /// </summary
        [JsonProperty("accStats")]
        public AccStat? AccuracyStats { get; set; }

        /// <summary>
        /// The amount of Combo the player got.
        /// </summary
        [JsonProperty("combo")]
        public int Combo { get; set; }

        /// <summary>
        /// The amount of Combo the player got.
        /// </summary
        [JsonProperty("alltrickpp")]
        public SkillPPTotals? AlltrickPP { get; set; }

        [JsonIgnore]
        public virtual RulesetInfo? Ruleset { get; set; }

        [JsonIgnore]
        public List<ModInfo>? Mods { get; set; } = default!;

        [JsonProperty("mods")]
        public List<string> ModsString { get; set; }
        public Score(int iD, uint beatmap, uint beatmapset, long totalScore, AccStat accuracy, int combo, SkillPPTotals alltrickPP, bool isLazer, List<ModInfo> mods)
        {
            ID = iD;
            BeatmapID = beatmap;
            BeatmapsetID = beatmapset;
            TotalScore = totalScore;
            AccuracyStats = accuracy;
            Combo = combo;
            AlltrickPP = alltrickPP;
            IsLazer = isLazer;
            Mods = mods;


            List<string> totalstringtab = new List<string>();
            if (Mods?.Count > 0)
            {
                foreach (var mod in Mods)
                {
                    totalstringtab.Add(mod.Name);
                    Console.WriteLine(mod.Name);
                }
            }
            ModsString = totalstringtab;
        }
    }
}
