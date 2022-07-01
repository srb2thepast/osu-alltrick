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

                        RulesetStore.Osu,
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

                        RulesetStore.Osu,
                        RulesetStore.Mania,
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

                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
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

                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
                        RulesetStore.Catch,
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

                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
                        RulesetStore.Catch,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
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

                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
                        RulesetStore.Catch,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
                        RulesetStore.Catch,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
                        RulesetStore.Catch,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Osu,
                        RulesetStore.Mania,
                        RulesetStore.Taiko,
                    }
                };
                Child.ScaleTo(0.5f);
            });
        }
    }
}

// () => {}
