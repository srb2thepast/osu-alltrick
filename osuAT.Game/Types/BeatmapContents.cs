using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
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
        
        private string folderLocation = default!;
        
        [JsonIgnore]
        public string FolderLocation => folderLocation  // very likely to be null

        [JsonIgnore]
        public BeatmapDifficulty DifficultyInfo { get; set; } // also also very likely to be null (also depends on foldername)

        public ProcessorWorkingBeatmap Workmap { get; set; }
        
        private List<Mod> appliedMods { get; set; }

        public Beatmap PlayableMap => Workmap.GetPlayableBeatmap(rulesetInstance.RulesetInfo, appliedMods);
        [JsonIgnore]
        public List<HitObject> HitObjects => Workmap.GetBeatmap .HitObjects.ToList(); // also very likely to be null (depends on foldername)

        [JsonIgnore]
        public List<DifficultyHitObject> DiffHitObjects => diffcalc.GetDifficultyHitObjects(PlayableMap, 1.0).ToList(); // also very likely to be null (depends on foldername)

        [JsonIgnore]
        public RulesetInfo ContentRuleset {get; set;}

        private IExtendedDifficultyCalculator diffcalc;

        /// <summary>
        /// Creates a new BeatmapContents with default params. 
        /// </summary>
        public BeatmapContents() {
            HitObjects = new List<HitObject> { };
            DiffHitObjects = new List<DifficultyHitObject> { };
            return null;
        }

        /// <summary>
        /// Creates a new BeatmapContents based off of a .osu file.
        /// </summary>
        public BeatmapContents(string osufile,RulesetInfo ruleset = RulesetStore.Osu,List<ModInfo> mods = new List<ModInfo>()) {
            if (!(File.Exists(path)))
            {
                throw new ArgumentNullException($"The path of this beatmap does not exist!!! : {path}");
            }
            folderLocation = osufile;
            ContentRuleset = ruleset;
            var rulesetInstance = RulesetStore.ConvertToOsuRuleset(ruleset);

            string path = SaveStorage.SaveData.OsuPath + @"\" + osufile;
            Console.WriteLine(path);
            Workmap = ProcessorWorkingBeatmap.FromFileOrId(path);
            DifficultyInfo = PlayableMap.Difficulty;

            diffcalc = RulesetStore.GetDiffCalcObj(ruleset, Workmap);

            List<Mod> osuModList = new List<Mod>();
            foreach (ModInfo mod in mods) {
                Console.WriteLine(mod);
                osuModList.Add(ModStore.ConvertToOsuMod(mod));
                Console.WriteLine(ModStore.ConvertToOsuMod(mod));
            }
            appliedMods = osuModList;
            
        }
    }
}
