using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Effects;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osu.Framework.Graphics.Containers;
using osu.Framework.Audio;
using osuTK;
using osuTK.Graphics;
using osuAT.Game.Objects;
using osu.Framework.Graphics.UserInterface;
using static osuAT.Game.SettingsButton;
using osu.Framework;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;

namespace osuAT.Game.Screens
{
    public partial class HomeScreen : Screen
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
                ImpButton.CanOpen = value;
                if (!value)
                {
                    SetButton.HideBox();
                    ImpButton.HideBox();
                }
                allowbuttons = value;
            }
        }

        [Resolved]
        private AudioManager audio { get; set; }

        public SettingsButton SetButton;
        public ImportButton ImpButton;


        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures,Texture defaultPFP)
        {
            System.Console.WriteLine(audio);
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
                        // Exit Button
                         new ExitButton {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.Centre,
                            X = 60
                        },

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
                                Texture = defaultPFP,
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
                            Text = Updater.DevelopmentBuild? Updater.CurrentVersion.Split("v")[1] : $"version {Updater.CurrentVersion.Split("v")[1]}",
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
                        ImpButton = new ImportButton(this) {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 65,
                            X = -250,
                        },
                        SetButton = new SettingsButton(this) {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 65,
                            X = 250,
                        },
                    }
                },
            };

           SettingsButton.UsernameChanged += () =>
            {
                UsernameText.Text = SaveStorage.SaveData.PlayerUsername;
            };
        }

        private partial class ExitButton : CompositeDrawable, IHasTooltip
        {
            public LocalisableString TooltipText { get; }
            public ExitButton()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                Size = new Vector2(100);
                TooltipText = "Exit osu!alltrick";
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                InternalChildren = new Drawable[]
                {
                    new ClickableContainer{
                        AutoSizeAxes = Axes.Both,
                        Action = () => {
                            SaveStorage.Save();
                            Dependencies.Get<osu.Framework.Game>().RequestExit();
                        },
                        Children = new Drawable[] {
                            new Sprite{
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(100),
                                Texture = textures.Get("FigmaVectors/DiamondStar")
                            },
                            new SpriteIcon {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Rotation = 45,
                                Size = new Vector2(45),
                                Colour = Colour4.FromHex("#FF66AB"),
                                Shadow = true,
                                ShadowOffset = new Vector2(2f),
                                ShadowColour = Colour4.Black.MultiplyAlpha(0.5f),
                                Icon = FontAwesome.Solid.PlusCircle,
                            },
                        },
                    },
                };
                Size = InternalChildren[0].Size;
            }
        }

        private partial class HomeBackground : Container {
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
