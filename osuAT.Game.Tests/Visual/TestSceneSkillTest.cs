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
        private static int scoreCount = 1;

        private Score scoreA = new()
        {
            ScoreRuleset = RulesetStore.Osu,
            IsLazer = false,
            OsuID = 3152295822,
            BeatmapInfo = new Beatmap
            {
                MapID = 651507,
                MapsetID = 1380717,
                SongArtist = "VINXIS",
                SongName = "Sidetracked Day(Short Ver.)",
                DifficultyName = "deathstream",
                MapsetCreator = "Sotarks",
                StarRating = 7.20,
                MaxCombo = 300,
                FolderLocation = @"Songs\771159 VINXIS - Sidetracked Day (Short Ver)\VINXIS - Sidetracked Day (Short Ver.) (Sotarks) [deathstream].osu"
            },
            Grade = "SH",
            Accuracy = 99.51,
            AccuracyStats = new AccStat(285, 15, 0, 0),
            Combo = 300,
            TotalScore = 11627,
            Mods = new List<ModInfo> {ModStore.Hardrock, ModStore.Doubletime},
            DateCreated = System.DateTime.Today
        };

        private Score scoreB = new()
        {
            ScoreRuleset = RulesetStore.Osu,
            IsLazer = false,
            OsuID = 3152295822,
            BeatmapInfo = new Beatmap
            {
                MapID = 651507,
                MapsetID = 1380717,
                SongArtist = "a_hisa",
                SongName = "FREEDOM DIVE Arles",
                DifficultyName = "owo",
                MapsetCreator = "Naidaaka",
                StarRating = 7.93,
                MaxCombo = 2336,
                FolderLocation = @"Songs\257607 xi - FREEDOM DiVE\xi - FREEDOM DiVE (elchxyrlia) [Arles].osu"
            },
            Grade = "SH",
            Accuracy = 0.03,
            AccuracyStats = new AccStat(5, 0, 0, 2030),
            Combo = 5,
            TotalScore = 3250,
            Mods = new List<ModInfo> { }, //{ ModStore.Doubletime },
            DateCreated = System.DateTime.Today
        };

        private void createScoreSliders(Score score)
        {
            AddLabel($"---SCORE {scoreCount}---");

            AddLabel("---Beatmap Settings---");
            AddSliderStep("HP Drain", 0f, 11f, score.BeatmapInfo.DifficultyInfo.DrainRate, v =>  score.BeatmapInfo.DifficultyInfo.DrainRate = v);
            AddSliderStep("Circle Size", 0f, 10f, score.BeatmapInfo.DifficultyInfo.CircleSize, v => score.BeatmapInfo.DifficultyInfo.CircleSize = v);
            AddSliderStep("Overall Difficulty", 0f, 10f, score.BeatmapInfo.DifficultyInfo.OverallDifficulty, v => score.BeatmapInfo.DifficultyInfo.OverallDifficulty = v);
            AddSliderStep("Approach Rate", 0f, 11f, score.BeatmapInfo.DifficultyInfo.ApproachRate, v => { score.BeatmapInfo.DifficultyInfo.ApproachRate = v; });
            // AddSliderStep("Slider Multiplier", 0.0, 11.0, score.BeatmapInfo.DifficultyInfo.SliderMultiplier, v => score.BeatmapInfo.DifficultyInfo.SliderMultiplier = v);
            // AddSliderStep("Slider Tick Rate", 0.0, 11.0, score.BeatmapInfo.DifficultyInfo.SliderTickRate, v => score.BeatmapInfo.DifficultyInfo.SliderTickRate = v);

            AddLabel("---Score Settings---");
            AddSliderStep("Accuracy", 0f, 100f, score.Accuracy, v => score.Accuracy = v);
            AddSliderStep("Combo", 0, score.BeatmapInfo.MaxCombo, score.BeatmapInfo.MaxCombo, v => score.Combo = v);
            scoreCount++;
        }

        public void TestCalc()
        {
            AddStep("Calculate", () =>
            {
                ScoreDisplay scoreAdisplay;
                ScoreDisplay scoreBdisplay;
                //scoreA.Register();
                //scoreB.Register();
                Child = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Children = new Drawable[] {
                         scoreAdisplay = new ScoreDisplay {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = -100,
                            Scale = new Vector2(0.7f),
                            Skill = Skill.Flowaim,
                            Current = scoreA
                        },

                         scoreBdisplay = new ScoreDisplay {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Y = 100,
                            Scale = new Vector2(0.7f),
                            Skill = Skill.Flowaim,
                            Current = scoreB
                        }
                    }
                };
                scoreAdisplay.Appear();
                scoreBdisplay.Appear();

            });
        }

        public TestSceneSkillTest()
        {
            scoreA.Register();
            scoreB.Register();
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            TestCalc();
            createScoreSliders(scoreA);
            createScoreSliders(scoreB);

        }



    }
}
