using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using osuAT.Game;
using osuAT.Game.Skills;

namespace osuAT.Game.Types
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
        public double CalcAcc() {
            return 0.0;
        }
    }

    public class Score
    {
        /// <summary>
        /// The score's ID based on it's placement in the save file. It can be null if this score has not been saved
        /// to file yet.
        /// </summary>
        /// <remarks>This should not be set while constructing.</remarks>
        [JsonProperty("id")]
        public Guid ID { get; set; }

        /// <summary>
        /// The Ruleset name of the Ruleset this score was set in.
        /// </summary>
        [JsonProperty("rulesetname")]
        public string RulesetName { get; set; }

        /// <summary>
        /// The Ruleset this score was set in based on the <see cref="RulesetInfo"/> class.
        /// </summary>
        [JsonIgnore]
        public RulesetInfo ScoreRuleset { get; set; }

        /// <summary>
        /// Whether or not the score was set in osu!lazer.
        /// </summary>
        [JsonProperty("islazer")]
        public bool IsLazer { get; set; }

        /// <summary>
        /// The score's ID based on osu!web. It can be null depending on how the score is imported.
        /// </summary>
        [JsonProperty("osu_scoreid")]
        public long? OsuID { get; set; }


        /// <summary>
        /// All of the information about the beatmap this score was set on.
        /// </summary>
        [JsonProperty("beatmapInfo")]
        public Beatmap BeatmapInfo { get; set; }

        /// <summary>
        /// The letter grade the score got.
        /// </summary>
        [JsonProperty("grade")]
        public string Grade { get; set; }

        /// <summary>
        /// The accuracy the player got.
        /// </summary>
        [JsonProperty("accuracy")]
        public double Accuracy { get; set; }

        /// <summary>
        /// The details on how many misses, 300s, 100s, and 50s the player got.
        /// </summary>
        [JsonProperty("accStats")]
        public AccStat AccuracyStats { get; set; }

        /// <summary>
        /// The amount of Combo the player got.
        /// </summary>
        [JsonProperty("combo")]
        public int Combo { get; set; }

        /// <summary>
        /// The Total Score the player got.
        /// </summary>
        [JsonProperty("totalscore")]
        public long TotalScore { get; set; }

        /// <summary>
        /// The Mods the score was set with based on the <see cref="ModInfo"/> class.
        /// </summary>
        [JsonIgnore]
        public List<ModInfo> Mods { get; set; }

        /// </summary>
        /// A list of the name of each mod this score was set with.
        /// </summary>
        /// <remarks>This should not be set while constructing.</remarks>
        [JsonProperty("modlist")]
        public List<string> ModsString { get; set; }

        [JsonProperty("perfect_combo")]
        public bool PerfectCombo { get; set; }

        /// <summary>
        /// The cached alltrickpp values for this skill.
        /// </summary>
        /// <remarks>This should not be set while constructing.</remarks>
        [JsonProperty("alltrickpp")]
        public Dictionary<string, double> AlltrickPP { get; set; }

        /// <summary>
        /// The date this score was created **in osu!alltrick**.
        /// </summary>
        /// <remarks>This should not be set while constructing.</remarks>
        [JsonProperty("created_at")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The positon of the score in it's parent list.
        /// </summary>
        /// <remarks>This should not be set while constructing.</remarks>
        [JsonIgnore]
        public int IndexPosition { get; set; }

        /// <summary>
        /// Fills in all the missing properties that are not supposed to be set while constructing.
        /// </summary>
        public void Register(bool calcPP = true, bool setDate = true, int index = -1,bool setGUID = true,bool loadBeatmapContents = true)
        {

            if (setGUID) {ID = Guid.NewGuid(); }
            PerfectCombo = BeatmapInfo.MaxCombo == Combo;
            if (setDate) { DateCreated = DateTime.Today; }
            ScoreRuleset ??= RulesetStore.GetByName(RulesetName);
            if (BeatmapInfo.FolderName != "deleted" && BeatmapInfo.FolderName != default)
            {
                if (loadBeatmapContents || calcPP) { BeatmapInfo.LoadMapContents(ScoreRuleset); }
                if (calcPP) { AlltrickPP = Skill.CalcAll(this); }
            }
            else {
                Console.WriteLine("Deleted or unset folder detected. Skipping beatmapinfo contents.");
            }
            RulesetName = ScoreRuleset.Name;
            Mods = Mods;
            IndexPosition = index;
            if (ModsString is null) {
                ModsString = new List<string>();
                foreach (ModInfo mod in Mods)
                {
                    ModsString.Add(mod.Name);
                }
            }
            if (Mods is null) {
                Mods = new List<ModInfo>();
                foreach (string mod in ModsString) {
                    if (mod == "None") continue;
                    Mods.Add(ModStore.GetByName(mod));
                }
            }
        }

        public Score Clone() {
            Score newscore = new Score
            {
                ID = ID,
                ScoreRuleset = ScoreRuleset,
                RulesetName = RulesetName,
                IsLazer =IsLazer,
                OsuID = OsuID,
                BeatmapInfo = BeatmapInfo,
                Grade = Grade,
                Accuracy = Accuracy,
                AccuracyStats = AccuracyStats,
                Combo = Combo,
                PerfectCombo = PerfectCombo,
                TotalScore = TotalScore,
                Mods = Mods,
                ModsString = ModsString,
                AlltrickPP = AlltrickPP,
                DateCreated = DateCreated,
                IndexPosition = IndexPosition,
            };
            return newscore;
        }
    }

}
