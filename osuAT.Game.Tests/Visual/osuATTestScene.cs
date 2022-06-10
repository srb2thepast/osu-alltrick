using osu.Framework.Testing;

namespace osuAT.Game.Tests.Visual
{
    public class osuATTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new osuATTestSceneTestRunner();

        private class osuATTestSceneTestRunner : osuATGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
