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
        public List<HitObject> HitObjects { get; set; } // also very likely to be null
    }
}
