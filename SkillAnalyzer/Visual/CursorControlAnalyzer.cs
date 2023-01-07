using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;
using osu.Game.Rulesets.Osu;

namespace SkillAnalyzer.Visual
{
    public partial class CursorControlAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => @"lapix - Nexta (DTM9 Nowa) [ending].osu";
        //protected override List<ModInfo> AppliedMods => new List<ModInfo> { };
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
