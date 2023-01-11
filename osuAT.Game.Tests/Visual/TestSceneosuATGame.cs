using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuAT.Game.Objects;
using osuAT.Game.Screens;
using osuAT.Game.Skills;
using osuAT.Game.Skills.Resources;

namespace osuAT.Game.Tests.Visual
{
    public partial class TestSceneosuATGame : osuATTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.


        private osuATGame game;

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            game = new osuATGame();
            game.SetHost(host);
            AddGame(game);
        }

        [Test]
        public void TestMovetoBox()
        {
            HomeScreen mainscreen = game.MainScreen;
            Dictionary<ISkill, SkillBox> skilldict = mainscreen?.SkillCont?.SkillDict;

            AddToggleStep("Focus on Flowaim", (b) =>
            {
                // if (b == true) { mainscreen?.SkillCont?.FocusOnBox(skilldict[Skill.Flowaim]); } else { mainscreen.SkillCont.Defocus(); }
            });
            AddWaitStep("Wait a bit", 5);
        }
    }
}
