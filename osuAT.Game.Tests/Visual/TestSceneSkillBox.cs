using System.Collections.Generic;
using osu.Framework.Graphics;
using NUnit.Framework;
using osuAT.Game.Objects;
using osuAT.Game.Skill;
using osuAT.Game.Types;

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
                Child = new SkillBox
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    SkillName = "Flow Aim",
                    SkillPrimaryColor = Colour4.FromHex("#99FF69"),
                    SkillSecondaryColor = Colour4.FromHex("#00FFF0"),
                    MiniHeight = 100,
                    TextSize = 83,
                    Scale = new osuTK.Vector2(0.9f)
                };
                
            });
        }
    }
}
