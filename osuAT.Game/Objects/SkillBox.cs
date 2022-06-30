using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Colour;
using osu.Framework.Input.Events;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;
using osuAT.Game.Types;
using osuTK;


namespace osuAT.Game.Objects
{
    public class SkillBox : Container
    {
        public enum State
        {
            miniBox = 1,
            FullBox = 2
        }
        

        public string SkillName; // Name
        public int TextSize; // Text Size

        public Colour4 SkillPrimaryColor; // Primary Color
        public Colour4 SkillSecondaryColor; // Secondary Color

        public Texture Background; // Skill Background
        public int MiniHeight = 100; // Minibox Height
        public SkillLevel Level;

        public MiniSkillBox MiniBox;
        public FullSkillBox FullBox;

        public SkillBox()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            
            MiniBox = new MiniSkillBox(
                    SkillName,
                    TextSize,
                    SkillPrimaryColor,
                    SkillSecondaryColor,
                    Background,
                    MiniHeight,
                    Level);
            FullBox = new FullSkillBox(
                    SkillName,
                    TextSize,
                    SkillPrimaryColor,
                    SkillSecondaryColor,
                    Background,
                    MiniHeight,
                    Level);

            InternalChild = new Container
            {
                FullBox
            };
            FullBox.ScaleTo(3);

        }


    }
}
