using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game.Skills
{
    public class SliderAimSkill : ISkill
    {
        #region
        public string Name => "Slider Aim";

        public string Identifier => "slideraim";

        public string Version => "0.004";

        public string Summary => "The ability to follow slider bodies \n accurately.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.LightGreen;

        public Colour4 SecondaryColor => Colour4.FromHex("#F0A1B7");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize { get; }

        public float MiniHeight => 224;

        public int BoxNameSize => 63;

        public Vector2 BoxPosition => new Vector2(953, -950);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500, 3000, 6000, 9000, 10000);
        #endregion


        public class SliderAimSkillCalculator : StrainSkillCalc
        {
            public SliderAimSkillCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            public override void Setup()
            {

            }

            protected override double DecayFactor => 0.01;

            private double curWorth = 0;
            [HiddenDebugValue]
            private double lastSliderApperance = 0;

            public override void CalcNext(OsuDifficultyHitObject osuHit)
            {

                // Slider velocity buff:
                // https://github.com/ppy/osu/blob/master/osu.Game.Rulesets.Osu/Difficulty/Evaluators/AimEvaluator.cs
                if (osuHit.BaseObject is Slider slideHit && osuHit.TravelTime > 0)
                {
                    lastSliderApperance = CurrentIndex;
                    double sliderWorth = (osuHit.TravelDistance / 2) / (osuHit.TravelTime) * 80;
                    curWorth = sliderWorth;
                    TotalStrainWorth += sliderWorth / 3;
                    // If sliderWorth is more than 100, you get an overall strain buff!
                    StrainPosition += 0.8 - sliderWorth / 100;
                    CurTotalPP = GetPositionAppliedStrain(TotalStrainWorth) * 4;
                    if (CurrentIndex == EndIndex - 1)
                    {
                        Console.WriteLine($"{osuHit.TravelDistance}/{osuHit.TravelTime} | " + sliderWorth + " pos:" + (StrainPosition) + $" reps: {slideHit.SpanCount()}");
                    }

                    // Miss and combo scaling
                    CurTotalPP *= SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo);
                    CurTotalPP *= SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo);
                    CurTotalPP *= SharedMethods.SimpleAccNerf(FocusedScore.Accuracy);
                    return;
                }
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new SliderAimSkillCalculator(score);
    }
}
