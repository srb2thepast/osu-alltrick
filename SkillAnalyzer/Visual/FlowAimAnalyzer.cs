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
        protected override string OsuPath => @"C:\Users\alexh\Documents\osu!alltrick\osuAT\SkillAnalyzer\tools\MapMaker";
        protected override string MapLocation => @"output.osu"; // @"Songs\1392153 Release Hallucination - Chronostasis (1)\Release Hallucination - Chronostasis (Seni) [A Brilliant Petal Frozen In an Everlasting Moment].osu";
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
