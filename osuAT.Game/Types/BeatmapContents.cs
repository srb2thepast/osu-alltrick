using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Audio;
using osu.Framework.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osuAT.Game.Types;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using OsuBeatmap = osu.Game.Beatmaps.Beatmap;
using osu.Game.Rulesets.Osu.Difficulty;
using OsuApiHelper;

namespace osuAT.Game.Types
{
    public class BeatmapContents
    {
        public string FolderLocation { get; private set; }

        public BeatmapDifficulty DifficultyInfo { get; set; }

        public ProcessorWorkingBeatmap Workmap { get; set; }

        public IBeatmap PlayableMap { get; set; }

        public List<HitObject> HitObjects { get; set;  }
   

        public List<DifficultyHitObject> DiffHitObjects { get; set;  }

        public RulesetInfo ContentRuleset {get; set;}

        private IExtendedDifficultyCalculator diffcalc;

        /// <summary>
        /// Creates a an empty BeatmapContents. Should only be used for testing.
        /// </summary>
        public BeatmapContents() {

        }

        /// <summary>
        /// Creates a new BeatmapContents based off of a .osu file.
        /// </summary>
        public BeatmapContents(string osufile,RulesetInfo ruleset = null,List<ModInfo> mods = null)
        {
            string path = SaveStorage.ConcateOsuPath(osufile);
            Console.WriteLine(path);
            if (!(File.Exists(path)))
            {
                throw new ArgumentNullException($"The path of this beatmap does not exist!!! : {osufile}");
            }

            ruleset ??= RulesetStore.Osu;
            mods ??= new List<ModInfo>();
            List<Mod> osuModList = new List<Mod>();
            foreach (ModInfo mod in mods)
            {
                osuModList.Add(ModStore.ConvertToOsuMod(mod));
            }

            FolderLocation = osufile;
            ContentRuleset = ruleset;
            Workmap = ProcessorWorkingBeatmap.FromFileOrId(path);
            PlayableMap = Workmap.GetPlayableBeatmap(RulesetStore.ConvertToOsuRuleset(ContentRuleset).RulesetInfo, osuModList);
            DifficultyInfo = PlayableMap.Difficulty;
            diffcalc = RulesetStore.GetDiffCalcObj(ruleset, Workmap);
            ContentRuleset = ruleset; 
            HitObjects = Workmap.Beatmap.HitObjects.ToList();
            DiffHitObjects = diffcalc.GetDifficultyHitObjects(PlayableMap, 1.0).ToList();
            
        }
    }
}
