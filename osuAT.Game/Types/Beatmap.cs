using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using osuAT.Game.Types.BeatmapParsers;
using Newtonsoft.Json;

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

        // The three below should not be set while constructing.

        [JsonIgnore]
        public BeatmapDifficultyInfo DifficultyInfo { get; set; } // also also very likely to be null (also depends on foldername)

        [JsonIgnore]
        public List<HitObject> HitObjects { get; set; } // also very likely to be null (depends on foldername)
        
        [JsonIgnore]
        public RulesetInfo ContentRuleset {get; set;}
        
        /// <summary>
        /// Sets the HitObjects, DifficultyInfo, and ContentRuleset parameters, basically anything
        /// related to the acutal objects IN the beatmap rather than just metadata.
        /// </summary>
        public void LoadMapContents(RulesetInfo ruleset) {
            // note to self: convert BeatmapFileParser from several seperate sections into a 
            // main method that returns each of data from the requested section
            // so that the programming isnt looping through the whole beatmap twice [DONE]
            if (FolderName == default || FolderName == "deleted") {
                HitObjects = new List<HitObject> { }; 
            }

            string path = SaveStorage.SaveData.OsuPath + @"\" + FolderName;
            if (!File.Exists(path))
            {
                throw new ArgumentNullException($"The path of this beatmap does not exist!!! : {path}");
            }
            BeatmapFileParser.ParseOsuFile(
                path, 
                this, 
                new List<Section> {
                    Section.HitObjects,
                    Section.Difficulty
                },
                ruleset
            );
            ContentRuleset = ruleset;

        }

    }
}
