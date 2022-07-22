using System.Collections.Generic;
using osu.Framework.Graphics;
using NUnit.Framework;
using osuAT.Game.Objects;
using osuAT.Game.Skills;
using osuAT.Game.Types;
using osuTK;

namespace osuAT.Game.Tests.Visual
{
    public class TestSceneSkillBox : osuATTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.
        private FullSkillBox box;
        
        [Test]
        public void TestFullSkillBox()
        {
            
            AddStep("Create new fullskillbox", () =>
            {
                Child = box = new FullSkillBox
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(2.7f),
                    Skill = Skill.Flowaim,
                    
                    
                };

                box.Appear(0);
                box.InfoBox.InfoBook.CurrentPage.Value = 1;

            });
            AddStep("Go to page 0", () => { box.InfoBox.InfoBook.CurrentPage.Value = 0; });
            AddWaitStep("Wait a bit", 2);
            AddStep("Go to page 1", () => { box.InfoBox.InfoBook.CurrentPage.Value = 1; });
            AddWaitStep("Wait a bit", 2);
            AddStep("Go to page 2", () => { box.InfoBox.InfoBook.CurrentPage.Value = 2; });
        }

    }
}
