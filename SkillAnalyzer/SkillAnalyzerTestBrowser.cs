using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Framework.Allocation;
using osu.Game;
using osu.Game.Tests;
using osu.Game.Tests.Visual;
using osuAT.Game;
using osuAT.Game.Tests;

namespace SkillAnalyzer
{
    public class SkillAnalyzerTestBrowser : OsuGameBase
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddRange(new Drawable[]
            {
                new TestBrowser("SkillAnalyzer"),
                new CursorContainer()
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            
            this.Window.Title = "osu!alltrick's SkillAnalyzer";
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
