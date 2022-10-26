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
        public string FolderLocation { get; set; } = default!; // very likely to be null

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
                FolderLocation = map.BeatmapInfo.File.Filename,
                OnlineMD5Hash = map.BeatmapInfo.OnlineMD5Hash,
                HitObjects = map.HitObjects
            };
            return newmap;
        }

        public Beatmap() {

        }
        public string GetLocalBackgroundFile(LargeTextureStore textures)
        {
            if (FolderLocation == default || FolderLocation == "deleted")
            {
                Console.WriteLine("No folder provided.");
                return null;
            }
            using (var stream = File.OpenRead(SaveStorage.SaveData.OsuPath + @"\" + FolderLocation))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("[") && trimmed == "[Events]")
                    {
                        line = reader.ReadLine();
                        if (line.Trim() == "//Background and Video events")
                        {
                            line = reader.ReadLine();
                            List<string> FolderSplit = FolderLocation.Split("\\").ToList();
                            FolderSplit.RemoveAt(FolderSplit.Count-1);
                            return String.Join(@"\",FolderSplit) + @"\" + line.Split(",")[2].Trim('"').ToStandardisedPath();
                        }
                        Console.WriteLine("Background section not found.");
                        return null;
                    }
                }
            }
            Console.WriteLine("No sections found! Maybe the beatmap file was empty?");
            return null;
        }

        public Texture GetLocalBackground(LargeTextureStore textures)
        {
            return textures.Get(GetLocalBackgroundFile(textures));
        }

        public Texture GetOnlineCover(LargeTextureStore textures,bool Scale2x = false)
        {
            string fallback = "";
            string scale = Scale2x ? "@2x" : "";
            if (OsuApi.IsKeyValid())
            {
                return textures.Get($"assets.ppy.sh/beatmaps/{MapsetID}/covers/cover{scale}.jpg") ?? textures.Get(fallback);
            }
            throw new InvalidOperationException("Cannot get beatmap cover without osu!api.");
        }

        /// <summary>
        /// Sets the HitObjects, DifficultyInfo, and ContentRuleset parameters, basically anything
        /// related to the acutal objects IN the beatmap rather than just metadata.
        /// </summary>
        public ProcessorWorkingBeatmap LoadMapContents(RulesetInfo ruleset, List<ModInfo> mods = null,AudioManager audio = null)
        {
            mods ??= new List<ModInfo>();
            if (FolderLocation == default || FolderLocation == "deleted")
            {
                HitObjects = new List<HitObject> { };
                DiffHitObjects = new List<DifficultyHitObject> { };
                return null;
            }
            Console.WriteLine("hi");

            string path = SaveStorage.SaveData.OsuPath + @"\" + FolderLocation;
            Console.WriteLine(path);

            if (!(File.Exists(path)))
            {
                throw new ArgumentNullException($"The path of this beatmap does not exist!!! : {path}");
            }

            // [!] Create ability to get the hitobjects from DifficultyCalculator.CreateHitobjects()

            var Workmap = ProcessorWorkingBeatmap.FromFileOrId(path,audio);
            var rulesetInstance = RulesetStore.ConvertToOsuRuleset(ruleset);
            var diffcalc = RulesetStore.GetDiffCalcObj(ruleset, Workmap);
            List<Mod> osuModList = new List<Mod>();
            foreach (ModInfo mod in mods) {
                Console.WriteLine(mod);
                osuModList.Add(ModStore.ConvertToOsuMod(mod));
                Console.WriteLine(ModStore.ConvertToOsuMod(mod));
            }

            var PlayableMap = Workmap.GetPlayableBeatmap(rulesetInstance.RulesetInfo, osuModList);
            var diffhitobjs = diffcalc.GetDifficultyHitObjects(PlayableMap, 1.0);
            HitObjects = PlayableMap.HitObjects.ToList();
            DiffHitObjects = diffhitobjs.ToList();
            DifficultyInfo = PlayableMap.Difficulty;
            Console.WriteLine(osuModList.Count);
            Console.WriteLine(PlayableMap.Difficulty.CircleSize);
            ContentRuleset = ruleset;
            return Workmap;
        }
    }
}
