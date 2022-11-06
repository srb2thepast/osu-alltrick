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
using osu.Framework.Graphics.Primitives;

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

        [JsonProperty("file_md5")]
        public string OnlineMD5Hash { get; set; } = default!;

        [JsonIgnore]
        private BeatmapContents contents;

        [JsonIgnore]
        public BeatmapContents Contents
        {
            get
            {
                if (contents == null)
                {
                    throw new NotSupportedException("You must call LoadMapContents before tryig to access.");
                }
                return contents;
            }
            set => contents = value;
        }

        /// <summary>
        /// Sets the HitObjects, DifficultyInfo, and ContentRuleset parameters, basically anything
        /// related to the acutal objects IN the beatmap rather than just metadata.
        /// </summary>
        public void LoadMapContents(RulesetInfo ruleset, List<ModInfo> mods = null)
        {
            contents = new BeatmapContents(FolderLocation,ruleset,mods);
        }

        public Beatmap()
        {

        }

       

        public string GetLocalBackgroundFile(LargeTextureStore textures)
        {
            if (FolderLocation == default || FolderLocation == "deleted")
            {
                Console.WriteLine("No folder provided.");
                return null;
            }
            using (var stream = File.OpenRead(SaveStorage.ConcateOsuPath(FolderLocation)))
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
                            return string.Join(@"\",FolderSplit) + @"\" + line.Split(",")[2].Trim('"').ToStandardisedPath();
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
            return textures.Get("TestBG");
            // return textures.Get(SaveStorage.ConcateOsuPath(GetLocalBackgroundFile(textures)));
        }
        
    }
}
