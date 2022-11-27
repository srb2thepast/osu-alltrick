using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osuAT.Game.Types;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using static osuAT.Game.Skills.AimSkill;
using osu.Framework.Utils;
using osuAT.Game.Skills.Resources;

namespace osuAT.Game.Skills
{
    public class CursorControlSkill : ISkill
    {
        #region
        public string Name => "Cursor Control";

        public string Identifier => "cursorcontrol";

        public string Version => "0.004";

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

        public Vector2 BoxPosition => new Vector2(1875, -500);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500,3000, 6000, 9000, 10000);
        #endregion


        public class CursorControlCalculator : SkillCalcuator
        {
            public CursorControlCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };


            [HiddenDebugValue]
            private List<double> hitAngleDiffs;
            [HiddenDebugValue]
            private double distTotal;
            private double avgDist;
            private double curAngle;
            private double timeDiffSum;
            private double curBPM;
            private double avgAng;
            [HiddenDebugValue]
            private double angTotal;
            private int angCount;

            private double curAngStrainWorth = 0;
            private double curWorth = 0;
            private double totalAngStrainWorth = 0;
            private double angDifficulty = 0;

            public override void Setup()
            {
                hitAngleDiffs = new List<double>() {};
                avgDist = 0;
                distTotal = 0;
                avgAng = 0;
                angTotal = 0;
                angCount = 0;
                curAngle = 0;
                timeDiffSum = 0;
                curBPM = 0;
            }

            public override void CalcNext(OsuDifficultyHitObject diffHit) { 
                var lastDiffHit = (OsuDifficultyHitObject)diffHit.Previous(0);
                if (lastDiffHit == null) return;
                if (lastDiffHit.Angle == null) return;
                if (diffHit.Angle == null) return;
                curAngle = (double)diffHit.Angle * (180 / Math.PI);

                // Angle Difficulty
                curAngStrainWorth = -Math.Clamp(((double)curAngle) / (135 / 0.9), 0, 1) + 0.9;
                totalAngStrainWorth += curAngStrainWorth;
                totalAngStrainWorth = Math.Max(0, totalAngStrainWorth);
                angDifficulty = 30 * Math.Log(totalAngStrainWorth + 1);
                curWorth = Math.Max(curWorth, angDifficulty * diffHit.MinimumJumpDistance / diffHit.DeltaTime);
                CurTotalPP = curWorth;

                // Miss and combo scaling
                CurTotalPP *= SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo);

                avgDist = distTotal / (CurrentIndex+1);
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new CursorControlCalculator(score);
    }
}
