using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using HidSharp.Reports;
using Newtonsoft.Json;
using osu.Framework.IO.Network;
using SQLitePCL;
using Squirrel;
using Squirrel.Sources;

namespace osuAT.Game
{
    public class GithubRelease
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("tag_name")]
        public string ReleaseTag { get; set; }
    }

    public static class Updater
    {
        public static string CurrentVersion => "v1.00.0";

        public static string RepoUrl = @"https://github.com/srb2thepast/osu-alltrick/";

        [Obsolete("WIP, Does not work atm.",true)]
        [SupportedOSPlatform("windows")]
        public static async void CheckForUpdatesWindows()
        {
            var updateManager = new GithubUpdateManager(repoUrl: RepoUrl);
            Console.WriteLine($"Current version : {updateManager.CurrentlyInstalledVersion}");

            var newVersion = await updateManager.UpdateApp();
            if (newVersion != null)
            {
                Console.WriteLine($"Installing new update {newVersion.Version}");
                UpdateManager.RestartApp();
            }
        }

        public static async void CheckForUpdates()
        {
            var releases = new JsonWebRequest<List<GithubRelease>>(@"https://api.github.com/repos/srb2thepast/osu-alltrick/releases");
            await releases.PerformAsync();
            var latest = releases.ResponseObject[0];
            Console.WriteLine($"Latest version : {latest.ReleaseTag}");
            Console.WriteLine($"Current version : {CurrentVersion}");

            if (latest.ReleaseTag != CurrentVersion) {
                Process.Start(new ProcessStartInfo(@"https://github.com/srb2thepast/osu-alltrick/releases") { UseShellExecute = true });
            }
        }
    }
}
