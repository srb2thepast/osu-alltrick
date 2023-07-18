using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osuAT.Game.Objects;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game.Skills.Resources
{
    // When a skill is ready to be used, dont forget to add it to Skill.cs
    public abstract class SkillCalcuator
    {
        /// <summary>
        /// The rulesets this skill can support.
        /// </summary>
        public abstract RulesetInfo[] SupportedRulesets { get; }

        /// <summary>
        /// The score this SkillCalculator is assigned to process.
        /// </summary>
        public Score FocusedScore { get; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public int CurrentIndex { get; set; }

        /// <summary>
        /// Creates a new SkillCalculator that will calculate the value of the score given for this skill.
        /// </summary>
        protected SkillCalcuator(Score score)
        {
            FocusedScore = score;
            EndIndex = EndIndex == default ? (FocusedScore.BeatmapInfo != null ? FocusedScore.BeatmapInfo.Contents.DiffHitObjects.Count - 1 : 0) : EndIndex;
        }

        /// <summary>
        /// The amount of PP the current <see cref="FocusedScore"/> is worth.
        /// </summary>
        public double CurTotalPP { get; protected set; } = 0;

        /// <summary>
        /// Completes any necessary setup to calculate the skill (ex. variables that depend on info from the score or
        /// initalizing any non-static variables).
        /// </summary>
        public virtual void Setup()
        { }

        /// <summary>
        /// Calculates the pp worth of the FocusedScore.
        /// </summary>
        public virtual double SkillCalc()
        {
            if (!SupportedRulesets.Contains(FocusedScore.ScoreRuleset)) return -1;
            if (FocusedScore.BeatmapInfo.FolderLocation == default) return -2;
            if (FocusedScore.BeatmapInfo.Contents.HitObjects == default) return -3;
            Setup();
            for (var i = StartIndex; i <= EndIndex; i++)
            {
                var diffHitObj = FocusedScore.BeatmapInfo.Contents.DiffHitObjects[i];
                CurrentIndex = i;
                CalcNext(diffHitObj);
            };
            return CurTotalPP;
        }

        /// <summary>
        /// Asynchronously calculates the pp worth of the FocusedScore.
        /// </summary>
        public async Task<double> SkillCalcAsync()
        {
            await Task.Run(SkillCalc);
            return CurTotalPP;
        }

        // [~] Find out a way to structure multi-mode support based off of a mode's general DiffHitObj class when the time comes.
        /// <summary>
        /// Sets <see cref="CurTotalPP"/> to how much pp the hit object given is worth with respect to all objects coming before it.
        /// </summary>
        /// <remarks>The code that goes here runs in the for-loop block in <see cref="SkillCalc()"/>.</remarks>

        public virtual void CalcNext(OsuDifficultyHitObject diffHitObj) => throw new NotImplementedException($"Calculations for ruleset {FocusedScore.RulesetName} is not supported.");

        /*
        public virtual void CalcNext(CatchDifficultyHitObject diffHitObj) => throw new NotImplementedException($"Calculations for ruleset {FocusedScore.RulesetName} is not supported.");
        public virtual void CalcNext(ManiaDifficultyHitObject diffHitObj) => throw new NotImplementedException($"Calculations for ruleset {FocusedScore.RulesetName} is not supported.");
        public virtual void CalcNext(TaikoDifficultyHitObject diffHitObj) => throw new NotImplementedException($"Calculations for ruleset {FocusedScore.RulesetName} is not supported.");
        */

        public virtual void CalcNext(DifficultyHitObject diffHitObj)
        {
            if (FocusedScore.ScoreRuleset == RulesetStore.Osu)
            {
                CalcNext((OsuDifficultyHitObject)diffHitObj);
                return;
            }

            throw new NotSupportedException("Attempted to calculate for ruleset that is not implemented by default.");
        }
    }
}
