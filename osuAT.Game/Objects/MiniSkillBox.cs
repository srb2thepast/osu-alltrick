using System;
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
using osuAT.Game.Skills;
using osuTK;

namespace osuAT.Game.Objects
{

    public class MiniSkillBox : CompositeDrawable
    {

        public ISkill Skill;
        public SkillBox ParentBox;

        // private SkillContainer OverallContainer;
        
        private BufferedContainer miniBG;
        private ClickableContainer miniBox;
        private Container mainbox;
        private Container stars;
        private Container outerstars;
        private bool transitioning = false;
        public MiniSkillBox(ISkill skill,SkillBox parentbox)
        {
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
            Size = new Vector2(352, 224);

            Skill = skill;
            ParentBox = parentbox;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            var HSVPrime = Skill.PrimaryColor.ToHSV();
            Size = new Vector2(352, Skill.MiniHeight);
            InternalChild = miniBox = new ClickableContainer
            {
                Size = new Vector2(352, Skill.MiniHeight),
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Children = new Drawable[] {
                    new Container {
                        Size = new Vector2(352, Skill.MiniHeight),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[] {
                            
                            // MainBox
                            mainbox = new Container {
                                Size = new Vector2(352, Skill.MiniHeight),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,

                                Masking = true,
                                CornerRadius = 30,
                                Children = new Drawable[]
                                    {
                                        // White Box
                                        new Box
                                        {
                                            Size = new Vector2(352, Skill.MiniHeight),
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
                                                    Size = new Vector2(352, Skill.MiniHeight),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Texture = textures.Get(Skill.Cover),
                                                }.WithEffect(new GlowEffect
                                                {
                                                    // BlurSigma = new Vector2(1f),
                                                    Strength = 1,
                                                    Colour = new ColourInfo
                                                    {
                                                        BottomLeft = Colour4.Black,
                                                        TopRight = Colour4.DimGray,
                                                    },
                                                }),
                                                new Box {
                                                    Size = new Vector2(352  , Skill.MiniHeight),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Colour = new ColourInfo{
                                                        BottomLeft = Colour4.Black,
                                                        TopLeft = Colour4.Gray,
                                                        BottomRight = Colour4.Gray,
                                                        TopRight = Colour4.White,
                                                    },
                                                    Alpha = 0.3f
                                                },
                                            }
                                        },

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
                                                            Colour = Skill.SecondaryColor,
                                                            Y = 5 + (Skill.MiniHeight/224-1)*80,
                                                        },
                                                        new Circle
                                                        {
                                                            Size = new Vector2(267, 20),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Colour = Skill.PrimaryColor,
                                                            Y =  (Skill.MiniHeight/224-1)*80,

                                                        },
                                                    }
                                                },
                                                new SpriteText {

                                                    Text = Skill.Name,
                                                    Anchor = Anchor.Centre,
                                                    Y = -10 - (Skill.MiniHeight/264-1)*10,
                                                    Origin = Anchor.Centre,
                                                    Font = new FontUsage("VarelaRound", size: Skill.BoxNameSize),
                                                    Colour = Skill.PrimaryColor,
                                                    Shadow = true,
                                                    ShadowColour = Skill.SecondaryColor
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
                                                    Colour = ColourInfo.GradientHorizontal(Skill.PrimaryColor, Skill.SecondaryColor),
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
                                            Y = 86,
                                            Children = new Drawable[] {

                                                //new StarShad(textures,Skill.PrimaryColor, Skill.SecondaryColor, new Vector2(10, (Skill.MiniHeight/224-1)*100)) { Alpha = ((Skill.GetSkillPP() > Skill.Benchmarks.Learner)? 1 : 0) },
                                                //new StarShad(textures,Skill.PrimaryColor, Skill.SecondaryColor, new Vector2(55, (Skill.MiniHeight/224-1)*100)) { Alpha = ((Skill.GetSkillPP() > Skill.Benchmarks.Experienced)? 1 : 0) },
                                                //new StarShad(textures,Skill.PrimaryColor, Skill.SecondaryColor, new Vector2(100, (Skill.MiniHeight/224-1)*100)) { Alpha = ((Skill.GetSkillPP() > Skill.Benchmarks.Confident)? 1 : 0) },
                                                //new StarShad(textures,Skill.PrimaryColor, Skill.SecondaryColor, new Vector2(145, (Skill.MiniHeight/224-1)*100)) { Alpha = ((Skill.GetSkillPP() > Skill.Benchmarks.Proficient)? 1 : 0) },
                                                //new StarShad(textures,Skill.PrimaryColor, Skill.SecondaryColor, new Vector2(190, (Skill.MiniHeight/224-1)*100)) { Alpha = ((Skill.GetSkillPP() > Skill.Benchmarks.Mastery)? 1 : 0) },
                                            }
                                        },

                                    }
                            },

                            // OuterStars
                            outerstars = new Container {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                X = 96,
                                Y = 70,
                                Alpha = (Skill.GetSkillPP() >= Skill.Benchmarks.Chosen)? 1:0,
                                Children = new Drawable[] {
                                    new Sprite {
                                        Position = new Vector2(-200, -145),
                                        Size = new Vector2(70, 70),
                                        Texture = textures.Get("FigmaVectors/DiamondStar"),
                                        Colour = Skill.PrimaryColor
                                    }.WithEffect(new GlowEffect
                                    {
                                        BlurSigma = new Vector2(1f),
                                        Strength = 0.3f,
                                        Colour = Skill.PrimaryColor,
                                        PadExtent = true,
                                    }),
                                    new Sprite {
                                        Position = new Vector2(126, 58),
                                        Size = new Vector2(70, 70),
                                        Texture = textures.Get("FigmaVectors/DiamondStar"),
                                        Colour = Skill.SecondaryColor
                                    }.WithEffect(new GlowEffect
                                    {
                                        BlurSigma = new Vector2(1f),
                                        Strength = 0.3f,
                                        Colour = Skill.SecondaryColor.Darken(0.2f),
                                        PadExtent = true,

                                    }),

                                }
                            },
                        }
                    }
                }
            };
            miniBox.ScaleTo(0.5f);

            var skillPP = Skill.GetSkillPP();
            int lv = 0;
            if (skillPP > Skill.Benchmarks.Learner) { lv = 1; }
            if (skillPP > Skill.Benchmarks.Experienced) { lv = 2; }
            if (skillPP > Skill.Benchmarks.Confident) { lv = 3; }
            if (skillPP > Skill.Benchmarks.Proficient) { lv = 4; }
            if (skillPP > Skill.Benchmarks.Mastery) { lv = 5; } 
            Console.WriteLine(lv);

            for (int i = 0; i < lv; i++) {
                //stars.Add(new StarShad(textures, Skill.PrimaryColor, Skill.SecondaryColor,
                //    new Vector2(
                //         lv % 2 == 1 ? i * 45 + (-22.5f * (lv - 1)) : i * 45 + (-45 * (lv - 2) / 2 - 22.5f),
                //        (Skill.MiniHeight / 224 - 1) * 100
                //    )
               //));
            }

            miniBox.Action = () => {fireTransition();};
        }


        private class StarShad : Container
        {
            public Container StarShading;
            public StarShad(TextureStore textures,Colour4 MainColor, Colour4 ShadowColor, Vector2 Pos)
            {

                Child = StarShading = new Container
                {
                    Position = Pos,
                    Children = new Drawable[] {
                        new Sprite {
                            Y=5,
                            Origin = Anchor.Centre,
                            Size = new Vector2(40,40),
                            Texture = textures.Get("FigmaVectors/StarFull"),
                            Colour = ShadowColor,
                        },
                        new Sprite {
                            Origin = Anchor.Centre,
                            Size = new Vector2(40,40),
                            Texture = textures.Get("FigmaVectors/StarFull"),
                            Colour = MainColor,
                        },
                        new Sprite {
                            Origin = Anchor.Centre,
                            Size = new Vector2(43,43),
                            Texture = textures.Get("FigmaVectors/StarThin"),
                        },
                    }
                };

            }
        }
        protected override bool OnHover(HoverEvent e)
        {
            if (transitioning == true) base.OnHover(e);
            miniBox.Child.ScaleTo(1.1f, 200, Easing.Out);
            return base.OnHover(e);

        }
        protected override void OnHoverLost(HoverLostEvent e)
        {
            if (transitioning == true) return;
            miniBox.Child.ScaleTo(1f, 100, Easing.Out);
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {

            fireTransition();
            return true;
        }

        private void fireTransition() {
            if (ParentBox.ParentCont.FocusedBox == ParentBox && ParentBox.State == SkillBoxState.MiniBox)
            {
                transitioning = true;
                ParentBox.TransitionToFull();

            }
            else
            {
                ParentBox.ParentCont.FocusOnBox(ParentBox);
            }
        }

        public void Slideout()
        {
            mainbox.Delay(300).MoveToX(358, 600, Easing.InOutCubic);
            outerstars[0].Delay(500).MoveTo(new Vector2(0, 0), 600, Easing.InOutCubic);
            outerstars[1].Delay(500).MoveTo(new Vector2(0, 0), 600, Easing.InOutCubic);
            outerstars.Delay(500).MoveTo(new Vector2(148, -100), 600, Easing.InOutCubic);
            outerstars.Delay(500).ScaleTo(0.5f, 600, Easing.InOutCubic);
            mainbox.Delay(700).FadeOut();
        }

        public void Slidein()
        {
            mainbox.Delay(0).FadeIn();
            mainbox.Delay(300).MoveToX(0, 600, Easing.InOutCubic);
            outerstars[0].Delay(500).MoveTo(new Vector2(-200, -145), 600, Easing.InOutCubic);
            outerstars[1].Delay(500).MoveTo(new Vector2(126, 58), 600, Easing.InOutCubic);
            outerstars.Delay(500).MoveTo(new Vector2(96, 70), 600, Easing.InOutCubic);
            outerstars.Delay(500).ScaleTo(1, 600, Easing.InOutCubic);
        }

        protected override bool OnDoubleClick(DoubleClickEvent e)
        {
            System.Console.WriteLine("hihi");

            return base.OnDoubleClick(e);
        }

    }
}
