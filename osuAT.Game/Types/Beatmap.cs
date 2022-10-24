using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osuAT.Game.Types;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using OsuBeatmap = osu.Game.Beatmaps.Beatmap;


namespace osuAT.Game.Types
{
    public class Beatmap
    {

        /// <summary>
        /// This beatmap's ID.
        /// </summary>
        [JsonProperty("beatmapid")]
        public int MapID { get; set; }

        /// <summary>
        /// The ID of the beatmapset containing this beatmap.
        /// </summary>
        [JsonProperty("beatmapsetid")]
        public int MapsetID { get; set; }

        [JsonProperty("artist")]
        public string SongArtist { get; set; } // preferably unicode

        [JsonProperty("songname")]
        public string SongName { get; set; }

        [JsonProperty("creator")]
        public string MapsetCreator { get; set; }

        [JsonProperty("diffname")]
        public string DifficultyName { get; set; }

        [JsonProperty("star_rating")]
        public double StarRating { get; set; }

        [JsonProperty("maxcombo")]
        public int MaxCombo { get; set; }

        [JsonProperty("beatmap_path")]
        public string FolderName { get; set; } = default!; // very likely to be null

        // [!] Add MD5 Hash of current beatmap to allow comparison against online's MD5 to prevent people from editing the beatmap's file.
        [JsonProperty("file_md5")]
        public string OnlineMD5Hash { get; set; } = default!; // depends on File.(osuDir + FolderName) existing

        // The three below should not be set while constructing.

        [JsonIgnore]
        public BeatmapDifficulty DifficultyInfo { get; set; } // also also very likely to be null (also depends on foldername)

        [JsonIgnore]
        public List<HitObject> HitObjects { get; set; } // also very likely to be null (depends on foldername)

        [JsonIgnore]
        public List<DifficultyHitObject> DiffHitObjects { get; set; } // also very likely to be null (depends on foldername)

        [JsonIgnore]
        public RulesetInfo ContentRuleset {get; set;}

        public static explicit operator Beatmap(OsuBeatmap map) {

            // [!] Add DiffHitObjects here
            Beatmap newmap = new Beatmap()
            {
                MapID = map.BeatmapInfo.OnlineID,
                MapsetID = map.BeatmapInfo.BeatmapSet.OnlineID,
                SongArtist = map.BeatmapInfo.Metadata.ArtistUnicode,
                SongName = map.BeatmapInfo.Metadata.TitleUnicode,
                MapsetCreator = map.BeatmapInfo.BeatmapSet.Metadata.Author.Username,
                DifficultyName = map.BeatmapInfo.Metadata.TitleUnicode,
                StarRating = 0,
                MaxCombo = BeatmapExtensions.GetMaxCombo(map),
                FolderName = map.BeatmapInfo.File.Filename,
                OnlineMD5Hash = map.BeatmapInfo.OnlineMD5Hash,
                HitObjects = map.HitObjects
            };
            return newmap;
        }

        public Beatmap() {

        }

        /// <summary>
        /// Sets the HitObjects, DifficultyInfo, and ContentRuleset parameters, basically anything
        /// related to the acutal objects IN the beatmap rather than just metadata.
        /// </summary>
        public void LoadMapContents(RulesetInfo ruleset, List<ModInfo> mods = null)
        {
            mods ??= new List<ModInfo>();
            if (FolderName == default || FolderName == "deleted")
            {
                HitObjects = new List<HitObject> { };
                DiffHitObjects = new List<DifficultyHitObject> { };
                return;
            }
            Console.WriteLine("hi");

            string path = SaveStorage.SaveData.OsuPath + @"\" + FolderName;
            Console.WriteLine(path);

            if (!(File.Exists(path)))
            {
                throw new ArgumentNullException($"The path of this beatmap does not exist!!! : {path}");
            }
                 
            // [!] Create ability to get the hitobjects from DifficultyCalculator.CreateHitobjects()

            var Workmap = ProcessorWorkingBeatmap.FromFileOrId(path);
            var rulesetInstance = RulesetStore.ConvertToOsuRuleset(ruleset);
            var diffcalc = RulesetStore.GetDiffCalcObj(ruleset, Workmap);
            List<Mod> osuModList = new List<Mod>();
            foreach (ModInfo mod in mods) {
                osuModList.Add(ModStore.ConvertToOsuMod(mod));
                Console.WriteLine(ModStore.ConvertToOsuMod(mod));
            }
            var PlayableMap = Workmap.GetPlayableBeatmap(rulesetInstance.RulesetInfo, osuModList);
            var PlayableMapNomod = Workmap.GetPlayableBeatmap(rulesetInstance.RulesetInfo, null);

            var diffhitobjs = diffcalc.GetDiffHitobjects(PlayableMap);
            HitObjects = PlayableMap.HitObjects.ToList();
            DiffHitObjects = diffhitobjs.ToList();
            DifficultyInfo = PlayableMap.Difficulty;
            Console.WriteLine(osuModList.Count);
            Console.WriteLine(PlayableMap.Difficulty.CircleSize);
            ContentRuleset = ruleset;

        }

        // note to self: make the two methods below

        /// <summary>
        /// Returns a list of what each hitobject from this.HitObjects would be modified when the mod given is applied.
        /// </summary>
        /// <remarks>ex: adding hardrock would flip each hitcircle, halftime would make them more spaced out, etc.</remarks>
        /// <returns></returns>
        public List<HitObject> GetModdedHitobjects() {
            return new List<HitObject>();
        }

        /// <summary>
        /// Returns a list of what this.DifficultyInfo would be modified when the mod given is applied.
        /// </summary>
        /// <remarks>ex: Adding doubletime increases the AR, adding easy decreases the CS.</remarks>
        /// <returns></returns>
        public BeatmapDifficulty GetModdedDiffInfo() {
            return new BeatmapDifficulty();
        }

    }
}
