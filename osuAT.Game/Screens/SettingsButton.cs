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
using System;
using System.Runtime.CompilerServices;
using Markdig.Extensions.SelfPipeline;
using System.IO;
using osu.Framework.Bindables;
using osu.Game.Overlays.BeatmapListing;
using System.Linq;
using osu.Framework.Extensions.ObjectExtensions;

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
            UsernameChanged += () =>
            {

            };
        }

        public static event Action UsernameChanged;
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
            private SpriteText usernameDisplayText;
            private SuperTextBox usernameText;
            private ArrowedContainer langOption;
            private ArrowedContainer asiOption; // AutoScoreImportation Text
            private FileSelector fileSelect;
            private Container fileSelectContainer;
            private SpriteText locationText;

            public class OsuATButton : BasicButton
            {

                [BackgroundDependencyLoader]
                private void load()
                {

                }
                protected override SpriteText CreateText() => new SpriteText
                {
                    Depth = -1,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Font = new FontUsage("VarelaRound", size: 45),
                    Colour = Colour4.White,
                    Shadow = true,
                    ShadowOffset = new Vector2(0, 0.1f),
                };
            }

            // [!] add exit button
            private class osuATFileSelector : BasicFileSelector {
                public osuATFileSelector() {
                    ShowHiddenItems.Value = true;
                    CurrentPath.ValueChanged += (ValueChangedEvent<DirectoryInfo> pathinfo) =>
                    {
                        foundexe = false;
                        foundsongsfolder = false;
                        if (selectButton != default)
                        {
                            RemoveInternal(selectButton, true);
                            selectButton = default;
                        }
                    };
                }

                [BackgroundDependencyLoader]
                private void load() {
                    AddInternal(new OsuATButton()
                    {
                        Masking = true,
                        CornerRadius = 10,
                        Anchor = Anchor.Centre,
                        Text = "Exit",
                        Position = new Vector2(350, -290),
                        Size = new Vector2(80, 50),
                        Scale = new Vector2(0.95f),
                        BackgroundColour = Colour4.LightPink,
                        FlashColour = Colour4.Black,
                        Action = OnExitClicked
                    });
                }

                public Action OnOsuPathFound = () => { };
                public Action OnExitClicked = () => { };
                private bool foundexe = false;
                private bool foundsongsfolder = false;
                private Button selectButton;

                protected override DirectorySelectorDirectory CreateDirectoryItem(DirectoryInfo directory, string displayName = null)
                {
                    if (directory.Name == "Songs") {
                        foundsongsfolder = true;
                        Console.WriteLine("songs found");
                    }
                    CheckButtonFound();
                    return new BasicDirectorySelectorDirectory(directory, displayName);
                }

                protected override DirectoryListingFile CreateFileItem(FileInfo file) {
                    var fileitem = base.CreateFileItem(file);
                    if (file.Name == "osu!.exe")
                    {
                        foundexe = true;
                        Console.WriteLine("osu! found");
                    }
                    CheckButtonFound();
                    return fileitem;
                }

                protected void CheckButtonFound() {
                    if (foundexe && foundsongsfolder && selectButton == default)
                    {
                        AddInternal(selectButton = new OsuATButton
                        {
                            Masking = true,
                            CornerRadius = 10,
                            Anchor = Anchor.BottomRight,
                            Text = "Select this directory",
                            Position = new Vector2(-300, -100),
                            Size = new Vector2(330, 60),
                            Scale = new Vector2(0.75f),
                            BackgroundColour = Colour4.Plum,
                            FlashColour = Colour4.Goldenrod,
                            Margin = new MarginPadding(10),
                            Action = OnOsuPathFound
                        });
                        selectButton.Enabled.Value = true;
                        Console.WriteLine(CurrentFile);
                        Console.WriteLine(CurrentPath);
                        Console.WriteLine("both found");
                    }
                }

                protected override Drawable CreateHiddenToggleButton() => Empty();
            }

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
                                CornerRadius = 5,
                                Size = new Vector2(500,50),
                                Text = "",
                                TextFont = new FontUsage("VarelaRound", size: 50),
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                            },

                            // Username
                            usernameDisplayText = new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,115),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = Colour4.Gray,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "Username:"
                            },

                            usernameText = new SuperTextBox
                            {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(256,110),
                                Colour = Colour4.Gray,
                                Masking =true,
                                CornerRadius = 5,
                                Size = new Vector2(500,50),
                                Text = "Please set your api key first.",
                                TextFont = new FontUsage("VarelaRound", size: 50),
                                Shadow = true,
                                ReadOnly = ApiScoreProcessor.ApiKeyValid? false : true,
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
                                Text = "Auto Score Importing: "
                            },
                            asiOption = new ArrowedContainer{
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(470,230),
                                Objects = new Drawable[] {
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
                                },
                                OnObjectSwitched = delegate(Drawable focusedObject) {
                                    if (focusedObject is SpriteText textObject) {
                                        ScoreImporter.Enabled = textObject.Text == "ON";
                                    }
                                }
                            },
                            // Path option
                            locationText = new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,290),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = (SaveStorage.OsuPathIsValid())? Colour4.White : Colour4.Red,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = $"osu! path: {SaveStorage.SaveData.OsuPath}",
                            },
                            new OsuATButton {
                                Masking = true,
                                CornerRadius = 15,
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Size = new Vector2(150,50),
                                Position = new Vector2(60,350),
                                Colour = Colour4.White,
                                FlashColour = Colour4.White,
                                BackgroundColour = Colour4.LightPink,
                                Text = "Select",
                                Action = () => { fileSelectContainer.FadeIn(400,Easing.OutCubic); }
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
                    fileSelectContainer = new Container {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Masking = true,
                        CornerRadius = 20,
                        Y = 100,
                        Scale = new Vector2(1.5f),
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[] {
                            new Box {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Colour4.Black,
                                RelativeSizeAxes = Axes.Both,
                            },
                            fileSelect = new osuATFileSelector {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Scale = new Vector2(0.98f),
                                RelativeSizeAxes = Axes.Both,
                                OnOsuPathFound = () => {
                                    locationText.Text = $"osu! path: {fileSelect.CurrentPath.Value.FullName}";
                                    SaveStorage.SaveData.OsuPath = fileSelect.CurrentPath.Value.FullName;
                                    locationText.Colour = (SaveStorage.OsuPathIsValid())? Colour4.White : Colour4.Red;
                                    fileSelectContainer.FadeOut(400,Easing.OutCubic);
                                },
                                OnExitClicked = () => { fileSelectContainer.FadeOut(400,Easing.OutCubic); }
                            },
                        }
                    }
                };
                fileSelectContainer.Hide();
                apikeyText.Text = SaveStorage.SaveData.APIKey;
                Console.WriteLine(SaveStorage.SaveData.PlayerUsername);
                usernameText.Text = ApiScoreProcessor.ApiKeyValid? SaveStorage.SaveData.PlayerUsername : "Please set your api key first.";
                usernameText.Colour = ApiScoreProcessor.ApiKeyValid ? Colour4.White : Colour4.Gray;
                usernameDisplayText.Colour = ApiScoreProcessor.ApiKeyValid ? Colour4.White : Colour4.Gray;
                apikeyText.OnCommit += new TextBox.OnCommitHandler((TextBox box, bool target) => {

                    // check if it's a valid api key. if it is, display a checkmark
                    // then, save it to savestorage.
                    // otherwise, display a red X
                    OsuApiKey.Key = apikeyText.Text;
                    ApiScoreProcessor.UpdateKeyValid();
                    bool requestedReset = apikeyText.Text.ToUpper().Contains("RESET");
                    if (ApiScoreProcessor.ApiKeyValid)
                    {
                        SaveStorage.SaveData.APIKey = apikeyText.Text;
                        OsuApiKey.Key = apikeyText.Text;
                        apikeyText.FlashColour(Color4.Green, 3000, Easing.InOutCubic);
                        usernameText.ReadOnly = false;
                        usernameText.Colour = Colour4.White;
                        usernameText.Text = SaveStorage.SaveData.PlayerUsername;
                        usernameDisplayText.Colour = Colour4.White;
                        return;
                    }
                    usernameText.ReadOnly = true;
                    usernameText.Colour = Colour4.Gray;
                    usernameText.Text = "Please set your api key first.";
                    usernameDisplayText.Colour = Colour4.Gray;
                    if (requestedReset)
                    {
                        SaveStorage.SaveData.APIKey = "RESET";
                        OsuApiKey.Key = "RESET";
                        SaveStorage.Save();
                        apikeyText.FlashColour(Color4.Yellow, 3000, Easing.InOutCubic);
                        return;
                    }
                    apikeyText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                });
                usernameText.OnCommit += new TextBox.OnCommitHandler((TextBox box, bool target) =>
                {
                    if (ApiScoreProcessor.ApiKeyValid)
                    {
                        OsuUser player = OsuApi.GetUser(usernameText.Text);
                        if (player == default)
                        {
                            usernameText.FlashColour(Color4.Red, 3000, Easing.InOutCubic);
                            return;
                        }
                        SaveStorage.SaveData.PlayerID = player.ID;
                        SaveStorage.SaveData.PlayerUsername = usernameText.Text;
                        UsernameChanged.Invoke();
                    }
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
