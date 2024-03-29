﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osuAT.Game;
using osuAT.Game.Skills;

// [~] Maybe in the future we could finally have the location of misses in a beatmap avaliable based off of a replay! (https://discord.com/channels/546120878908506119/757615676310552598/1033540314914373642) <- PP Discord

namespace osuAT.Game.Types
{
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
        [JsonIgnore]
        public double Accuracy => AccuracyStats.CalcAcc();

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
        /// The cached alltrickpp values for this skill. Follows a format of (ISkill.ID: pp).
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
        public async Task Register(bool calcPP = true, bool setDate = true, int index = -1, bool setGUID = true, bool loadBeatmapContents = true, bool async = false)
        {
            if (setGUID) { ID = Guid.NewGuid(); }
            if (setDate) { DateCreated = DateTime.Today; }
            PerfectCombo = BeatmapInfo.MaxCombo == Combo;
            ScoreRuleset ??= RulesetStore.GetByName(RulesetName);
            RulesetName = ScoreRuleset.Name;
            Mods = Mods;
            IndexPosition = index;
            if (ModsString is null)
            {
                ModsString = new List<string>();
                foreach (ModInfo mod in Mods)
                {
                    ModsString.Add(mod.Name);
                }
            }
            if (Mods is null)
            {
                Mods = new List<ModInfo>();
                foreach (string mod in ModsString)
                {
                    if (mod == "None") continue;
                    Mods.Add(ModStore.GetModInfoByName(mod));
                }
            }

            if ((calcPP || loadBeatmapContents) && !BeatmapInfo.FolderLocationIsValid(true))
            {
                Console.WriteLine($"Deleted or unset folder detected for map {OsuID}, {BeatmapInfo.FolderLocation}. Skipping beatmapinfo contents.");
                if (calcPP)
                {
                    AlltrickPP = await Skill.CalcAll(this, async);
                }
                // [!] TODO: Test above. Process:
                // 1. Delete a map one of the scores saved are connected to.
                // 2. Update one of the skills, causing every score to recalculate.
                // 3. See results.
                return;
            }
            if (loadBeatmapContents || calcPP) { BeatmapInfo.LoadMapContents(ScoreRuleset, Mods); }
            if (calcPP) { AlltrickPP = await Skill.CalcAll(this, async); }
        }

        public Score Clone()
        {
            Score newscore = new Score
            {
                ID = ID,
                ScoreRuleset = ScoreRuleset,
                RulesetName = RulesetName,
                OsuID = OsuID,
                BeatmapInfo = BeatmapInfo,
                Grade = Grade,
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
