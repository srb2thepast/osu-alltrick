using NUnit.Framework;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public class CursorControlAnalyzer : SkillAnalyzeScene
    {
        public CursorControlAnalyzer() {
            
        }

        [Test]
        public void TestCheckCurAvgSpacing()
        {
            EnableSkillStep(Skill.CursorControl);
            AddSeekStep("2:21:200");
            AddDebugValueAssert("curavgspacing > 50", Skill.CursorControl, "curAvgSpacing", (val) => {
                return (double)val > 50;
            });
        }
    }
}
