using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuAT.Game.UserInterface;
using osuAT.Game.Screens;
using osuTK;
using osuTK.Graphics;
using osuAT.Game.Objects;

namespace osuAT.Game
{


    public class SettingsButton : CompositeDrawable
    {
        private SettingsBox settingsBox;
        private HomeScreen mainScreen;
        public bool CanOpen = true;
        public bool SettingsOpen => settingsBox.Alpha > 0;

        public SettingsButton(HomeScreen mainScreen)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            this.mainScreen = mainScreen;
        }

        public SettingsButton()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }
        public void HideBox()
        {
            settingsBox.Hide();
        }

        public void ShowBox()
        {
            settingsBox.Show();
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            InternalChildren = new Drawable[]
            {
                new ClickableContainer{
                    AutoSizeAxes = Axes.Both,
                    Action = () => {
                        if (!(CanOpen)) return;
                        if (settingsBox.Alpha==0) {
                            if (mainScreen != null) { mainScreen.CurrentlyFocused = false; }
                            ShowBox(); return;
                        }
                        if (mainScreen != null) { mainScreen.CurrentlyFocused = true; }
                        HideBox();
                    }, 
                    Children = new Drawable[] {
                         new SpriteIcon {

                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            
                            Size = new Vector2(38,38),
                            Colour = Colour4.LightSlateGray,
                            Shadow = true,
                            ShadowOffset = new Vector2(0,4f),
                            ShadowColour = Colour4.Black,
                            Icon = FontAwesome.Solid.Cog,
                        },
                    },
                },
                settingsBox = new SettingsBox{ Anchor = Anchor.Centre,X = -250,Y = 400, Alpha = 0, BypassAutoSizeAxes = Axes.Both}
            };
            Size = InternalChildren[0].Size;
        }

        public class SettingsBox : CompositeDrawable
        {
            private bool force = false;
            private Container background;


            private SuperPasswordTextBox apikeyText;
            private ArrowedContainer langOption;
            private ArrowedContainer asiOption; // AutoScoreImportation Text


            public SettingsBox(bool forcecompletion = false)
            {
                force = forcecompletion;
                Origin = Anchor.Centre;
                Size = new Vector2(1000, 600);
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {

                InternalChildren = new Drawable[]
                {
                    background = new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(1000,600),
                        Masking = true,
                        CornerRadius = 20,
                        BorderThickness = 10,
                        BorderColour = Colour4.White,
                        Children = new Drawable[] {}
                    },

                    new Container{
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(1000,600),
                        Masking = true,
                        CornerRadius = 60,
                        BorderThickness = 15,
                        BorderColour = Colour4.White,
                        Children = new Drawable[] {
                            new Box {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.FromHex("FF56A2")
                            },
                            new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,60),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "API Key: "
                            },
                            apikeyText = new SuperPasswordTextBox
                            {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(216,65),
                                Colour = Colour4.White,
                                Masking =true,
                                CornerRadius = 20,
                                Size = new Vector2(500,50), 
                                Text = "",
                                TextFont = new FontUsage("VarelaRound", size: 50),
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                OnDefocus = () => {
                                   // check if it's a valid api key. if it is, display a checkmark
                                   // otherwise, display a red X
                                   // then, save it to savestorage.
                                   SaveStorage.SaveData.APIKey = apikeyText.Text;
                                }
                            },
                            new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,115),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "Language: "
                            },
                            new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,175),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "Automatic Score Importation: "
                            },
                            langOption = new ArrowedContainer{
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(260,115),
                                Objects = new Drawable[] {
                                    new SpriteText {
                                        Anchor = Anchor.TopLeft,
                                        Origin = Anchor.CentreLeft,
                                        Y = 28,
                                        Font = new FontUsage("VarelaRound", size: 50),
                                        Colour = Colour4.White,
                                        Shadow = true,
                                        ShadowOffset = new Vector2(0,0.1f),
                                        Text = "English"
                                    },
                                    new SpriteText {
                                        Anchor = Anchor.TopLeft,
                                        Origin = Anchor.CentreLeft,
                                        Y = 28,
                                        Font = new FontUsage("VarelaRound", size: 50),
                                        Colour = Colour4.White,
                                        Shadow = true,
                                        ShadowOffset = new Vector2(0,0.1f),
                                        Text = "None"
                                    },
                                }
                            },
                            asiOption = new ArrowedContainer{
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(610,175),
                                Objects = new Drawable[] {
                                    new SpriteText {
                                        Anchor = Anchor.TopLeft,
                                        Origin = Anchor.CentreLeft,
                                        Y = 28,
                                        Font = new FontUsage("VarelaRound", size: 50),
                                        Colour = Colour4.White,
                                        Shadow = true,
                                        ShadowOffset = new Vector2(0,0.1f),
                                        Text = "OFF"
                                    },
                                    new SpriteText {
                                        Anchor = Anchor.TopLeft,
                                        Origin = Anchor.CentreLeft,
                                        Y = 28,
                                        Font = new FontUsage("VarelaRound", size: 50),
                                        Colour = Colour4.White,
                                        Shadow = true,
                                        ShadowOffset = new Vector2(0,0.1f),
                                        Text = "ON"
                                    },
                                }
                            }
                        }

                    },


                    new Sprite {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Y = -280,
                        Scale = new Vector2(0.4f),
                        Texture = textures.Get("OATlogo"),
                    },
                };
            }

            protected override bool OnClick(ClickEvent e)
            {
                return true;
            }

            protected override bool OnDragStart(DragStartEvent e)
            {
                return true;
            }
        }

    }
}
