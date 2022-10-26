using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Framework.Allocation;

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

            this.Window.Title = "osu!alltrick";
            SaveStorage.Init();
            ScoreImporter.Init();
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
