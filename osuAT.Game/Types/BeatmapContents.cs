using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Difficulty;
using OsuApiHelper;
using osuAT.Game.Types;
using OsuBeatmap = osu.Game.Beatmaps.Beatmap;

namespace osuAT.Game.Types
{
    public class BeatmapContents
    {
        public string FolderLocation { get; }

        public BeatmapDifficulty DifficultyInfo { get; set; }

        public ProcessorWorkingBeatmap Workmap { get; set; }

        public IBeatmap PlayableMap { get; set; }

        public List<HitObject> HitObjects { get; set; }

        public List<DifficultyHitObject> DiffHitObjects { get; set; }

        public RulesetInfo ContentRuleset { get; set; }

        private readonly IExtendedDifficultyCalculator diffcalc;

        /// <summary>
        /// Creates a an empty BeatmapContents. Should only be used for testing.
        /// </summary>
        public BeatmapContents()
        { }

        /// <summary>
        /// Creates a new BeatmapContents based off of a .osu file.
        /// </summary>
        public BeatmapContents(string osufile, RulesetInfo ruleset = null, List<ModInfo> mods = null)
        {
            string path = SaveStorage.ConcateOsuPath(osufile);
            Console.WriteLine(path);
            if (!SaveStorage.ExistsInOsuDirectory(osufile, true))
            {
                throw new ArgumentNullException($"The path of this beatmap does not exist or is invalid!!! : {osufile}");
            }

            ruleset ??= RulesetStore.Osu;
            mods ??= new List<ModInfo>();
            List<Mod> osuModList = new List<Mod>();

            double rate = 1;
            foreach (ModInfo mod in mods)
            {
                Mod osuMod;
                osuModList.Add(osuMod = ModStore.ConvertToOsuMod(mod));
                if (osuMod is IApplicableToRate rateMod)
                {
                    rate = rateMod.ApplyToRate(0, rate);
                }
            }

            FolderLocation = osufile;
            ContentRuleset = ruleset;
            Workmap = ProcessorWorkingBeatmap.FromFileOrId(path);
            PlayableMap = Workmap.GetPlayableBeatmap(RulesetStore.ConvertToOsuRuleset(ContentRuleset).RulesetInfo, osuModList);
            DifficultyInfo = PlayableMap.Difficulty;
            diffcalc = RulesetStore.GetDiffCalcObj(ruleset, Workmap);
            ContentRuleset = ruleset;
            DiffHitObjects = diffcalc.GetDifficultyHitObjects(PlayableMap, rate).ToList();
            HitObjects = PlayableMap.HitObjects.ToList();
        }
    }
}
