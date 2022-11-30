using System;
using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Framework.Platform;
using osuAT.Game.Screens;
using osu.Game.Database;
using Realms;

namespace osuAT.Game
{
    public partial class osuATGame : osuATGameBase
    {
        
        public ScreenStack ScreenStack;
        public HomeScreen MainScreen = new HomeScreen();
        private Storage storage { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Child = ScreenStack = new ScreenStack(baseScreen: MainScreen) { RelativeSizeAxes = Axes.Both };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

        }
        
    }
}
