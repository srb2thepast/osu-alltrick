using System.Numerics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;

namespace osuAT.Game.Objects
{
    public class SkillBox : CompositeDrawable
    {

        public string SkillName = "Empty Skill";
        public Colour4 SkillPrimaryColor = Colour4.Purple;
        public Colour4 SkillSecondaryColor = Colour4.Black;
        public int HScale = 100;

        private Container box;
        public SkillBox()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            var HSVPrime = SkillPrimaryColor.ToHSV();
            InternalChild = box = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,

                Masking = true,
                CornerRadius = 4,
                BorderThickness = 4,
                BorderColour = Colour4.FromHSV(HSVPrime.X, HSVPrime.Y + 1, HSVPrime.Z),

                Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Height = HScale,
                            Colour = SkillPrimaryColor,

                        },
                        new SpriteText {
                            Text = SkillName,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = FontUsage.Default.With(size:30),
                            Colour = Colour4.Black,
                            Padding = new MarginPadding
                            {

                                Horizontal = 15,
                                Vertical = 1
                            },

                        },
                    }
            };

        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

        }

    }
}
