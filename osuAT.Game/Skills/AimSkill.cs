﻿using System;
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
    public class AimSkill : ISkill
    {
        #region
        public string Name => "Aim";

        public string Identifier => "overallaim";

        public string Version => "0.004";

        public string Summary => "The ability to move your cursor \n to any circle.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#F7E65D");

        public Colour4 SecondaryColor => Colour4.FromHex("#F0A1B7");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize { get; }

        public float MiniHeight => 264;

        public int BoxNameSize => 150;

        public Vector2 BoxPosition => new Vector2(2690, -500);
        #endregion

        public class AimCalculator : SkillCalcuator
        {
            public AimCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private double angDifficulty;
            private double aimDifficulty;
            private double curAngle;

            private double curAngStrainWorth;
            private double curWorth;

            [HiddenDebugValue]
            private double highestWorth;

            private double totalAngStrainWorth;

            public override void Setup()
            {
                angDifficulty = 0;
                aimDifficulty = 0;
                curAngle = 0;

                curAngStrainWorth = 0;
                curWorth = 0;

                highestWorth = 0;
                totalAngStrainWorth = 0;
            }

            public override void CalcNext(OsuDifficultyHitObject diffHit)
            {
                if (diffHit.Angle == null) return;
                curAngle = (double)diffHit.Angle * (180 / Math.PI);

                // Aim Difficulty
                aimDifficulty = (diffHit.MinimumJumpDistance / diffHit.DeltaTime) / 2;

                // Angle Difficulty
                curAngStrainWorth = Math.Clamp(aimDifficulty / 4, 0, 1) * Math.Clamp(-5 * (curAngle) / 90 + 5, -10, 1);
                totalAngStrainWorth += curAngStrainWorth;
                totalAngStrainWorth = Math.Max(0, totalAngStrainWorth);
                angDifficulty = 15 * Math.Log(totalAngStrainWorth + 1);

                curWorth = aimDifficulty * 2.5 + (aimDifficulty * angDifficulty) * 2.5;
                highestWorth = Math.Max(highestWorth, curWorth);

                // Miss and combo scaling
                CurTotalPP = highestWorth;
                CurTotalPP *= SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SharedMethods.SimpleAccNerf(FocusedScore.Accuracy);
            }
        }

        public Type SkillCalcType => typeof(AimCalculator);

        public SkillCalcuator GetSkillCalc(Score score) => new AimCalculator(score);
    }
}
