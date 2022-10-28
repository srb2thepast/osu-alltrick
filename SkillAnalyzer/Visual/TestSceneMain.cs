// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Game;
using osu.Game.Screens;
using osu.Game.Screens.Play;
using osu.Game.Screens.Edit;
using osu.Game.Beatmaps;
using osu.Game.Tests.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Osu;
using osu.Game.Overlays;
using osu.Game.Tests;
using osu.Game.Tests.Visual;
using osuTK.Input;
using OsuRulesetInfo = osu.Game.Rulesets.RulesetInfo;
using ATBeatmap = osuAT.Game.Types.Beatmap;
using ATRulesetStore = osuAT.Game.Types.RulesetStore;
using osu.Framework.Testing;
using osu.Game.Screens.Edit.Components.Timelines.Summary;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Game.Graphics.Cursor;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Screens.Edit.Components;
using osuTK;
using osuTK.Graphics;
using osuAT.Game;
using osuAT.Game.Types;
using osuAT.Game.Skills;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Difficulty.Skills;
using static SkillAnalyzer.SkillAnalyzerTestBrowser;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    public class TestSceneMain : EditorTestScene
    {
        protected string MapLocation = @"Songs\1045600 MOMOIRO CLOVER Z - SANTA SAN\MOMOIRO CLOVER Z - SANTA SAN (A r M i N) [1-2-SANTA].osu";
        protected IBeatmap FocusedBeatmap;
        protected WorkingBeatmap WorkFocusedMap;
        // protected override bool IsolateSavingFromDatabase => false;
        protected override Ruleset CreateEditorRuleset() => new OsuRuleset();
        #region Classes + Hiding & Overrides
        protected class AnalyzerEditor : TestEditor
        {
            public AnalyzerEditor(EditorLoader loader) : base(loader) {

            }
            public class AnalyzerSummaryTimeline : SummaryTimeline
            {
                // add graph here
            }

            public class AnalyzerBottomBar : CompositeDrawable
            {
                

                public TestGameplayButton TestGameplayButton { get; private set; }

                [BackgroundDependencyLoader]
                private void load(OverlayColourProvider colourProvider, Editor editor)
                {
                    Anchor = Anchor.BottomLeft;
                    Origin = Anchor.BottomLeft;

                    RelativeSizeAxes = Axes.X;

                    Height = 60;

                    Masking = true;
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Colour = Color4.Black.Opacity(0.2f),
                        Type = EdgeEffectType.Shadow,
                        Radius = 10f,
                    };

                    InternalChildren = new Drawable[]
                    {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background4,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.Absolute, 170),
                            new Dimension(),
                            new Dimension(GridSizeMode.Absolute, 220),
                            new Dimension(GridSizeMode.Absolute, 120),
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new TimeInfoContainer { RelativeSizeAxes = Axes.Both },
                                new AnalyzerSummaryTimeline { RelativeSizeAxes = Axes.Both },
                                new PlaybackControl { RelativeSizeAxes = Axes.Both },
                                TestGameplayButton = new TestGameplayButton
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Left = 10 },
                                    Size = new Vector2(1),
                                    Action = editor.TestGameplay,
                                }
                            },
                        }
                    },
                }
                    };
                }
            }

            private AnalyzerBottomBar bottomBar;

            [BackgroundDependencyLoader]
            private void load() {
                OsuContextMenuContainer ContextMenu = (OsuContextMenuContainer)InternalChildren[2];
                CompositeDrawable editBottomBar = (CompositeDrawable)ContextMenu[2];
                ContextMenu.Remove(editBottomBar,true);
                ContextMenu.Add(bottomBar = new AnalyzerBottomBar());

            }
            SummaryTimeline timeline;
            public new bool Save()
            {
                Console.WriteLine("heya");
                return false;
            }

        }

        protected class AnalyzerEditorLoader : TestEditorLoader
        {
            protected override TestEditor CreateTestEditor(EditorLoader loader)
            {
                return new AnalyzerEditor(loader);
            }
        }
       
        private AnalyzerEditorLoader editorLoader;
        protected new TestEditor Editor => editorLoader.Editor;
        protected new EditorBeatmap EditorBeatmap => Editor.ChildrenOfType<EditorBeatmap>().Single();
        protected new EditorClock EditorClock => Editor.ChildrenOfType<EditorClock>().Single();

        #endregion

        
        public static event EventHandler<List<ISkill>> ListChanged;


        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {

            ///
            LabelledGraph skillGraph;
            BindableWithCurrent<List<ISkill>> current = new BindableWithCurrent<List<ISkill>>(new List<ISkill>());

            //
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
            int BeatmapHighestSpike = 50;
            Add(
                skillGraph = new LabelledGraph(
                new SpacedBarGraph
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Size = new Vector2(4, 3),
                    Scale = new Vector2(0.2f,0.2f)*new Vector2(2, 1),
                    MaxValue = BeatmapHighestSpike,
                    BarSpacing = 1f
                }) {
                    Anchor = Anchor.TopLeft,
                    Scale = new Vector2(0.9f)
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
                    current.Current.TriggerChange();
                };
                buttonSelectScroll.Add(newbox);
                i++;
            };

            skillGraph.SetValues(new SortedList<string, float> {
                {"flowaim",50 },
                {"consistency",30 },
                {"aim",80 },
                {"speed",80 },
            });

            current.Current.ValueChanged += delegate (ValueChangedEvent<List<ISkill>> skillList)
            {
                Console.WriteLine("invoked");
                skillList.NewValue.ForEach(d => Console.WriteLine(d.GetType()));
                // ListChanged.Invoke(this, skillList.NewValue);
                Console.WriteLine("catW");
                //
            };
            /////

            // BindableWithCurrent<List<ISkill>>
            Console.WriteLine("hi");
            Children.ForEach(d => { Console.WriteLine(d.GetType()); });
            Console.WriteLine(Audio);
            /*
            SkillAnalyzerTestBrowser.ListChanged += delegate (object sender, List<ISkill> skillList)
            {
                Console.WriteLine("catW");
            };
            */
        }

        protected void ReloadSkills(ValueChangedEvent<List<ISkill>> enabledList)
        {
            Console.WriteLine("Changed");
            foreach (ISkill skill in enabledList.NewValue) {
                Console.WriteLine(skill.GetType());
            }
        }

        protected override void LoadEditor()
        {
            base.Beatmap.Value = CreateWorkingBeatmap(base.Ruleset.Value);


            LoadScreen(editorLoader = new AnalyzerEditorLoader());
        }

        protected override IBeatmap CreateBeatmap(OsuRulesetInfo ruleset)
        {
            var osuatmap = new ATBeatmap()
            {
                FolderLocation = MapLocation
            };
            osuatmap.LoadMapContents(ATRulesetStore.GetByIRulesetInfo(ruleset));
            WorkFocusedMap = osuatmap.Contents.Workmap;
            FocusedBeatmap = WorkFocusedMap.Beatmap;
            WorkFocusedMap.LoadTrack();
            WorkFocusedMap.Track.Start();
            FocusedBeatmap.BeatmapInfo.BeatDivisor = 4;

            return FocusedBeatmap;
        }

        [Test]
        public void TestSeekToFirst()
        {
            AddAssert("track not running", () => !EditorClock.IsRunning);
        }

        public override void SetUpSteps()
        {
            AddStep("load editor", LoadEditor);
            AddUntilStep("wait for editor to load", () => Editor?.ReadyForUse ?? false);
            AddUntilStep("wait for beatmap updated", () => !base.Beatmap.IsDefault);
        }
    }
}
