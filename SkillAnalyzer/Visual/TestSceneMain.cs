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
using osuAT.Game;
using osuAT.Game.Types;
using OsuRulesetInfo = osu.Game.Rulesets.RulesetInfo;
using ATBeatmap = osuAT.Game.Types.Beatmap;
using ATRulesetStore = osuAT.Game.Types.RulesetStore;
using osu.Framework.Testing;
using osu.Game.Screens.Edit.Components.Timelines.Summary;

namespace SkillAnalyzer.Visual
{
    public class TestSceneMain : EditorTestScene
    {
        protected string MapLocation = @"Songs\1045600 MOMOIRO CLOVER Z - SANTA SAN\MOMOIRO CLOVER Z - SANTA SAN (A r M i N) [1-2-SANTA].osu";
        protected IBeatmap FocusedBeatmap;
        protected WorkingBeatmap WorkFocusedMap;
        // protected override bool IsolateSavingFromDatabase => false;
        protected override Ruleset CreateEditorRuleset() => new OsuRuleset();

        protected class AnalyzerEditor : TestEditor {
            SummaryTimeline timeline;
        }
        private TestEditorLoader editorLoader;
        protected new TestEditor Editor => editorLoader.Editor;
        protected new EditorBeatmap EditorBeatmap => Editor.ChildrenOfType<EditorBeatmap>().Single();
        protected new EditorClock EditorClock => Editor.ChildrenOfType<EditorClock>().Single();

        [BackgroundDependencyLoader]
        private void load(AudioManager audio) {
            Console.WriteLine(Audio);
        }

        protected override void LoadEditor()
        {
            base.Beatmap.Value = CreateWorkingBeatmap(base.Ruleset.Value);


            LoadScreen(editorLoader = new TestEditorLoader());
        }

        protected override IBeatmap CreateBeatmap(OsuRulesetInfo ruleset)
        {
            var osuatmap = new ATBeatmap()
            {
                FolderLocation = MapLocation
            };
            WorkFocusedMap = osuatmap.LoadMapContents(ATRulesetStore.GetByIRulesetInfo(ruleset),audio: Audio);
            FocusedBeatmap = WorkFocusedMap.Beatmap;
            WorkFocusedMap.LoadTrack();
            WorkFocusedMap.Track.Start();
            FocusedBeatmap.BeatmapInfo.BeatDivisor = 4;
            Console.WriteLine(WorkFocusedMap.Track);

            return FocusedBeatmap;
        }

        [Test]
        public void TestSeekToFirst()
        {
            AddAssert("track not running", () => !EditorClock.IsRunning);
        }

        [Test]
        public void PlayTrack() {
        }

        public override void SetUpSteps()
        {
            AddStep("load editor", LoadEditor);
            AddUntilStep("wait for editor to load", () => Editor?.ReadyForUse ?? false);
            AddUntilStep("wait for beatmap updated", () => !base.Beatmap.IsDefault);
        }
    }
}
