using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public partial class PrecisionAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => "t+pazolite & Camellia feat. Nanahira - Boku no Yume, Mecha Kuso Mugen Waki (Len) [Classic Another].osu";

        protected override List<ModInfo> AppliedMods => new List<ModInfo> { };

        [Test]
        public void TestAddPrecision()
        {
            EnableSkillStep(Skill.Precision);
        }
    }
}
