using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Rulesets.Osu;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public partial class CursorControlAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => @"Hoshimachi Suisei - GHOST (Amateurre) [Shooting Star].osu";
        protected override List<ModInfo> AppliedMods => new() { };

        [Test]
        public void TestCheckCurAvgSpacing()
        {
            EnableSkillStep(Skill.CursorControl);
            AddSeekStep("2:21:200");
            AddDebugValueAssert("curavgspacing > 50", Skill.CursorControl, "curAvgSpacing", (val) =>
            {
                return (double)val > 50;
            });
        }
    }
}
