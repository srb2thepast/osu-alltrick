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
                AccuracyStats = new AccStat(2020, 15, 0, 0),
                Combo = 2333,
                TotalScore = 116276034,
                Mods = new List<ModInfo>(),
                DateCreated = System.DateTime.Today
            };
            dummyscore.Register();
            dummyscore.AlltrickPP = new Dictionary<string, double>() { {"flowaim",727} };
            AddStep("create score display", () => {
                ScoreDisplay display = new ScoreDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,

                    Current = dummyscore,
                    Skill = Skill.Flowaim
                };
                Child = display;
                display.Appear();
            });
        }
    }
}
