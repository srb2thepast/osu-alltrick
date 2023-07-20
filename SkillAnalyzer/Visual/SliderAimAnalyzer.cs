using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public partial class SliderAimAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => "ShinRa-Bansho - Mugen Shitto Gekijou 666 (Hey lululu) [Jealous Theater 666th Act].osu";

        protected override List<ModInfo> AppliedMods => new List<ModInfo> { ModStore.Nightcore, ModStore.Hardrock};

        public SliderAimAnalyzer()
        {
        }

        [Test]
        public void TestCheckCurAvgSpacing()
        {
            EnableSkillStep(Skill.SliderAim);
            AddSeekStep("2:21:200");
            AddDebugValueAssert("curavgspacing > 50", Skill.SliderAim, "curAvgSpacing", (val) =>
            {
                return (double)val > 50;
            });
        }
    }
}
