using System;
using System.Collections.Generic;
using System.IO;
using NuGet.Packaging.Core;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Overlays.BeatmapSet;
using osuAT.Game;
using osuAT.Game.Objects;
using osuAT.Game.Skills;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;
using static osuAT.Game.Skills.AimSkill;

namespace osuAT.Game.Tests.Visual
{
    // i got lazy making this so execpt some bad code 
    public partial class TestSceneSaveStorage : osuATTestScene
    {

        private Score dummyscore;
        private BasicTextBox savelocation;
        private TextFlowContainer savecontents;

        private void saveDummy()
        {
            SaveStorage.AddScore(dummyscore);
        }
        private void saveSStorage()
        {
            FileInfo info = SaveStorage.Save();
            string content = SaveStorage.Read();
            string contentstring = content.Replace(",", "\n").Replace("{", "\n {").Replace("}", "}\n");
            savelocation.Text = info.FullName;
            savecontents.Text = contentstring;
            System.Console.WriteLine(contentstring);
        }

        private void ReloadText(string savepath = null)
        {
            savepath ??= Path.GetFullPath(SaveStorage.SaveFileFullPath);
            string content = SaveStorage.Read();
            string contentstring = content.Replace(",", "\n").Replace("{", "\n {").Replace("}", "}\n");
            savelocation.Text = savepath;
            savecontents.Text = contentstring;
            System.Console.WriteLine(contentstring);
        }

        [SetUp]
        public void SetUp()
        {
            SaveStorage.Init(new NativeStorage("testing"));
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
        public void TestAddNomod()
        {
            AddStep("create dummy score", makeNomodDummy);
            addThenSave();
        }

        [Test]
        public void TestAddFourmod()
        {
            AddStep("create dummy score", makeFourmodDummy);
            addThenSave();
        }

        [Test]
        public void TestAddThenRemove()
        {
            AddStep("create dummy score", makeNomodDummy);
            addThenSave();
            AddStep("remove dummy score from storage", () =>
            {
                SaveStorage.RemoveScore(dummyscore.ID);
            });
            AddAssert("removed from main branch", () => { return !(SaveStorage.SaveData.Scores.ContainsKey(dummyscore.ID)); });
            AddAssert("removed from top plays", checkScoreRemovedFromTop);
            AddStep("save the storage", saveSStorage);
        }

        private class testSkill : ISkill
        {
            public string Name => "Test Skill";

            public string Identifier => "testskill";

            public string Version => "0.0";

            public string Summary => throw new NotImplementedException();

            public int SummarySize => throw new NotImplementedException();

            public Colour4 PrimaryColor => throw new NotImplementedException();

            public Colour4 SecondaryColor => throw new NotImplementedException();

            public string Background => throw new NotImplementedException();

            public string Cover => throw new NotImplementedException();

            public string BadgeBG => throw new NotImplementedException();

            public (Vector2, Vector2) BadgePosSize => throw new NotImplementedException();

            public float MiniHeight => throw new NotImplementedException();

            public int BoxNameSize => throw new NotImplementedException();

            public Vector2 BoxPosition => throw new NotImplementedException();

            public SkillGoals Benchmarks => throw new NotImplementedException();

            public SkillCalcuator GetSkillCalc(Score score) => new FlowAimSkill.FlowAimCalculator(score);
        }

        private testSkill skillInstance = new testSkill();

        [Test]
        public void TestAddThenRemoveSkill()
        {
            AddStep("create dummy score", makeNomodDummy);
            addThenSave();
            AddStep("add testSkill to SkillList", () =>
            {
                Skill.SkillList.Add(skillInstance);
            });
            AddStep("re-initalize savestorage", () =>
            {
                SaveStorage.Init(new NativeStorage("testing"));
                ReloadText();
            });
            AddAssert("skill added", () => { return SaveStorage.SaveData.TotalSkillPP.ContainsKey(skillInstance.Identifier); });
            AddStep("remove testSkill from SkillList and SaveStorage", () =>
            {
                Skill.SkillList.Remove(skillInstance);
                SaveStorage.RemoveSkillFromSave(skillInstance);
                ReloadText();
            });
            AddAssert("skill removed", () => { return !SaveStorage.SaveData.TotalSkillPP.ContainsKey(skillInstance.Identifier); });
            AddStep("save the storage", saveSStorage);


        }

        [Test]
        public void TestDeleteSaveFile()
        {
            AddStep("delete test save file", () =>
            {
                File.Delete(SaveStorage.SaveFileFullPath);
            });
        }

        private bool checkScoreRemovedFromTop()
        {
            Console.WriteLine(SaveStorage.SaveData.Scores.ContainsKey(dummyscore.ID));
            var modeList = new List<string> { "overall", dummyscore.ScoreRuleset.Name };
            foreach (KeyValuePair<string, double> scoreSkillPP in dummyscore.AlltrickPP)
            {
                foreach (var mode in modeList)
                {
                    var SkillList = SaveStorage.SaveData.AlltrickTop[scoreSkillPP.Key][mode];
                    for (int i = SkillList.Count - 1; i >= 0; i--)
                    {
                        Tuple<Guid, double> skillListScore = SkillList[i];
                        if (skillListScore.Item1 == dummyscore.ID)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;

        }


        private void addThenSave()
        {
            AddStep("add dummy score to storage", saveDummy);
            AddStep("save the storage", saveSStorage);
            AddAssert("score exists", () =>
            {
                return SaveStorage.SaveData.Scores.ContainsKey(dummyscore.ID);
            });

        }

        private void makeNomodDummy()
        {
            _ = new List<ModInfo>
                {
                    ModStore.Hidden,
                    ModStore.Doubletime,
                    ModStore.Hardrock,
                    ModStore.Flashlight
                };
            dummyscore = new Score()
            {
                ScoreRuleset = RulesetStore.Osu,
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
                    MaxCombo = 2336,
                    FolderLocation = @"Songs\651507 a_hisa - Logical Stimulus\a_hisa - Logical Stimulus (Naidaaka) [owo].osu"
                },
                Grade = "SH",
                AccuracyStats = new AccStat(2020, 15, 0, 0),
                Combo = 2333,
                TotalScore = 116276034,
                Mods = new List<ModInfo>(),
                DateCreated = System.DateTime.Today
            };
            dummyscore.Register();
        }

        private void makeFourmodDummy()
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
                    MaxCombo = 2336,
                    FolderLocation = @"Songs\651507 a_hisa - Logical Stimulus\a_hisa - Logical Stimulus (Naidaaka) [owo].osu"
                },
                Grade = "SH",
                AccuracyStats = new AccStat(2020, 15, 0, 0),
                Combo = 2333,
                TotalScore = 116276034,
                Mods = ModList,
                DateCreated = System.DateTime.Today
            };
            dummyscore.Register();
        }


    }
}

