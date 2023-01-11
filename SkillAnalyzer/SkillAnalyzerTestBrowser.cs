using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Handlers.Tablet;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Game;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Catch.UI;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Tests;
using osu.Game.Tests.Visual;
using osuAT.Game;
using osuAT.Game.Skills;
using osuAT.Game.Tests;
using osuTK;
using osuTK.Graphics;
using SkillAnalyzer.Visual;
using Skill = osuAT.Game.Skills.Skill;

namespace SkillAnalyzer
{

    public partial class SkillAnalyzerTestBrowser : TestBrowser
    {
        public SkillAnalyzerTestBrowser(string assemblyName) : base(assemblyName)
        {

        }



        protected override void LoadComplete()
        {
            base.LoadComplete();
        }
    }

    public partial class SkillAnalyzerTestBrowserLoader : OsuGameBase
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
            Window.Title = "osu!alltrick's SkillAnalyzer";
            SaveStorage.Init(new NativeStorage("SkillAnalyzer"));
            foreach (var handler in Host.AvailableInputHandlers)
            {
                if (handler is ITabletHandler tabhandler)
                {
                    Schedule(() =>
                    {
                        tabhandler.AreaSize.Value = new Vector2(75.6f, 48.17f);
                        tabhandler.AreaOffset.Value = new Vector2(73.42f, 50.18f);
                    });
                }
            }
        }

        public override void SetHost(GameHost host)
        {
            Logger.Level = LogLevel.Important;
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
