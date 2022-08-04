using System;
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
        /// The MAJOR.MINOR [MA.MIN] version of this skill's SkillCalc. This value is saved in <see cref="SaveStorage"/>.
        /// </summary>
        /// <remarks>
        /// Updating this value will recalc all scores in the savedata for this skill.
        /// This value should always increment, and NEVER decrement.
        /// If you are reverting changes, you must still increment the value.
        /// </remarks>

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
        /// The path of image to be used when displaying the SkillLevel Badge in this skill's <see cref="FullSkillBox"/>.
        /// </summary>
        public string BadgeBG { get; }

        public (Vector2, Vector2) BadgePosSize { get; }

        /// <summary>
        /// The height of this skill's <see cref="MiniSkillBox"/>.
        /// </summary>
        public float MiniHeight { get; }

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
        /// The people who contributed to this skill.
        /// </summary>
        public Contributor[] Contributors => new Contributor[] {};

        /// <summary>
        /// This skill's (possibly custom) Contributor Page.
        /// </summary>
        public Page ContributorPage => new DefaultContributorPage {
            Skill = this,
            Contribs = Contributors
        };

        /// <summary>
        /// Returns the SkillPP of this skill from the SaveStorage.
        /// </summary>
        public double SkillPP => SaveStorage.SaveData.TotalSkillPP[Identifier];

        /// <summary>
        /// Returns the current SkillLevel based on the current Skill's SkillPP.
        /// </summary>
        public SkillLevel Level
        {
            get
            {
                double skillPP = SkillPP;
                if (skillPP > Benchmarks.Chosen) { return (SkillLevel.Chosen); }
                if (skillPP > Benchmarks.Mastery) { return (SkillLevel.Mastery); }
                if (skillPP > Benchmarks.Proficient) { return (SkillLevel.Proficient); }
                if (skillPP > Benchmarks.Confident) { return (SkillLevel.Confident); }
                if (skillPP > Benchmarks.Experienced) { return (SkillLevel.Experienced); }
                if (skillPP > Benchmarks.Learner) { return (SkillLevel.Learner); }
                return (SkillLevel.None);
            }
        }

        /// <summary>
        /// The rulesets this skill can support.
        /// </summary>
        public RulesetInfo[] SupportedRulesets { get; }

    }
}
