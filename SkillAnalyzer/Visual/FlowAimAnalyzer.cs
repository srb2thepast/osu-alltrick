using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Rulesets.Osu;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public partial class FlowAimAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => @"Release Hallucination - Chronostasis (Seni) [A Brilliant Petal Frozen In an Everlasting Moment].osu";
        protected override List<ModInfo> AppliedMods => new List<ModInfo> { };
        public FlowAimAnalyzer()
        {

        }



        [Test]
        public void TestAddFlowAim()
        {
            EnableSkillStep(Skill.Flowaim);
        }
        [Test]
        public void TestCompareSpikes()
        {
            EnableSkillStep(Skill.Flowaim);

            AddSeekStep("5:15:445");
            AddDebugValueAssert("aim difficulty > 16", Skill.Flowaim, "aimDifficulty", (val) =>
            {
                return (double)val > 16;
            });

            AddSeekStep("0:41:620");
            AddDebugValueAssert("aim difficulty < 12", Skill.Flowaim, "aimDifficulty", (val) =>
            {
                return (double)val < 12;
            });
        }
    }
}
