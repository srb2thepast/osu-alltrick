using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types;

namespace osuAT.Game.Objects
{
    public class SkillBoxInfo
    {
        /// <summary>
        /// The name of the skill.
        /// </summary>
        public string SkillName { get; set; } // Name

        /// <summary>
        /// The size of the Skill Name.
        /// </summary>
        public int TextSize { get; set; } // Text Size

        /// <summary>
        /// The primary color to be used when the display is active.
        /// </summary>
        public Colour4 SkillPrimaryColor { get; set; } // Primary Color
        /// <summary>
        /// The secondary color to be used when the display is active.
        /// </summary>
        public Colour4 SkillSecondaryColor { get; set; } // Secondary Color

        /// <summary>
        /// The background image for the skill.
        /// </summary>
        public Texture Background { get; set; } // Skill Background

        /// <summary>
        /// The secondary color to be used when the display is active.
        /// </summary>
        public int MiniHeight { get; set; } // Minibox Height

        /// <summary>
        /// The SkillLevel to be used in the Level display.
        /// </summary>
        public SkillLevel Level { get; set; }

    }
}
