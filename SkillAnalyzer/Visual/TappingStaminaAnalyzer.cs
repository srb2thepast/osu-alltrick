using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Rulesets.Osu;
using osuAT.Game.Types;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public partial class TappingStaminaAnalyzer : SkillAnalyzeScene
    {
        protected override string MapLocation => @"Blend S\K A Z M A S A - Bon Appetit S (Oldskool HappyHardcore Remix) (Short Ver.) (BarkingMadDog) [blend s (220 bpm)].osu";
        protected override List<ModInfo> AppliedMods => new() {  };

        [Test]
        public void TestCheckStreamLength()
        {
            EnableSkillStep(Skill.TappingStamina);

            // csl >= the count of notes that are definetly a stream. Here, 32 stream notes appear.
            AddSeekStep("00:17:537");
            AddDebugValueAssert("curStreamLength > 32", Skill.TappingStamina, "curStreamLength", (val) =>
            {
                return (double)val > 32;
            });
            // End of first section, the collection of strain from all those streams, despite the sliders, makes it feel
            // about the same way you'd feel if you just streamed 170 notes in a row.
            AddSeekStep("00:35:322");
            AddDebugValueAssert("csl builds up to > 170", Skill.TappingStamina, "curStreamLength", (val) =>
            {
                return (double)val > 170;
            });

            // continous bursts and streams, although it drops from time to time due to seconds of freedom,
            // the length it feels like still rises to be above 300.
            AddSeekStep("00:50:050");
            AddDebugValueAssert("csl builds to > 300", Skill.TappingStamina, "curStreamLength", (val) =>
            {
                return (double)val > 300;
            });

            // even this short of a time frame is a lot of breathing time, and so csl drops to < 200.
            // Remove the two repeat sliders and take a look to see.
            AddSeekStep("00:50:834");
            AddDebugValueAssert("csl falls to < 200", Skill.TappingStamina, "curStreamLength", (val) =>
            {
                return (double)val < 200;
            });

            // due to all of the continous tapping strain from the kiai section (which built up to around a length of 170),
            // the final ending part of the map is even harder than the first, causing csl to build way higher.
            AddSeekStep("02:06:180");
            AddDebugValueAssert("csl builds to > 380", Skill.TappingStamina, "curStreamLength", (val) =>
            {
                return (double)val > 300;
            });
        }
    }
}
