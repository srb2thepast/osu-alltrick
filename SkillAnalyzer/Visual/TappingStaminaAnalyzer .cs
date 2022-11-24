using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public class TappingStaminaAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => @"Songs\807850 THE ORAL CIGARETTES - Mou Ii kai\THE ORAL CIGARETTES - Mou ii Kai (Nevo) [Rain].osu";
        protected override List<ModInfo> AppliedMods => new List<ModInfo> { ModStore.Doubletime };
        public TappingStaminaAnalyzer() {
            
        }

        [Test]
        public void TestCheckCurAvgSpacing()
        {
            EnableSkillStep(Skill.TappingStamina) ;
            AddSeekStep("2:21:200");
            AddDebugValueAssert("curWorth > 1000", Skill.TappingStamina, "curWorth", (val) => {
                return (double)val > 50;
            });
        }
    }
}
