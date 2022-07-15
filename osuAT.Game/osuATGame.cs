using System;
using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Framework.Platform;

namespace osuAT.Game
{
    public class osuATGame : osuATGameBase
    {
        public ScreenStack ScreenStack;
        public HomeScreen MainScreen = new HomeScreen();
        //private Storage storage { get; set; };

        [BackgroundDependencyLoader]
        private void load()
        {
            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Child = ScreenStack = new ScreenStack { RelativeSizeAxes = Axes.Both };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            ScreenStack.Push(MainScreen);
        }
        
    }
}
