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

namespace SkillAnalyzer.Visual
{

    // [!] TODO: Get beatmap audio working
    public class TestSceneMain : EditorTestScene
    {
        protected string MapLocation = @"Songs\257607 xi - FREEDOM DiVE\xi - FREEDOM DiVE (elchxyrlia) [Arles].osu"; // @"Songs\1045600 MOMOIRO CLOVER Z - SANTA SAN\MOMOIRO CLOVER Z - SANTA SAN (A r M i N) [1-2-SANTA].osu";
        protected IBeatmap FocusedBeatmap;
        protected WorkingBeatmap WorkFocusedMap;
        protected ATBeatmap ATFocusedMap;

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
                ContextMenu.Remove(editBottomBar, true);
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
            public new AnalyzerEditor Editor;
            protected override TestEditor CreateTestEditor(EditorLoader loader)
            {
                Editor = new AnalyzerEditor(loader);
                return Editor;
            }
        }

        private AnalyzerEditorLoader editorLoader;
        protected new TestEditor Editor => editorLoader.Editor;
        protected new EditorBeatmap EditorBeatmap => Editor.ChildrenOfType<EditorBeatmap>().Single();
        protected new EditorClock EditorClock => Editor.ChildrenOfType<EditorClock>().Single();

        #endregion

        private Score dummyScore;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            
            /////
            SkillAnalyzerTestBrowser.ListChanged += UpdateBars;
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
        private List<ISkill> currentSkillList = new List<ISkill>();

        protected void UpdateBars(List<ISkill> skillList) {
            // Console.WriteLine("invoked");
            if (skillList == default | editorLoader == null) return;
            if (!Editor?.ReadyForUse ?? false) return;
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
                    skillColors.Add(ColourInfo.GradientVertical(skill.PrimaryColor, skill.SecondaryColor));
                    if (largestPP < skillpp) largestPP = skillpp;
                    Console.WriteLine(skill.Identifier + ": " + skillpp);
                }
            );

            // ListChanged.Invoke(this, skillList.NewValue);
            //  Console.WriteLine("catWa");
            SkillGraph.SBarGraph.MaxValue = (largestPP < 500)? 500 : largestPP;
            SkillGraph.SetValues(skillNameList, skillColors);
        }


        protected void UpdateBars(object sender,List<ISkill> skillList)
        {
            UpdateBars(skillList);
        }


        protected List<DifficultyHitObject> CachedMapDiffHits;
        protected List<HitObject> CachedMapHits;
        private bool canUpdateBars = false;
        protected override void Update()
        {
            base.Update();
            if (canUpdateBars) {
                Schedule(() => {
                    int closeindex = getClosestHitObjectIndex(EditorClock.CurrentTime);
                    if (cachedlasthitindex != closeindex)
                    {
                        dummyScore.Combo = closeindex;
                        dummyScore.BeatmapInfo.Contents.DiffHitObjects = new List<DifficultyHitObject>(CachedMapDiffHits);
                        dummyScore.BeatmapInfo.Contents.DiffHitObjects = dummyScore.BeatmapInfo.Contents.DiffHitObjects.Where(d => {
                            
                            return d.Index <= closeindex;
                        }).ToList();
                        dummyScore.Combo = GetMaxCombo(dummyScore.BeatmapInfo); // calculated combo with current amount of hit objects
                        dummyScore.BeatmapInfo.MaxCombo = dummyScore.Combo;
                        Console.WriteLine($"" +
                            $"combo: {dummyScore.Combo} / {dummyScore.BeatmapInfo.MaxCombo} \n" +
                            $"editor time: {EditorClock.CurrentTime} \n" +
                            $"closest index: {closeindex} \n" +
                            $"cached diff: {CachedMapDiffHits.Count} \n" +
                            $"score diff: {dummyScore.BeatmapInfo.Contents.DiffHitObjects.Count} \n" +
                            $"map diff: {ATFocusedMap.Contents.DiffHitObjects.Count} \n" + "\n------------------");
                        UpdateBars(currentSkillList);
                    }
                });
            }

        }


        private int cachedlasthitindex = 0;
        private int lasthitindex = 0;
        private double cachedCurTime = 0;
        /// <summary>
        /// Returns the closest hitobject's time position to the editor clock's currenttime.
        /// </summary>
        /// <returns></returns>
        private int getClosestHitObjectIndex(double currentTime) {
            var hitList = CachedMapDiffHits;
            if (cachedCurTime > currentTime) { // person seeked backwards, so just reset
                lasthitindex = 0;
            }
            cachedCurTime = currentTime;
            for (int i = lasthitindex; i < hitList.Count; i++) {
                var time = hitList[i].StartTime;
                if (time >= currentTime) {
                    cachedlasthitindex = lasthitindex;
                    lasthitindex = i;
                    return i;
                }
            }
            return hitList.Count;
        }

        protected void FinishedLoading() {
            canUpdateBars = true;
            UpdateBars(currentSkillList);
        }

        protected ATBeatmap ConvertWorkmapToATMap(WorkingBeatmap map) {
            // [!] Add DiffHitObjects here
            ATBeatmap newmap = new ATBeatmap()
            {
                MapID = map.BeatmapInfo.OnlineID,
                MapsetID = map.BeatmapInfo.BeatmapSet.OnlineID,
                SongArtist = map.BeatmapInfo.Metadata.ArtistUnicode,
                SongName = map.BeatmapInfo.Metadata.TitleUnicode,
                MapsetCreator = map.BeatmapInfo.BeatmapSet.Metadata.Author.Username,
                DifficultyName = map.BeatmapInfo.Metadata.TitleUnicode,
                StarRating = 1,
                FolderLocation = MapLocation,
            };
            newmap.LoadMapContents(RulesetStore.Osu);
            newmap.MaxCombo = GetMaxCombo(newmap);

            return newmap;
        }

        public int GetMaxCombo(ATBeatmap newmap)
        {
            int combo2 = 0;
            foreach (DifficultyHitObject hitObject in newmap.Contents.DiffHitObjects)
            {
                addCombo(hitObject.BaseObject, ref combo2);
            }

            return combo2;
        }

        private void addCombo(HitObject hitObject, ref int combo)
        {
            if (hitObject.CreateJudgement().MaxResult.AffectsCombo())
            {
                combo++;
            }

            foreach (HitObject nestedHitObject in hitObject.NestedHitObjects)
            {
                addCombo(nestedHitObject, ref combo);
            }
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
            ATFocusedMap = ConvertWorkmapToATMap(WorkFocusedMap);
            CachedMapDiffHits = new List<DifficultyHitObject>(ATFocusedMap.Contents.DiffHitObjects);
            CachedMapHits = ATFocusedMap.Contents.HitObjects;
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
        public void TestSeekToFirst()
        {
            AddAssert("track not running", () => !EditorClock.IsRunning);
        }

        public override void SetUpSteps()
        {
            AddStep("load editor", LoadEditor);
            AddUntilStep("wait for editor to load", () => Editor?.ReadyForUse ?? false);
            AddStep("do finished loading function", FinishedLoading);
            AddUntilStep("wait for beatmap updated", () => !base.Beatmap.IsDefault);
        }
    }
}
