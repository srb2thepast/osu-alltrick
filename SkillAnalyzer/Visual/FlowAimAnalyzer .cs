using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;
using osu.Game.Rulesets.Osu;

namespace SkillAnalyzer.Visual
{
    public class FlowAimAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => @"Songs\859515 K A Z M A S A - Bon Appetit S (Oldskool HappyHardcore Remix) (Short Ver)\K A Z M A S A - Bon Appetit S (Oldskool HappyHardcore Remix) (Short Ver.) (BarkingMadDog) [blend s (250 bpm)].osu";
        protected override List<ModInfo> AppliedMods => new List<ModInfo> { };
        public FlowAimAnalyzer() {
            
        }

        [Test]
        public void TestAddFlowAim()
        {
            EnableSkillStep(Skill.Flowaim);

            AddSeekStep("3:21:200");
        }
    }
}
