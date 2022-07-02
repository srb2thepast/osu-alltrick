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
        /// The score's ID based on it's placement in the save file. It can be null if this score has not been saved
        /// to file yet.
        /// </summary>
        [JsonIgnore]
        public int? ID { get; set; }


        /// <summary>
        /// The score's ID based on osu!web. It can be null depending on how the score is imported.
        /// </summary>
        [JsonProperty("scoreid")]
        public int? OsuID { get; set; }

        /// <summary>
        /// Whether or not the score was set in osu!lazer.
        /// </summary>
        [JsonProperty("islazer")]
        public bool IsLazer { get; set; }

        /// <summary>
        /// The ID of the beatmap this score was set on.
        /// </summary>
        [JsonProperty("beatmap")]
        public uint BeatmapID { get; set; }

        /// <summary>
        /// The ID of the beatmapset this score was set on.
        /// </summary>
        [JsonProperty("beatmapset")]
        public uint BeatmapsetID { get; set; }

        /// <summary>
        /// The location of the beatmap on the player's computer. There is a very high chance of it being null.
        /// </summary>
        [JsonIgnore]
        public string? BeatmapLocation { get; set; }

        /// <summary>
        /// The date this score was created **in osu!alltrick**.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime DateCreated { get; set; }


        /// <summary>
        /// The accuracy the player got.
        /// </summary>
        [JsonProperty("accuracy")]
        public double Accuracy { get; set; }

        /// <summary>
        /// The Total Score the player got.
        /// </summary>
        [JsonProperty("totalscore")]
        public long TotalScore { get; set; }

        /// <summary>
        /// The details on how many misses, 300s, 100s, and 50s the player got.
        /// </summary>
        [JsonProperty("accStats")]
        public AccStat? AccuracyStats { get; set; }

        /// <summary>
        /// The amount of Combo the player got.
        /// </summary>
        [JsonProperty("combo")]
        public int Combo { get; set; }

        /// <summary>
        /// The cached alltrickpp values for this skill.
        /// </summary>
        [JsonProperty("alltrickpp")]
        public SkillPPTotals? AlltrickPP { get; set; }

        /// <summary>
        /// The Ruleset this score was set in based on the <see cref="RulesetInfo"/> class.
        /// </summary>
        [JsonIgnore]
        public virtual RulesetInfo? Ruleset { get; set; }

        /// <summary>
        /// The Mods the score was set with based on the <see cref="ModInfo"/> class.
        /// </summary>
        [JsonIgnore]
        public List<ModInfo>? Mods { get; set; } = default!;

        /// </summary>
        /// A list of the name of each mod this score was set with.
        /// </summary>
        [JsonProperty("modlist")]
        public List<string> ModsString { get; set; }
        public Score(int osuid, uint beatmap, uint beatmapset,double accuracy, long totalScore, AccStat accstat, int combo, SkillPPTotals alltrickPP, bool isLazer, List<ModInfo> mods)
        {
            OsuID = osuid;
            IsLazer = isLazer;
            BeatmapID = beatmap;
            BeatmapsetID = beatmapset;
            TotalScore = totalScore;
            Accuracy = accuracy;
            AccuracyStats = accstat;
            Combo = combo;
            AlltrickPP = alltrickPP;
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
