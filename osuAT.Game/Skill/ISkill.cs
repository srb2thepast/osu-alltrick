using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;

#nullable enable

namespace osuAT.Game.Skill
{
    // When a skill is ready to be put into savedata, dont forget to add it to:
    // SkillPPTotals.cs
    // SkillTopScores.cs

    public interface ISkill
    {
        /// <summary>
        /// The name of the skill.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The skill's Primary Color to be displayed in it's <see cref="SkillBox"/>.
        /// </summary>
        public Colour4 PrimaryColor { get; } // Primary Color

        /// <summary>
        /// The skill's Secondary Color to be displayed in it's <see cref="SkillBox"/>.
        /// </summary>
        public Colour4 SecondaryColor { get; } // Secondary Color

        /// <summary>
        /// The image to be used when displaying this skill's <see cref="SkillBox"/>.
        /// </summary>
        public Texture Background { get; }

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
        /// The skill's <see cref="SkillLevel"/> Benchmarks.
        /// </summary>
        public SkillGoals Benchmarks { get; }

        /// <summary>
        /// The skill's PP Calculator System.
        /// </summary>
        public double SkillCalc(Score score);

        /// <summary>
        /// The rulesets the skill can support.
        /// </summary>
        public RulesetInfo[] SupportedRulesets { get; }


    }
}
