using NUnit.Framework;
using osu.Framework.Graphics;
using osuAT.Game.Objects.Displays;

using osuAT.Game.Objects.LazerAssets.Mod;
using osuAT.Game.Types;

namespace osuAT.Game.Tests.Visual.Display
{
    public class TestSceneRulesetDisplay : osuATTestScene
    {
        [Test]
        public void TestRuleset()
        {
            AddStep("create ruleset display", () =>
            {
                Child = new RulesetDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RulesetList = new RulesetInfo[] {

                        Rulesets.Osu,
                    }
                };
            });
            AddStep("new 2 ruleset display", () =>
            {
                Child = new RulesetDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RulesetList = new RulesetInfo[] {

                        Rulesets.Osu,
                        Rulesets.Mania,
                    }
                };
            });
            AddStep("new 3 ruleset display", () =>
            {
                Child = new RulesetDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RulesetList = new RulesetInfo[] {

                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                    }
                };
            });
            AddStep("new 4 ruleset display", () =>
            {
                Child = new RulesetDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RulesetList = new RulesetInfo[] {

                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                        Rulesets.Catch,
                    }
                };
            });
            AddStep("new 9 ruleset display", () =>
            {
                Child = new RulesetDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RulesetList = new RulesetInfo[] {

                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                        Rulesets.Catch,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                    }
                };
            });
            AddStep("new 1293871239087123 ruleset display", () =>
            {
                Child = new RulesetDisplay
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RulesetList = new RulesetInfo[] {

                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                        Rulesets.Catch,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                        Rulesets.Catch,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                        Rulesets.Catch,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Osu,
                        Rulesets.Mania,
                        Rulesets.Taiko,
                    }
                };
                Child.ScaleTo(0.5f);
            });
        }
    }
}
