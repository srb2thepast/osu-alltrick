using System;
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

        [JsonIgnore]
        public List<HitObject> HitObjects // also very likely to be null (depends on foldername)

        [JsonIgnore]
        public BeatmapDifficultyInfo DifficultyInfo // also also very likely to be null (also depends on foldername)

        [JsonIgnore]
        public RUlesetInfo ContentRuleset {get; set;}
        
        /// <summary>
        /// Sets the HitObjects parameter and DifficultyInfo parameters.
        /// </summary>
        public void ReloadContents(RulesetInfo ruleset) {
            // note to self: convert BeatmapFileParser from several seperate sections into a 
            // main method that returns each of data from the requested section
            // so that the programming isnt looping through the whole beatmap twice
            HitObjects = BeatmapFileParser.ParseOsuFileHitObjects(SaveStorage.SaveData.OsuPath + @"\" + BeatmapInfo.FolderName,ruleset)
            DifficultyInfo = BeatmapFileParser.ParseOsuFileDifficulty(SaveStorage.SaveData.OsuPath + @"\" + BeatmapInfo.FolderName)
            

        }

    }
}
