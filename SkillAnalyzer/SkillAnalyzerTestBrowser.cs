using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Game;
using osu.Game.Tests;
using osu.Game.Tests.Visual;
using osuAT.Game;
using osuAT.Game.Tests;
using osu.Framework.Input.Handlers.Tablet;
using osuTK;
using osu.Game.Beatmaps;
using osu.Game.Graphics.UserInterface;
using System;
using osu.Framework.Extensions.IEnumerableExtensions;
using osuAT.Game.Skills;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Difficulty.Skills;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer
{

    public class SkillAnalyzerTestBrowser : TestBrowser {
        public SkillAnalyzerTestBrowser(string assemblyName) : base(assemblyName) {

        }

        FillFlowContainer buttonSelectFlow;
        BasicScrollContainer buttonSelectScroll;

        protected override void LoadComplete()
        {
            base.LoadComplete();
            var leftcontainer = Children[1];
            Remove(leftcontainer,true);
            var maincontainer = (Container)Children[0];
            
            Add(new Container
            {
                RelativeSizeAxes = Axes.Y,
                Size = new Vector2(200, 1),
                Masking = true,
                Children = new Drawable[]
                {
                    new SafeAreaContainer
                    {
                        SafeAreaOverrideEdges = Edges.Left | Edges.Top | Edges.Bottom,
                        RelativeSizeAxes = Axes.Both,
                        Child = new Box
                        {
                            Colour = Colour4.DimGray,
                            RelativeSizeAxes = Axes.Both
                        }
                    },
                    buttonSelectFlow = new FillFlowContainer
                    {
                        Direction = FillDirection.Vertical,
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            buttonSelectScroll = new BasicScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Masking = false,
                            }

                        }
                        }
                    }
                }
            );

            int i = 0;
            foreach (ISkill skill in Skill.SkillList) {
                buttonSelectScroll.Add(new SettingsCheckbox()
                {
                    LabelText = skill.Name,
                    Y = i*50+20
                });
                i++;
            };
            
        }
    }

    public class SkillAnalyzerTestBrowserLoader : OsuGameBase
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddRange(new Drawable[]
            {
                new SkillAnalyzerTestBrowser("SkillAnalyzer"),
                new CursorContainer()
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            
            this.Window.Title = "osu!alltrick's SkillAnalyzer";
            SaveStorage.Init();
            ScoreImporter.Init();
            foreach (var handler in Host.AvailableInputHandlers)
            {
                if (handler is ITabletHandler tabhandler)
                {
                    Schedule(() => {
                        tabhandler.AreaSize.Value = new Vector2(75.6f, 48.17f);
                        tabhandler.AreaOffset.Value = new Vector2(73.42f, 50.18f);
                    });
                }
            }
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
