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
using static osuAT.Game.Skills.Resources.SharedMethods;

namespace osuAT.Game.Skills
{
    public class AimStaminaSkill : ISkill
    {
        #region
        public string Name => "Aim Stamina";

        public string Identifier => "aimstamina";

        public string Version => "0.003";

        public string Summary => "The ability for your aim to endure \n continous strain.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#F7E65D");

        public Colour4 SecondaryColor => Colour4.FromHex("#F0A1B7");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize { get; }

        public float MiniHeight => 224;

        public int BoxNameSize => 63;

        public Vector2 BoxPosition => new Vector2(1875, 1080);

        #endregion

        public class AimCalculator : SkillCalcuator
        {
            public AimCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private double jerkDifficulty;
            private double aimDifficulty;
            private double aimStrainDifficutly;
            private double curAngle;

            private double curWorth;

            [HiddenDebugValue]
            private double highestWorth;

            private double jerkAngWorth;
            private double totalJerkStrainWorth;

            public override void CalcNext(OsuDifficultyHitObject diffHit)
            {
                if (diffHit.Angle == null) return;
                curAngle = (double)diffHit.Angle * (180 / Math.PI);

                // Aim Difficulty
                aimDifficulty = diffHit.MinimumJumpDistance / diffHit.DeltaTime / 4;

                // Aim Continous Strain
                aimStrainDifficutly += 0.6 * (aimDifficulty - aimStrainDifficutly);

                // Jerk Angle Difficulty
                jerkAngWorth = Math.Clamp((-1.5 * (curAngle - (double)Angle.Triangle) / (double)Angle.Line) + 0.5, 0, 1);
                totalJerkStrainWorth += jerkAngWorth;
                totalJerkStrainWorth = Math.Max(0, jerkAngWorth) * 0.995;
                jerkDifficulty = 30 * Math.Log(totalJerkStrainWorth + 1);

                curWorth = aimStrainDifficutly * jerkDifficulty * 15;
                highestWorth = Math.Max(highestWorth, curWorth);

                // Miss and combo scaling
                CurTotalPP = highestWorth;
                CurTotalPP *= MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SimpleAccNerf(FocusedScore.Accuracy);
            }
        }

        public Type SkillCalcType => typeof(AimCalculator);

        public SkillCalcuator GetSkillCalc(Score score) => new AimCalculator(score);
    }
}
