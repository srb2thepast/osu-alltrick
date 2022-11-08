using System;
using System.Reflection;
using System.IO;
using osu.Framework.Platform;
using osu.Framework;
using osuAT.Game;
using Squirrel;
using osu.Framework.Logging;

namespace osuAT.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            try
            {
                Updater.CheckForUpdates();
            }
            catch {
                Logger.Log("Failed to update.");
            }

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
