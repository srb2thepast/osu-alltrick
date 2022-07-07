using NUnit.Framework;
using osuAT.Game;
using osuAT.Game.Types;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using System.IO;

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Colour;
using osu.Framework.Input.Events;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;
using osuAT.Game.Objects;
using osuAT.Game.Skill;
using osuTK;
using System.Collections.Generic;

namespace osuAT.Game.Tests.Visual
{
    // i got lazy making this so execpt some bad code 
    public class TestSceneSaveStorage : osuATTestScene
    {

        private Score dummyscore;
        private BasicTextBox savelocation;
        private TextFlowContainer savecontents;
        private SkillPPTotals samplePPTotals = new SkillPPTotals {
            FlowAim = 100
        };

        private void saveDummy() {
            (FileInfo,int) info = SaveStorage.SaveScore(dummyscore);
            string contentstring = SaveStorage.Read().Replace(",", "\n").Replace("{", "\n {").Replace("}", "}\n");
            savelocation.Text = info.Item1.FullName;
            savecontents.Text = contentstring;
            System.Console.WriteLine(contentstring);
        }


        [SetUp]
        public void SetUp() {
            Child = new Container
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Children = new Drawable[] {
                    savelocation = new BasicTextBox
                    {
                        Text = "Location Box",
                        Size = new Vector2(800, 30),
                        Anchor = Anchor.TopCentre,
                        Y = -300,
                        Origin = Anchor.Centre,
                        ReadOnly = true,

                    },

                    new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Width = 300,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.White.Opacity(0.1f)
                            },
                            savecontents = new TextFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Text = "Content Box",
                            }
                        }
                    }
                }
            };
        }


        [Test]
        public void TestNomod()
        {
            AddStep("create dummy score", () =>
            {
               dummyscore = new Score()
                {
                    ScoreRuleset = RulesetStore.Osu,
                    IsLazer = false,
                    OsuID = 3152295822,
                    BeatmapInfo = new Beatmap
                    {
                        MapID = 651507,
                        MapsetID = 1380717,
                        SongArtist = "a_hisa",
                        SongName = "Logical Stimulus",
                        DifficultyName = "owo",
                        MapsetCreator = "Naidaaka",
                        StarRating = 7.93,
                        MaxCombo = 2336
                    },
                    Grade = "SH",
                    Accuracy = 99.51,
                    AccuracyStats = new AccStat(2020, 15, 0, 0),
                    Combo = 2333,
                    TotalScore = 116276034,
                    Mods = new List<ModInfo>(),
                    DateCreated = System.DateTime.Today
                };
                dummyscore.Register();
            });
            AddStep("save dummy score to file", saveDummy);
            AddAssert("score exists", () => {
                return true;
            });
        }

        [Test]
        public void TestFourmod()
        {
            AddStep("create dummy score", () =>
            {
                List<ModInfo> ModList = new List<ModInfo>
                {
                    ModStore.Hidden,
                    ModStore.Doubletime,
                    ModStore.Hardrock,
                    ModStore.Flashlight
                };

                dummyscore = new Score()
                {
                    ScoreRuleset = RulesetStore.Osu,
                    IsLazer = false,
                    OsuID = 3152295822,
                    BeatmapInfo = new Beatmap
                    {
                        MapID = 651507,
                        MapsetID = 1380717,
                        SongArtist = "a_hisa",
                        SongName = "Logical Stimulus",
                        DifficultyName = "owo",
                        MapsetCreator = "Naidaaka",
                        StarRating = 7.93,
                        MaxCombo = 2336
                    },
                    Grade = "SH",
                    Accuracy = 99.51,
                    AccuracyStats = new AccStat(2020, 15, 0, 0),
                    Combo = 2333,
                    TotalScore = 116276034,
                    Mods = ModList,
                    DateCreated = System.DateTime.Today
                };
                dummyscore.Register();
            });
            AddStep("save dummy score to file", saveDummy);
            AddAssert("score exists", () => {
                return SaveStorage.SaveData.Scores.Contains(dummyscore);
            });
            AddSliderStep("flowaim pptotal", 0, 20000, 100, amount => { samplePPTotals = new SkillPPTotals { FlowAim = amount};  });
        }

        

        
    }
}

// () => {}
