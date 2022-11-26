using NUnit.Framework;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public class SliderAimAnalyzer : SkillAnalyzeScene
    {
        public SliderAimAnalyzer() {
            
        }

        [Test]
        public void TestCheckCurAvgSpacing()
        {
            EnableSkillStep(Skill.SliderAim);
            AddSeekStep("2:21:200");
            AddDebugValueAssert("curavgspacing > 50", Skill.SliderAim, "curAvgSpacing", (val) => {
                return (double)val > 50;
            });
        }
    }
}
