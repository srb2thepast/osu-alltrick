using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;
using osu.Game.Rulesets.Osu;

namespace SkillAnalyzer.Visual
{
    public partial class FlowAimAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => @"AimPatterns\Hexagon.osu";
        protected override List<ModInfo> AppliedMods => new List<ModInfo> { ModStore.Nightcore };
        public FlowAimAnalyzer() { 
            
        }

        [Test]
        public void TestAddFlowAim()
        {
            EnableSkillStep(Skill.Flowaim);

            AddSeekStep("3:21:200  ");
        }
    }
}
