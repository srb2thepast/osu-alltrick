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
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osuTK.Graphics.OpenGL;
using NuGet.Packaging.Rules;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using osu.Game.Rulesets.Mods;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using HtmlAgilityPack;
using System.Threading.Tasks;
using osu.Game.Screens.Edit.Compose;
using osu.Framework.Graphics.Sprites;
using System.Runtime.InteropServices;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using osu.Game.Graphics;
using osuAT.Game.Tests.Visual;
using osu.Framework.Graphics.Cursor;
using osu.Game.Screens.Play.Break;
using osu.Framework.Screens;
using Skill = osuAT.Game.Skills.Skill;
using RulesetStore = osuAT.Game.Types.RulesetStore;
using Beatmap = osu.Game.Beatmaps.Beatmap;
using OsuRulesetInfo = osu.Game.Rulesets.RulesetInfo;
using ATBeatmap = osuAT.Game.Types.Beatmap;
using ATRulesetStore = osuAT.Game.Types.RulesetStore;
using osu.Game.Graphics.Sprites;

namespace SkillAnalyzer.Visual
{

    public class TestSceneMain : osuATTestScene
    {

        public TestSceneMain() {

        }

        [Test]
        public void LoadEditor()
        {
            var scene = new MainScreen();
            Add(
                scene
            );
            AddStep("attempt to load", () => scene.ManuallyAddAttributeTests());
            // AddStep("load", () => Add(new MainScreen()));
        }

        
    }
    // [!] TODO: Get beatmap audio working
    internal class MainScreen : EditorTestScene
    {


        public static List<ISkill> CurSkillList = new List<ISkill>();
        protected override Ruleset CreateEditorRuleset() => new OsuRuleset();
        protected string MapLocation = @"Songs\257607 xi - FREEDOM DiVE\xi - FREEDOM DiVE (elchxyrlia) [Arles].osu"; // @"Songs\1045600 MOMOIRO CLOVER Z - SANTA SAN\MOMOIRO CLOVER Z - SANTA SAN (A r M i N) [1-2-SANTA].osu";
        protected IBeatmap FocusedBeatmap;
        protected WorkingBeatmap WorkFocusedMap;
        protected ATBeatmap ATFocusedMap;
        protected List<DifficultyHitObject> CachedMapDiffHits;
        private Score dummyScore;

        protected LabelledBarGraph SkillGraph;
        private TextFlowContainer debugContainer;
        // Skill.Id<ParamName,Value>
        private Dictionary<string, Dictionary<string, object>> skillDebugTextCached = new Dictionary<string, Dictionary<string, object>>();
        private string debugTextCached;

        private bool sceneLoaded = false;
        private bool canUseEditor = false;

        protected static event EventHandler<bool> EditorLoaded;

        // settings //
        private bool scaleByCombo = false;
        // //////// //

        // protected override bool IsolateSavingFromDatabase => false;
        #region Classes + Hiding & Overrides

        // Editor
        [Cached(typeof(IBeatSnapProvider), null)]
        [Cached(typeof(Editor))]
        protected class AnalyzerEditor : TestEditor
        {
            private TextFlowContainer debugTextFlow;

            public AnalyzerEditor(EditorLoader loader) : base(loader) {

            }
            public class AnalyzerSummaryTimeline : SummaryTimeline
            {
                [BackgroundDependencyLoader]
                private void load(OverlayColourProvider colourProvider) {
                    //Add(new StrainVisualizer {

                //});
                }
            }

            public class AnalyzerTimeInfoContainer : TimeInfoContainer
            {

                [Resolved]
                private EditorClock editorClock { get; set; }

                OsuSpriteText msTime;
                [BackgroundDependencyLoader]
                private void load(OsuColour colours, OverlayColourProvider colourProvider)
                {
                    // [!] Torus License
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
                                new AnalyzerTimeInfoContainer { RelativeSizeAxes = Axes.Both },
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
                    foreach (FieldInfo property in props) {
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
                    centerField.Scale = new Vector2(0.8f);
                    centerField.Position = new Vector2(110,75);
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

            SummaryTimeline timeline;
            public new bool Save()
            {
                Console.WriteLine("heya");
                return false;
            }

        }

        // Editor Loader
        protected class AnalyzerEditorLoader : TestEditorLoader
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


        public MainScreen() {
            foreach (ISkill skill in Skill.SkillList) {
                skillDebugTextCached.Add(skill.Identifier, new Dictionary<string, object>());    
            }
        }

        protected override IBeatmap CreateBeatmap(OsuRulesetInfo ruleset)
        {
            var osuatmap = new ATBeatmap()
            {
                FolderLocation = MapLocation
            };
            var atRuleset = ATRulesetStore.GetByIRulesetInfo(ruleset);
            osuatmap.LoadMapContents(atRuleset);
            WorkFocusedMap = osuatmap.Contents.Workmap;
            FocusedBeatmap = WorkFocusedMap.Beatmap;
            WorkFocusedMap.LoadTrack();
            WorkFocusedMap.Track.Start();
            FocusedBeatmap.BeatmapInfo.BeatDivisor = 4;
            ATFocusedMap = WorkFocusedMap.ConvertToATMap(MapLocation);
            CachedMapDiffHits = new List<DifficultyHitObject>(ATFocusedMap.Contents.DiffHitObjects);
            dummyScore = new Score
            {
                RulesetName = atRuleset.Name,
                ScoreRuleset = atRuleset,
                Combo = 0,
                BeatmapInfo = ATFocusedMap,
                Mods = new List<ModInfo>(),
                AccuracyStats = new AccStat(ATFocusedMap.Contents.HitObjects.Count, 0, 0, 0),
            };
            return FocusedBeatmap;
        }

        protected void CreateSkillGraph()
        {
            // graph
            BasicScrollContainer buttonSelectScroll;
            int beatmapHighestSpike = 500;
            SkillGraph = new LabelledBarGraph(new SpacedBarGraph
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Y = 310,
                Size = new Vector2(4, 3),
                Scale = new Vector2(0.2f, 0.2f) * new Vector2(2, 0.5f),
                MaxValue = beatmapHighestSpike,
                BarSpacing = 0.2f
            })
            {
                Anchor = Anchor.TopLeft,
                Scale = new Vector2(0.8f),
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
                SkillGraph
            );

            for (int i = 0; i<Skill.SkillList.Count; i++)
            {
                ISkill skill = Skill.SkillList[i];
                Box bgbox = new Box
                {
                    Size = new Vector2(190,0),
                    Y = (i) * 32+20,
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
					UpdateBars(CurSkillList,true);
                };
                newbox.Current.Value = CurSkillList.Contains(newbox.Skill);
                buttonSelectScroll.Add(bgbox);
                buttonSelectScroll.Add(newbox);
                bgbox.Height = newbox.Height/19 *23;
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
                    Anchor = Anchor.TopRight,
                    Children = new Drawable[]
                    {
                        new Box {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FrameworkColour.GreenDarker
                        },
                        debugContainer = new TextFlowContainer()
                        {
                            Padding = new MarginPadding{ Left = 7},
                            RelativeSizeAxes = Axes.Both,
                        },
                    }
                }
           );
            CreateSkillGraph();
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
            EditorLoaded.Invoke(this,true);
        }

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

        protected void UpdateBars(bool skipCurhitCheck) => UpdateBars(CurSkillList,true);
        protected void UpdateBars() => UpdateBars(CurSkillList);

        protected void UpdateBars(List<ISkill> skillList,bool skipCurhitCheck = false) {

            if (!canUseEditor) return;
            if (CurSkillList == default | editorLoader == null) return;
            if (debugContainer.Parent == null) return;
            

            // update the score
            updateClosestHitObjectIndex(EditorClock.CurrentTime);

            if (curhitindex == previoushitindex && !skipCurhitCheck)
                return;

            List<DifficultyHitObject> cachedClone = new List<DifficultyHitObject>(CachedMapDiffHits);
            dummyScore.BeatmapInfo.Contents.DiffHitObjects = cachedClone.Where(d =>
            {
                return d.Index < curhitindex;
            }).ToList();
            dummyScore.Combo = dummyScore.BeatmapInfo.GetMaxCombo(); // calculated combo with current amount of hit objects
            
            if (scaleByCombo)
            {
                dummyScore.BeatmapInfo.MaxCombo = CachedMapDiffHits.GetMaxCombo();
            }
            else
            {
                dummyScore.BeatmapInfo.MaxCombo = dummyScore.Combo;
            }

                    /* Debug
            Console.WriteLine($"" +
                    $"combo: {dummyScore.Combo} / {dummyScore.BeatmapInfo.MaxCombo} \n" +
                    $"editor timee: {EditorClock.CurrentTime} \n" +
                    $"closest index: {curhitindex} \n" +
                    $"cached diff: {CachedMapDiffHits.Count} \n" +
                    $"score diff: {dummyScore.BeatmapInfo.Contents.DiffHitObjects.Count} \n" +
                    $"map diff: {ATFocusedMap.Contents.DiffHitObjects.Count}");
                    */
            // Bar section
            float largestPP = 0;
            SortedList<string, float> skillNameList = new SortedList<string, float>();
            List<ColourInfo> skillColors = new List<ColourInfo>();

            skillList.ForEach(
                skill =>
                {
                    float skillpp = (float)skill.SkillCalc(dummyScore);
                    if (skillpp < 0) skillpp = 0;

                    skillNameList.Add(skill.Identifier, skillpp);
                    if (largestPP < skillpp) largestPP = skillpp;
                    // Console.WriteLine(skill.Identifier + ": " + skillpp);
                }
            );

            skillNameList.ForEach(skillName => {
                skillColors.Add(ColourInfo.GradientVertical(Skill.GetSkillByID(skillName.Key).PrimaryColor, Skill.GetSkillByID(skillName.Key).SecondaryColor));
            });

            SkillGraph.SBarGraph.MaxValue = (largestPP < 500) ? 500 : largestPP;
            SkillGraph.SetValues(skillNameList, skillColors);

            // Debug text section
            debugContainer.Text = "";
            debugTextCached = "";
            skillList.ForEach(skill =>
            {
                bool titlemade = false;
                // Console.WriteLine($"Cur Skill: {skill.GetType()}");
                var props = skill.GetType().GetFields(
                         BindingFlags.NonPublic |
                         BindingFlags.Instance);
                foreach (FieldInfo property in props)
                {
                    var attrs = property.GetCustomAttributes<SkillDebugValueAttribute>(true);

                    if (attrs.Count() > 0 && titlemade == false) {
                        debugContainer.AddText($"{skill.Identifier}", t => {
                            t.Font = new FontUsage("VarelaRound", size: 23);
                            t.Colour = skill.PrimaryColor;
                            t.Shadow = true;
                        });
                        debugTextCached += $"\n{skill.Identifier}";
                        titlemade = true;
                    }
                    foreach (object attr in attrs)
                    {
                        var propval = property.GetValue(skill);
                        if (propval is float floatval)
                        {
                            propval = Math.Truncate(floatval * 100) / 100;
                        }
                        if (propval is double dubval)
                        {
                            propval = Math.Truncate(dubval * 100) / 100;
                        }
                        debugContainer.AddText($"{property.Name}:\n   -> {propval}", t => {
                            t.Font = new FontUsage("VarelaRound", size: 17);
                            t.Colour = Colour4.White;
                            t.Shadow = true;
                        });
                        debugTextCached += $"\n{property.Name}:\n   -> {propval}";
                        skillDebugTextCached[skill.Identifier][property.Name] = propval;
                    }
                }
            });
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
        private int updateClosestHitObjectIndex(double currentTime) {
            // [!] Big bug where the acutaly closest hit object isnt return due to the amount of time
            // it takes for the clock to seek to a point.
            var hitList = CachedMapDiffHits;
            previoushitindex = curhitindex;
            if (cachedCurTime > currentTime) { // person seeked backwards, so just reset
                curhitindex = 0;
            }
            cachedCurTime = currentTime;
            // starts at curhitindex to avoid constantly looping through the whole map
            for (int i = curhitindex; i < hitList.Count; i++) { 
                var time = hitList[i].StartTime;
                if (time >= currentTime) {
                    curhitindex = i;
                    return i;
                }
            }
            return hitList.Count-1;
        }

        
        protected override void LoadEditor()
        {
            base.Beatmap.Value = CreateWorkingBeatmap(base.Ruleset.Value);


            LoadScreen(editorLoader = new AnalyzerEditorLoader());
        }

        [Test]
        public async void Setup()
        {
            RunAllSteps();
            AddStep("load editor then enable bar", LoadEditor);
            AddUntilStep("wait until editor is loaded", () => canUseEditor);
        }

        [Test]
        public void TestCheckCurAvgSpacing() {

            AddStep("Seek to 2:12:404", () => EditorClock.Seek(convertTimeToMs("2:12:404")));
            debugContainer.Text = "";
            AddUntilStep("Add flowaim", () => {
                if (!CurSkillList.Contains(Skill.Flowaim))
                    CurSkillList.Add(Skill.Flowaim);
                return CurSkillList.Contains(Skill.Flowaim);
            });
            AddAssert("curAvgSpacing > 50", () => {
                Dictionary<string,object> directory = skillDebugTextCached[Skill.Flowaim.Identifier];
                double amount = (double)directory["curAvgSpacing"];
                return amount > 50;
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

        [Test]
        public void Options()
        {
            AddAssert("SETTINGS:", () => false); // Added a false assert so that it doesn't automatically enable all settings
            AddToggleStep("Overall Combo Scaling", (d) => { scaleByCombo = d; });
        }

        public override void SetUpSteps() {}
    }
}
