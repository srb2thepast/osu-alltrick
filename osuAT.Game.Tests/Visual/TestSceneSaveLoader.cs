using NUnit.Framework;
using osuAT.Game;
using osuAT.Game.Types;
using osuAT.Game.Types.Score;
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
using osuAT.Game.Skills;
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
            FileInfo info = SaveStorage.SaveScore(dummyscore);
            string contentstring = SaveStorage.Read().Replace(",", "\n").Replace("{", "\n {").Replace("}", "}\n");
            savelocation.Text = info.FullName;
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

                dummyscore = new Score(4000, 1, 1, 1, 100, new AccStat(0, 1, 0, 0), 1, samplePPTotals, true, ModList);
            });
            AddStep("save dummy score to file", saveDummy);
            AddAssert("score exists", () => {
                return SaveStorage.SaveData.Scores.Contains(dummyscore);
            });
            AddSliderStep("flowaim pptotal", 0, 20000, 100, amount => { samplePPTotals = new SkillPPTotals { FlowAim = amount};  });
        }

        [Test]
        public void TestNomod()
        {
            AddStep("create dummy score", () =>
            {
                dummyscore = new Score(69420727, 1, 1, 1, 100, new AccStat(0, 1, 0, 0), 1, samplePPTotals, true, null);
            });
            AddStep("save dummy score to file", saveDummy);
            AddAssert("score exists", () => {
                return true;
            });
        }

        
    }
}

// () => {}
