using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using osu.Framework;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuAT.Game;
using Squirrel;

namespace osuAT.Desktop
{
    [SupportedOSPlatform("windows")]
    public static class Program
    {
        public static void Main()
        {

            try
            {
                Updater.CheckForUpdates();
            }
            catch
            {
                Logger.Log("Failed to update.");
            }

            using (GameHost host = Host.GetSuitableDesktopHost(@"osuAT", new HostOptions { BindIPC = true }))
            using (osu.Framework.Game game = new osuATGame())
                try
                {
                    if (!host.IsPrimaryInstance)
                    {
                        return;
                    }
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
