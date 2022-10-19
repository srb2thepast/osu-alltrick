using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using NUnit.Framework;
using osuAT.Game.Objects.Displays;
using osuAT.Game.Objects;
using osuAT.Game.Skills;
using osuAT.Game.Types;
using osuTK;
using osuTK.Graphics;
namespace osuAT.Game.Tests.Visual
{
    public class TestSceneSkillTest : osuATTestScene
    {

        private Score scoreA = new Score()
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
                MaxCombo = 2336,
                FolderName = @"Songs\651507 a_hisa - Logical Stimulus\a_hisa - Logical Stimulus (Naidaaka) [owo].osu"
            },
            Grade = "SH",
            Accuracy = 99.51,
            AccuracyStats = new AccStat(2020, 15, 0, 0),
            Combo = 2333,
            TotalScore = 116276034,
            Mods = new List<ModInfo>{ },
            DateCreated = System.DateTime.Today
        };

        public TestSceneSkillTest() {
            scoreA.Register();
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }


        [Test]
        public void TestCalc()
        {
            AddStep("Calculate", () =>
            {
                scoreA.Register();
                Child = new Container{
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Children = new Drawable[] {
                        new ScoreDisplay {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Scale = new Vector2(0.7f),
                            Skill = Skill.Flowaim,
                            Current = scoreA
                        }
                    }
                };

            });
        }

    }
}
