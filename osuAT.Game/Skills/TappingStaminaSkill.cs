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

namespace osuAT.Game.Skills
{

    public class TappingStaminaSkill : ISkill
    {
        #region Info
        public string Name => "Tapping Stamina";

        public string Identifier => "tapstamina";

        public string Version => "0.004";

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

        public SkillGoals Benchmarks => new SkillGoals(600, 1500, 3000, 6000, 9000, 10000);

        #endregion

        public class TappingStaminaCalculator : SkillCalcuator
        {
            public TappingStaminaCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private double curStreamLength = 0;
            private double curMSSpeed = 0;
            private double bPMBuff = 0;
            private double lenMult = 0;
            private double curWorth = 0;
            public override void Setup()
            {
                curStreamLength = 0;
                curMSSpeed = 0;
                bPMBuff = 0;
                lenMult = 0;
                curWorth = 0;
            }

            public override void CalcNext(OsuDifficultyHitObject diffHitObj)
            {
                var diffHit = (OsuDifficultyHitObject)diffHitObj;
                _ = (OsuHitObject)diffHit.LastObject;
                var lastDiffHit = diffHit.Previous(0);
                if (lastDiffHit == null) return;

                // Strain-based Stream Length
                curStreamLength += Math.Clamp(SharedMethods.BPMToMS(180) / (diffHit.StartTime - lastDiffHit.StartTime), 0, 1);
                curStreamLength -= curStreamLength * 0.75 * (1 - Math.Clamp(SharedMethods.BPMToMS(180, 2) / (diffHit.StartTime - lastDiffHit.StartTime), 0, 1));
                curMSSpeed = diffHit.StartTime - lastDiffHit.StartTime;

                // Length multiplier
                lenMult = 2 * Math.Log((curStreamLength * 1 / 40) + 1);

                // BPMBuff
                bPMBuff = 2 * Math.Pow(1.02, SharedMethods.MSToBPM(curMSSpeed));

                // Final Value (Returns the most difficult stream)
                curWorth = Math.Max(curWorth, bPMBuff * lenMult);

                CurTotalPP = (
                    curWorth *
                    SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo) *
                    SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo) *
                    SharedMethods.SimpleAccNerf(FocusedScore.Accuracy)
                );
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new TappingStaminaCalculator(score);
    }
}
