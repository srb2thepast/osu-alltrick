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

        [Test]
        public void TestSkillBox()
        {
            
            AddStep("Create new skillbox", () =>
            {
                Child = new FullSkillBox
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(2.7f),
                    Skill = Skill.Flowaim,
                    
                    
                };
                
                
            });
        }
    }
}
