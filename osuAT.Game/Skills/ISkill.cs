using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;

namespace osuAT.Game.Skills
{
    // When a skill is ready to be used, dont forget to add it to Skill.cs   

    public interface ISkill
    {
        /// <summary>
        /// The displayed name of this skill.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The skill's identification when saving.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The MAJOR.MINOR [M.MIN] version of this skill. This value is saved in <see cref="SaveStorage"/>.
        /// </summary>
        /// <remarks>
        /// Updating this value will recalc all scores in the savedata for this skill.
        /// This value should always increment, and NEVER decrement.
        /// If you are reverting changes, you must still increment the value.
        /// </remarks>
        /// 
        /// Major should be incremented when a full-scale rework is created.
        /// Minor should be incremented in all other cases.
        public string Version { get; }

        /// <summary>
        /// A short summary of this skill.
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// The TextSize of this skill's summary.
        /// </summary>
        public int SummarySize { get; }

        /// <summary>
        /// The skill's Primary Color to be displayed in it's <see cref="SkillBox"/>.
        /// </summary>
        public Colour4 PrimaryColor { get; } // Primary Color

        /// <summary>
        /// The skill's Secondary Color to be displayed in it's <see cref="SkillBox"/>.
        /// </summary>
        public Colour4 SecondaryColor { get; } // Secondary Color

        /// <summary>
        /// The path of image to be used when displaying this skill's <see cref="FullSkillBox"/>.
        /// </summary>
        public string Background { get; }

        /// <summary>
        /// The path of image to be used when displaying this skill's <see cref="MiniSkillBox"/>.
        /// </summary>
        public string Cover { get; }

        /// <summary>
        /// The height of this skill's <see cref="MiniSkillBox"/>.
        /// </summary>
        public double MiniHeight { get; }

        /// <summary>
        /// The text size of the Skill's Name when displayed in a <see cref="SkillBox"/>.
        /// </summary>
        public int BoxNameSize { get; }

        /// <summary>
        /// The position of this skill's <see cref="SkillBox"/> when applied to a <see cref="SkillContainer"/>.
        /// </summary>
        public Vector2 BoxPosition { get; }

        /// <summary>
        /// This skill's <see cref="SkillLevel"/> Benchmarks.
        /// </summary>
        public SkillGoals Benchmarks { get; }

        /// <summary>
        /// This skill's PP Calculator System.
        /// </summary>
        public double SkillCalc(Score score) { return 0; }

        /// <summary>
        /// The rulesets this skill can support.
        /// </summary>
        public List<RulesetInfo> SupportedRulesets { get; }


    }
}
