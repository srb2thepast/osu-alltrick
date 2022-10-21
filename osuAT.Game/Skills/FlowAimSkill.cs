using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types.BeatmapParsers;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;


namespace osuAT.Game.Skills
{
    public class FlowAimSkill : ISkill
    {

        public string Name => "Flow Aim";

        public string Identifier => "flowaim";

        public string Version => "0.002";

        public string Summary => "The ability to move your cursor \n in a fluid motion.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#99FF69");

        public Colour4 SecondaryColor => Colour4.FromHex("#00FFF0");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize => (new Vector2(-20, 60), new Vector2(700, 440));

        public float MiniHeight => 224;

        public int BoxNameSize => 83;

        public Vector2 BoxPosition => new Vector2(00, 400);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500,3000, 6000, 9000, 10000);

        public RulesetInfo[] SupportedRulesets => new RulesetInfo[]{ RulesetStore.Osu };

        /// <summary>
        /// Returns a pp value based off of the most spaced out stream of a map.
        /// </summary>
        /// <remarks> Current faults:
        /// - Hard-capping what is considered a stream based off of ms
        /// - Strain is not considered
        /// - Angle is not considered
        /// - Only considers one stream rather than the map as a whole
        /// ~ Difficulty Spike of Spacing is considered as a simple parabola.
        /// - Difficulty Spike of CS is considered as a linear curve
        /// - Difficulty Spike of BPM is considered as a linear curve
        /// - Difficulty of Stream Count is considered as a linear curve
        /// - No combo diff-spike
        /// </remarks>
        public double SkillCalc(Score score)
        {
            if (!SupportedRulesets.Contains(score.ScoreRuleset)) return -1;
            if (score.BeatmapInfo.FolderName == default) return -2;
            if (score.BeatmapInfo.HitObjects == default) return -3;

            double speedMult = 1;
            if (score.Mods.Contains(ModStore.Doubletime) || score.Mods.Contains(ModStore.Nightcore))
            {
                speedMult = 1.5;
            }
            if (score.Mods.Contains(ModStore.Halftime))
            {
                speedMult = 0.75;
            }

            float csMult = score.BeatmapInfo.DifficultyInfo.CircleSize / 4;

            double focusedHighestPP = 0;

            float curSpacingSum = 0;
            int curlength = 0;
            double curTimediffSum = 0;

            float focusedAvgSpacing = 0; // the most streched out one is calculated
            int focusedLength = 0;
            double focusedAvgTimediff = 0; // the average difference between the starttime of each object

            int totalCount = score.BeatmapInfo.HitObjects.Count;

            for (int i = 0; i < score.BeatmapInfo.HitObjects.Count; i++) {
                HitObject HitObj = score.BeatmapInfo.HitObjects[i];
                HitObject HitObj_prev0 = (i >= 1)? score.BeatmapInfo.HitObjects[i - 1]: null;

                // if this circle appears within 150ms of the last one, it (might be) a circle in a stream!
                // So it only runs if it's a circle and it appears within 150ms of the previous circle in the loop.
                // And also if it's not the first circle of the map (because there would be no previous circle).
                if (!(HitObj is OsuParser.Spinner) && HitObj_prev0 != null && (HitObj.StartTime - HitObj_prev0.StartTime) * speedMult < 150)
                {
                    curlength++;
                    curTimediffSum += HitObj.StartTime - HitObj_prev0.StartTime;
                    curSpacingSum += Math.Abs((HitObj.Position - HitObj_prev0.Position).Length);
                }

                // Otherwise, it's considered the end of a stream.
                else {
                    if (curlength == 0) continue;
                    float curAvgSpacing = 0;
                    curAvgSpacing = curSpacingSum / curlength;

                    // THIS LINE is where calculations are done.
                    // The reason it's in the loop is so that the stream that outputs the most PP is the one
                    // that's returned.
                    // Preferably (and by standard), the total pp should be calculated outside the loop.
                    double curHighestPP =
                        ((
                        (Math.Pow(curAvgSpacing, 2.2) * csMult * // spacing and circle size (exponentional)
                        Math.Log((curlength * 4) + 1, 10)))/40) * // length of season (logarithmic)
                        ((double)score.Combo/score.BeatmapInfo.MaxCombo * // Combo Multiplier (linear)
                        SharedMethods.MissPenalty(score.AccuracyStats.CountMiss,score.BeatmapInfo.MaxCombo) // Miss Multiplier
                        );

                    if (curHighestPP >= focusedHighestPP)
                    {
                        Console.WriteLine(curlength);
                        Console.WriteLine(curAvgSpacing);
                        focusedAvgSpacing = curAvgSpacing;
                        focusedLength = curlength;
                        focusedAvgTimediff = curTimediffSum/curlength;
                        focusedHighestPP = curHighestPP;
                    }
                    curSpacingSum = 0;
                    curTimediffSum = 0;
                    curlength = 0;
                }
            }
            Console.WriteLine(score.BeatmapInfo.SongName + " " + focusedLength.ToString() + " " + focusedAvgSpacing.ToString());
            Console.WriteLine(((double)score.Combo / score.BeatmapInfo.MaxCombo));
            return (int)focusedHighestPP;
        }
    }
}
