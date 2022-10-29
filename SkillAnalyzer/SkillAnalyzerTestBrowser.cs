using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Framework.Allocation;
using osu.Game;
using osu.Game.Tests;
using osu.Game.Tests.Visual;
using osu.Framework.Input.Handlers.Tablet;
using osu.Game.Beatmaps;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Difficulty.Skills;
using osuAT.Game;
using osuAT.Game.Skills;
using osuAT.Game.Tests;
using Skill = osuAT.Game.Skills.Skill;
using SkillAnalyzer.Visual;
using osuTK;
using osuTK.Graphics;
using osu.Framework.Bindables;
using NUnit.Framework;
using osu.Game.Configuration;
using osu.Game.Rulesets.Catch.UI;
using osu.Framework.Logging;

namespace SkillAnalyzer
{

    public class SkillAnalyzerTestBrowser : TestBrowser {
        public SkillAnalyzerTestBrowser(string assemblyName) : base(assemblyName) {

        }

        private readonly BindableWithCurrent<List<ISkill>> current = new BindableWithCurrent<List<ISkill>>(new List<ISkill>());

        public static event EventHandler<List<ISkill>> ListChanged;

        public static LabelledBarGraph SkillGraph;
        public static SpacedBarGraph SkillGraphFC;

        protected void CreateSkillGraph()
        {
            ListChanged += new EventHandler<List<ISkill>>(delegate (Object o, List<ISkill> skillList)
            {
                //snip
            });
            var leftcontainer = Children[1];
            Remove(leftcontainer, true);
            var maincontainer = (Container)Children[0];
            BasicScrollContainer buttonSelectScroll;
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
                            Colour = new Color4(25, 25, 25, 255),
                            RelativeSizeAxes = Axes.Both
                        }
                    },
                    new FillFlowContainer
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
            });
            int BeatmapHighestSpike = 500;
            Add(
                SkillGraph = new LabelledBarGraph(
                new SpacedBarGraph
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Size = new Vector2(4, 3),
                    Scale = new Vector2(0.2f, 0.2f) * new Vector2(2, 1),
                    MaxValue = BeatmapHighestSpike,
                    BarSpacing = 0.2f
                })
                {
                    Anchor = Anchor.TopLeft,
                    Scale = new Vector2(0.9f),
                    Position = new Vector2(-300, 200)
                });

            int i = 0;
            foreach (ISkill skill in Skill.SkillList)
            {
                buttonSelectScroll.Add(
                    new Box
                    {
                        Size = new Vector2(190, 23),
                        Y = i * 30 + 20,
                        X = 3,
                        Anchor = Anchor.TopLeft,
                        Colour = new Color4(0.15f, 0.15f, 0.15f, 1)
                    }
                );
                SkillCheckbox newbox = new SkillCheckbox(skill)
                {
                    Y = i * 30 + 20,

                };
                newbox.Current.ValueChanged += delegate (ValueChangedEvent<bool> Enabled)
                {
                    var newList = current.Current.Value;
                    if (Enabled.NewValue)
                    {
                        newList.Add(newbox.Skill);
                    }
                    else
                    {
                        newList.Remove(newbox.Skill);
                    }
                    Console.WriteLine(current.Current.Value);
                    ListChanged.Invoke(this, newList);
                };
                buttonSelectScroll.Add(newbox);
                i++;
            };

            SkillGraph.SetValues(
                new SortedList<string, float> {
                    { "test",2},
                    { "testa",30},
                    { "testb",7},
                    { "testc",50},
                    { "testd",2},
                    { "teste",32},
                    { "testf",7},
                    { "testg",230},
                }
            );

            SkillGraph.SetValues(
                new SortedList<string, float> {
                    { "test",2},
                    { "testa",30},
                    { "testb",7},
                    { "testc",50},
                }
            );
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            CreateSkillGraph();
            
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
            // Logger.Level = LogLevel.Verbose;
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
