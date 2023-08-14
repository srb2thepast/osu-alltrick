using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Graphics;
using osuAT.Game.Objects;
using osuAT.Game.Skills;
using osuAT.Game.Types;
using osuTK;
using TagLib.Id3v2;

namespace osuAT.Game.Tests.Visual
{
    public partial class TestSceneSkillBox : osuATTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.
        private FullSkillBox box;
        private MiniSkillBox mbox;

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
                    CurSkill = Skill.Flowaim,


                };

                box.Appear(0);

            });
            AddStep("Go to page 0", () => { box.InfoBox.InfoBook.CurrentPage.Value = 0; });
            AddWaitStep("Wait a bit", 2);
            AddStep("Go to page 1", () => { box.InfoBox.InfoBook.CurrentPage.Value = 1; });
            AddWaitStep("Wait a bit", 2);
            AddStep("Go to page 2", () => { box.InfoBox.InfoBook.CurrentPage.Value = 2; });
        }

        [Test]
        public void TestMiniSkillBox()
        {
            
            AddStep("Create new miniskillbox", () =>
            {
                Child = mbox = new MiniSkillBox
                (Skill.Flowaim, null)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(2.7f),
                };

            });
            AddStep("Set Box Worth Display", () =>
            {
                mbox.SetWorthDisplay(1, 100);
            });
        }

    }
}
