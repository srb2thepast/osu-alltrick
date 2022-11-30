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
using osuAT.Game.Skills.Resources;
using osu.Game.Graphics.UserInterfaceV2;

namespace osuAT.Game.Skills
{

    public class FlowAimSkill : ISkill
    {
        #region Info
        public string Name => "Flow Aim";

        public string Identifier => "flowaim";

        public string Version => "0.008";

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

        public SkillGoals Benchmarks => new SkillGoals(600, 1500,3000, 6000, 9000, 10000);

        #endregion

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


            private double curStreamLength = 0;
            private double curMSSpeed = 0;
            [HiddenDebugValue]
            private double aimTotal = 0;


            [HiddenDebugValue]
            private double totalAngStrainWorth = 0;

            private double flowPatternMult = 0;
            private double angCurDiff = 0;
            private double aimCurDiff = 0;

            private double lenMult = 0;
            private double aimDifficulty = 0;
            private double angDifficulty = 0;
            private double curWorth = 0;


            public override void Setup()
            {

            }

            public override void CalcNext(OsuDifficultyHitObject diffHitObj)
            {
                var diffHit = (OsuDifficultyHitObject)diffHitObj;
                var lastDiffHit = (OsuDifficultyHitObject)diffHit.Previous(0);
                if (lastDiffHit == null) return;
                if (lastDiffHit.Angle == null) return;
                if (diffHit.Angle == null) return;
                double curAngle = (double)diffHit.Angle * (180/Math.PI);
                double lastAngle = (double)lastDiffHit.Angle * (180 / Math.PI);

                // Strain-based Stream Length
                curStreamLength += Math.Clamp(SharedMethods.BPMToMS(180) / (diffHit.StartTime - lastDiffHit.StartTime), 0, 1);
                curStreamLength -= curStreamLength * 0.75 * (1 - Math.Clamp(SharedMethods.BPMToMS(180, 2) / (diffHit.StartTime - lastDiffHit.StartTime), 0, 1));
                curMSSpeed = diffHit.StartTime - lastDiffHit.StartTime;

                // Length multiplier
                lenMult = 1.3 * Math.Log((curStreamLength / 60) + 1);

                // Aim Difficulty
                aimCurDiff = 2*(diffHit.LazyJumpDistance / diffHit.DeltaTime);
                aimTotal += aimCurDiff * flowPatternMult;
                aimTotal *= 0.9;
                aimDifficulty = aimTotal;

                // Flow Pattern Multiplier
                flowPatternMult = Math.Clamp(((double)curAngle - 60) / 75, 0, 1)/2 + Math.Clamp(((double)lastAngle - 60) / 75, 0, 1) / 2;

                // Angle Difficulty 
                angCurDiff = 20*(-Math.Clamp(1.5 * ((double)curAngle-60) / 180, -1, 1) + 1);
                totalAngStrainWorth += angCurDiff * flowPatternMult;
                totalAngStrainWorth *= 0.9;
                totalAngStrainWorth = Math.Max(0,totalAngStrainWorth);

                angDifficulty = 1.5 *Math.Log(totalAngStrainWorth+1);

                // Final value
                curWorth = 1.25*lenMult * flowPatternMult * (10*aimDifficulty + (aimDifficulty * angDifficulty));

                CurTotalPP = Math.Max(CurTotalPP,curWorth);
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new FlowAimCalculator(score);
    }
}
