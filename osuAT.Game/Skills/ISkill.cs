using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types;
using osuAT.Game.Types.Score;

#nullable enable

namespace osuAT.Game.Skills
{
    // When a skill is ready to be put into savedata, add it to the SkillPPTotals.cs class.
    public interface ISkill
    {
        /// <summary>
        /// The name of the skill.
        /// </summary>
        public string SkillName { get; set; }

        /// <summary>
        /// The skill's <see cref="SkillLevel"/> Benchmarks.
        /// </summary>
        public SkillGoals SkillBenchmarks { get; set; }

        /// <summary>
        /// The skill's PP Calculator System.
        /// </summary>
        /// <param name="ruleset">The ruleset .</param>
        public double SkillCalc(RulesetInfo ruleset, Score score, ModInfo[]? mods);

        /// <summary>
        /// The rulesets the skill can support.
        /// </summary>
        public RulesetInfo[] SupportedRulesets { get; set; }


    }
}
