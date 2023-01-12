﻿using System;
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
        protected override string MapLocation => @"AimPatterns\Hexagon.osu";
        protected override List<ModInfo> AppliedMods => new List<ModInfo> { ModStore.Nightcore };
        public FlowAimAnalyzer()
        {

        }

        [Test]
        public void TestAddFlowAim()
        {
            EnableSkillStep(Skill.Flowaim);

            AddSeekStep("3:21:200  ");
        }
    }
}