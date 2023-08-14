using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osuAT.Game.Objects;
using osuAT.Game.Objects.LazerAssets.Mod;
using osuAT.Game.Types;

namespace osuAT.Game.Tests.Visual.Display
{
    public partial class TestScenePlayCheckButton : TestScene
    {
        [SetUp]
        public void Setup()
        {
            AddStep("Create PlayCheck Button", () =>
            {
                Child = new PlayCheckButton
                {
                };
            });
        }
    }
}
