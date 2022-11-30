using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Graphics.Colour;
using osu.Framework.Extensions.LocalisationExtensions;
using osuAT.Game.Types;
using osuAT.Game.Objects.Displays;
using osu.Framework.Input.Events;
using osuTK;
using osuAT.Game.Skills.Resources;

namespace osuAT.Game.Objects
{

    public partial class FullSkillBox : CompositeDrawable
    {

        public ISkill CurSkill;
        public SkillBox ParentBox;
        public SkillInfo InfoBox;

        public Container InnerBox; 
        private ClickableContainer backBut;
        private ScoreContainer scoreContainer;


        public partial class SkillInfo : CompositeDrawable
        {
            public ISkill CurSkill;
            public int TextSize; // Text Size 

            public Texture Background; // Skill Background
            public int MiniHeight; // Minibox Height
            public SkillLevel Level { get; }
            public RulesetInfo[] SupportedRulesets = { RulesetStore.Osu };


            private ppBar ppBox;
            private SkillLevelDisplay levelDisplay;
            private Circle pfpBorder1;
            private Circle pfpBorder2;

            #region Updatable CircularProgresses 
            private CircularProgress skillprogress { get; set; }
            private CircularProgress chosenprogress { get; set; }

            public Book InfoBook { get; private set; }
            public Page Page0 { get; private set; }
            public Page Page1 { get; private set; }
            public Page Page2 { get; private set; }

            
            #endregion

            #region Clickable Bubbles
            public ClickableContainer Bubble1 { get; private set; }
            public ClickableContainer Bubble2 { get; private set; }
            public ClickableContainer Bubble3 { get; private set; }
            private Container bubbleContainer;
            #endregion

            private partial class ppBar : Container
            {
                public ISkill CurSkill;
                private readonly BindableWithCurrent<int> performance = new BindableWithCurrent<int>();
                public Bindable<int> Performance
                {
                    get => performance.Current;
                    set => performance.Current = value;
                }

                private Bindable<int> performPoints = new BindableInt();
                private BufferedContainer ppText;

                public ppBar() {
                    AutoSizeAxes = Axes.Both;
                }

                [BackgroundDependencyLoader]
                private void load(LargeTextureStore textures)
                {
                    ColourInfo VerticalGrad = new ColourInfo
                    {
                        TopLeft = CurSkill.PrimaryColor,
                        TopRight = CurSkill.PrimaryColor,
                        BottomLeft = CurSkill.SecondaryColor,
                        BottomRight = CurSkill.SecondaryColor,
                    };
                    ColourInfo HorizontalGrad = new ColourInfo
                    {
                        TopLeft = CurSkill.PrimaryColor,
                        BottomLeft = CurSkill.PrimaryColor,
                        TopRight = CurSkill.SecondaryColor,
                        BottomRight = CurSkill.SecondaryColor,
                    };
                    Child = new Container
                    {
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
                                Colour = CurSkill.SecondaryColor,
                                Y = 2,
                            },
                            new Circle
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 10,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = CurSkill.PrimaryColor,
                            },
                            ppText = new SpriteText {
                                Text = (Math.Truncate((double)performPoints.Value)).ToString() + "pp/" + ((CurSkill.Level < SkillLevel.Chosen)? CurSkill.Benchmarks.Mastery.ToString() : CurSkill.Benchmarks.Chosen.ToString())+"pp",
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
                                Strength = 30,
                                Colour = HorizontalGrad,
                                PadExtent = true,

                            }).WithEffect(new OutlineEffect
                            {
                                BlurSigma = new Vector2(0),
                                Strength = 1,
                                Colour = Colour4.White,
                                PadExtent = true,
                            }).WithEffect(new GlowEffect
                            {
                                BlurSigma = new Vector2(0.0f),
                                Strength = 0.4f,
                                Colour = HorizontalGrad,
                                PadExtent = true,

                            }),
                        }
                    };

                }

                protected override void LoadComplete()
                {
                    ColourInfo HorizontalGrad = new ColourInfo
                    {
                        TopLeft = CurSkill.PrimaryColor,
                        BottomLeft = CurSkill.PrimaryColor,
                        TopRight = CurSkill.SecondaryColor,
                        BottomRight = CurSkill.SecondaryColor,
                    };
                    Performance.BindValueChanged(c =>
                    {
                       this.TransformBindableTo(performPoints, c.NewValue, 1000, Easing.InOutQuint);
                    });
                    performPoints.Value = Performance.Value;
                    performPoints.BindValueChanged(pp =>
                    {
                        ppText = new SpriteText
                        {
                            Text = pp.NewValue.ToLocalisableString("0") + "pp/" + ((CurSkill.Level < SkillLevel.Mastery) ? CurSkill.Benchmarks.Mastery.ToString() : CurSkill.Benchmarks.Chosen.ToString()) + "pp",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 9.5f,
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
                            Strength = 30,
                            Colour = HorizontalGrad,
                            PadExtent = true,

                        }).WithEffect(new OutlineEffect
                        {
                            BlurSigma = new Vector2(0),
                            Strength = 1,
                            Colour = Colour4.White,
                            PadExtent = true,
                        });
                            Clear();
                        Add(
                            new Circle
                            {
                                Y = 12,
                                RelativeSizeAxes = Axes.X,
                                Height = 10,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = CurSkill.SecondaryColor,
                            });
                        Add(
                            new Circle
                            {
                                Y = 10,
                                RelativeSizeAxes = Axes.X,
                                Height = 10,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = CurSkill.PrimaryColor,
                            });
                        Add(ppText);
                        
                    }, true);
                }

                public void Appear()
                {
                    reloadPPText((int)CurSkill.SkillPP);
                    

                }
                private void reloadPPText(int pp)
                {
                    ColourInfo HorizontalGrad = new ColourInfo
                    {
                        TopLeft = CurSkill.PrimaryColor,
                        BottomLeft = CurSkill.PrimaryColor,
                        TopRight = CurSkill.SecondaryColor,
                        BottomRight = CurSkill.SecondaryColor,
                    };
                    Remove(ppText,true);
                    ppText = new SpriteText
                    {
                        Text = pp.ToLocalisableString("0") + "pp/" + ((CurSkill.Level < SkillLevel.Chosen) ? CurSkill.Benchmarks.Mastery.ToString() : CurSkill.Benchmarks.Chosen.ToString()) + "pp",
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
                        Strength = 30,
                        Colour = HorizontalGrad,
                        PadExtent = true,

                    }).WithEffect(new OutlineEffect
                    {
                        BlurSigma = new Vector2(0),
                        Strength = 1,
                        Colour = Colour4.White,
                        PadExtent = true,
                    });
                    Add(ppText);
                }
            }

            public Dictionary<Page, ClickableContainer> PageBubbleDict { get; private set; }

            public SkillInfo()
            {
                Origin = Anchor.Centre;
                Anchor = Anchor.Centre;

            }


            [BackgroundDependencyLoader]
            private void load(LargeTextureStore textures)
            {
                #region temp vars
                CircularProgress backprogress;
                CircularProgress learnerProg;
                CircularProgress experiencedProg;
                CircularProgress confidentProg;
                CircularProgress proficientProg;
                CircularProgress masteryProg;
                Container skillDescription;
                #endregion

                ColourInfo VerticalGrad = new ColourInfo
                {
                    TopLeft = CurSkill.PrimaryColor,
                    TopRight = CurSkill.PrimaryColor,
                    BottomLeft = CurSkill.SecondaryColor,
                    BottomRight = CurSkill.SecondaryColor,
                };
                ColourInfo HorizontalGrad = new ColourInfo
                {
                    TopLeft = CurSkill.PrimaryColor,
                    BottomLeft = CurSkill.PrimaryColor,
                    TopRight = CurSkill.SecondaryColor,
                    BottomRight = CurSkill.SecondaryColor,
                };

                InternalChild = new Container
                {
                    Size = new Vector2(130, 180),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    CornerRadius = 25,
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
                        new Box {
                            Size = new Vector2(350,220),
                            X = 40,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Colour = new ColourInfo{
                                TopLeft = Colour4.Black.MultiplyAlpha(0),
                                TopRight = Colour4.Black.MultiplyAlpha(0),
                                BottomLeft = Colour4.Black.MultiplyAlpha(0.7f),
                                BottomRight = Colour4.Black.MultiplyAlpha(0.7f),
                            }
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
                                    Colour = CurSkill.PrimaryColor,
                                    CornerRadius = 30,
                                    Alpha = 0.9f
                                },

                                // SkillText
                                new SpriteText {
                                    Text = CurSkill.Name,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Font = new FontUsage("VarelaRound", size: Math.Clamp(CurSkill.BoxNameSize/3.8f,0,23)),
                                    Colour = Colour4.White,
                                    Shadow =true,
                                    ShadowColour = CurSkill.PrimaryColor,
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
                                    Colour = ColourInfo.GradientHorizontal(CurSkill.PrimaryColor, CurSkill.SecondaryColor),
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

                        InfoBook = new Book {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Masking = true,
                            CornerRadius = 25,
                            Size = new Vector2(130, 180),
                            PageOffset = new Vector2(130,0),
                            Pages = new Page[] {
                                Page0 = new Page {
                                    Index = 0,
                                    Size = new Vector2(130, 180),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,

                                    Children = new Drawable[] {
                                        // Circular Progress Bar, PFP, Bubbles
                                        new Container {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.TopCentre,
                                            Origin = Anchor.Centre,
                                            Y = 60,
                                            Children = new Drawable[] {
                                                // Faded Circle Background
                                                new Circle {
                                                    Size = new Vector2(60,60),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = CurSkill.PrimaryColor,
                                                    Alpha = 0.1f
                                                },
                                                // SkillLevel CircularProgresses  
                                                new Container {
                                                    AutoSizeAxes = Axes.Both,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Children = new Drawable[] {
                                                        // Transparent Progress
                                                        backprogress = new CircularProgress{
                                                            Size = new Vector2(60,60),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            InnerRadius = 0.2f,
                                                            Colour = Colour4.FromHex("#5E5E5E"),
                                                            Alpha = 0.3f
                                                        },
                                                        // Background SkillLevel based progresses
                                                        masteryProg = new CircularProgress{
                                                            Size = new Vector2(60,60),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            InnerRadius = 0.2f,
                                                            Colour = Colour4.FromHex("#2613FF"),
                                                            Alpha = 0.4f
                                                        },
                                                        proficientProg = new CircularProgress{
                                                            Size = new Vector2(60,60),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            InnerRadius = 0.2f,
                                                            Colour = Colour4.FromHex("#FF1313"),
                                                            Alpha = 0.4f
                                                        },
                                                        confidentProg = new CircularProgress{
                                                            Size = new Vector2(60,60),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            InnerRadius = 0.2f,
                                                            Colour = Colour4.FromHex("#FFE072"),
                                                            Alpha = 0.4f
                                                        },
                                                        experiencedProg = new CircularProgress{
                                                            Size = new Vector2(60,60),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            InnerRadius = 0.2f,
                                                            Colour = Colour4.FromHex("#72D5FF"),
                                                            Alpha = 0.4f
                                                        },
                                                        learnerProg = new CircularProgress{
                                                            Size = new Vector2(60,60),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            InnerRadius = 0.2f,
                                                            Colour = Colour4.FromHex("#00FF19"),
                                                            Alpha = 0.4f
                                                        },


                                                        // Colored Progress
                                                        skillprogress = new CircularProgress{
                                                            Size = new Vector2(60,60),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            InnerRadius = 0.2f,
                                                            Colour = VerticalGrad
                                                        },
                                                    }
                                                },

                                                // Player PFP
                                                new Container {
                                                    AutoSizeAxes = Axes.Both,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Children = new Drawable[] {
                                                        // Border
                                                        pfpBorder1 = new Circle{
                                                            Size = new Vector2(37,37),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Colour = Colour4.Black.MultiplyAlpha(0.3f),
                                                            Alpha = (CurSkill.Level >= SkillLevel.Mastery) ? 1 : 0

                                                        },
                                                        pfpBorder2 = new Circle{
                                                            Size = new Vector2(34,34),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Colour = Colour4.FromHex("EDEDED"),
                                                            Alpha = (CurSkill.Level >= SkillLevel.Mastery) ? 0 : 1
                                                        },
                                                        // Chosen Progress
                                                        chosenprogress = new CircularProgress{
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
                                                                    Texture = textures.Get("avatar-guest")
                                                                }
                                                            }
                                                        }
                                                    }
                                                }


                                            }
                                        },

                                        // PP Bar
                                        ppBox = new ppBar {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            CurSkill = CurSkill,
                                        },

                                        // Rulesets
                                        new RulesetDisplay {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Scale = new Vector2(0.25f),
                                            Y = 29,
                                            RulesetList = CurSkill.GetSkillCalc(new Score() { }).SupportedRulesets
                                        },

                                        // Skill Description
                                        skillDescription = new Container {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Y = 45,
                                            Children = new Drawable[] {
                                            }
                                        },

                                
                                        // SkillLevel Display
                                        levelDisplay = new SkillLevelDisplay{
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Scale = new Vector2(0.25f),
                                            Y = 73,
                                            Skill = CurSkill
                                        }
                                    }
                                        },
                                Page1 = new Page {
                                    Index = 1,
                                    Name = "Page1",
                                    Size = new Vector2(130, 180),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,

                                    Children = new Drawable[]  {
                                        // PlayButton
                                        new BufferedContainer {
                                            Size = new Vector2(80, 55),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Masking = true,
                                            CornerRadius = 10,
                                            Y = -30,
                                            BorderThickness = 2,
                                            BorderColour = Colour4.AliceBlue,
                                            Children = new Drawable[] {
                                                new Sprite {
                                                    Size = new Vector2(1280/12, 720/12),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Texture = textures.Get("Contributors/osuphd"),
                                                },
                                                new Circle{
                                                    Size = new Vector2(22),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = Colour4.Orange,
                                                },
                                                new SpriteIcon{
                                                    Size = new Vector2(20),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = Colour4.White,

                                                    Icon = FontAwesome.Solid.ExclamationCircle
                                                },
                                                new SpriteText {
                                                    Anchor = Anchor.Centre,
                                                    Y = 15,
                                                    Origin = Anchor.Centre,
                                                    Colour = Colour4.White,
                                                    Font = new FontUsage("VarelaRound", size: 11),
                                                    Text = "Work In Progress",
                                                    ShadowColour = Colour4.Orange,
                                                    Shadow = true
                                                        
                                                }
                                                /* Play Button new Sprite{
                                                    X = 2,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Scale = new Vector2(0.04f),
                                                    Colour = Colour4.White,
                                                    Texture = textures.Get("FigmaVectors/PlayButton")
                                                
                                                }*/
                                            }
                                        },

                                        // "Created by" Text
                                        new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Y = 8,
                                            Text = "Created with care by:",
                                            Font = new FontUsage("VarelaRound", size: 10),
                                            Shadow = true,
                                            ShadowOffset = new Vector2(0, 0.05f),
                                            Spacing = new Vector2(0.1f, 0),
                                            Padding = new MarginPadding
                                            {

                                                Horizontal = 5,
                                            },

                                        }.WithEffect(new OutlineEffect
                                        {
                                            BlurSigma = new Vector2(0.5f),
                                            Strength = 5,
                                            Colour = Colour4.White,
                                            PadExtent = true,
                                        }).WithEffect(new GlowEffect
                                        {
                                            BlurSigma = new Vector2(0.5f),
                                            Strength = 5,
                                            Colour = ColourInfo.GradientHorizontal(CurSkill.PrimaryColor, CurSkill.SecondaryColor),
                                            PadExtent = true,

                                        }),

                                        // DigitalHypno's PFP
                                        new BufferedContainer {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Masking = true,
                                            CornerRadius = 10,
                                            Y = 40,
                                            Child = new Sprite {
                                                Size = new Vector2(50, 50),
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Texture = textures.Get("Contributors/DigitalHypno"),
                                            }
                                        },

                                        // DigitalHypno's username
                                        new SpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Y = 75,
                                            Text = "DigitalHypno",
                                            Font = new FontUsage("VarelaRound", size: 15),
                                            Colour = Colour4.White,
                                            Shadow = true,
                                            ShadowOffset = new Vector2(0, 0.05f),
                                            Spacing = new Vector2(0.1f, 0),
                                            Padding = new MarginPadding
                                            {

                                                Horizontal = 5,
                                            },

                                        }.WithEffect(new OutlineEffect
                                        {
                                            BlurSigma = new Vector2(0.5f),
                                            Strength = 5,
                                            Colour = Colour4.White,
                                            PadExtent = true,
                                        }).WithEffect(new GlowEffect
                                        {
                                            BlurSigma = new Vector2(0.5f),
                                            Strength = 5,
                                            Colour = ColourInfo.GradientHorizontal(CurSkill.PrimaryColor, CurSkill.SecondaryColor),
                                            PadExtent = true,

                                        }),
                                    }
                                },
                                Page2 = CurSkill.ContributorPage
                            }
                        },
                        
                        // Bubbles 
                        bubbleContainer = new Container {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            X = 41,
                            Y = -30,
                            Children = new Drawable[] {
                                Bubble1 = new ClickableContainer {
                                    Action = () => { InfoBook.CurrentPage.Value = 0; },
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                            X = -7,
                                            Y = -18,
                                    Children = new Circle[] {
                                        // Bubble1
                                        new Circle {
                                            Size = new Vector2(11.5f),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.White,
                                        },
                                        new Circle {
                                            Size = new Vector2(9),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = CurSkill.PrimaryColor,
                                        },
                                    }
                                },
                                Bubble2 = new ClickableContainer {
                                    Action = () => { InfoBook.CurrentPage.Value = 1; },
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Children = new Circle[] {
                                        // Bubble2 
                                        new Circle {
                                            Size = new Vector2(11.5f),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.White,
                                        },
                                        new Circle {
                                            Size = new Vector2(9),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.Gray
                                        }
                                    }
                                },
                                Bubble3 = new ClickableContainer {
                                    Action = () => { InfoBook.CurrentPage.Value = 2; },
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    X = -7,
                                    Y = 18,
                                    Children = new Circle[] {
                                        // Bubble3
                                        new Circle {
                                            Size = new Vector2(11.5f),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.White,
                                        },
                                        new Circle {
                                            Size = new Vector2(9),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Colour4.Gray,
                                        },
                                    }
                                }
                            }
                        },
                    }
                };

                InfoBook.CurrentPage.Value = 0;
                Page1.Hide();
                Page2.Hide();

                // Repositon Bubbles when the page is changed
                InfoBook.CurrentPage.ValueChanged += (ValueChangedEvent<int> value) =>
                {
                    // page0
                    if (value.NewValue == value.OldValue) return;

                    if (value.NewValue == 0)
                    {
                        Bubble1.MoveTo(new Vector2(-7,-18),500,Easing.InOutCubic);
                        Bubble2.MoveTo(new Vector2(0,0), 500, Easing.InOutCubic);
                        Bubble3.MoveTo(new Vector2(-7, 18), 500, Easing.InOutCubic);
                    }
                    // page1
                    if (value.NewValue == 1)
                    {
                        Bubble1.MoveTo(new Vector2(8, -20), 500, Easing.InOutCubic);
                        Bubble2.MoveTo(new Vector2(16, 0), 500, Easing.InOutCubic);
                        Bubble3.MoveTo(new Vector2(8, 20), 500, Easing.InOutCubic);

                    }
                    // page2
                    if (value.NewValue == 2)
                    {
                        Bubble1.MoveTo(new Vector2(-57, -27), 500, Easing.InOutCubic);
                        Bubble2.MoveTo(new Vector2(-40, -27), 500, Easing.InOutCubic);
                        Bubble3.MoveTo(new Vector2(-23, -27), 500, Easing.InOutCubic);

                    }
                };

                

                PageBubbleDict = new Dictionary<Page, ClickableContainer>
                {
                    {Page0,Bubble1},
                    {Page1,Bubble2},
                    {Page2,Bubble3}
                };

                #region set progresses
                backprogress.Current.Value = 1;
                learnerProg.Current.Value = (float)CurSkill.Benchmarks.Learner / CurSkill.Benchmarks.Mastery;
                experiencedProg.Current.Value = (float)CurSkill.Benchmarks.Experienced / CurSkill.Benchmarks.Mastery;
                confidentProg.Current.Value = (float)CurSkill.Benchmarks.Confident / CurSkill.Benchmarks.Mastery;
                proficientProg.Current.Value = (float)CurSkill.Benchmarks.Proficient / CurSkill.Benchmarks.Mastery;
                masteryProg.Current.Value = 1;
                #endregion


                int i = 0;
                foreach (string line in CurSkill.Summary.Split('\n'))
                {
                    skillDescription.Add(new SpriteText
                    {
                        Y = CurSkill.SummarySize * i * 0.9f,
                        Text = line,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Font = new FontUsage("VarelaRound", size: CurSkill.SummarySize),
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
                        Colour = ColourInfo.GradientHorizontal(CurSkill.PrimaryColor, CurSkill.SecondaryColor),
                        PadExtent = true,

                    }).WithEffect(new OutlineEffect
                    {
                        BlurSigma = new Vector2(0),
                        Strength = 5,
                        Colour = Colour4.White,
                        PadExtent = true,
                    })
                    );
                    i++;
                }
                
            }
            public void Appear(float delay = 0)
            {
                levelDisplay.Appear();
                levelDisplay.SwitchLevel(CurSkill.Level);
                ColourInfo HorizontalGrad = new ColourInfo
                {
                    TopLeft = CurSkill.PrimaryColor,
                    BottomLeft = CurSkill.PrimaryColor,
                    TopRight = CurSkill.SecondaryColor,
                    BottomRight = CurSkill.SecondaryColor,
                };
                skillprogress.Delay(300 + delay).FillTo((CurSkill.Level < SkillLevel.Mastery) ? (float)CurSkill.SkillPP / CurSkill.Benchmarks.Mastery : 1, 1000, Easing.InOutCubic);
                chosenprogress.Delay(1000 + delay).FillTo((CurSkill.Level >= SkillLevel.Mastery) ? Math.Clamp((float)CurSkill.SkillPP / CurSkill.Benchmarks.Chosen, 0,1) : 0, 1000, Easing.InOutCubic);
                ppBox.Performance.Value = (int)CurSkill.SkillPP;

                if (CurSkill.Level >= SkillLevel.Mastery)
                {
                    pfpBorder1.FadeIn(600, Easing.InOutSine);
                    pfpBorder2.FadeOut(600,Easing.InOutSine);
                }
            }

            public void Disappear(float delay = 0)
            {
                skillprogress.Delay(delay).FillTo(0, 0, Easing.InOutCubic);
                chosenprogress.Delay(delay).FillTo(0, 0, Easing.InOutCubic);
            }
            
        }

        
        public FullSkillBox(ISkill skill, SkillBox parentbox)
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            CurSkill = skill;
            ParentBox = parentbox;


        }
        public FullSkillBox()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            
            InternalChild = InnerBox = new Container
            {
                Size = new Vector2(350, 220),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,

                Masking = true,
                BorderThickness = ((CurSkill.Level < SkillLevel.Mastery) ? 0 : 3),
                BorderColour = new ColourInfo
                {
                    TopLeft = CurSkill.PrimaryColor,
                    BottomLeft = CurSkill.PrimaryColor,
                    TopRight = CurSkill.SecondaryColor,
                    BottomRight = CurSkill.SecondaryColor,
                },
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
                                Texture = textures.Get(CurSkill.Background),
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

                    // SkillInfo Box
                    InfoBox = new SkillInfo{CurSkill = CurSkill,},

                    // ScoreContainer Scrollbox
                    scoreContainer = new ScoreContainer {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        X = -80,
                        Skill = CurSkill
                    },
                    
                    // Back button
                    backBut = new ClickableContainer {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Position = new Vector2(-120,-98),
                        Scale = new Vector2(0.6f),
                        Name = "BackButton",
                        Children = new Drawable[] {
                            // Border
                            new Circle() {
                                Size = new Vector2(39,19),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Colour4.White
                            },
                            // Background
                            new Circle() {
                                Size = new Vector2(35,15),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Colour4.LightSlateGray
                            },

                            // Arrow
                            new Container {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Children = new Drawable[] {
                                    // Arrow stem
                                    new Circle() {
                                        Size = new Vector2(20,5),
                                        X = 0,
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                    },

                                    // Arrow Top Branch
                                    new Circle() {
                                        Rotation = 45,
                                        Size = new Vector2(10,5),
                                        Position = new Vector2(-16,2),
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.Centre,
                                    },

                                    // Arrow Bottom Branch
                                    new Circle() {
                                        Rotation = -45,
                                        Size = new Vector2(10,5),
                                        Position = new Vector2(-16,-2),
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.Centre,
                                    }
                                }
                            }
                        }
                    },
                }
            };
            backBut.Action = () => { ParentBox.TransitionToMini(); };
            
        }

        public void Appear(float delay = 0) {


            InfoBox.Appear(delay);
            scoreContainer.Appear(delay);
        }

        public void Disappear(float delay) {
            scoreContainer.HideScores(delay);
            InfoBox.Disappear(delay);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.IsPressed(osuTK.Input.Key.Escape) && ParentBox.State == SkillBoxState.FullBox) {
                ParentBox.TransitionToMini();
                return true;
            }
            return false;
        }
        



    }
}
