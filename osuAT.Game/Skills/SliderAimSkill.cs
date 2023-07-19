using System;
using System.Collections.Generic;
using System.Linq;
using DiffPlex;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;

(int)Angle.Triangle

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

        #endregion

        public class SliderAimSkillCalculator : SkillCalcuator
        {
            public SliderAimSkillCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            public override void Setup()
            {
            }

            private double curWorth;

            [HiddenDebugValue]
            private double lastSliderApperance;

            private double lastStrainNerf = 1;

            [HiddenDebugValue]
            private double highestWorth;

            private double sliderDifficulty;

            private double repeatBuff;

            public override void CalcNext(OsuDifficultyHitObject osuHit)
            {
                // Slider velocity buff:
                // https://github.com/ppy/osu/blob/master/osu.Game.Rulesets.Osu/Difficulty/Evaluators/AimEvaluator.cs
                if (osuHit.BaseObject is Slider slideHit && osuHit.TravelTime > 0)
                {
                    lastSliderApperance = CurrentIndex;
                    repeatBuff = Math.Log(slideHit.SpanCount() + 1) * 1.6;
                    sliderDifficulty = osuHit.TravelDistance / osuHit.TravelTime * 120 * repeatBuff;
                    if (CurrentIndex == EndIndex - 1)
                    {
                        Console.WriteLine($"{osuHit.TravelDistance}/{osuHit.TravelTime} | " + sliderDifficulty + $" reps: {slideHit.SpanCount()}");
                    }

                    lastStrainNerf = Math.Max(osuHit.DeltaTime / 80, 1);
                    curWorth += sliderDifficulty;
                };

                if (osuHit.BaseObject is HitCircle)
                {
                    lastStrainNerf = Math.Max(1.1 * osuHit.DeltaTime / 80, 1);
                    sliderDifficulty = 0;
                }
                curWorth /= lastStrainNerf;

                highestWorth = Math.Max(highestWorth, curWorth);

                CurTotalPP = highestWorth;
                // Miss and combo scaling
                CurTotalPP *= SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SharedMethods.SimpleAccNerf(FocusedScore.Accuracy);
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new SliderAimSkillCalculator(score);
    }
}
