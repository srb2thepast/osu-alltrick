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
using osuAT.Game.Types;
using osuAT.Game.Screens;
using OsuApiHelper;
using osuTK;
using osuTK.Graphics;
using osuAT.Game.Objects;
using osu.Game.Overlays.BeatmapSet;
using osu.Framework.Graphics.Animations;
using osu.Framework.IO.Network;
using OsuMemoryDataProvider;
using System;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Overlays.Changelog;
using System.IO;
using System.Linq;

namespace osuAT.Game
{


    public class ImportButton : CompositeDrawable
    {
        private MapIDBox beatmapBox;
        private HomeScreen mainScreen;
        public bool CanOpen = true;
        public bool SettingsOpen => false;

        public ImportButton(HomeScreen mainScreen)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            this.mainScreen = mainScreen;
        }

        public void HideBox()
        {
            beatmapBox.FadeOut(200,Easing.OutCubic);
        }

        public void ShowBox()
        {
            beatmapBox.Show();
        }

        [BackgroundDependencyLoader ]
        private void load(TextureStore textures)
        {
            InternalChildren = new Drawable[]
            {
                new ClickableContainer{
                    AutoSizeAxes = Axes.Both,
                    Action = () => {
                        if (!(CanOpen)) return;
                        if (beatmapBox.Alpha==0) {
                            ShowBox(); return;
                        }
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
                            Icon = FontAwesome.Solid.Download,
                        },
                    },
                },
                beatmapBox = new MapIDBox{ Anchor = Anchor.Centre,X = 250,Y = 140, Alpha = 0, BypassAutoSizeAxes = Axes.Both}
            };
            Size = InternalChildren[0].Size;
        }

        public class MapIDBox : CompositeDrawable
        {
            private bool force = false;
            private Container background;
            private SuperTextBox beatmapidText;
            private SpriteText infoText;


            public MapIDBox(bool forcecompletion = false)
            {
                force = forcecompletion;
                Origin = Anchor.Centre;
                Size = new Vector2(600, 180);
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
                        Size = new Vector2(600,200),
                        Masking = true,
                        CornerRadius = 20,
                        BorderThickness = 10,
                        BorderColour = Colour4.White,
                        Children = new Drawable[] {}
                    },

                    new Container{
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
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

                            // Beatmap ID
                            new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,60),
                                Font = new FontUsage("VarelaRound", size: 50),
                                Colour = Colour4.White,
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                                Text = "Beatmap ID: "
                            },
                            beatmapidText = new SuperTextBox 
                            {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(286,55),
                                Colour = Colour4.White,
                                Masking =true,
                                CornerRadius = 20,
                                Size = new Vector2(250,50), 
                                Text = "",
                                TextFont = new FontUsage("VarelaRound", size: 50),
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                            },
                            infoText = new SpriteText {
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Position = new Vector2(60,100),
                                Colour = Colour4.White,
                                Size = new Vector2(450,50),
                                Text = "",
                                Font = new FontUsage("VarelaRound", size: 40),
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

                if (ApiScoreProcessor.ApiReqs > 30) {
                    // Please wait text
                }
                beatmapidText.OnCommit += new TextBox.OnCommitHandler(async (TextBox box, bool target) => {
                    Console.WriteLine("commit");

                    ApiScoreProcessor.ApiReqs += 1;
                    if (OsuApi.IsKeyValid())
                    {

                        ApiScoreProcessor.ApiReqs += 1;
                        beatmapidText.Text.ToCharArray().ForEach(ch =>
                        {
                            if (!"1234567890".Contains(ch))
                            {
                                beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);                                                                                         
                                return;
                            }
                        });
                        processMap(beatmapidText.Text);
                        return;
                    }

                    beatmapidText.Text = "Please input your API Key first.";
                    beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                });
            }

            private void processMap(string mapID)
            {
                if (ApiScoreProcessor.ApiReqs >= 30)
                {
                    beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                    infoText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                    Console.WriteLine($"Could not process beatmap scores. ({ApiScoreProcessor.ApiReqs} API Requests were sent!!!!)");
                    return;
                }
                bool invalidID = false;
                beatmapidText.Text.ForEach(ch => {
                    if (!"1234567890".Contains(ch))
                    {
                        infoText.Text = "Invalid map ID!";
                        beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                        infoText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                        invalidID = true;
                    }
                });
                if (invalidID) return;
                ApiScoreProcessor.ApiReqs += 1;
                var osuScorelist = ScoreImporter.OsuApiGetScores(mapID, SaveStorage.SaveData.PlayerID.ToString());

                // return if no scores were set on this map by the player
                if (osuScorelist == null || !(osuScorelist.Count >= 0))
                {
                    infoText.Text = "No scores found!";
                    beatmapidText.FlashColour(Color4.Red, 3000, Easing.InOutCubic);
                    infoText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                    return;
                }

                var osuScore = osuScorelist[0];
                OsuBeatmap osuMap = default;
                OsuBeatmap mapRet() { osuMap = OsuApi.GetBeatmap(beatmapidText.Text); return osuMap; };
                ProcessResult result = ApiScoreProcessor.SaveToStorageIfValid(osuScore, mapRet);
                if ((int)result < 1)
                {
                    switch (result) {
                        case ProcessResult.AlreadySaved:
                            infoText.Text = "This score has already been saved!";
                            break;
                        case ProcessResult.BetterScoreSaved:
                            infoText.Text = "A better score has already been saved!";
                            break;
                        case ProcessResult.FailedScore:
                        infoText.Text = "This is a failed score!";
                            break;
                        case ProcessResult.UnrankedMap:
                        infoText.Text = "This map is unranked!";
                            break;
                    }
                    beatmapidText.FlashColour(Color4.Red, 3000, Easing.InOutCubic);
                    infoText.FlashColour(Color4.Red, 3000, Easing.InOutCubic);
                    return;
                }
                infoText.Text = $"{osuMap.Title} has sucessfully been imported!";
                beatmapidText.FlashColour(Color4.Green, 3000, Easing.InOutCubic);
            }


            public override void Show()
            {
                base.Show();
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
