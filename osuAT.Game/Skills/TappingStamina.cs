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
using Sentry.Infrastructure;

namespace osuAT.Game.Skills
{

    public class TappingStaminaSkill : ISkill
    {
        #region Info
        public string Name => "Tapping Stamina";

        public string Identifier => "tapstamina";

        public string Version => "0.002";

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

        public Vector2 BoxPosition => new Vector2(-2400, 100);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500,3000, 6000, 9000, 10000);

        #endregion

        /// <summary>
        /// Returns a pp value based off of the most spaced out stream of a map.
        /// </summary>
        /// <remarks> Current faults:
        /// - Hard-capping what is considered a stream based off of ms
        /// - Strain is not considered
        /// - Angle is not considered
        /// - Only considers one stream rather than the map as a whole
        /// ~ Difficulty Spike of Spacing is considered as a simple parabola.
        /// - Difficulty Spike of CS is considered as a linear curve
        /// - Difficulty Spike of BPM is a un-tuned parabola
        /// - Difficulty of Stream Count is considered as a linear curve
        /// - No combo diff-spike
        /// - Jumps can be considered streams if they're fast enough
        /// </remarks>
        public class FlowAimCalculator : SkillCalcuator
        {
            public FlowAimCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private double CurStreamLength = 0;
            private double CurMSSpeed = 0;
            private double BPMBuff = 0;
            private double LenMult = 0;
            private double CurWorth = 0;
            public override void Setup()
            {
                CurStreamLength = 0;
            }

            public override void CalcNext(OsuDifficultyHitObject diffHitObj)
            {
                var diffHit = (OsuDifficultyHitObject)diffHitObj;
                var hitObj = (OsuHitObject)diffHit.BaseObject;
                var lastHitObj = (OsuHitObject)diffHit.LastObject;
                var lastDiffHit = diffHit.Previous(0);
                if (lastDiffHit == null) return;

                // Strain-based Stream Length
                CurStreamLength += Math.Clamp(SharedMethods.BPMToMS(180, 4) / (hitObj.StartTime - lastHitObj.StartTime), 0, 1);
                CurStreamLength -= CurStreamLength * (1-Math.Clamp(SharedMethods.BPMToMS(100) / (hitObj.StartTime - lastHitObj.StartTime), 0,1));
                CurMSSpeed = diffHit.StartTime - lastDiffHit.StartTime;

                // Length multiplier
                LenMult = 2 * Math.Log((CurStreamLength* 1 / 40) + 1);

                // BPMBuff
                BPMBuff = 2 * Math.Pow(1.02, SharedMethods.MSToBPM(CurMSSpeed));

                // Final Value (Returns the most difficult stream)
                CurWorth = Math.Max(CurWorth,BPMBuff * LenMult);

                // Miss penalty and combo scaling
                CurTotalPP = (
                    CurWorth *
                    SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo) *
                    SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo)
                );
                Console.WriteLine(SharedMethods.MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo) + "/" + SharedMethods.LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo));
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new FlowAimCalculator(score);
    }
}
