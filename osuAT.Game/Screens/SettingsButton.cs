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
using OsuApiHelper;
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
            private SuperTextBox usernameText;
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

                            // Api Key
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
                            },

                            // Username
                            new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,115),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "Username:"
                            },

                            usernameText = new SuperTextBox
                            {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(256,110),
                                Colour = Colour4.White,
                                Masking =true,
                                CornerRadius = 20,
                                Size = new Vector2(500,50),
                                Text = "",
                                TextFont = new FontUsage("VarelaRound", size: 50),
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                            },

                            // Language
                            new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,170),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "Language: "
                            },
                            langOption = new ArrowedContainer{
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(260,170),
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

                            // ASI Option
                            new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,230),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "Automatic Score Importation: "
                            },
                            asiOption = new ArrowedContainer{
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(610,230),
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
                            },

                            // Credit
                            new SpriteText {
                                Anchor = Anchor.BottomRight,
                                Origin = Anchor.Centre,
                                Position = new Vector2(-380,-90),
                                Font = new FontUsage("VarelaRound", size: 40),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "o!at - created by srb2thepast"
                            },
                            new SpriteText {
                                Anchor = Anchor.BottomRight,
                                Origin = Anchor.Centre,
                                Position = new Vector2(-380,-55),
                                Font = new FontUsage("VarelaRound", size: 40),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "inspired by digitalhypno's osu!phd"
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
                apikeyText.Text = SaveStorage.SaveData.APIKey;
                usernameText.Text = SaveStorage.SaveData.PlayerUsername;

                apikeyText.OnCommit += new TextBox.OnCommitHandler((TextBox box, bool target) => {

                    // check if it's a valid api key. if it is, display a checkmark
                    // then, save it to savestorage.
                    // otherwise, display a red X
                    OsuApiKey.Key = apikeyText.Text;
                    if (OsuApi.IsKeyValid()) {
                        SaveStorage.SaveData.APIKey = apikeyText.Text;
                        OsuApiKey.Key = apikeyText.Text;
                        apikeyText.FlashColour(Color4.Green, 3000, Easing.InOutCubic);
                        // Set UserID
                        OsuUser player = OsuApi.GetUser(usernameText.Text);
                        if (player == default)
                        {
                            usernameText.FlashColour(Color4.Red, 3000, Easing.InOutCubic);
                            return;
                        }
                        SaveStorage.SaveData.PlayerUsername = usernameText.Text;
                        return;
                    }
                    OsuApiKey.Key = SaveStorage.SaveData.APIKey;
                    apikeyText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);

                });
                usernameText.OnCommit += new TextBox.OnCommitHandler((TextBox box, bool target) =>
                {
                    if (OsuApi.IsKeyValid())
                    {
                        OsuUser player = OsuApi.GetUser(usernameText.Text);
                        if (player == default)
                        {
                            usernameText.FlashColour(Color4.Red, 3000, Easing.InOutCubic);
                            return;
                        }
                        SaveStorage.SaveData.PlayerID = player.ID;
                        System.Console.WriteLine(OsuApi.GetUser(usernameText.Text).CountryCode);
                    }
                    SaveStorage.SaveData.PlayerUsername = usernameText.Text;
                    usernameText.FlashColour(Color4.Green, 3000, Easing.InOutCubic);    
                });
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
