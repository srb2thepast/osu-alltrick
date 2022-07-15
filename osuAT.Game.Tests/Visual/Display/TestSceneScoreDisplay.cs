using System.Collections.Generic;
using osu.Framework.Graphics;
using osuAT.Game.Objects.Displays;
using osuAT.Game.Types;
using osuAT.Game.Skills;


namespace osuAT.Game.Tests.Visual.Display
{
    public class TestSceneScoreDisplay : osuATTestScene
    {
        public TestSceneScoreDisplay()
        {
            List<ModInfo> ModList = new List<ModInfo>
            {
                ModStore.Hidden,
                ModStore.Doubletime,
                ModStore.Hardrock,
                ModStore.Flashlight
            };
            Score dummyscore = new Score()
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
            AddStep("create score display", () => {
                ScoreDisplay display = new ScoreDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,

                    Current = dummyscore,
                    PrimaryColor = Colour4.FromHex("#99FF69"),
                    SecondaryColor = Colour4.FromHex("#00FFF0"),

                };
                Child = display;
                display.Appear();
            });
        }
    }
}
