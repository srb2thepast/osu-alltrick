using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osuAT.Game.Objects.Displays;
using osuAT.Game.Skills;
using osuAT.Game.Types;

namespace osuAT.Game.Tests.Visual.Display
{
    public partial class TestSceneScoreDisplay : osuATTestScene
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
                    SongName = "Mou Ii Kai",
                    DifficultyName = "owo",
                    MapsetCreator = "Naidaaka",
                    StarRating = 7.93,
                    MaxCombo = 2336,
                    FolderLocation = @"Songs\807850 THE ORAL CIGARETTES - Mou Ii kai\THE ORAL CIGARETTES - Mou ii Kai (Nevo) [Rain].osu"

                },
                Grade = "SH",
                AccuracyStats = new AccStat(2020, 15, 0, 1),
                Combo = 2333,
                TotalScore = 116276034,
                ModsString = new List<string> { "Hidden", "Nightcore" },
                DateCreated = System.DateTime.Today,
            };
            _ = dummyscore.Register();
            AddStep("create score display", () =>
            {
                ScoreDisplay display = new ScoreDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,

                    Current = dummyscore,
                    Skill = Skill.TappingStamina
                };
                Child = display;
                display.Appear();
            });
        }
    }
}
