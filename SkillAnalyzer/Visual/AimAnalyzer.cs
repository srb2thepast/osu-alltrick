using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Rulesets.Osu;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public partial class AimAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => @"Natsuzora Yell (TV Size) (Riana) [Nameless' Extra].osu";
        protected override List<ModInfo> AppliedMods => new List<ModInfo> { ModStore.Doubletime };

        public AimAnalyzer()
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
            EnableSkillStep(Skill.Aim);
        }
    }
}
