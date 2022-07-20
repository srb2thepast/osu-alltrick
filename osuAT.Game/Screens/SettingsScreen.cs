using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Effects;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osu.Framework.Graphics.Containers;
using osuTK;
using osuTK.Graphics;
using osuAT.Game.Objects;

namespace osuAT.Game
{
    public class SettingsScreen : Screen
    {
        private Drawable background;
        public SkillContainer SkillCont;
        public Container TopBar;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {

            InternalChildren = new Drawable[]
            {
                background = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(1000,800),
                    Masking = true,
                    CornerRadius = 20,
                    BorderThickness = 10,
                    BorderColour = Colour4.White,
                    Children = new Drawable[] {
                        new Box {
                            Colour = Color4.Violet,
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },

            };

        }


    }
}
