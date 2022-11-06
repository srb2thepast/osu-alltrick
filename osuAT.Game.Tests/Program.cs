using System;
using System.IO;
using osu.Framework;
using osu.Framework.Platform;

namespace osuAT.Game.Tests
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost("osu!AT_Tests"))
            using (var game = new osuATTestBrowser())
                host.Run(game);
        }
    }
}
