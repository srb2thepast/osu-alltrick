using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Framework.Allocation;
using NUnit.Framework.Internal;
using NUnit.Framework;
using osu.Framework.Testing.Drawables.Steps;
using osuTK.Graphics;
using System.Diagnostics;
using System.Reflection;
using System;

namespace osuAT.Game.Tests
{
    public class osuATTestBrowser : osuATGameBase
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddRange(new Drawable[]
            {
                new TestBrowser("osuAT"),
                new CursorContainer()
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Window.Title = "osu!alltrick";
        }

        public new Storage GetStorage() => Host.GetStorage("osu!AT-Tests");
        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }


    }
}
