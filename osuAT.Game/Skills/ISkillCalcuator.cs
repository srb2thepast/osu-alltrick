using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Sprites;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Colour;
using osu.Game.Rulesets.Difficulty.Preprocessing;
namespace osuAT.Game.Skills
{

    public interface ISkillCalcuator
    {
        /// <summary>
        /// The rulesets this skill can support.
        /// </summary>
        public RulesetInfo[] SupportedRulesets { get; }

        /// <summary>
        /// This skill's PP Calculator System.
        /// </summary>
        public double SkillCalc(Score score) { return -1; }

        /// <summary>
        /// Returns the most pp possible in a map for this skill.
        /// </summary>
        public double SkillCalc(Beatmap map, RulesetInfo ruleset, Mods[] mods) 
        { 
            return SkillCalc(new Score {
            RulesetName = ruleset.Name,
            ScoreRuleset = ruleset,
            Combo = map.MaxCombo,
            Beatmap = map
            }) 
        }
        /// <summary>
        /// The people who contributed to this skill's development.
        /// </summary>
        public Contributor[] Contributors => new Contributor[] {};

        /// <summary>
        /// Returns a list of how the Skill's PP changes over the course of a list of hit
        /// </summary>
        public List<double> SkillCalcHitlist(Beatmap map, RulesetInfo ruleset, Mods[] mods) {
            
        }

    }
}
