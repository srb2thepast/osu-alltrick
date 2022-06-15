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
using osuTK;

namespace osuAT.Game.Objects
{
    public class SimpleSkillBox : CompositeDrawable
    {
        public enum State
        {
            MiniBox = 1,
            FullBox = 2
        }

        public string SkillName = "Empty Skill";
        public Colour4 SkillPrimaryColor = Colour4.Purple;
        public Colour4 SkillSecondaryColor = Colour4.Black;
        public int HScale = 100;
        public State Status = State.MiniBox;
        public int TextSize = 83;
        private Sprite miniBG;
        private Container box;
        private Container stars;

        public SimpleSkillBox()
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
                CornerRadius = 50,
                
                Children = new Drawable[]
                    {

                        
                        new Box
                        {
                            Size = new Vector2(352,224),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Colour = Colour4.White,

                            
                        },
                        new BufferedContainer {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children = new Drawable[] {

                                miniBG = new Sprite {
                                    Size = new Vector2(352,224),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Texture = textures.Get("TestBG"),
                                    Alpha = 1f,
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
                                            Size = new Vector2(267,20),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = SkillSecondaryColor,
                                            Y = 5,
                                        },
                                        new Circle
                                        {
                                            Size = new Vector2(267,20),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = SkillPrimaryColor,

                                        },
                                    }
                                },
                                new SpriteText {

                                    Text = SkillName,
                                    Anchor = Anchor.Centre,
                                    Y = -10,
                                    Origin = Anchor.Centre,
                                    Font = new FontUsage("VarelaRound",size:TextSize), // FontUsage.Default.With(size:80)
                                    Colour = SkillPrimaryColor + (Colour4.White/2),
                                    Shadow = true,
                                    ShadowColour = SkillSecondaryColor
                                    //Padding = new MarginPadding
                                    //{
                                    //    Horizontal = 15,
                                    //    Vertical = 1
                                    //},
                                    // 

                                }.WithEffect(new GlowEffect
                                {
                                    BlurSigma = new Vector2(2f),
                                    Strength = 5f,
                                    Colour = ColourInfo.GradientHorizontal(SkillPrimaryColor, SkillSecondaryColor),
                                    PadExtent = true,

                                }).WithEffect(new OutlineEffect
                                {
                                BlurSigma = new Vector2(3f),
                                Strength = 4f,
                                Colour = Colour4.White,
                                PadExtent = true,
                                }),
                                
                            },
                            
                            
                        },
                        stars = new Container {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Y = 66,
                                    X = -120,
                                    Children = new Drawable[] {

                                        new StarShad (SkillPrimaryColor,SkillSecondaryColor,new Vector2(10,0)),
                                        new StarShad (SkillPrimaryColor,SkillSecondaryColor,new Vector2(55,0)),
                                        new StarShad (SkillPrimaryColor,SkillSecondaryColor,new Vector2(100,0)),
                                        new StarShad (SkillPrimaryColor,SkillSecondaryColor,new Vector2(145,0)),
                                        new StarShad (SkillPrimaryColor,SkillSecondaryColor,new Vector2(190,0)),
                                    }
                                }
                    }
            };
        }
        private class Icon : Container, IHasTooltip
        {
            public LocalisableString TooltipText { get; }

            public SpriteIcon SpriteIcon { get; }

            public Icon(string name, IconUsage icon)
            {
                TooltipText = name;

                Child = SpriteIcon = new SpriteIcon
                {
                    Icon = icon,
                    Size = new Vector2(60),
                };
            }
        }

        private class StarShad : Container
        {
            public Container StarShading;
            public StarShad(Colour4 MainColor, Colour4 ShadowColor,Vector2 Pos)
            {

                Child = StarShading = new Container {
                    Position = Pos,
                    Children = new Drawable[] {
                        new SpriteIcon {
                            Y=4,
                            Size = new Vector2(40,40),
                            Icon = FontAwesome.Solid.Star,
                            Colour = ShadowColor.Lighten(0.4f),
                        },
                        new SpriteIcon {
                            Y = -9,
                            X = -6,
                            Size = new Vector2(40,40),
                            Icon = FontAwesome.Solid.Star,
                            Colour = MainColor,
                        }.WithEffect(new OutlineEffect
                        {
                        BlurSigma = new Vector2(3f),
                        Strength = 1f,
                        Colour = ShadowColor,
                        PadExtent = true,
                        }),
                    }
                };
            }
        }


        protected override void LoadComplete()
        {
            base.LoadComplete();

        }

    }
}
