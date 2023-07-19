using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;
using static osuAT.Game.Skills.Resources.SharedMethods;

namespace osuAT.Game.Skills
{
    public class FlowAimSkill : ISkill
    {
        #region Info

        public string Name => "Flow Aim";

        public string Identifier => "flowaim";

        public string Version => "0.010";

        public string Summary => "The ability to move your cursor \n in a fluid motion.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#99FF69");

        public Colour4 SecondaryColor => Colour4.FromHex("#00FFF0");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize => (new Vector2(-20, 60), new Vector2(700, 440));

        public float MiniHeight => 224;

        public int BoxNameSize => 83;

        public Vector2 BoxPosition => new Vector2(953, 1300);

        #endregion Info

        /// <summary>
        /// Returns a pp value based off of the most spaced out stream of a map.
        /// </summary>
        /// <remarks> Current faults:
        /// ~ Difficulty Spike of Spacing is considered as a simple parabola.
        /// - Difficulty Spike of BPM is a un-tuned parabola
        /// - Difficulty of Stream Count is considered as a linear curve
        /// </remarks>
        public class FlowAimCalculator : SkillCalcuator
        {
            public FlowAimCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            private double curStreamLength;
            private double curStreamSpeedBuff;
            private double curStreamSpeedNerf;
            private double curStreamSpaceBuff;
            private double curStreamSpaceMult;

            private double curMSSpeed;
            private double msSpeedStrain;

            [HiddenDebugValue]
            private double totalAngStrainWorth;

            private double flowPatternMult;
            private double angCurDiff;
            private double aimCurDiff;

            private double lenMult;
            private double aimDifficulty;
            private double angDifficulty;
            private double curWorth;

            [HiddenDebugValue]
            private double highestWorth;

            public override void Setup()
            { }

            public override void CalcNext(OsuDifficultyHitObject diffHitObj)
            {
                var diffHit = diffHitObj;
                var lastDiffHit = (OsuDifficultyHitObject)diffHit.Previous(0);
                if (lastDiffHit == null) return;
                if (lastDiffHit.Angle == null) return;
                if (diffHit.Angle == null) return;
                if (diffHit.BaseObject is Spinner) return;
                double curAngle = (double)diffHit.Angle * (180 / Math.PI);
                double lastAngle = (double)lastDiffHit.Angle * (180 / Math.PI);

                // Smoothly scaling MS speed
                curMSSpeed = diffHit.StartTime - lastDiffHit.StartTime;
                msSpeedStrain += 0.7 * (curMSSpeed - msSpeedStrain);

                // Flow Pattern Multiplier
                flowPatternMult = Math.Clamp(((double)curAngle - 60) / 60, 0, 1) / 2 + Math.Clamp(((double)lastAngle - 60) / 60, 0, 1) / 2;

                // Aim Difficulty
                const double aim_difficulty_weight = 10;
                aimCurDiff = aim_difficulty_weight * (diffHit.LazyJumpDistance / diffHit.DeltaTime) * (0.25 + 0.75 * flowPatternMult);
                aimDifficulty += ((double)1 / 18) * ((aimCurDiff) - aimDifficulty);

                // Strain-based Stream Length
                // --- Calculate Speed worth (is the stream fast enough to be considered one requiring fast tapping?)
                curStreamSpeedBuff = Math.Clamp(BPMToMS(160) / (diffHit.StartTime - lastDiffHit.StartTime), 0, 1);
                curStreamSpeedNerf = 0.75 * (1 - Math.Clamp(BPMToMS(180, 2) / (diffHit.StartTime - lastDiffHit.StartTime), 0, 1));

                // --- Calculate Spacing worth (is the stream spaced enough to be considered a stream requiring any sort of flow aim?)
                // This should, in theory, prevent a 100 note burst ending in a 10 note spaced stream from being 110 notes long.
                curStreamSpaceBuff = Math.Clamp(aimDifficulty / (aim_difficulty_weight / 2), 0, 1);
                curStreamSpaceMult = Math.Clamp((aimDifficulty - (aim_difficulty_weight / 4)) / (aim_difficulty_weight / 4), 0, 1);

                double curStreamBuff = (curStreamSpeedBuff + curStreamSpaceBuff - 1) / 2; // Math.Max(Math.Pow((curStreamSpeedBuff + curStreamSpaceBuff) - 1,2), 0);
                double curStreamNerf = (1 - flowPatternMult) * (curStreamSpeedNerf + (1 - curStreamSpaceMult)) / 2;
                curStreamLength += curStreamBuff;
                curStreamLength -= curStreamLength * curStreamNerf;

                // Length multiplier
                lenMult = Math.Log((curStreamLength / (double)StreamLengths.Stream) + 1);

                // Angle Difficulty
                angCurDiff = 20 * (-Math.Clamp(1.5 * ((double)curAngle - 60) / 180, -1, 1) + 1);
                totalAngStrainWorth += angCurDiff * flowPatternMult;
                totalAngStrainWorth *= 0.9;
                totalAngStrainWorth = Math.Max(0, totalAngStrainWorth);

                angDifficulty = 1.5 * Math.Log(totalAngStrainWorth + 1);

                // Final value
                curWorth = 2 * lenMult * flowPatternMult * (aimDifficulty + (aimDifficulty * angDifficulty));
                highestWorth = Math.Max(highestWorth, curWorth);

                CurTotalPP = highestWorth;
                CurTotalPP *= MissPenalty(FocusedScore.AccuracyStats.CountMiss, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= LinearComboScaling(FocusedScore.Combo, FocusedScore.BeatmapInfo.MaxCombo);
                CurTotalPP *= SimpleAccNerf(FocusedScore.Accuracy);
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new FlowAimCalculator(score);
    }
}
