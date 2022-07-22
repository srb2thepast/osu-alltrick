using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Effects;
using osuAT.Game.Skills;
using osuTK;
namespace osuAT.Game.Objects.Displays
{
    public class SkillLevelDisplay : CompositeDrawable
    {

        private Container rulesetContainer;
        private CircularContainer container;
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
            InternalChild = container = new CircularContainer {
                Masking = true,
                Size = new Vector2(378, 93),
                Children = new Drawable[] {
                    new Circle {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Y = 8,
                        Colour = Skill.SecondaryColor.Lighten(10),
                        Size = new Vector2(378, 73),
                    },

                    new CircularContainer {
                        Size = new Vector2(378, 73),
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
                                Size = new Vector2(378, 73),
                            },

                            // Background image
                            new Sprite {
                                Size = new Vector2(700,440),
                                X = -40,
                                Y = 80,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Texture = textures.Get("TestBG2"),
                                Alpha = 0.4f
                            },
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
                            // Container
                            rulesetContainer = new Container{
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            }

                        }
                    },
                }
            };

            // Position the circles
            new Container
            {
                Size = new Vector2(378, 73),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                Children = new Drawable[] {
                    new Circle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = ColourInfo.GradientHorizontal(Skill.PrimaryColor, Skill.SecondaryColor),
                        Size = new Vector2(40,40),
                    },
                    new Circle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = ColourInfo.GradientHorizontal(Skill.PrimaryColor, Skill.SecondaryColor),
                        Size = new Vector2(40,40),
                    },
                }
            };

            container.FadeIn(500, Easing.InOutCubic);
            container.Children[0].Delay(100).FadeIn(500, Easing.InOutCubic);
            container.MoveToY(0,800, Easing.InOutCubic);
        }

        public void Appear()
        {
            container.Y = 20;
            container.Alpha = 0;
            container.Children[0].Alpha = 0;
            container.FadeIn(500, Easing.InOutCubic);
            container.Children[0].Delay(100).FadeIn(500, Easing.InOutCubic);
            container.MoveToY(0, 800, Easing.InOutCubic);
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
