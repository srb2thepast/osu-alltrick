// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization.Serializers;
using NuGet.Configuration;
using NuGet.Packaging.Rules;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Testing;
using osu.Game;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Cursor;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Screens;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Components;
using osu.Game.Screens.Edit.Components.Timelines.Summary;
using osu.Game.Screens.Edit.Compose;
using osu.Game.Screens.Play;
using osu.Game.Tests.Beatmaps;
using osu.Game.Tests.Visual;
using osuAT.Game;
using osuAT.Game.Screens;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Tests.Visual;
using osuAT.Game.Types;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;
using ATBeatmap = osuAT.Game.Types.Beatmap;
using ATRulesetStore = osuAT.Game.Types.RulesetStore;
using OsuBeatmap = osu.Game.Beatmaps.Beatmap;
using OsuRulesetInfo = osu.Game.Rulesets.RulesetInfo;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer.Visual
{
    // [~] TODO: Get beatmap audio working
    // [!] Remove AnalyzerPlaybackControl 
    public abstract partial class SkillAnalyzeScene : EditorTestScene
    {
        protected virtual string OsuPath => @"C:\Users\alexh\Documents\osu!alltrick\osuAT\SkillAnalyzer\Songs";
        protected override Ruleset CreateEditorRuleset() => new OsuRuleset();
        protected virtual string MapLocation => @"THE ORAL CIGARETTES - Mou ii Kai (Nevo) [Rain].osu";
        protected virtual List<ModInfo> AppliedMods => new List<ModInfo> { ModStore.Doubletime };

        public static List<ISkill> CurSkillList = new List<ISkill>();
        protected IBeatmap FocusedBeatmap;
        protected WorkingBeatmap WorkFocusedMap;
        protected ATBeatmap ATFocusedMap;
        protected List<DifficultyHitObject> CachedMapDiffHits;
        protected Score DummyScore;

        private LabelledBarGraph skillGraph;
        private TextFlowContainer debugContainer;
        private Dictionary<string, Dictionary<string, object>> skillDebugTextCached = new Dictionary<string, Dictionary<string, object>>(); // <Skill.Id,<ParamName,Value>>
        private bool sceneLoaded = false;
        private bool canUseEditor = false;
        protected static event EventHandler<bool> EditorLoaded;

        // settings //
        private bool scaleByCombo = false;
        // //////// //
        public SkillAnalyzeScene()
        {
            SaveStorage.SaveData.OsuPath = OsuPath;

            foreach (ISkill skill in Skill.SkillList)
            {
                skillDebugTextCached.Add(skill.Identifier, new Dictionary<string, object>());
            }
        }

        #region Classes + Hiding & Overrides

        protected override bool IsolateSavingFromDatabase => true;

        // Editor
        [Cached(typeof(IBeatSnapProvider), null)]
        [Cached(typeof(Editor))]
        protected partial class AnalyzerEditor : TestEditor
        {
            public AnalyzerPlaybackControl Playback;

            public AnalyzerEditor(EditorLoader loader) : base(loader)
            {

            }

            protected partial class AnalyzerSummaryTimeline : SummaryTimeline
            {
                [BackgroundDependencyLoader]
                private void load(OverlayColourProvider colourProvider)
                {
                    //Add(new StrainVisualizer {

                    //});
                }
            }

            protected partial class AnalyzerTimeInfoContainer : TimeInfoContainer
            {

                [Resolved]
                private EditorClock editorClock { get; set; }

                private OsuSpriteText msTime;
                [BackgroundDependencyLoader]
                private void load(OsuColour colours, OverlayColourProvider colourProvider)
                {
                    FontUsage torus2 = OsuFont.Numeric;
                    msTime = new OsuSpriteText
                    {
                        Colour = colours.Orange1,
                        Anchor = Anchor.CentreLeft,
                        Font = torus2.With(null, 18f, FontWeight.SemiBold),
                        Position = new Vector2(2f, 5f),
                        Text = "| 00000ms",
                        Scale = new Vector2(0.8f),
                        X = 65,
                        Y = 7
                    };
                    Add(msTime);
                }

                protected override void Update()
                {
                    base.Update();
                    msTime.Text = $"| {Math.Truncate(editorClock.CurrentTime)}ms";
                }
            }
            public partial class AnalyzerPlaybackControl : PlaybackControl
            {
                [Resolved]
                private EditorClock editorClock { get; set; }

                private BindableNumber<double> freqAdjust = new BindableDouble(1);

                private Track track;

                public double RateMult = 1;

                public void SetRateMult(double value)
                {
                    RateMult = value;

                    track?.RemoveAdjustment(AdjustableProperty.Frequency, freqAdjust);
                    freqAdjust.Value *= RateMult;
                    Console.WriteLine(RateMult + "| " + freqAdjust.Value);
                    track?.AddAdjustment(AdjustableProperty.Frequency, freqAdjust);
                    editorClock.Seek(10);
                }

                [BackgroundDependencyLoader]
                private void load()
                {
                    Track.BindValueChanged(tr =>
                    {
                        track = tr.NewValue;
                        SetRateMult(RateMult);
                    }, true);
                }
            }

            protected partial class AnalyzerBottomBar : CompositeDrawable
            {


                public TestGameplayButton TestGameplayButton { get; private set; }
                public AnalyzerPlaybackControl Playback { get; private set; }

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
                            new Dimension(GridSizeMode.Absolute, 160),
                            new Dimension(GridSizeMode.Absolute, 10),
                            //new Dimension(GridSizeMode.Absolute, 120),
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new AnalyzerTimeInfoContainer { RelativeSizeAxes = Axes.Both },
                                new AnalyzerSummaryTimeline { RelativeSizeAxes = Axes.Both },
                                Playback = new AnalyzerPlaybackControl { RelativeSizeAxes = Axes.Both, Width = 1.1f,X = -10},
                                Drawable.Empty()
                                /*TestGameplayButton = new TestGameplayButton
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Left = 10 },
                                    Size = new Vector2(1),
                                    Action = editor.TestGameplay,
                                }*/
                            },
                        }
                    },
                }
                    };
                }
            }


            private AnalyzerBottomBar bottomBar;

            [BackgroundDependencyLoader]
            private void load()
            {

            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                OsuContextMenuContainer ContextMenu = (OsuContextMenuContainer)InternalChildren[2];
                Container screenContainer = (Container)ContextMenu[0];
                Container<EditorScreen> composeScreenContainer = (Container<EditorScreen>)screenContainer.Child;

                Container editTopBar = (Container)ContextMenu[1];
                CompositeDrawable editBottomBar = (CompositeDrawable)ContextMenu[2];


                ContextMenu.Remove(editBottomBar, true);
                ContextMenu.Add(new Box()
                {
                    Colour = Colour4.FromHex("#404444"),
                    RelativeSizeAxes = Axes.X,
                    Height = editTopBar.Height,
                });
                ContextMenu.Remove(editTopBar, true);
                ContextMenu.Add(bottomBar = new AnalyzerBottomBar());
                bottomBar.Width = 0.73f;
                bottomBar.X = -375;
                bottomBar.Anchor = Anchor.BottomCentre;
                bottomBar.RelativeSizeAxes = Axes.X;
                ContextMenu.ChangeChildDepth(bottomBar, bottomBar.Depth - 2000);
                Playback = bottomBar.Playback;
                //editorScreen.Scale = new Vector2(0.95f);
                //editorScreen.X = editorScreen.Parent.BoundingBox.Width * editorScreen.Scale.X / 30;
                //editorScreen.Y = editorScreen.Parent.BoundingBox.Height * editorScreen.Scale.X  / 20 ;

                //editTopBar.Y = editTopBar.Height;
                EditorLoaded += (obj, ev) =>
                {
                    EditorScreen composeScreen = (EditorScreen)composeScreenContainer.Child;

                    var props = composeScreen.GetType().GetFields(
                             BindingFlags.NonPublic |
                             BindingFlags.Instance);

                    // Lol
                    HitObjectComposer popoverCont = null;
                    foreach (FieldInfo property in props)
                    {
                        Console.WriteLine(property.Name + "=" + property.GetValue(composeScreen));
                        if (property.Name == "composer")
                        {
                            popoverCont = (HitObjectComposer)property.GetValue(composeScreen);
                        }
                    }
                    //
                    Container leftBar = null;
                    Container rightBar = null;
                    Container centerField = null;
                    props = typeof(CompositeDrawable).GetFields(
                             BindingFlags.NonPublic |
                             BindingFlags.Instance);
                    foreach (FieldInfo property in props)
                    {
                        if (property.Name == "internalChildren")
                        {
                            IReadOnlyList<Drawable> intChildren = (IReadOnlyList<Drawable>)property.GetValue(popoverCont);

                            centerField = (Container)intChildren[0];
                            if (intChildren.Count == 1)
                                break; // already removed
                            leftBar = (Container)intChildren[1];
                            rightBar = (Container)intChildren[2];
                        }
                    }
                    // [~] maybe put the centerfield in a DrawSizeFillPerservingContaininer instead?
                    //centerField.Scale = new Vector2(0.8f);
                    //centerField.Position = new Vector2(110,75);
                    leftBar?.RemoveAndDisposeImmediately();
                    rightBar?.RemoveAndDisposeImmediately();
                    Console.WriteLine(
                    $"-----++Current editor path:++-----\n" +
                    $"{ContextMenu}\n" +
                    $"{screenContainer}\n" +
                    $"{composeScreenContainer}\n" +
                    $"{composeScreen}\n" +
                    $"{popoverCont}\n" +
                    $" \n");
                    Console.WriteLine("++--------------++");



                };
            }

            private SummaryTimeline timeline;
            public new bool Save()
            {
                Console.WriteLine("heya");
                return false;
            }

        }

        // Editor Loader
        protected partial class AnalyzerEditorLoader : TestEditorLoader
        {
            public new AnalyzerEditor Editor;
            protected override TestEditor CreateTestEditor(EditorLoader loader)
            {
                Editor = new AnalyzerEditor(loader);
                return Editor;
            }
        }

        // Hiding
        private AnalyzerEditorLoader editorLoader;
        protected new AnalyzerEditor Editor => editorLoader.Editor;
        protected new EditorBeatmap EditorBeatmap => Editor.ChildrenOfType<EditorBeatmap>().Single();
        protected new EditorClock EditorClock => Editor.ChildrenOfType<EditorClock>().Single();

        #endregion



        protected override IBeatmap CreateBeatmap(OsuRulesetInfo ruleset)
        {
            ATFocusedMap = new ATBeatmap()
            {
                FolderLocation = MapLocation
            };
            var atRuleset = ATRulesetStore.GetByIRulesetInfo(ruleset);
            ATFocusedMap.LoadMapContents(atRuleset, AppliedMods);
            WorkFocusedMap = ATFocusedMap.Contents.Workmap;
            List<Mod> osuModList = new List<Mod>();
            foreach (ModInfo mod in AppliedMods)
            {
                osuModList.Add(ModStore.ConvertToOsuMod(mod));
            }
            FocusedBeatmap = WorkFocusedMap.GetPlayableBeatmap(ruleset, osuModList);
            WorkFocusedMap.LoadTrack();
            WorkFocusedMap.Track.Start();
            FocusedBeatmap.BeatmapInfo.BeatDivisor = 4;
            CachedMapDiffHits = new List<DifficultyHitObject>(ATFocusedMap.Contents.DiffHitObjects);
            Console.WriteLine("Diffhits: " + ATFocusedMap.Contents.DiffHitObjects[0].StartTime + " | playable: " + ATFocusedMap.Contents.HitObjects[1].StartTime);
            Console.WriteLine("FB Diffhits: " + FocusedBeatmap.HitObjects[1].StartTime);
            DummyScore = new Score
            {
                RulesetName = atRuleset.Name,
                ScoreRuleset = atRuleset,
                Combo = 0,
                BeatmapInfo = ATFocusedMap,
                Mods = AppliedMods,
                AccuracyStats = new AccStat(ATFocusedMap.Contents.HitObjects.Count, 0, 0, 0),
            };
            return FocusedBeatmap;
        }

        protected IBeatmap SwitchFocusedMap(string mapLocation, OsuRulesetInfo ruleset, List<ModInfo> mods)
        {
            ATFocusedMap = new ATBeatmap()
            {
                FolderLocation = MapLocation
            };
            var atRuleset = ATRulesetStore.GetByIRulesetInfo(ruleset);
            ATFocusedMap.LoadMapContents(atRuleset, mods);
            WorkFocusedMap = ATFocusedMap.Contents.Workmap;
            List<Mod> osuModList = new List<Mod>();
            foreach (ModInfo mod in AppliedMods)
            {
                osuModList.Add(ModStore.ConvertToOsuMod(mod));
            }
            FocusedBeatmap = WorkFocusedMap.GetPlayableBeatmap(ruleset, osuModList);
            WorkFocusedMap.LoadTrack();
            WorkFocusedMap.Track.Start();
            FocusedBeatmap.BeatmapInfo.BeatDivisor = 4;
            CachedMapDiffHits = new List<DifficultyHitObject>(ATFocusedMap.Contents.DiffHitObjects);
            Console.WriteLine("Diffhits: " + ATFocusedMap.Contents.DiffHitObjects[0].StartTime + " | playable: " + ATFocusedMap.Contents.HitObjects[1].StartTime);
            Console.WriteLine("FB Diffhits: " + FocusedBeatmap.HitObjects[1].StartTime);
            DummyScore = new Score
            {
                RulesetName = atRuleset.Name,
                ScoreRuleset = atRuleset,
                Combo = 0,
                BeatmapInfo = ATFocusedMap,
                Mods = AppliedMods,
                AccuracyStats = new AccStat(ATFocusedMap.Contents.HitObjects.Count, 0, 0, 0),
            };
            return FocusedBeatmap;
        }

        #region Loading

        protected void CreateskillGraph()
        {
            // graph
            BasicScrollContainer buttonSelectScroll;
            int beatmapHighestSpike = 500;
            skillGraph = new LabelledBarGraph(new SpacedBarGraph
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Y = 310,
                Size = new Vector2(4, 3),
                Scale = new Vector2(0.17f, 0.17f) * new Vector2(2, 0.5f),
                MaxValue = beatmapHighestSpike,
                BarSpacing = 0.2f
            })
            {
                Anchor = Anchor.TopLeft,
                Scale = new Vector2(0.77f),
                Position = new Vector2(-200, 280)
            };
            // skill select scroll
            Add(new Container
            {
                Margin = new MarginPadding
                {
                    Top = 143,
                    Bottom = 143 * 4,
                },
                Size = new Vector2(155, 678),
                Masking = true,
                RelativeSizeAxes = Axes.Y,
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

            Add(
                skillGraph
            );

            for (int i = 0; i < Skill.SkillList.Count; i++)
            {
                ISkill skill = Skill.SkillList[i];
                Box bgbox = new Box
                {
                    Size = new Vector2(190, 0),
                    Y = (i) * 32 + 20,
                    Scale = new Vector2(0.9f),
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.CentreLeft,
                    Colour = new Color4(0.15f, 0.15f, 0.15f, 1)
                };
                SkillCheckbox newbox = new SkillCheckbox(skill)
                {
                    Y = i * 32 + 20,
                    Scale = new Vector2(0.9f),
                    Origin = Anchor.CentreLeft,
                };
                newbox.Current.ValueChanged += delegate (ValueChangedEvent<bool> Enabled)
                {
                    if (Enabled.NewValue && !CurSkillList.Contains(newbox.Skill))
                    {
                        CurSkillList.Add(newbox.Skill);
                    }
                    if (!Enabled.NewValue && CurSkillList.Contains(newbox.Skill))
                    {
                        CurSkillList.Remove(newbox.Skill);
                    }
                    UpdateBars(CurSkillList, true);
                };
                newbox.Current.Value = CurSkillList.Contains(newbox.Skill);
                buttonSelectScroll.Add(bgbox);
                buttonSelectScroll.Add(newbox);
                bgbox.Height = newbox.Height / 19 * 23;
            };
            skillGraph.SetValues(
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

            skillGraph.SetValues(
                new SortedList<string, float> {
                    { "test",2},
                    { "testa",30},
                    { "testb",7},
                    { "testc",51},
                }
            );

        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            Console.WriteLine("hiaaaaaaa");
            Console.WriteLine(Audio);

            // Debug Container
            Add(
                new Container
                {
                    Size = new Vector2(140, 678),
                    X = -140,
                    Y = 144,
                    Margin = new MarginPadding { Bottom = -60 },
                    Anchor = Anchor.TopRight,
                    RelativeSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        new Box {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FrameworkColour.GreenDarker
                        },
                        debugContainer = new TextFlowContainer()
                        {
                        Masking = false,
                        Margin = new MarginPadding { Left = 4 },
                            RelativeSizeAxes = Axes.Both,
                            LineSpacing = -0f,
                        },
                    }
                }
           );
            CreateskillGraph();

            LoadDefaultSteps();
        }


        protected override void LoadComplete()
        {
            base.LoadComplete();
            sceneLoaded = true;
        }

        protected void EditorFinishedLoading()
        {
            canUseEditor = true;
            UpdateBars(CurSkillList);

            double rate = 1;
            List<Mod> osuModList = new List<Mod>();
            foreach (ModInfo mod in AppliedMods)
            {
                Mod osuMod;
                osuModList.Add(osuMod = ModStore.ConvertToOsuMod(mod));
                if (osuMod is IApplicableToRate rateMod)
                {
                    rate = rateMod.ApplyToRate(0, rate);
                }
            }
            Editor.Playback.SetRateMult(rate);
            EditorLoaded.Invoke(this, true);
        }

        #endregion

        protected override void Update()
        {
            if (!sceneLoaded)
                return;
            if (editorLoader != default)
            {
                if ((Editor?.ReadyForUse ?? false) == true && (!canUseEditor)) { EditorFinishedLoading(); Console.WriteLine("line"); }
            }
            base.Update();
            if (canUseEditor)
            {
                Schedule(() =>
                {
                    UpdateBars();
                });
            }
        }

        protected void UpdateBars(bool skipCurhitCheck) => UpdateBars(CurSkillList, true);
        protected void UpdateBars() => UpdateBars(CurSkillList);

        protected void UpdateBars(List<ISkill> skillList, bool skipCurhitCheck = false)
        {

            if (!canUseEditor) return;
            if (CurSkillList == default | editorLoader == null) return;
            if (debugContainer.Parent == null) return;


            // update the score
            var p = curhitindex;
            updateClosestHitObjectIndex(EditorClock.CurrentTime);
            if (p != curhitindex)
            {
                Console.WriteLine($"Current Index:  {curhitindex}");
            }
            if (curhitindex == previoushitindex && !skipCurhitCheck)
                return;

            DummyScore.Combo = DummyScore.BeatmapInfo.GetMaxCombo(); // calculated combo with current amount of hit objects

            if (scaleByCombo)
            {
                DummyScore.BeatmapInfo.MaxCombo = CachedMapDiffHits.GetMaxCombo();
            }
            else
            {
                DummyScore.BeatmapInfo.MaxCombo = DummyScore.Combo;
            }

            /* Debug
            Console.WriteLine($"" +
            $"combo: {DummyScore.Combo} / {DummyScore.BeatmapInfo.MaxCombo} \n" +
            $"editor timee: {EditorClock.CurrentTime} \n" +
            $"closest index: {curhitindex} \n" +
            $"cached diff: {CachedMapDiffHits.Count} \n" +
            $"score diff: {DummyScore.BeatmapInfo.Contents.DiffHitObjects.Count} \n" +
            $"map diff: {ATFocusedMap.Contents.DiffHitObjects.Count}");
            */


            // Bar & debug section
            float largestPP = 0;
            SortedList<string, float> skillNameList = new SortedList<string, float>();
            List<ColourInfo> skillColors = new List<ColourInfo>();
            debugContainer.Text = "";

            skillList.ForEach(
                skill =>
                {
                    // SkillBar Updater
                    SkillCalcuator calculator = skill.GetSkillCalc(DummyScore);
                    calculator.EndIndex = curhitindex;
                    float skillpp = (float)calculator.SkillCalc();
                    if (skillpp < 0) skillpp = 0;

                    skillNameList.Add(skill.Identifier, skillpp);
                    if (largestPP < skillpp) largestPP = skillpp;
                    // Console.WriteLine(skill.Identifier + ": " + skillpp);

                    // BarUpdater
                    var props = calculator.GetType().GetFields(
                             BindingFlags.NonPublic |
                             BindingFlags.Instance);

                    if (props.Count() > 0)
                    {
                        debugContainer.AddText($"{skill.Identifier}", t =>
                        {
                            t.Font = new FontUsage("VarelaRound", size: 23);
                            t.Colour = skill.PrimaryColor;
                            t.Shadow = true;
                        });
                    }
                    foreach (FieldInfo property in props)
                    {
                        if (property.GetCustomAttribute(typeof(HiddenDebugValueAttribute)) != default)
                            continue;
                        var propval = property.GetValue(calculator);
                        if (propval is float floatval)
                        {
                            propval = Math.Truncate(floatval * 100) / 100;
                        }
                        if (propval is double dubval)
                        {
                            propval = Math.Truncate(dubval * 100) / 100;
                        }
                        debugContainer.AddText($"{property.Name}:\n   -> {propval}\n", t =>
                        {
                            t.Font = new FontUsage("VarelaRound", size: 17);
                            t.Colour = Colour4.White;
                            t.Shadow = true;
                        });
                        skillDebugTextCached[skill.Identifier][property.Name] = propval;
                    }
                }
            );


            skillNameList.ForEach(skillName =>
            {
                skillColors.Add(ColourInfo.GradientVertical(Skill.GetSkillByID(skillName.Key).PrimaryColor, Skill.GetSkillByID(skillName.Key).SecondaryColor));
            });

            skillGraph.SBarGraph.MaxValue = (largestPP < 500) ? 500 : largestPP;
            skillGraph.SetValues(skillNameList, skillColors);
            // Console.WriteLine("------------------");
        }

        private int previoushitindex = 0;
        private int curhitindex = 0;
        private double cachedCurTime = 0;
        /// <summary>
        /// Returns the closest hitobject's time position to the editor clock's currenttime.
        /// </summary>
        /// <remarks>It's possible for  could be heavily improved.</remarks>
        /// <returns></returns>
        private int updateClosestHitObjectIndex(double currentTime)
        {
            var hitList = CachedMapDiffHits;
            previoushitindex = curhitindex;
            if (cachedCurTime > currentTime)
            { // person seeked backwards, so go backwards from the last curhitindex
                cachedCurTime = currentTime;
                for (int i = curhitindex; i >= 0; i--)
                {
                    var time = hitList[i].StartTime * EditorClock.Track.Value.Rate;

                    if (time < currentTime)
                    {
                        curhitindex = i;
                        return i;
                    }
                }
                curhitindex = 0;
                return 0;
            }

            cachedCurTime = currentTime;
            // [~] this could be made a bit better
            // starts at curhitindex to avoid constantly looping through the whole map
            for (int i = Math.Max(0, curhitindex - 1); i < hitList.Count; i++)
            {
                var time = hitList[i].StartTime * EditorClock.Track.Value.Rate;
                if (time >= currentTime)
                {
                    curhitindex = Math.Max(0, i - 1);
                    return Math.Max(0, i - 1);
                }
            }
            curhitindex = hitList.Count - 1;
            return hitList.Count - 1;
        }

        protected override void LoadEditor()
        {
            base.Beatmap.Value = CreateWorkingBeatmap(base.Ruleset.Value);


            LoadScreen(editorLoader = new AnalyzerEditorLoader());

        }

        protected void LoadDefaultSteps()
        {

            AddLabel($"Setup");
            Setup();
            AddLabel("Options");
            Options();
        }

        public void Setup()
        {
            RunAllSteps();
            AddStep("load editor then enable bar", LoadEditor);
            AddUntilStep("wait until editor is loaded", () => canUseEditor);
        }

        public virtual void Options()
        {
            AddAssert("SETTINGS:", () => false); // Added a false assert so that it doesn't automatically enable all settings
            AddToggleStep("Overall Combo Scaling", (d) => { scaleByCombo = d; });
        }

        #region Step Methods

        protected delegate bool DebugValAssert(object obj);
        protected void AddDebugValueAssert(string description, ISkill skill, string key, DebugValAssert assert)
        {
            AddAssert(description, () =>
            {
                Dictionary<string, object> directory = skillDebugTextCached[skill.Identifier];
                var value = directory[key];
                return assert(value);
            });
        }

        protected void AddSeekStep(string time, bool stopclock = true)
        {
            AddSeekStep(convertTimeToMs(time), stopclock);
        }
        protected void AddSeekStep(double miliseconds, bool stopclock = true)
        {
            AddStep("Seek to " + miliseconds.ToEditorFormattedString(), () =>
            {
                EditorClock.Seek(miliseconds);
                if (stopclock)
                    EditorClock.Stop();

            });
        }

        protected void EnableSkillStep(ISkill skill)
        {
            AddUntilStep($"Add {skill.Identifier}", () =>
            {
                if (!CurSkillList.Contains(skill))
                    CurSkillList.Add(skill);
                UpdateBars(true);
                return CurSkillList.Contains(skill);
            });

        }

        [Obsolete("WIP")] // [!]
        protected void AddSwitchFocusedMapStep(string mapName, string mapLocation, OsuRulesetInfo ruleset, List<ModInfo> mods)
        {
            AddStep($"Switch to beatmap {mapName}", () =>
            {
                SwitchFocusedMap(mapLocation, ruleset, mods);
                Editor.SwitchToDifficulty(FocusedBeatmap.BeatmapInfo);
            });
        }

        private double convertTimeToMs(string time)
        {
            string[] split = time.Split(':');
            int min = int.Parse(split[0]);
            int sec = int.Parse(split[1]);
            int ms = int.Parse(split[2]);
            return min * 60 * 1000 + sec * 1000 + ms;
        }
        #endregion


        public override void SetUpSteps() { }
    }
}
