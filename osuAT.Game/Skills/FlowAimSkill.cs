using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osuAT.Game.Types;

using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Difficulty;
using osuAT.Game.Objects;
using osuTK;


namespace osuAT.Game.Skills
{
    
    public class FlowAimSkill : ISkill
    {

        public string Name => "Flow Aim";

        public string Identifier => "flowaim";

        public string Version => "0.003";

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
        /// - Difficulty Spike of BPM is a un-tuned parabola
        /// - Difficulty of Stream Count is considered as a linear curve
        /// - No combo diff-spike
        /// - Jumps can be considered streams if they're fast enough
        /// </remarks>
        public double SkillCalc(Score score)
        {
            if (!SupportedRulesets.Contains(score.ScoreRuleset)) return -1;
            if (score.BeatmapInfo.FolderName == default) return -2;
            if (score.BeatmapInfo.HitObjects == default) return -3;


            float csMult = score.BeatmapInfo.DifficultyInfo.CircleSize / 4;

            double focusedHighestPP = 0;

            float curSpacingSum = 0;
            int curlength = 0;
            double curTimediffSum = 0;

            float focusedAvgSpacing = 0; // the most streched out one is calculated
            int focusedLength = 0;
            double focusedAvgTimediff = 0; // the average difference between the starttime of each object

            for (int i = 0; i < score.BeatmapInfo.DiffHitObjects.Count; i++) {
                // [!] add generic support based off of a mode's general hitobject class
                var DiffHitObj = score.BeatmapInfo.DiffHitObjects[i];
                var HitObj = (OsuHitObject)DiffHitObj.BaseObject;
                var LastHitObj = (OsuHitObject)DiffHitObj.LastObject;


                // if this circle appears within 150ms of the last one, it (might be) a circle in a stream!
                // So it only runs if it's a circle and it appears within 150ms of the previous circle in the loop.
                // And also if it's not the first circle of the map (because there would be no previous circle).
                if ((HitObj is HitCircle) && (DiffHitObj.StartTime - DiffHitObj.LastObject.StartTime) < 150)
                {
                    curlength++;
                    curTimediffSum += DiffHitObj.StartTime - DiffHitObj.LastObject.StartTime;
                    curSpacingSum += Math.Abs((HitObj.Position - LastHitObj.Position).Length);
                }

                // Otherwise, it's considered the end of a stream.
                else {
                    if (curlength == 0) continue;
                    float curAvgSpacing = curSpacingSum / curlength;
                    double curAvgTimediff = curTimediffSum / curlength;

                    // THIS LINE is where calculations are done.
                    // The reason it's in the loop is so that the stream that outputs the most PP is the one
                    // that's returned.
                    // Preferably (and by standard), the total pp should be calculated outside the loop.
                    double curHighestPP =
                        Math.Pow(
                        (Math.Pow((curAvgSpacing / 1.3 * Math.Log(curlength) / 3), 2.2) * csMult * // spacing and circle size (exponentional + shorter streams decrease this mult)
                        Math.Log((curlength ) + 1, 10)) / 40 * // length of stream (logarithmic)
                        ((double)score.Combo / score.BeatmapInfo.MaxCombo * // Combo Multiplier (linear)
                        SharedMethods.MissPenalty(score.AccuracyStats.CountMiss, score.BeatmapInfo.MaxCombo) // Miss Multiplier
                        ),
                        (84/curAvgTimediff)); // BPM buff math.pow

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
            Console.WriteLine(score.BeatmapInfo.SongName + " LEN: " + focusedLength.ToString() + " ATD: " + focusedAvgTimediff.ToString() + " ASP: " + focusedAvgSpacing.ToString());
            Console.WriteLine(((double)score.Combo / score.BeatmapInfo.MaxCombo));
            return (int)focusedHighestPP;
        }
    }
}
