using System;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Internal;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Framework.Testing.Drawables.Steps;
using osuTK.Graphics;

namespace osuAT.Game.Tests
{
    public partial class osuATTestBrowser : osuATGameBase
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

        public Storage GetStorage() => Host.GetStorage("osu!AT-Tests");
        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }


    }
}
