using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Framework.Utils;
using osuAT.Game.Objects;
using osuAT.Game.Objects.LazerAssets.Mod;
using osuAT.Game.Types;

namespace osuAT.Game.Tests.Visual.Display
{
    public partial class TestSceneBoxWorthDisplay : TestScene
    {
        private BoxWorthDisplay box;

        [SetUp]
        public void Setup()
        {
            AddStep("Create Top 10 Display", () =>
            {
                Child = box = new BoxWorthDisplay(RNG.Next(1000), RNG.Next(11)) { Scale = new osuTK.Vector2(2) };
                box.Appear();
            });

            AddStep("Create Random Display", () =>
            {
                Child = box = new BoxWorthDisplay(RNG.Next(1000), RNG.Next(101)) { Scale = new osuTK.Vector2(2) };
                box.Appear();
            });

            AddStep("Create Display with placement > 100", () =>
            {
                Child = box = new BoxWorthDisplay(RNG.Next(1000), -1) { Scale = new osuTK.Vector2(2) };
                box.Appear();
            });
        }
    }
}
