using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Graphics.Colour;
using osuAT.Game.Types;
using osuAT.Game.Objects.Displays;
using osuTK;
namespace osuAT.Game.Objects
{

    public class FullSkillBox : CompositeDrawable
    {

        public string SkillName; // Name
        public int TextSize; // Text Size

        public Colour4 PrimaryColor; // Primary Color
        public Colour4 SecondaryColor; // Secondary Color

        public Texture Background; // Skill Background
        public int MiniHeight; // Minibox Height
        public SkillLevel Level { get; }
        public RulesetInfo[] SupportedRulesets = {RulesetStore.Osu};
        private Container fullBox;
        private CircularProgress backprogress;
        private CircularProgress skillprogress;

        private Circle bubble1;
        private Circle bubble2;
        private Circle bubble3;

        public class SkillInfo : CompositeDrawable
        {
            public string SkillName; // Name
            public int TextSize; // Text Size

            public Colour4 PrimaryColor; // Primary Color
            public Colour4 SecondaryColor; // Secondary Color

            public Texture Background; // Skill Background
            public int MiniHeight; // Minibox Height
            public SkillLevel Level { get; }
            public RulesetInfo[] SupportedRulesets = { RulesetStore.Osu };

            private Circle bubble1;
            private Circle bubble2;
            private Circle bubble3;
            private CircularProgress backprogress;
            private CircularProgress skillprogress;
            public SkillInfo()
            {
                Origin = Anchor.Centre;
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                ColourInfo VerticalGrad = new ColourInfo
                {
                    TopLeft = PrimaryColor,
                    TopRight = PrimaryColor,
                    BottomLeft = SecondaryColor,
                    BottomRight = SecondaryColor,
                };
                ColourInfo HorizontalGrad = new ColourInfo
                {
                    TopLeft = PrimaryColor,
                    BottomLeft = PrimaryColor,
                    TopRight = SecondaryColor,
                    BottomRight = SecondaryColor,
                };
                InternalChild = new Container
                { // todo 6/30/2022: create SkillInfo as a seperate class instead
                    Size = new Vector2(130, 180),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    CornerRadius = 35,
                    X = 80,
                    Children = new Drawable[] {
                        // Background Image (top play)
                        new Sprite
                        {
                            Size = new Vector2(350, 220),
                            X = 40,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Texture = textures.Get("TestBG"),
                            Alpha = 0.7f
                        },

                        // Skill Text
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,

                            Y = 14,

                            Children = new Drawable[] {

                                // Circle Shadow
                                new Circle
                                {
                                    Size = new Vector2(100, 20),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Colour = Colour4.Black,
                                    CornerRadius = 30,
                                    Y = 1.5f,

                                }.WithEffect(new BlurEffect
                                {
                                    Sigma = new Vector2(2f, 2f),
                                    Strength = 0.5f,


                                }),

                                // Circle
                                new Circle
                                {
                                    Size = new Vector2(100, 20),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Colour = PrimaryColor,
                                    CornerRadius = 30,
                                    Alpha = 0.9f
                                },

                                // SkillText
                                new SpriteText
                                {
                                    Text = SkillName,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Font = new FontUsage("VarelaRound", size: 20),
                                    Colour = Colour4.White,
                                    Shadow = true,
                                    ShadowColour = PrimaryColor,
                                    Y = -0.5f,
                                    Padding = new MarginPadding
                                    {

                                        Horizontal = 15,
                                        Vertical = 1
                                    },

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

                                // Frame
                                new Container
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.Centre,
                                }
                            }
                        },

                        // Circular Progress Bar, PFP, Bubbles
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Y = 60,
                            Children = new Drawable[] {
                                // Bubbles
                                new Container
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    X = 41,
                                    Children = new Drawable[] {
                                        // Bubble1 Outline
                                        new Circle
                                        {
                                            Size = new Vector2(11.5f),
                                            X = -7,
                                            Y = -18,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.White,
                                        },
                                        bubble1 = new Circle
                                        { // todo 6/30/2022: create bubble as a seperate class instead
                                            Size = new Vector2(9),
                                            X = -7,
                                            Y = -18,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = PrimaryColor,
                                        },
                                        // Bubble2 Outline
                                        new Circle
                                        {
                                            Size = new Vector2(11.5f),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.White,
                                        },
                                        bubble2 = new Circle
                                        {
                                            Size = new Vector2(9),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.IndianRed, // should be Colour4.LightGray when page 2 is completed.
                                        },
                                        // Bubble3 Outline
                                        new Circle
                                        {
                                            Size = new Vector2(11.5f),
                                            X = -7,
                                            Y = 18,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.White,
                                        },
                                        bubble3 = new Circle
                                        {
                                            Size = new Vector2(9),
                                            X = -7,
                                            Y = 18,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.IndianRed, // should be lightgray when page 3 is completed.
                                        },
                                    }
                                },
                                //
                                new Circle
                                {
                                    Size = new Vector2(60, 60),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Colour = PrimaryColor,
                                    Alpha = 0.1f
                                },
                                // Transparent Progress
                                backprogress = new CircularProgress
                                {
                                    Size = new Vector2(60, 60),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    InnerRadius = 0.2f,
                                    Colour = Colour4.Black,
                                    Alpha = 0.5f
                                },
                                // Colored Progress
                                skillprogress = new CircularProgress
                                {
                                    Size = new Vector2(60, 60),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    InnerRadius = 0.2f,
                                    Colour = VerticalGrad
                                },

                                // Player PFP
                                new Container
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Children = new Drawable[] {
                                        // Border
                                        new Circle
                                        {
                                            Size = new Vector2(37, 37),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = VerticalGrad
                                        },
                                        // PFP
                                        new Container
                                        {
                                            Size = new Vector2(31, 31),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Masking = true,
                                            CornerRadius = 15.5f,
                                            Children = new Drawable[] {
                                                new Sprite
                                                {
                                                    Size = new Vector2(31, 31),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Texture = textures.Get("TestPFP")
                                                }
                                            }
                                        }
                                    }
                                }


                            }
                        },

                        // PP Bar
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 10,
                            Children = new Drawable[] {

                                new Circle
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 10,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Colour = SecondaryColor,
                                    Y = 2,
                                },
                                new Circle
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 10,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Colour = PrimaryColor,
                                },
                                new SpriteText
                                {
                                    Text = "964pp/2000pp",
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Y = -0.5f,
                                    Font = new FontUsage("VarelaRound", size: 10),
                                    Colour = Colour4.White,
                                    Shadow = true,
                                    ShadowOffset = new Vector2(0, 0.05f),
                                    Spacing = new Vector2(0.2f, 0),
                                    Padding = new MarginPadding
                                    {

                                        Horizontal = 5,
                                    },

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
                            }
                        },
                        // Rulesets
                        new RulesetDisplay
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 29,
                            Scale = new Vector2(0.25f),
                            RulesetList = new RulesetInfo[] {

                                RulesetStore.Osu,
                                RulesetStore.Mania,
                                RulesetStore.Taiko,
                                RulesetStore.Catch,
                                RulesetStore.Osu,
                            }
                        },
                        // Skill Description
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 45,
                            Children = new Drawable[] {
                                // Line 1
                                new SpriteText
                                {
                                    Text = "A short summary of this skill.",
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Font = new FontUsage("VarelaRound", size: 11),
                                    Colour = Colour4.White,
                                    Shadow = true,
                                    ShadowOffset = new Vector2(0, 0.05f),
                                    Spacing = new Vector2(0.1f, 0),
                                    Padding = new MarginPadding
                                    {

                                        Horizontal = 5,
                                    },

                                }.WithEffect(new GlowEffect
                                {
                                    BlurSigma = new Vector2(0.5f),
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
                                // Line 2
                                new SpriteText
                                {
                                    Text = "[Lorem Ipsum]",
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Y = 10,
                                    Font = new FontUsage("VarelaRound", size: 11),
                                    Colour = Colour4.White,
                                    Shadow = true,
                                    ShadowOffset = new Vector2(0, 0.05f),
                                    Spacing = new Vector2(0.1f, 0),
                                    Padding = new MarginPadding
                                    {

                                        Horizontal = 5,
                                    },

                                }.WithEffect(new GlowEffect
                                {
                                    BlurSigma = new Vector2(0.5f),
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
                            }
                        }

                    }
                };
            }
        }

        public FullSkillBox(string name, int textsize, Colour4 primarycolor, Colour4 secondarycolor, Texture bg, int height, SkillLevel slevel)
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;


            SkillName = name;
            TextSize = textsize;
            PrimaryColor = primarycolor;
            SecondaryColor = secondarycolor;
            Background = bg;
            MiniHeight = height;
            Level = slevel;
           
        }
        public FullSkillBox()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            ColourInfo VerticalGrad = new ColourInfo
            {
                TopLeft = PrimaryColor,
                TopRight = PrimaryColor,
                BottomLeft = SecondaryColor,
                BottomRight = SecondaryColor,
            };
            ColourInfo HorizontalGrad = new ColourInfo
            {
                TopLeft = PrimaryColor,
                BottomLeft = PrimaryColor,
                TopRight = SecondaryColor,
                BottomRight = SecondaryColor,
            };

            InternalChild = fullBox = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,

                Masking = true,
                CornerRadius = 35,

                Children = new Drawable[]
                    {

                       // Background
                        new Container {
                            Size = new Vector2(350,220),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children = new Drawable[] {
                                // Black Box
                                new Box
                                {
                                    Size = new Vector2(350,220),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,

                                    Colour = Colour4.Black,
                                    Alpha = 0.9f
                                },
                                // Background Image
                                new Sprite {
                                    Size = new Vector2(460.8f,259.2f), // reminder to set this as a changeable variable
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Texture = textures.Get("TestBG2"),
                                    Alpha = 0.4f
                                },
                                
                                // Gray Box
                                new Box
                                {
                                    Size = new Vector2(350,220),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,

                                    Colour = Colour4.Gray,
                                    Alpha = 0.3f
                                },  
                            }
                        },

                        // Skill Info
                        new Container { // todo 6/30/2022: create SkillInfo as a seperate class instead
                            Size = new Vector2(130,180),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Masking = true,
                            CornerRadius = 35,
                            X = 80,
                            Children = new Drawable[] {
                                // Background Image (top play)
                                new Sprite {
                                    Size = new Vector2(350,220),
                                    X = 40,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Texture = textures.Get("TestBG"),
                                    Alpha = 0.7f
                                },

                                // Skill Text
                                new Container {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.Centre,

                                    Y = 14,

                                    Children = new Drawable[] {

                                        // Circle Shadow
                                        new Circle
                                        {
                                            Size = new Vector2(100,20),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.Black,
                                            CornerRadius = 30,
                                            Y = 1.5f,

                                        }.WithEffect(new BlurEffect
                                        {
                                            Sigma = new Vector2(2f, 2f),
                                            Strength = 0.5f,


                                        }),

                                        // Circle
                                        new Circle  
                                        {
                                            Size = new Vector2(100,20),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = PrimaryColor,
                                            CornerRadius = 30,
                                            Alpha = 0.9f
                                        },

                                        // SkillText
                                        new SpriteText {
                                            Text = SkillName,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Font = new FontUsage("VarelaRound", size: 20),
                                            Colour = Colour4.White,
                                            Shadow =true,
                                            ShadowColour = PrimaryColor,
                                            Y = -0.5f,
                                            Padding = new MarginPadding
                                            {

                                                Horizontal = 15,
                                                Vertical = 1
                                            },

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

                                        // Frame
                                        new Container {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.TopCentre,
                                            Origin = Anchor.Centre,
                                        }
                                    }
                                },

                                // Circular Progress Bar, PFP, Bubbles
                                new Container {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.Centre,
                                    Y = 60,
                                    Children = new Drawable[] {
                                        // Bubbles
                                        new Container {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            X = 41,
                                            Children = new Drawable[] {
                                                // Bubble1 Outline
                                                new Circle {
                                                    Size = new Vector2(11.5f),
                                                    X = -7,
                                                    Y = -18,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = Colour4.White,
                                                },
                                                bubble1 = new Circle { // todo 6/30/2022: create bubble as a seperate class instead
                                                    Size = new Vector2(9),
                                                    X = -7,
                                                    Y = -18,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = PrimaryColor,
                                                },
                                                // Bubble2 Outline
                                                new Circle {
                                                    Size = new Vector2(11.5f),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = Colour4.White,
                                                },
                                                bubble2 = new Circle {
                                                    Size = new Vector2(9),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = Colour4.IndianRed, // should be Colour4.LightGray when page 2 is completed.
                                                },
                                                // Bubble3 Outline
                                                new Circle {
                                                    Size = new Vector2(11.5f),
                                                    X = -7,
                                                    Y = 18,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = Colour4.White,
                                                },
                                                bubble3 = new Circle {
                                                    Size = new Vector2(9),
                                                    X = -7,
                                                    Y = 18,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = Colour4.IndianRed, // should be lightgray when page 3 is completed.
                                                },
                                            }
                                        },
                                        //
                                        new Circle {
                                            Size = new Vector2(60,60),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = PrimaryColor,
                                            Alpha = 0.1f
                                        },
                                        // Transparent Progress
                                        backprogress = new CircularProgress{
                                            Size = new Vector2(60,60),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            InnerRadius = 0.2f,
                                            Colour = Colour4.Black,
                                            Alpha = 0.5f
                                        },
                                        // Colored Progress
                                        skillprogress = new CircularProgress{
                                            Size = new Vector2(60,60),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            InnerRadius = 0.2f,
                                            Colour = VerticalGrad
                                        },

                                        // Player PFP
                                        new Container {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Children = new Drawable[] {
                                                // Border
                                                new Circle{
                                                    Size = new Vector2(37,37),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = VerticalGrad
                                                },
                                                // PFP
                                                new Container {
                                                    Size = new Vector2(31,31),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Masking = true,
                                                    CornerRadius = 15.5f,
                                                    Children = new Drawable[] {
                                                        new Sprite {
                                                            Size = new Vector2(31,31),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Texture = textures.Get("TestPFP")
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        
                                    }
                                },

                                // PP Bar
                                new Container {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Y = 10,
                                    Children = new Drawable[] {

                                        new Circle {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 10,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = SecondaryColor,
                                            Y = 2,
                                        },
                                        new Circle
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 10,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = PrimaryColor,
                                        },
                                        new SpriteText {
                                            Text = "964pp/2000pp",
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Y = -0.5f,
                                            Font = new FontUsage("VarelaRound", size: 10),
                                            Colour = Colour4.White,
                                            Shadow =true,
                                            ShadowOffset = new Vector2(0,0.05f),
                                            Spacing = new Vector2(0.2f,0),
                                            Padding = new MarginPadding
                                            {

                                                Horizontal = 5,
                                            },

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
                                    }
                                },
                                // Rulesets
                                new RulesetDisplay {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Y = 29,
                                    Scale = new Vector2(0.25f),
                                    RulesetList = new RulesetInfo[] {

                                        RulesetStore.Osu,
                                        RulesetStore.Mania,
                                        RulesetStore.Taiko,
                                        RulesetStore.Catch,
                                        RulesetStore.Osu,
                                    }
                                },
                                // Skill Description
                                new Container {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Y = 45,
                                    Children = new Drawable[] {
                                        // Line 1
                                        new SpriteText {
                                            Text = "A short summary of this skill.",
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Font = new FontUsage("VarelaRound", size: 11),
                                            Colour = Colour4.White,
                                            Shadow =true,
                                            ShadowOffset = new Vector2(0,0.05f),
                                            Spacing = new Vector2(0.1f,0),
                                            Padding = new MarginPadding
                                            {

                                                Horizontal = 5,
                                            },

                                        }.WithEffect(new GlowEffect
                                        {
                                            BlurSigma = new Vector2(0.5f),
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
                                        // Line 2
                                        new SpriteText {
                                            Text = "[Lorem Ipsum]",
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Y = 10,
                                            Font = new FontUsage("VarelaRound", size: 11),
                                            Colour = Colour4.White,
                                            Shadow =true,
                                            ShadowOffset = new Vector2(0,0.05f),
                                            Spacing = new Vector2(0.1f,0),
                                            Padding = new MarginPadding
                                            {

                                                Horizontal = 5,
                                            },

                                        }.WithEffect(new GlowEffect
                                        {
                                            BlurSigma = new Vector2(0.5f),
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
                                    }
                                }

                            }
                                
                        },

                        // ScoreDisplay List
                        new Container {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children = new Drawable[] {
                            }
                        }

                    }
            };
            backprogress.FillTo(6);
            skillprogress.FillTo(1, 1000, Easing.InOutCubic);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

        }

    }
}
