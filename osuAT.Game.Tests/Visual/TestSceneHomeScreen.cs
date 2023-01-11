using NUnit;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osuAT.Game.Screens;

namespace osuAT.Game.Tests.Visual
{
    public partial class TestSceneHomeScreen : osuATTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.


        public TestSceneHomeScreen()
        {
            Add(new ScreenStack(new HomeScreen()) { RelativeSizeAxes = Axes.Both });
        }
    }
}
