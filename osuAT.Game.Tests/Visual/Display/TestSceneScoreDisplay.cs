using osu.Framework.Graphics;
using osuAT.Game.Objects.Displays;

namespace osuAT.Game.Tests.Visual.Display
{
    public class TestSceneScoreDisplay : osuATTestScene
    {
        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
        // You can make changes to classes associated with the tests and they will recompile and update immediately.
        public TestSceneScoreDisplay()
        {
            Add(new ScoreDisplay
            {
                Anchor = Anchor.Centre,
            });
        }
    }
}
