using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuTK;
namespace osuAT.Game.Objects.Displays
{
    public class ScoreDisplay : CompositeDrawable
    {

        public string SongName;
        public string SongAuthor;
        public string MapDiffname;
        public string MapCreator;
        public double StarRating;
        public double ScorePP;

        public int TextSize; // Text Size

        public Colour4 PrimaryColor; // Primary Color
        public Colour4 SecondaryColor; // Secondary Color

        public Texture Background; // Background
        public ScoreDisplay()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            InternalChild = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,

                Masking = true,
                CornerRadius = 35,

                Children = new Drawable[]
                    {
                        new Box
                        {
                            Size = new Vector2(350,220),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,

                            Colour = PrimaryColor,


                        },
                        new SpriteText {
                            Text = "Trust in you (TV Size)",
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
