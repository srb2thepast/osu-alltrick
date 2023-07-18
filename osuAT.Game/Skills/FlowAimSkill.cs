using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game.Skills
{
    public class FlowAimSkill : ISkill
    {
        #region Info

        public string Name => "Flow Aim";

        public string Identifier => "flowaim";

        public string Version => "0.009";

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

        public Vector2 BoxPosition => new Vector2(953, 1300);

        #endregion Info

        /// <summary>
        /// Returns a pp value based off of the most spaced out stream of a map.
        /// </summary>
        /// <remarks> Current faults:
        /// ~ Difficulty Spike of Spacing is considered as a simple parabola.
        /// - Difficulty Spike of BPM is a un-tuned parabola
        /// - Difficulty of Stream Count is considered as a linear curve
        /// </remarks>
        public class FlowAimCalculator : SkillCalcuator
        {
            public FlowAimCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private double curStreamLength;
            private double curMSSpeed;

            [HiddenDebugValue]
            private double aimTotal;

            [HiddenDebugValue]
            private double totalAngStrainWorth;

            private double flowPatternMult;
            private double angCurDiff;
            private double aimCurDiff;

            private double lenMult;
            private double aimDifficulty;
            private double angDifficulty;
            private double curWorth;

            [HiddenDebugValue]
            private double highestWorth;

            public override void Setup()
            { }

            public override void CalcNext(OsuDifficultyHitObject diffHitObj)
            {
                var diffHit = diffHitObj;
                var lastDiffHit = (OsuDifficultyHitObject)diffHit.Previous(0);
                if (lastDiffHit == null) return;
                if (lastDiffHit.Angle == null) return;
                if (diffHit.Angle == null) return;
                double curAngle = (double)diffHit.Angle * (180 / Math.PI);
                double lastAngle = (double)lastDiffHit.Angle * (180 / Math.PI);

                // Strain-based Stream Length
                curStreamLength += Math.Clamp(SharedMethods.BPMToMS(180) / (diffHit.StartTime - lastDiffHit.StartTime), 0, 1);
                curStreamLength -= curStreamLength * 0.75 * (1 - Math.Clamp(SharedMethods.BPMToMS(180, 2) / (diffHit.StartTime - lastDiffHit.StartTime), 0, 1));
                curMSSpeed = diffHit.StartTime - lastDiffHit.StartTime;

                // Length multiplier
                lenMult = 1.3 * Math.Log((curStreamLength / 60) + 1);

                // Aim Difficulty
                aimCurDiff = 10 * (diffHit.LazyJumpDistance / diffHit.DeltaTime);
                aimTotal += aimCurDiff * flowPatternMult;
                aimTotal *= 0.5; // - 1 * (1-flowPatternMult);
                aimDifficulty = aimTotal;

                // Flow Pattern Multiplier
                flowPatternMult = Math.Clamp(((double)curAngle - 60) / 60, 0, 1) / 2 + Math.Clamp(((double)lastAngle - 60) / 60, 0, 1) / 2;

                // Angle Difficulty
                angCurDiff = 20 * (-Math.Clamp(1.5 * ((double)curAngle - 60) / 180, -1, 1) + 1);
                totalAngStrainWorth += angCurDiff * flowPatternMult;
                totalAngStrainWorth *= 0.9;
                totalAngStrainWorth = Math.Max(0, totalAngStrainWorth);

                angDifficulty = 1.5 * Math.Log(totalAngStrainWorth + 1);

                // Final value
                curWorth = 1.25 * lenMult * flowPatternMult * (10 * aimDifficulty + (aimDifficulty * angDifficulty));
                highestWorth = Math.Max(highestWorth, curWorth);

                CurTotalPP = highestWorth;
                CurTotalPP *= SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SharedMethods.SimpleAccNerf(FocusedScore.Accuracy);
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new FlowAimCalculator(score);
    }
}
