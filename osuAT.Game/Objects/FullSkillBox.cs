using System;
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
using osuAT.Game.Skills;
using osuAT.Game.Objects.Displays;
using osu.Framework.Input.Events;
using osuTK;

namespace osuAT.Game.Objects
{

    public class FullSkillBox : CompositeDrawable
    {

        public ISkill Skill;
        public SkillBox ParentBox;

        public Container InnerBox; 
        private ClickableContainer backBut;
        private SkillInfo skillInfo;
        private ScoreContainer scoreContainer;
        

        public class SkillInfo : CompositeDrawable
        {
            public ISkill Skill;
            public int TextSize; // Text Size

            public Texture Background; // Skill Background
            public int MiniHeight; // Minibox Height
            public SkillLevel Level { get; }
            public RulesetInfo[] SupportedRulesets = { RulesetStore.Osu };


            public Container InnerBox;
            private CircularProgress backprogress;
            private CircularProgress skillprogress;
            private Container skillDescription;
            private Container page1;

            private Circle bubble1;
            private Circle bubble2;
            private Circle bubble3;
            public SkillInfo()
            {
                Origin = Anchor.Centre;
                Anchor = Anchor.Centre;
            }

            [BackgroundDependencyLoader]
            private void load(LargeTextureStore textures)
            {
                CircularProgress learnerProg;
                CircularProgress experiencedProg;
                CircularProgress confidentProg;
                CircularProgress proficientProg;
                CircularProgress masteryProg;

                ColourInfo VerticalGrad = new ColourInfo
                {
                    TopLeft = Skill.PrimaryColor,
                    TopRight = Skill.PrimaryColor,
                    BottomLeft = Skill.SecondaryColor,
                    BottomRight = Skill.SecondaryColor,
                };
                ColourInfo HorizontalGrad = new ColourInfo
                {
                    TopLeft = Skill.PrimaryColor,
                    BottomLeft = Skill.PrimaryColor,
                    TopRight = Skill.SecondaryColor,
                    BottomRight = Skill.SecondaryColor,
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
                        page1 = new Container {
                            Size = new Vector2(130, 180),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,

                            Children = new Drawable[] {
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
                                            Colour = Skill.PrimaryColor,
                                            CornerRadius = 30,
                                            Alpha = 0.9f
                                        },

                                        // SkillText
                                        new SpriteText {
                                            Text = Skill.Name,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Font = new FontUsage("VarelaRound", size: Math.Clamp(Skill.BoxNameSize/3.8f,0,23)),
                                            Colour = Colour4.White,
                                            Shadow =true,
                                            ShadowColour = Skill.PrimaryColor,
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
                                            Colour = ColourInfo.GradientHorizontal(Skill.PrimaryColor, Skill.SecondaryColor),
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
                                                    Colour = Skill.PrimaryColor,
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
                                        // Faded Circle Background
                                        new Circle {
                                            Size = new Vector2(60,60),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Skill.PrimaryColor,
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
                                            Colour = Skill.SecondaryColor,
                                            Y = 2,
                                        },
                                        new Circle
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 10,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Skill.PrimaryColor,
                                        },
                                        new SpriteText {
                                            Text = (Math.Truncate(Skill.SkillPP)).ToString() + "pp/" + ((Skill.Level < SkillLevel.Chosen)? Skill.Benchmarks.Mastery.ToString() : Skill.Benchmarks.Chosen.ToString())+"pp",
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
                                        })
                    .WithEffect(new GlowEffect
                     {
                         BlurSigma = new Vector2(0.0f),
                         Strength = 0.4f,
                         Colour = HorizontalGrad,
                         PadExtent = true,

                     }),
                                    }
                                },

                                // Rulesets
                                new RulesetDisplay {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Scale = new Vector2(0.25f),
                                    Y = 29,
                                    RulesetList = new RulesetInfo[] {

                                        RulesetStore.Osu,
                                        RulesetStore.Mania,
                                        RulesetStore.Taiko,
                                        RulesetStore.Catch,
                                        RulesetStore.Osu,
                                    }
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
                                new SkillLevelDisplay{
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Scale = new Vector2(0.25f),
                                    Y = 73,
                                    Skill = Skill
                                }
                            }
                        }
                    }
                };
                backprogress.Current.Value = 1;
                learnerProg.Current.Value = (float)Skill.Benchmarks.Learner / Skill.Benchmarks.Mastery;
                experiencedProg.Current.Value = (float)Skill.Benchmarks.Experienced / Skill.Benchmarks.Mastery;
                confidentProg.Current.Value = (float)Skill.Benchmarks.Confident / Skill.Benchmarks.Mastery;
                proficientProg.Current.Value = (float)Skill.Benchmarks.Proficient / Skill.Benchmarks.Mastery;
                masteryProg.Current.Value = 1;
                if (Skill.SkillPP >= Skill.Benchmarks.Mastery)
                {

                }


                int i = 0;
                foreach (string line in Skill.Summary.Split('\n'))
                {
                    skillDescription.Add(new SpriteText
                    {
                        Y = Skill.SummarySize * i * 0.9f,
                        Text = line,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Font = new FontUsage("VarelaRound", size: Skill.SummarySize),
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
                        Colour = ColourInfo.GradientHorizontal(Skill.PrimaryColor, Skill.SecondaryColor),
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
            public void Appear(float delay)
            {
                skillprogress.Delay(300 + delay).FillTo((Skill.Level < SkillLevel.Mastery) ? (float)Skill.SkillPP / Skill.Benchmarks.Mastery : 1, 1000, Easing.InOutCubic);
                
            }

            public void Disappear(float delay)
            {
                skillprogress.Delay(delay).FillTo(0, 0, Easing.InOutCubic);
            }
            
        }

        
        public FullSkillBox(ISkill skill, SkillBox parentbox)
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            Skill = skill;
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
                BorderThickness = ((Skill.Level < SkillLevel.Mastery) ? 0 : 3),
                BorderColour = new ColourInfo
                {
                    TopLeft = Skill.PrimaryColor,
                    BottomLeft = Skill.PrimaryColor,
                    TopRight = Skill.SecondaryColor,
                    BottomRight = Skill.SecondaryColor,
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
                                Texture = textures.Get(Skill.Background),
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
                    skillInfo = new SkillInfo{Skill = Skill,},

                    // ScoreContainer Scrollbox
                    scoreContainer = new ScoreContainer {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        X = -80,
                        ScoreList = SaveStorage.GetTrickTopScoreList(Skill),
                        Skill = Skill
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
            backBut.Action = () => { ParentBox.TransitionToMini(); System.Console.WriteLine("hieoihjergji"); };
        }

        public void Appear(float delay) {


            skillInfo.Appear(delay);
            scoreContainer.Appear(delay);
        }

        public void Disappear(float delay) {
            scoreContainer.HideScores(delay);
            skillInfo.Disappear(delay);
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
