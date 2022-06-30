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
using osuAT.Game.Objects;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game.Objects
{
    public class MiniSkillBox : CompositeDrawable
    {

        public string SkillName; // Name
        public int TextSize; // Text Size

        public Colour4 PrimaryColor; // Primary Color
        public Colour4 SecondaryColor; // Secondary Color

        public Texture Background; // Skill Background
        public int MiniHeight; // Minibox Height
        public SkillLevel Level;


        private Sprite miniBG;
        private Container miniBox;
        private Container stars;
        public MiniSkillBox(string name, int textsize, Colour4 primarycolor, Colour4 secondarycolor, Texture background, int height, SkillLevel level)
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;


            SkillName = name;
            TextSize = textsize;
            PrimaryColor = primarycolor;
            SecondaryColor = secondarycolor;
            Background = background;
            MiniHeight = height;
            Level = level;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            var HSVPrime = PrimaryColor.ToHSV();
            InternalChild = miniBox = new Container
            {
                AutoSizeAxes = Axes.Both,
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Children = new Drawable[] {
                    new Container {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[] {
                            // MainBox
                            new Container {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,

                                Masking = true,
                                CornerRadius = 50,

                                Children = new Drawable[]
                                    {
                                        // White Box
                                        new Box
                                        {
                                            Size = new Vector2(352, 224),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.White,
                                        },
                                        // Background
                                        new BufferedContainer {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Children = new Drawable[] {

                                                miniBG = new Sprite {
                                                    Size = new Vector2(352, 224),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Texture = textures.Get("TestBG"),
                                                },

                                            }
                                        }.WithEffect(new GlowEffect
                                        {
                                            BlurSigma = new Vector2(3f),
                                            Strength = 5f,
                                            Colour = new ColourInfo
                                            {
                                                BottomLeft = Colour4.Black,
                                                TopRight = Colour4.DimGray,
                                            },
                                            PadExtent = true,
                                        }),

                                        // Text and Underline
                                        new Container {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,

                                            Children = new Drawable[] {

                                                new Container {
                                                    AutoSizeAxes = Axes.Both,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Masking = true,
                                                    CornerRadius = 50,
                                                    Y = 50,

                                                    Children = new Drawable[] {
                                                        new Circle
                                                        {
                                                            Size = new Vector2(267, 20),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Colour = SecondaryColor,
                                                            Y = 5,
                                                        },
                                                        new Circle
                                                        {
                                                            Size = new Vector2(267, 20),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Colour = PrimaryColor,

                                                        },
                                                    }
                                                },
                                                new SpriteText {

                                                    Text = SkillName,
                                                    Anchor = Anchor.Centre,
                                                    Y = -10,
                                                    Origin = Anchor.Centre,
                                                    Font = new FontUsage("VarelaRound", size: TextSize),
                                                    Colour = PrimaryColor,
                                                    Shadow = true,
                                                    ShadowColour = SecondaryColor
                                                    //Padding = new MarginPadding
                                                    //{
                                                    //    Horizontal = 15,
                                                    //    Vertical = 1
                                                    //},
                                                    // 

                                                }.WithEffect(new GlowEffect
                                                {
                                                    BlurSigma = new Vector2(1),
                                                    Strength = 5,
                                                    Colour = ColourInfo.GradientHorizontal(PrimaryColor, SecondaryColor),
                                                    PadExtent = true,

                                                }).WithEffect(new OutlineEffect
                                                {
                                                    BlurSigma = new Vector2(0),
                                                    Strength = 5,
                                                    Colour = Colour4.White,
                                                    PadExtent = true,
                                                }),

                                            },


                                        },

                                        // Stars
                                        stars = new Container {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Y = 66,
                                            X = -120,
                                            Children = new Drawable[] {

                                                new StarShad(PrimaryColor, SecondaryColor, new Vector2(10, 0)),
                                                new StarShad(PrimaryColor, SecondaryColor, new Vector2(55, 0)),
                                                new StarShad(PrimaryColor, SecondaryColor, new Vector2(100, 0)),
                                                new StarShad(PrimaryColor, SecondaryColor, new Vector2(145, 0)),
                                                new StarShad(PrimaryColor, SecondaryColor, new Vector2(190, 0)),
                                            }
                                        },

                                    }
                            },

                            // OuterStars
                            new Container {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                X = 96,
                                Y = 70,
                                Children = new Drawable[] {
                                    new SpriteIcon {
                                        Position = new Vector2(-206, -148),
                                        Size = new Vector2(70, 70),
                                        Icon = FontAwesome.Solid.Star,
                                        Colour = PrimaryColor
                                    }.WithEffect(new GlowEffect
                                    {
                                        BlurSigma = new Vector2(1f),
                                        Strength = 0.3f,
                                        Colour = PrimaryColor,
                                        PadExtent = true,
                                    }),
                                    new SpriteIcon {
                                        Position = new Vector2(126, 58),
                                        Size = new Vector2(70, 70),
                                        Icon = FontAwesome.Solid.Star,
                                        Colour = SecondaryColor
                                    }.WithEffect(new GlowEffect
                                    {
                                        BlurSigma = new Vector2(1f),
                                        Strength = 0.3f,
                                        Colour = SecondaryColor.Darken(0.2f),
                                        PadExtent = true,

                                    }),

                                }
                            }
                            }
                    }
                }
            };
            miniBox.ScaleTo(0.5f);


        }


        private class StarShad : Container
        {
            public Container StarShading;
            public StarShad(Colour4 MainColor, Colour4 ShadowColor, Vector2 Pos)
            {

                Child = StarShading = new Container
                {
                    Position = Pos,
                    Children = new Drawable[] {
                        new SpriteIcon {
                            Y=5,
                            Size = new Vector2(40,40),
                            Icon = FontAwesome.Solid.Star,
                            Colour = ShadowColor,
                        },
                        new SpriteIcon {
                            Size = new Vector2(40,40),
                            Icon = FontAwesome.Solid.Star,
                            Colour = MainColor,
                        },
                    }
                };

            }
        }
        protected override bool OnHover(HoverEvent e)
        {
            miniBox.Child.ScaleTo(1.1f, 200, Easing.Out);
            return base.OnHover(e);

        }
        protected override void OnHoverLost(HoverLostEvent e)
        {
            miniBox.Child.ScaleTo(1f, 100, Easing.Out);
            base.OnHoverLost(e);
        }
        protected override bool OnDoubleClick(DoubleClickEvent e)
        {
            System.Console.WriteLine("hihi");
            return base.OnDoubleClick(e);
        }

    }
}
