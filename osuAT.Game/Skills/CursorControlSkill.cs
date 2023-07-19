using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;
using static osuAT.Game.Skills.Resources.SharedMethods;

namespace osuAT.Game.Skills
{
    public class CursorControlSkill : ISkill
    {
        #region
        public string Name => "Cursor Control";

        public string Identifier => "cursorcontrol";

        public string Version => "0.006";

        public string Summary => "Ability to control the movement \n of your aim.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#F7E65D");

        public Colour4 SecondaryColor => Colour4.FromHex("#F0A1B7");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize { get; }

        public float MiniHeight => 224;

        public int BoxNameSize => 63;

        public Vector2 BoxPosition => new(1875, -500);

        #endregion

        public class CursorControlCalculator : SkillCalcuator
        {
            public CursorControlCalculator(Score score) : base(score)
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

            private double flowPatternMult;

            // Uneven Flow
            private double soloFlowPatternMult;

            private double flowPatternCount;
            private double aubruptionWorth;

            private double totalAngStrainWorth;

            public override void Setup()
            {
                curAngle = 0;
                aimDifficulty = 0;
            }

            public override void CalcNext(OsuDifficultyHitObject diffHit)
            {
                var lastDiffHit = (OsuDifficultyHitObject)diffHit.Previous(0);
                if (lastDiffHit == null) return;
                if (lastDiffHit.Angle == null) return;
                if (diffHit.Angle == null) return;
                curAngle = (double)diffHit.Angle * (180 / Math.PI);
                double lastAngle = (double)lastDiffHit.Angle * (180 / Math.PI);

                // Flow Angle Difficulty
                flowPatternMult = Math.Clamp((curAngle - (int)Angle.Triangle) / (int)Angle.Triangle, 0, 1) / 2 + Math.Clamp(((double)lastAngle - (int)Angle.Triangle) / (int)Angle.Triangle, 0, 1) / 2;

                // Angle Difficulty
                curAngStrainWorth = -Math.Clamp(curAngle / ((int)Angle.Octagon / 0.9), 0, 1) + 0.9;
                totalAngStrainWorth += curAngStrainWorth;
                totalAngStrainWorth = Math.Max(0, totalAngStrainWorth);
                angDifficulty = 10 * Math.Log(totalAngStrainWorth + 1);
                aimDifficulty = diffHit.MinimumJumpDistance / diffHit.DeltaTime / 2;

                // // Uneven Flow
                soloFlowPatternMult = Math.Clamp((curAngle - (int)Angle.Triangle) / (int)Angle.Triangle, 0, 1) / 2 + Math.Clamp(((double)lastAngle - (int)Angle.Triangle) / (int)Angle.Triangle, 0, 1) / 2;
                flowPatternCount = Math.Max(0, flowPatternCount + (soloFlowPatternMult * 10 - 9) * 3);

                if (flowPatternCount >= 1)
                    aubruptionWorth = Math.Pow(Math.E, -(Math.Pow(flowPatternCount - 3, 2) / (4 * flowPatternCount)));
                else
                    aubruptionWorth = 0;

                curWorth = angDifficulty * aimDifficulty + (angDifficulty * aimDifficulty * (aubruptionWorth + 1));
                highestWorth = Math.Max(highestWorth, curWorth);

                // Miss and combo scaling
                CurTotalPP = highestWorth;
                CurTotalPP *= MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SimpleAccNerf(FocusedScore.Accuracy);
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new CursorControlCalculator(score);
    }
}
