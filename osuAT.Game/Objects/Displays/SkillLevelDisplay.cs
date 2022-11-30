using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Effects;
using osuAT.Game.Types;
using osuTK;
using osuAT.Game.Skills.Resources;

namespace osuAT.Game.Objects.Displays
{
    public partial class SkillLevelDisplay : CompositeDrawable
    {
        private CircularContainer maincont;
        private CircularContainer levelContainer;
        private CircularContainer container;
        private LargeTextureStore texturecache;

        private string levelText { get {
                if ((int)Skill.Level == 1) return "Learner";
                if ((int)Skill.Level == 2) return "Experienced";
                if ((int)Skill.Level == 3) return "Confident";
                if ((int)Skill.Level == 4) return "Proficent";
                if ((int)Skill.Level == 5) return "Mastery";
                if ((int)Skill.Level == 6) return "Chosen";
                return "None";
            }
        }
        public ISkill Skill;
        public SkillLevelDisplay()
        {
            AutoSizeAxes = Axes.Both;
            Origin = Anchor.Centre;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            texturecache = textures;
            SwitchLevel(Skill.Level);
            // Position the circles



        }

        private class CircleShad : Container
        {
            public ColourInfo MainColour = Colour4.White;
            public ColourInfo ShadowColour = Colour4.Gray;
            public Vector2 Spacing = new Vector2(0,4);
            public new Vector2 Size;
            public CircleShad()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                AutoSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load() {
                Child = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[] {
                        new Circle {
                            Position = Spacing,
                            BypassAutoSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = Size,
                            Colour = ShadowColour,
                        },
                        new Circle {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = Size,
                            Colour = MainColour,
                        }
                    }
                };
            }
        }

        public void SwitchLevel(SkillLevel level)
        {
            ClearInternal();
            if (level == SkillLevel.None)
            {

                InternalChild = container = new CircularContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    Size = new Vector2(398, 93)
                };
                return;
            }
            InternalChild = container = new CircularContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                Size = new Vector2(398, 93),
                Children = new Drawable[] {
                    new Circle {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Y = 8,
                        Colour = Skill.SecondaryColor.Lighten(10),
                        RelativeSizeAxes = Axes.X,
                        Height = 73,
                    },

                    maincont = new CircularContainer {
                        RelativeSizeAxes = Axes.X,
                        Height = 73,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            // Circle Gradient
                            new Circle
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = ColourInfo.GradientHorizontal(Skill.PrimaryColor, Skill.SecondaryColor),
                                RelativeSizeAxes = Axes.Both,
                            },

                            // Background image
                            new Sprite {
                                Position = Skill.BadgePosSize.Item1,
                                Size = Skill.BadgePosSize.Item2,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Texture = texturecache.Get(Skill.BadgeBG),
                                Alpha = 0.4f
                            },

                            // Container
                            levelContainer = new CircularContainer {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Children = new Drawable[] {
                                    new SpriteText {
                                        Text = levelText,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Font = new FontUsage("AveriaSansLibre", size: 70),
                                        Colour = Skill.PrimaryColor.Lighten(0.3f),
                                        Shadow =true,
                                        ShadowColour = Colour4.Gray,

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
                                        Strength = 0.4f ,
                                        Colour = Colour4.Black,
                                        PadExtent = true,
                                    }),
                                }
                            }

                        }
                    },
                }
            };

            // position circles
            CircularContainer circlecont = new CircularContainer
            {
                Size = new Vector2(378, 73),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true
            };
            float xdif = 0;

            switch (level) {
                case SkillLevel.Learner:
                    xdif = 120;
                    break;

                case SkillLevel.Experienced:
                    xdif = 170;
                    container.Size = new Vector2(428, 93);
                    circlecont.Size = new Vector2(428, 93);
                    break;

                case SkillLevel.Confident:
                    xdif = 145;
                    break;
                case SkillLevel.Proficient:
                    xdif = 135;
                    break;
                case SkillLevel.Mastery:
                    xdif = 120;
                    container.Size = new Vector2(438, 93);
                    circlecont.Size = new Vector2(438, 93);
                    circlecont.Add(new CircleShad
                    {
                        X = -xdif-30,
                        Spacing = new Vector2(0, 2.5f),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        MainColour = Skill.PrimaryColor,
                        ShadowColour = Skill.SecondaryColor + Colour4.Gray,
                        Size = new Vector2(15, 15)
                    });
                    circlecont.Add(new CircleShad
                    {
                        X = xdif+30,
                        Spacing = new Vector2(0, 2.5f),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        MainColour = Skill.SecondaryColor,
                        ShadowColour = Skill.SecondaryColor + Colour4.Gray,
                        Size = new Vector2(15,15)
                    });
                    break;
                case SkillLevel.Chosen:
                    xdif = 115;
                    container.Size = new Vector2(438, 93);
                    circlecont.Size = new Vector2(438, 93);
                    
                    for (int i = 1; i < 7; i++)
                    {
                        circlecont.Add(new CircleShad
                        {
                            X = (((i % 2 == 0) ? -1 : 1) * (10 + xdif + 20 * ((i % 2 == 0) ? i / 2 : i / 2 + 1))),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            MainColour = (i % 2 == 0) ? Skill.PrimaryColor : Skill.SecondaryColor,
                            ShadowColour = Skill.SecondaryColor + Colour4.Gray,
                            Size = new Vector2(15, 15)
                        });
                        circlecont.Add(new Sprite
                        {
                            X = (((i % 2 == 0) ? -1 : 1) * (10 + xdif + 20 * ((i % 2 == 0) ? i / 2 : i / 2 + 1))),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Texture = texturecache.Get("FigmaVectors/SmallDiamondStar"),
                            Size = new Vector2(10, 10)
                        });
                    }
                    break;
            }

            Vector2 circlesize = Skill.Level == SkillLevel.Chosen ? new Vector2(35, 35) : new Vector2(25, 25);


            for (int i = 0; i < 2; i++) {
                circlecont.Add(new CircleShad
                {
                    X = ((i % 2 == 0) ? -1 : 1) * xdif,
                    Spacing = new Vector2(0, 5),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    MainColour = (i % 2 == 0) ? Skill.PrimaryColor : Skill.SecondaryColor,
                    ShadowColour  = Skill.SecondaryColor + Colour4.Gray,
                    Size = circlesize
                });
                if (Skill.Level == SkillLevel.Chosen)
                {
                    circlecont.Add(new Sprite
                    {
                        X = ((i % 2 == 0) ? -1 : 1) * xdif,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Texture = texturecache.Get("FigmaVectors/StarThick"),
                        Colour = Colour4.GhostWhite,
                        Size = new Vector2(27, 27)
                    });
                    circlecont.Add(new Sprite
                    {
                        X = ((i % 2 == 0) ? -1 : 1) * xdif,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Texture = texturecache.Get("FigmaVectors/StarThick"),
                        Size = new Vector2(25, 25)
                    });
                }
            }

            maincont.Add(circlecont);
        }
        public void Appear()
        {
            container.Y = 0;
            container.Alpha = 1;
        }

        public void Disappear()
        {
            container.FadeOut(500, Easing.InOutCubic);
            container.Children[0].Delay(100).FadeOut(500, Easing.InOutCubic);
            container.MoveToY(20, 800, Easing.InOutCubic);
        }



        protected override void LoadComplete()
        {
            base.LoadComplete();

        }
    }
}
