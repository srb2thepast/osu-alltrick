using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osuAT.Game.Types;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using static osuAT.Game.Skills.AimSkill;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;

namespace osuAT.Game.Skills
{
    
    public class FlowAimSkill : ISkill
    {
        #region Info
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

        #endregion

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
        public class FlowAimCalculator : SkillCalcuator
        {
            public FlowAimCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private double lastObjectTimeDiff = 0;

            private float curAvgSpacing = 0;
            [HiddenDebugValue]
            private float curSpacingSum = 0;
            private double curTimeDiff = 0;
            private int curLength = 0;
            [HiddenDebugValue]
            private double curTimediffSum = 0;

            private float focusedAvgSpacing = 0; // the most streched out one is calculated
            private int focusedLength = 0;
            private double focusedAvgTimediff = 0; // the average difference between the starttime of each object
            [HiddenDebugValue]
            private float csMult;

            public override void Setup()
            {
                csMult = FocusedScore.BeatmapInfo.Contents.DifficultyInfo.CircleSize / 4;
            }

            public override void CalcNext(DifficultyHitObject diffHitObj)
            {
                if (diffHitObj is OsuDifficultyHitObject DiffHitObj)
                {
                    var HitObj = (OsuHitObject)DiffHitObj.BaseObject;
                    var LastHitObj = (OsuHitObject)DiffHitObj.LastObject;
                    lastObjectTimeDiff = DiffHitObj.StartTime - DiffHitObj.LastObject.StartTime;


                    // if this circle appears within 150ms of the last one, it (might be) a circle in a stream!
                    // So it only runs if it's a circle and it appears within 100ms of the previous circle in the loop.
                    // And also if it's not the first circle of the map (because there would be no previous circle).
                    if (diffHitObj.Index < FocusedScore.BeatmapInfo.Contents.DiffHitObjects.Count - 1 && HitObj is HitCircle && (DiffHitObj.StartTime - DiffHitObj.LastObject.StartTime) < 100)
                    {
                        curLength++;
                        curTimediffSum += DiffHitObj.StartTime - DiffHitObj.LastObject.StartTime;
                        curSpacingSum += Math.Abs((HitObj.Position - LastHitObj.Position).Length);
                        curAvgSpacing = curSpacingSum / curLength;
                        curTimeDiff = curTimediffSum / curLength;
                    }
                    // Otherwise, it's considered the end of a stream.
                    else
                    {
                        if (curLength == 0) return;
                        curAvgSpacing = curSpacingSum / curLength;
                        double curAvgTimediff = curTimediffSum / curLength;

                        // THIS LINE is where calculations are done.
                        // The reason it's in the loop is so that the stream that outputs the most PP is the one
                        // that's returned.
                        // Preferably (and by standard), the total pp should be calculated outside the loop.
                        double curHighestPP =
                            Math.Pow(
                            (Math.Pow((curAvgSpacing / 1.3 * Math.Log(curLength) / 3), 2.2) * csMult * // spacing and circle size (exponentional + shorter streams decrease this mult)
                            Math.Log((curLength) + 1, 10)) / 40 * // length of stream (logarithmic)
                        ((double)FocusedScore.Combo / FocusedScore.BeatmapInfo.MaxCombo * // Combo Multiplier (linear)
                        SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo) // Miss Multiplier
                        ),
                            (84 / curAvgTimediff)); // BPM buff math.pow

                        if (curHighestPP >= CurTotalPP)
                        {
                            focusedAvgSpacing = curAvgSpacing;
                            focusedLength = curLength;
                            focusedAvgTimediff = curTimediffSum / curLength;
                            CurTotalPP = curHighestPP;
                        }
                        curAvgSpacing = 0;
                        curSpacingSum = 0;
                        curTimediffSum = 0;
                        curLength = 0;
                        curTimeDiff = 0;
                    }
                }
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new FlowAimCalculator(score);
    }
}
