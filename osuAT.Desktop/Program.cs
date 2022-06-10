using osu.Framework.Platform;
using osu.Framework;
using osuAT.Game;

namespace osuAT.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"osuAT"))
            using (osu.Framework.Game game = new osuATGame())
                host.Run(game);
        }
    }
}
