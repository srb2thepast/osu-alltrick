using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Effects;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osu.Framework.Graphics.Containers;
using osuTK;
using osuTK.Graphics;
using osuAT.Game.Objects;

namespace osuAT.Game.Screens
{
    public class HomeScreen : Screen
    {
        private Drawable background;
        public SpriteText UsernameText;
        public SkillContainer SkillCont;
        public Container TopBar;
        public bool CurrentlyFocused = true;

        private bool allowbuttons = false;
        public bool AllowButtons
        {
            get => allowbuttons;
            set {
                SetButton.CanOpen = value;
                if (!value)
                {
                    SetButton.HideBox();
                }
            }
        }

        public SettingsButton SetButton;
        

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {

            InternalChildren = new Drawable[]
            {
                background = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[] {
                        new Box {
                            Colour = Color4.Violet,
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },
                SkillCont = new SkillContainer(this),


                TopBar = new Container {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.Centre,
                        Y = 60,
                        Scale = new Vector2(0.8f,0.8f),

                        Children = new Drawable[] {

                            // Username Section Background
                            new Circle {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(601,63),
                                Y = 60
                            },
                        


                            // Title Background 
                            new Circle {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(1110,93),
                            },
                            new Circle {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(1100,83),
                                Colour = Colour4.FromHex("#FF59A4")
                            },
                            /////////////

                            // PFP
                            new Circle {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(100,100)

                            },
                            new Circle {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(90,90),
                                Colour = Colour4.FromHex("#FF66AB")

                            },
                            new CircularContainer {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(85,85),
                                Masking = true,
                                Child = new Sprite{
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Texture = textures.Get(@"avatar-guest"),
                                    Size = new Vector2(87,87)
                                }
                            },
                            /////////////
                        
                            // Title text
                            new SpriteText {
                                Text = "osu!alltrick",
                                Anchor = Anchor.Centre,
                                Origin = Anchor.CentreLeft,
                                Spacing = new Vector2(16),
                                Y = -3,
                                X = -500,
                                Font = FontUsage.Default.With(size: 60),

                            },
                            new SpriteText {
                                Text = "version 0.15.5",
                                Anchor = Anchor.Centre,
                                Origin = Anchor.CentreLeft,
                                Spacing = new Vector2(10),
                                Y = -3,
                                X = 70,
                                Font = FontUsage.Default.With(size: 60),

                            },

                            // Username text
                            UsernameText = new SpriteText {
                                Text = SaveStorage.SaveData.PlayerUsername,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Y = 65,
                                Font = FontUsage.Default.With(size: 40),
                                Colour = Colour4.FromHex("#FF66AB"),
                            },
                            new Sprite{
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Y = 80,
                                X = -350,
                                Size = new Vector2(50),
                                Texture = textures.Get("FigmaVectors/StarFull")
                            },
                            new Sprite{
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Y = 80,
                                X = 350,
                                Size = new Vector2(50),
                                Texture = textures.Get("FigmaVectors/StarFull")
                            },
                            // Buttons
                            SetButton = new SettingsButton(this) {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Y = 65,
                                X = 250,
                            },
                        }
                    },

                

            };

        }

        private class HomeBackground : Container {
            private Container box;

            public HomeBackground()
            {
                AutoSizeAxes = Axes.Both;
                Origin = Anchor.Centre;
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                InternalChild = box = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                        },
                        new Sprite
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Texture = textures.Get("logo")
                        },
                    }
                };
            }   

            protected override void LoadComplete()
            {
                base.LoadComplete();
                box.Loop(b => b.RotateTo(0).RotateTo(360, 2500));
            }
        }

        
        
    }
}
