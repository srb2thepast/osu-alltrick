using System.Collections.Generic;
using osu.Framework.Input.Events;
using osu.Framework.Allocation;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuAT.Game.Objects;
using osuAT.Game.Skills;    
using NUnit.Framework;
using Newtonsoft.Json;

namespace osuAT.Game.Tests.Visual
{
    public class TestSceneosuATGame : osuATTestScene
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

            AddToggleStep("Focus on Flowaim", (b) => {
                if (b == true) { mainscreen.SkillCont.FocusOnBox(skilldict[Skill.Flowaim]); } else { mainscreen.SkillCont.Defocus(); }
            });
            AddWaitStep("Wait a bit", 5);
        }
    }
}
