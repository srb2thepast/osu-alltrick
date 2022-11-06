using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using ATBeatmap = osuAT.Game.Types.Beatmap;
namespace osuAT.Game.Types
{
    /// <summary>
    /// Edited version of osu.Game.BeatmapExitensions
    /// </summary>
    public static class ATBeatmapExtensions
    {
        public static int GetMaxCombo(this List<DifficultyHitObject> hitObjs)
        {
            int combo2 = 0;
            foreach (DifficultyHitObject hitObject in hitObjs)
            {
                addCombo(hitObject.BaseObject, ref combo2);
            }

            return combo2;

        }

        public static int GetMaxCombo(this ATBeatmap newmap)
        {
            int combo2 = 0;
            foreach (DifficultyHitObject hitObject in newmap.Contents.DiffHitObjects)
            {
                addCombo(hitObject.BaseObject, ref combo2);
            }

            return combo2;
        }

        [Obsolete]
        public static ATBeatmap ConvertToATMap(this WorkingBeatmap map,string folderlocation = "",List<ModInfo> mods = null)
        {
            ATBeatmap newmap = new ATBeatmap()
            {
                MapID = map.BeatmapInfo.OnlineID,
                MapsetID = map.BeatmapInfo.BeatmapSet.OnlineID,
                SongArtist = map.BeatmapInfo.Metadata.ArtistUnicode,
                SongName = map.BeatmapInfo.Metadata.TitleUnicode,
                MapsetCreator = map.BeatmapInfo.BeatmapSet.Metadata.Author.Username,
                DifficultyName = map.BeatmapInfo.Metadata.TitleUnicode,
                StarRating = 1,
                FolderLocation = folderlocation,
            };
            newmap.LoadMapContents(RulesetStore.Osu,mods);
            newmap.MaxCombo = newmap.GetMaxCombo();

            return newmap;
        }

        private static void addCombo(HitObject hitObject, ref int combo)
        {
            if (hitObject.CreateJudgement().MaxResult.AffectsCombo())
            {
                combo++;
            }

            foreach (HitObject nestedHitObject in hitObject.NestedHitObjects)
            {
                addCombo(nestedHitObject, ref combo);
            }
        }
    }
}
