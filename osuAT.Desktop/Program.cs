using System;
using System.IO;
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
                try
                {
                    host.Run(game);
                }
                catch (Exception err)
                {
                    File.WriteAllText("errlog.txt", err.StackTrace + "\n ----------- ERROR MESSAGE: \n ----------- " + err.Message);
                    Console.WriteLine("Logged");
                }
        }
    }
}
