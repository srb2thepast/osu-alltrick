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
using Sentry.Infrastructure;
using static osuAT.Game.Skills.AimSkill;
using static osuAT.Game.Skills.Resources.SharedMethods;

namespace osuAT.Game.Skills
{
    public class TappingStaminaSkill : ISkill
    {
        #region Info

        public string Name => "Tapping Stamina";

        public string Identifier => "tapstamina";

        public string Version => "0.005";

        public string Summary => "The ability of your tapping to \n endure continuous strain";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#ff9385");

        public Colour4 SecondaryColor => Colour4.FromHex("#ffabc6");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize => (new Vector2(-20, 60), new Vector2(700, 440));

        public float MiniHeight => 224;

        public int BoxNameSize => 54;

        public Vector2 BoxPosition => new Vector2(-1873, 1077.5f);

        #endregion Info

        public class TappingStaminaCalculator : SkillCalcuator
        {
            public TappingStaminaCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private double curStreamLength;
            private double curMSSpeed;
            private double msSpeedStrain;

            private double bPMBuff;
            private double lenMult;
            private double curWorth;
            private double bpmSpeedStrain;

            [HiddenDebugValue]
            private double highestWorth;

            public override void Setup()
            {
                msSpeedStrain = 80;
            }

            public override void CalcNext(OsuDifficultyHitObject diffHitObj)
            {
                var diffHit = diffHitObj;
                _ = (OsuHitObject)diffHit.LastObject;
                var lastDiffHit = diffHit.Previous(0);
                if (lastDiffHit == null) return;
                if (diffHit.BaseObject is Spinner) return;

                // Smoothly scaling MS speed
                curMSSpeed = diffHit.StartTime - lastDiffHit.StartTime;

                if (curMSSpeed > msSpeedStrain) // getting slower
                    msSpeedStrain += 0.3 * (curMSSpeed - msSpeedStrain);
                else // getting faster
                    msSpeedStrain += 0.5 * (curMSSpeed - msSpeedStrain);


                // Strain-based Stream Length
                curStreamLength += Math.Clamp(SharedMethods.BPMToMS(180) / msSpeedStrain, 0, 1);
                double curStreamPenalty = curStreamLength * (1 - Math.Clamp(SharedMethods.BPMToMS(160) / (msSpeedStrain), 0, 1));
                curStreamLength -= curStreamPenalty;
                curStreamLength = Math.Max(curStreamLength, 0);

                // Length multiplier
                lenMult = Math.Sqrt(curStreamLength/ (int)StreamLengths.DeathStream) * Math.Min(1,Math.Pow(2,curStreamLength/(int)StreamLengths.DeathStream-1));

                bpmSpeedStrain = SharedMethods.MSToBPM(msSpeedStrain);

                // BPMBuff
                bPMBuff = 200 * Math.Pow(1.01, SharedMethods.MSToBPM(msSpeedStrain) - 180);

                // Final Value (Returns the most difficult stream)
                curWorth = bPMBuff * lenMult;
                highestWorth = Math.Max(highestWorth, curWorth);

                CurTotalPP = (
                    highestWorth *
                    SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo) *
                    SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo) *
                    SharedMethods.SimpleAccNerf(FocusedScore.Accuracy)
                );
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new TappingStaminaCalculator(score);
    }
}
