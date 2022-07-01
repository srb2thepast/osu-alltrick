using osu.Framework.Graphics;
using osuAT.Game.Objects.Displays;

namespace osuAT.Game.Tests.Visual.Display
{
    public class TestSceneScoreDisplay : osuATTestScene
    {
        public TestSceneScoreDisplay()
        {
            Add(new ScoreDisplay
            {
                Anchor = Anchor.Centre,
                

            });
        }
    }
}
