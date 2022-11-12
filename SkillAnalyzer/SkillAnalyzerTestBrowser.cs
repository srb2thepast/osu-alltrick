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
using System.Threading.Tasks;

namespace SkillAnalyzer
{

    public class SkillAnalyzerTestBrowser : TestBrowser {
        public SkillAnalyzerTestBrowser(string assemblyName) : base(assemblyName) {

        }

        

        protected override void LoadComplete()
        {
            base.LoadComplete();
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
