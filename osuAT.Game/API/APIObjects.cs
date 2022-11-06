using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osuAT.Game.API
{
    public class OsuAPIBeatmap
    {
        public string Approved { get; set; }
        public string SubmitDate { get; set; }
        public string ApprovedDate { get; set; }
        public string LastUpdate { get; set; }
        public string Artist { get; set; }
        public string BeatmapId { get; set; }
        public string BeatmapsetId { get; set; }
        public string Bpm { get; set; }
        public string Creator { get; set; }
        public string CreatorId { get; set; }
        public string Difficultyrating { get; set; }
        public string DiffAim { get; set; }
        public string DiffSpeed { get; set; }
        public string DiffSize { get; set; }
        public string DiffOverall { get; set; }
        public string DiffApproach { get; set; }
        public string DiffDrain { get; set; }
        public string HitLength { get; set; }
        public string Source { get; set; }
        public string GenreId { get; set; }
        public string LanguageId { get; set; }
        public string Title { get; set; }
        public string TotalLength { get; set; }
        public string Version { get; set; }
        public string FileMd5 { get; set; }
        public string Mode { get; set; }
        public string Tags { get; set; }
        public string FavouriteCount { get; set; }
        public string Rating { get; set; }
        public string Playcount { get; set; }
        public string Passcount { get; set; }
        public string CountNormal { get; set; }
        public string CountSlider { get; set; }
        public string CountSpinner { get; set; }
        public string MaxCombo { get; set; }
        public string Storyboard { get; set; }
        public string Video { get; set; }
        public string DownloadUnavailable { get; set; }
        public string AudioUnavailable { get; set; }
    }
    public class OsuAPIScore
    {
        public string ScoreId { get; set; }
        public string Score { get; set; }
        public string Username { get; set; }
        public string Count300 { get; set; }
        public string Count100 { get; set; }
        public string Count50 { get; set; }
        public string Countmiss { get; set; }
        public string Maxcombo { get; set; }
        public string Countkatu { get; set; }
        public string Countgeki { get; set; }
        public string Perfect { get; set; }
        public string EnabledMods { get; set; }
        public string UserId { get; set; }
        public string Date { get; set; }
        public string Rank { get; set; }
    }

}
