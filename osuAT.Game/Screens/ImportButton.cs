// wip
/* using osu.Framework.Allocation;
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


    public class ImportButton : CompositeDrawable
    {
        private SettingsBox settingsBox;
        private HomeScreen mainScreen;

        public ImportButto(HomeScreen mainScreen)
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

        public class MapIDBox : CompositeDrawable
        {
            private bool force = false;
            private Container background;


            private SuperPasswordTextBox beatmapid;


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

                            // Score ID
                            new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,60),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "Score ID: "
                            },
                            beatmapidText = new SuperPasswordTextBox
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
                beatmapidText.OnCommit += new TextBox.OnCommitHandler((TextBox box, bool target) => {
                    if (OsuApi.IsKeyValid()) {
                        beatmapidText.FlashColour(Color4.Green, 3000, Easing.InOutCubic);
                        return;
                    }
                    beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);

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
*/
