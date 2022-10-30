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
using RulesetStore = osuAT.Game.Types.RulesetStore;
using Beatmap = osu.Game.Beatmaps.Beatmap;
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

namespace SkillAnalyzer.Visual
{

    // [!] TODO: Get beatmap audio working
    public class TestSceneMain : EditorTestScene
    {
        protected override Ruleset CreateEditorRuleset() => new OsuRuleset();

        protected string MapLocation = @"Songs\257607 xi - FREEDOM DiVE\xi - FREEDOM DiVE (elchxyrlia) [Arles].osu"; // @"Songs\1045600 MOMOIRO CLOVER Z - SANTA SAN\MOMOIRO CLOVER Z - SANTA SAN (A r M i N) [1-2-SANTA].osu";
        protected IBeatmap FocusedBeatmap;
        protected WorkingBeatmap WorkFocusedMap;
        protected ATBeatmap ATFocusedMap;
        protected List<DifficultyHitObject> CachedMapDiffHits;
        private TextFlowContainer debugContainer;
        private Score dummyScore;

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
                
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                OsuContextMenuContainer ContextMenu = (OsuContextMenuContainer)InternalChildren[2];
                Container screenContainer = (Container)ContextMenu[0];
                Container<EditorScreen> editorScreen = (Container<EditorScreen>)screenContainer.Child;
                
                CompositeDrawable editBottomBar = (CompositeDrawable)ContextMenu[2];
                ContextMenu.Remove(editBottomBar, true);
                ContextMenu.Add(bottomBar = new AnalyzerBottomBar());

                EditorLoaded += (obj, ev) =>
                {

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
        private AnalyzerEditorLoader editorLoader;

        // Hiding
        protected new AnalyzerEditor Editor => editorLoader.Editor;
        protected new EditorBeatmap EditorBeatmap => Editor.ChildrenOfType<EditorBeatmap>().Single();
        protected new EditorClock EditorClock => Editor.ChildrenOfType<EditorClock>().Single();

        #endregion


        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {

            ListChanged += UpdateBars;
            // BindableWithCurrent<List<ISkill>>
            Console.WriteLine("hiaaaaaa");
            Children.ForEach(d => { Console.WriteLine(d.GetType()); });
            Console.WriteLine(Audio);
            Add(
                new Container
                {
                    Size = new Vector2(135, 564),
                    Y = 144,
                    Children = new Drawable[]
                    {
                        new Box{
                            RelativeSizeAxes = Axes.Both,
                            Colour = FrameworkColour.GreenDarker
                        },
                        debugContainer = new TextFlowContainer()
                        {
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                }
           );
        }

        protected void FinishedLoading()
        {
            canUpdateBars = true;
            UpdateBars(CurSkillList);
            EditorLoaded.Invoke(this,true);
        }


        private bool sideBarLoaded = false;
        protected override void LoadComplete()
        {
            base.LoadComplete();
            sideBarLoaded = true;
        }


        protected override void Update()
        {
            base.Update();
            if (!sideBarLoaded)
                return;
                if (editorLoader != default)
            {
                if ((Editor?.ReadyForUse ?? false) == true && (!canUpdateBars)) { FinishedLoading(); Console.WriteLine("line"); }
            }
            if (canUpdateBars)
            {
                Schedule(() => {
                    int closeindex = getClosestHitObjectIndex(EditorClock.CurrentTime);
                    if (previoushitindex == closeindex)
                        return;
                    dummyScore.BeatmapInfo.Contents.DiffHitObjects = new List<DifficultyHitObject>(CachedMapDiffHits);
                    dummyScore.BeatmapInfo.Contents.DiffHitObjects = dummyScore.BeatmapInfo.Contents.DiffHitObjects.Where(d => {
                        return d.Index <= closeindex;
                    }).ToList();
                    dummyScore.Combo = dummyScore.BeatmapInfo.GetMaxCombo();// calculated combo with current amount of hit objects
                    if (scaleByCombo)
                    {
                        dummyScore.BeatmapInfo.MaxCombo = CachedMapDiffHits.GetMaxCombo();
                    }
                    else
                    {
                        dummyScore.BeatmapInfo.MaxCombo = dummyScore.Combo;
                    }
                    Console.WriteLine($"" +
                        $"combo: {dummyScore.Combo} / {dummyScore.BeatmapInfo.MaxCombo} \n" +
                        $"editor time: {EditorClock.CurrentTime} \n" +
                        $"closest index: {closeindex} \n" +
                        $"cached diff: {CachedMapDiffHits.Count} \n" +
                        $"score diff: {dummyScore.BeatmapInfo.Contents.DiffHitObjects.Count} \n" +
                        $"map diff: {ATFocusedMap.Contents.DiffHitObjects.Count} \n" + "\n------------------");
                    UpdateBars(currentSkillList);
                });
            }

        }


        private List<ISkill> currentSkillList = new List<ISkill>();

        private void updateSideBar() {

        }

        protected void UpdateBars(List<ISkill> skillList) {
            if (skillList == default | editorLoader == null) return;
            if (!Editor?.ReadyForUse ?? false) return;
            if (debugContainer == null) return;

            // Debug text section
            debugContainer.Text = "";
            skillList.ForEach(skill =>
            {
                bool titlemade = false;
                System.Console.WriteLine($"Cur Skill: {skill.GetType()}");
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
                        debugContainer.AddText($"{property.Name}:\n   -> {propval}\n", t => {
                            t.Font = new FontUsage("VarelaRound", size: 17);
                            t.Colour = Colour4.White;
                            t.Shadow = true;
                        });
                    }
                }
            });
            
            // Bar section
            float largestPP = 0;
            
            currentSkillList = skillList;
            SortedList<string, float> skillNameList = new SortedList<string, float>();
            List<ColourInfo> skillColors = new List<ColourInfo>();

            skillList.ForEach(
                skill =>
                {
                    float skillpp = (float)skill.SkillCalc(dummyScore);
                    if (skillpp < 0) skillpp = 0;
                    skillNameList.Add(skill.Identifier, skillpp);
                    if (largestPP < skillpp) largestPP = skillpp;
                    Console.WriteLine(skill.Identifier + ": " + skillpp);
                }
            );

            skillNameList.ForEach(skillName => {
                skillColors.Add(ColourInfo.GradientVertical(Skill.GetSkillByID(skillName.Key).PrimaryColor, Skill.GetSkillByID(skillName.Key).SecondaryColor));
            });

            // ListChanged.Invoke(this, skillList.NewValue);
            //  Console.WriteLine("catWa");
            SkillGraph.SBarGraph.MaxValue = (largestPP < 500)? 500 : largestPP;
            SkillGraph.SetValues(skillNameList, skillColors);
        }


        protected void UpdateBars(object sender,List<ISkill> skillList)
        {
            UpdateBars(skillList);
        }



        private bool canUpdateBars = false;
        private int previoushitindex = 0;
        private int curhitindex = 0;
        private double cachedCurTime = 0;
        /// <summary>
        /// Returns the closest hitobject's time position to the editor clock's currenttime.
        /// </summary>
        /// <remarks>It's possible for  could be heavily improved.</remarks>
        /// <returns></returns>
        private int getClosestHitObjectIndex(double currentTime) {
            // [!] Big bug where the acutaly closest hit object isnt return due to the amount of time
            // it takes for the clock to seek to a point.
            var hitList = CachedMapDiffHits;
            if (cachedCurTime > currentTime) { // person seeked backwards, so just reset
                curhitindex = 0;
            }
            cachedCurTime = currentTime;
            // starts at curhitindex to avoid constantly looping through the whole map
            for (int i = curhitindex; i < hitList.Count; i++) { 
                var time = hitList[i].StartTime;
                if (time >= currentTime) {
                    previoushitindex = curhitindex;
                    curhitindex = i;
                    return i;
                }
            }
            return hitList.Count-1;
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
        [Test]
        public async void Setup()
        {
            AddStep("load editor then enable bar", LoadEditor);
            AddUntilStep("wait for beatmap updated", () => !base.Beatmap.IsDefault);

        }

        [Test]
        public void TestSeekToFirst()
        {
            AddAssert("SETTINGS:", () => false); // Added a false assert so that it doesn't automatically enable all settings
            AddToggleStep("Overall Combo Scaling", (d) => { scaleByCombo = d; });
        }

        public override void SetUpSteps()
        {
        }
    }
}
