using osu.Framework.Testing;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK.Graphics;
using osuAT.Game.Objects;
using osuAT.Game.Objects.Displays;
using osuAT.Game.Types;
using osuAT.Game.Skills;
using osuTK;
using System;
using osuAT.Game.Skills.Resources;

namespace osuAT.Game.Tests.Visual
{
    [TestFixture]
    public class TestSceneSkills : TestScene
    {
        
        public TestSceneSkills()
        {
            Box background;
            FillFlowContainer flow;

            Add(new TooltipContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                background = new Box
                {
                    Colour = Color4.HotPink,
                    RelativeSizeAxes = Axes.Both,
                },
                new BasicScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = flow = new FillFlowContainer
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Spacing = new Vector2(0),
                        Direction = FillDirection.Full,
                    },
                }
                }
            });

            loadBoxes(flow);

            AddStep("reload all", () => loadBoxes(flow));
            AddStep("go to page 0", () => flow.Children.OfType<FullSkillBox>().ForEach(b => b.InfoBox.InfoBook.CurrentPage.Value = 0));
            AddStep("go to page 1", () => flow.Children.OfType<FullSkillBox>().ForEach(b => b.InfoBox.InfoBook.CurrentPage.Value = 1));
            AddStep("go to page 2", () => flow.Children.OfType<FullSkillBox>().ForEach(b => b.InfoBox.InfoBook.CurrentPage.Value = 2));
        }

        private void loadBoxes(FillFlowContainer flow) {
            flow.RemoveAll(d => { return true; }, true);
            foreach (ISkill skill in Skill.SkillList)
            {
                flow.Add(
                    new FullSkillBox
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        CurSkill = skill,
                        Scale = new Vector2(2),
                    }
                );
            }
            Schedule(() => flow.Children.OfType<FullSkillBox>().ForEach(b => b.Appear()));
        }

        private class Icon : Container, IHasTooltip
        {
            public LocalisableString TooltipText { get; }

            public SpriteIcon SpriteIcon { get; }

            public Icon(string name, IconUsage icon)
            {
                TooltipText = name;

                AutoSizeAxes = Axes.Both;
                Child = SpriteIcon = new SpriteIcon
                {
                    Icon = icon,
                    Size = new Vector2(60),
                };
            }
        }
    }
}
