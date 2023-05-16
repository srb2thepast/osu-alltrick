using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.IO.Network;
using osu.Framework.Screens;
using osu.Game.Overlays.BeatmapSet;
using osu.Game.Overlays.Changelog;
using OsuApiHelper;
using osuAT.Game.API;
using osuAT.Game.Objects;
using osuAT.Game.Screens;
using osuAT.Game.UserInterface;
using OsuMemoryDataProvider;
using osuTK;
using osuTK.Graphics;
using Veldrid;
using osu.Game.Graphics.Backgrounds;
using static osuAT.Game.SettingsButton.SettingsBox;
using FFmpeg.AutoGen;
using System.Threading.Tasks;
using osu.Game.Online.API;
using osu.Game.Scoring;

namespace osuAT.Game
{
    public partial class ImportButton : ButtonScreen
    {
        protected override IconUsage ButtonIcon => FontAwesome.Solid.Download;

        private HomeScreen mainScreen;

        private ImporterContainer beatmapBox = new()
        {
            Alpha = 0
        };

        protected override Drawable DisplayBox => beatmapBox;

        public ImportButton(HomeScreen mainScreen)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            this.mainScreen = mainScreen;
        }

        public partial class ImporterContainer : CompositeDrawable
        {
            private ArrowedContainer.ArrowButton leftArrow;
            private ArrowedContainer.ArrowButton rightArrow;

            private Container curBoxContainer;
            private Drawable curBox => boxes[curIndex];
            private int curIndex = 0;

            private List<Drawable> boxes;

            private MapIDBox beatmapBox = new()
            {
                Anchor = Anchor.Centre,
                X = 250,
                Y = 180,
                BypassAutoSizeAxes = Axes.Both
            };

            private TopPlaysBox topPlaysBox = new()
            {
                Anchor = Anchor.Centre,
                X = 250,
                Y = 290,
                BypassAutoSizeAxes = Axes.Both
            };

            public ImporterContainer()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                AutoSizeAxes = Axes.Both;
                boxes = new List<Drawable> {
                    beatmapBox,
                    topPlaysBox
                };
            }

            private void leftArrowClick()
            {
                curIndex = Math.Max(0, curIndex - 1);
                foreach (Drawable box in curBoxContainer.Children)
                {
                    if (box != curBox)
                    {
                        box.Alpha = 0;
                        continue;
                    }
                    box.Alpha = 1;
                }
            }

            private void rightArrowClick()
            {
                curIndex = Math.Min(boxes.Count - 1, curIndex + 1);
                foreach (Drawable box in curBoxContainer.Children)
                {
                    if (box != curBox)
                    {
                        box.Alpha = 0;
                        continue;
                    }
                    box.Alpha = 1;
                }
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                InternalChildren = new Drawable[]
                {
                    leftArrow = new ArrowedContainer.ArrowButton() {
                        Text = "<",
                        Shadow = true,
                        Font = new FontUsage("ChivoBold",size: 70),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Position = new Vector2(-90,170),
                        ClickAction = leftArrowClick
                    },
                    rightArrow = new ArrowedContainer.ArrowButton() {
                        Text = ">",
                        Shadow = true,
                        Font = new FontUsage("ChivoBold",size: 70),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Position = new Vector2(580,170),
                        ClickAction = rightArrowClick
                    },
                    curBoxContainer = new Container{
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        AutoSizeAxes = Axes.Both,
                        Children = boxes
                    }
                };
                leftArrowClick();
            }
        }

        public partial class MapIDBox : CompositeDrawable
        {
            private Container background;
            private SuperTextBox beatmapidText;
            private SpriteText infoText;

            public MapIDBox()
            {
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
                        Children = Array.Empty<Drawable>()
                    },
                    // Top text
                    background = new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Masking = true,
                        Position = new Vector2(100,-110),
                        Size = new Vector2(300,80),
                        CornerRadius = 20,
                        BorderThickness = 10,
                        BorderColour = Colour4.White,
                        Children = new Drawable[] {
                            new Box {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(300,80),
                                Colour = Colour4.FromHex("FF56A2")
                            },
                            new SpriteText {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Colour4.White,
                                Position = new Vector2(32,20),
                                Size = new Vector2(300,80),
                                Text = "Map Score Importer",
                                Font = new FontUsage("VarelaRound", size: 33),
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                            },
                        }
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

                if (ApiScoreProcessor.ApiReqs > 30)
                {
                    // Please wait text
                }
                beatmapidText.OnCommit += new TextBox.OnCommitHandler((TextBox box, bool target) =>
                {
                    Console.WriteLine("commit");

                    ApiScoreProcessor.ApiReqs += 1;
                    if (ApiScoreProcessor.ApiKeyValid)
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

                    infoText.Text = "Please add your API Key in the settings first.";
                    beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                    infoText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                });
            }

            private async void processMap(string mapID)
            {
                if (!SaveStorage.OsuPathIsValid())
                {
                    infoText.Text = "Invalid OsuPath! Please set it in the settings.";
                    beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                    infoText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                }
                if (ApiScoreProcessor.ApiReqs >= 30)
                {
                    beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                    infoText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                    Console.WriteLine($"Could not process beatmap scores: You are being rate limited. ({ApiScoreProcessor.ApiReqs} API Requests have been sent!!!!)");
                    return;
                }
                bool invalidID = false;
                beatmapidText.Text.ForEach(ch =>
                {
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
                var osuScorelist = await ApiScoreProcessor.OsuGetBeatmapScores(mapID, SaveStorage.SaveData.PlayerID.ToString());

                // return if no scores were set on this map by the player
                if (osuScorelist == null || osuScorelist.Count == 0)
                {
                    infoText.Text = "No scores found!";
                    beatmapidText.FlashColour(Color4.Red, 3000, Easing.InOutCubic);
                    infoText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                    return;
                }

                var osuScore = osuScorelist[0];
                OsuApiBeatmap osuMap = default;
                async Task<OsuApiBeatmap> mapRetAsync() { osuMap = await ApiScoreProcessor.OsuGetBeatmap(beatmapidText.Text); return osuMap; };
                ProcessResult result = await ApiScoreProcessor.SaveToStorageIfValid(osuScore, mapRetAsync);
                if ((int)result < 1)
                {
                    switch (result)
                    {
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

                        case ProcessResult.MapNotDownloaded:
                            infoText.Text = "This map isn't downloaded!";
                            break;
                    }
                    beatmapidText.FlashColour(Color4.Red, 3000, Easing.InOutCubic);
                    infoText.FlashColour(Color4.Red, 3000, Easing.InOutCubic);
                    return;
                }
                infoText.Text = $"{osuMap.Title} imported sucessfully!";
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

        public partial class TopPlaysBox : CompositeDrawable
        {
            private OsuATButton calculateButton;

            private TextFlowContainer infoText;

            private bool importing = false;

            public TopPlaysBox()
            {
                Origin = Anchor.Centre;
                Size = new Vector2(600, 400);
            }

            public void Defocused()
            {
            }

            public async void ImportTopPlays()
            {
                importing = true;

                ApiScoreProcessor.ApiReqs++;
                var topPlays = OsuApi.GetUserBest(SaveStorage.SaveData.PlayerID.ToString(), limit: 100);
                if (topPlays == null || topPlays.Count == 0)
                {
                    infoText.Text = "No top plays found. Make sure your settings are correct.";
                    return;
                }

                int i = 1;
                Console.WriteLine(topPlays.Count());
                ProcessResult result = ProcessResult.AlreadySaved;
                foreach (var osuScore in topPlays)
                {
                    Console.WriteLine("Restarting");
                    if (result != ProcessResult.AlreadySaved)
                        await Task.Delay(1000);
                    else
                        await Task.Delay(50);
                    Console.WriteLine("Delay ended");
                    OsuApiBeatmap osuMap = default;
                    async Task<OsuApiBeatmap> mapRet() { osuMap = await ApiScoreProcessor.OsuGetBeatmap(osuScore.MapID); return osuMap; };
                    result = await ApiScoreProcessor.AddToStorageIfValid(osuScore, mapRet);
                    Console.WriteLine("Getting messages");
                    string resultMsg = ApiScoreProcessor.GetResultMessages(result, osuScore);
                    Console.WriteLine(resultMsg);
                    infoText.AddText("\n#[" + i + "] " + resultMsg);
                    if (result == ProcessResult.RateLimited)
                    {
                        await Task.Delay(10000);
                        infoText.AddText("Rate limited. Waiting...");
                    }
                    i++;
                }

                infoText.AddText("\n [[ Finished importing all top plays! ]]");
                infoText.AddText("\n [[ Saving... ]]");
                SaveStorage.Save();
                importing = false;
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                InternalChildren = new Drawable[]
                {
                    new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(600,600),
                        Masking = true,
                        CornerRadius = 20,
                        BorderThickness = 10,
                        BorderColour = Colour4.White,
                        Children = Array.Empty<Drawable>()
                    },
                    // Top text
                    new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Masking = true,
                        Position = new Vector2(100,-220),
                        Size = new Vector2(300,80),
                        CornerRadius = 20,
                        BorderThickness = 10,
                        BorderColour = Colour4.White,
                        Children = new Drawable[] {
                            new Box {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(300,80),
                                Colour = Colour4.FromHex("FF56A2")
                            },
                            new SpriteText {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Colour4.White,
                                Position = new Vector2(32,20),
                                Size = new Vector2(300,80),
                                Text = "Top Plays Importer",
                                Font = new FontUsage("VarelaRound", size: 33),
                                Shadow = true,
                                ShadowOffset = new Vector2(0,0.1f),
                            },
                        }
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

                            calculateButton = new OsuATButton {
                                Masking = true,
                                CornerRadius = 15,
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.CentreLeft,
                                Size = new Vector2(450,50),
                                Position = new Vector2(75,60),
                                Colour = Colour4.White,
                                FlashColour = Colour4.White,
                                BackgroundColour = Colour4.DimGray,
                                Text = "Import all your top plays?",
                                Action = ImportTopPlays,
                                OnHoverAction = ()  => calculateButton.Text = "Import all your top plays!!",
                                OnHoverLostAction = ()  => calculateButton.Text = "Import all your top plays?"
                            },

                            new BasicScrollContainer() {
                                Position = new Vector2(20,120),
                                Width = 300*2 - 40,
                                Height = 130*2,
                                Masking = true,
                                CornerRadius = 15,
                                Scale = new Vector2(1),
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,

                                Children = new Drawable[]  {
                                    new Box {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Colour4.FromHex("FF56A2")
                                    },
                                    infoText = new TextFlowContainer {
                                        Anchor = Anchor.TopLeft,
                                        Origin = Anchor.TopLeft,
                                        Colour = Colour4.White,
                                        // Size = new Vector2(100,4000),
                                        AutoSizeAxes = Axes.Both,
                                        Scale = new Vector2(1.1f)
                                        // Font = new FontUsage("VarelaRound", size: 40),
                                        // Shadow = true,
                                        // ShadowOffset = new Vector2(0,0.1f),
                                    }
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

                for (int i = 0; i < 100; i++)
                {
                    // infoText.AddText("\n#[" + i + "] aaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                }

                if (ApiScoreProcessor.ApiReqs > 30)
                {
                    // Please wait text
                }

                //beatmapidText.OnCommit += new TextBox.OnCommitHandler((TextBox box, bool target) =>
                //{
                //    Console.WriteLine("commit");

                //    ApiScoreProcessor.ApiReqs += 1;
                //    if (ApiScoreProcessor.ApiKeyValid)
                //    {
                //        ApiScoreProcessor.ApiReqs += 1;
                //        beatmapidText.Text.ToCharArray().ForEach(ch =>
                //        {
                //            if (!"1234567890".Contains(ch))
                //            {
                //                beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                //                return;
                //            }
                //        });
                //        processMap(beatmapidText.Text);
                //        return;
                //    }

                //    infoText.Text = "Please add your API Key into    the settings first.";
                //    beatmapidText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                //    infoText.FlashColour(Color4.Red, 2000, Easing.InOutCubic);
                //});
            }
        }
    }
}
