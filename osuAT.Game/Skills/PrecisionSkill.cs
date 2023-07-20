using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game.Skills
{
    public class PrecisionSkill : ISkill
    {
        #region
        public string Name => "Precision";

        public string Identifier => "precision";

        public string Version => "0.002";

        public string Summary => "The ability to accurately aim\n at the center of circles.";

        public int SummarySize => 8;

        public Colour4 PrimaryColor => Colour4.FromHex("#F7E65D");

        public Colour4 SecondaryColor => Colour4.FromHex("#F0A1B7");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize { get; }

        public float MiniHeight => 224;

        public int BoxNameSize => 83;

        public Vector2 BoxPosition => new Vector2(1875, 400);
        #endregion

        public class PrecisionCalculator : SkillCalcuator
        {
            public PrecisionCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private double circleSizeWeight;

            private double curAimDiff;
            private double aimDifficulty;

            private double approachRateDifficulty;

            private double curWorth;

            [HiddenDebugValue]
            private double highestWorth;

            public override void Setup()
            {
                aimDifficulty = 0;
                curWorth = 0;
                highestWorth = 0;
            }

            public override void CalcNext(OsuDifficultyHitObject diffHit)
            {
                // Circle Size Weight
                double circleSize = FocusedScore.BeatmapInfo.Contents.DifficultyInfo.CircleSize;
                circleSizeWeight = 30 * Math.Pow(1.3, (2 * circleSize) - 12);

                // Aim Difficulty
                curAimDiff = 10 * (diffHit.MinimumJumpDistance / diffHit.DeltaTime) * Math.Pow(0.9, 0.1 * diffHit.DeltaTime);
                aimDifficulty += 0.2 * (curAimDiff - aimDifficulty);

                // Approach Rate Difficulty
                double approachMs = SharedMethods.ARToMS(FocusedScore.BeatmapInfo.Contents.DifficultyInfo.ApproachRate);
                approachRateDifficulty = 2 * Math.Pow(500, ((-approachMs + 1800) / 750) - 1);

                curWorth = circleSizeWeight * aimDifficulty + circleSizeWeight * approachRateDifficulty;

                highestWorth = Math.Max(highestWorth, curWorth);

                // Miss and combo scaling
                CurTotalPP = highestWorth;
                CurTotalPP *= SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SharedMethods.SimpleAccNerf(FocusedScore.Accuracy);
            }
        }

        public static Type SkillCalcType => typeof(PrecisionCalculator);

        public SkillCalcuator GetSkillCalc(Score score) => new PrecisionCalculator(score);
    }
}
