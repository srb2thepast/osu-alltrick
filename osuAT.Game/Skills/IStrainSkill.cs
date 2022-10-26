using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Sprites;
using osuAT.Game.Types;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osuTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Colour;

namespace osuAT.Game.Skills
{ 
    public abstract class IStrainSkill : ISkillCalcuator
    {

        protected double DecayWeight = 0.5;

        /// <summary>
        /// The rulesets this skill can support.
        /// </summary>
        public RulesetInfo[] SupportedRulesets { get; }

        /// <summary>
        /// This skill's PP Calculator System.
        /// </summary>
        public double SkillCalc(Score score) { return -1; }

        /// <summary>
        /// The people who contributed to this skill's development.
        /// </summary>
        public Contributor[] Contributors => new Contributor[] {};

        public List<double> GetBeatmapStrains(DifficultyHitObject hitobject)  {
            return null;
        }

        

    }
}
