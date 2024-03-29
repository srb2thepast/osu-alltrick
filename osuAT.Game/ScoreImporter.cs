using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Platform;
using OsuApiHelper;
using osuAT.Game.API;
using osuAT.Game.Objects.LazerAssets;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;

namespace osuAT.Game
{
    public class ScoreImporter
    {
        // looks like timers dont go to well with hot reload in o!f. alternative is needed
        public static int TickDelay = 1500;

        private static Timer scoreSetTimer = new Timer(TickDelay);
        private static StructuredOsuMemoryReader osuReader;
        private static OsuMemoryStatus lastScreen = OsuMemoryStatus.Playing;
        private static GeneralData gameData = new GeneralData();
        private static int instances = 0;
        public static bool Enabled = true;

        public static async void Init()
        {
            var gamestore = osuATGameBase.Dependencies.Get<osuATGameBase>();

            Console.WriteLine("initalised scoreimpoter");
            instances += 1;
            Console.WriteLine($"{instances} Instances.");
            if (instances > 1)
                return;
            osuReader = StructuredOsuMemoryReader.Instance.GetInstanceForWindowTitleHint("");
            while (true)
            {
                await Task.Delay(TickDelay);
                if (Enabled && !gamestore.Window.Focused)
                {
                    scoreSetTimer_Elapsed();
                }
            }
        }

        private static async void scoreSetTimer_Elapsed()
        {
            if (!SaveStorage.OsuPathIsValid() || OsuApiKey.Key == default || !(ApiScoreProcessor.ApiKeyValid)) return;
            if (ApiScoreProcessor.ApiReqs >= 30)
            {
                Console.WriteLine($"Automatic Rate limiting begun ({ApiScoreProcessor.ApiReqs} API Requests were sent!!!!)");
                return;
            }
            try
            {
                GeneralData gameData = new GeneralData();
                osuReader.TryRead(gameData);
#if DEBUG
                Console.WriteLine("last: " + lastScreen + " | current: " + gameData.OsuStatus);
#endif
                ApiScoreProcessor.ApiReqs = Math.Max(0, ApiScoreProcessor.ApiReqs - TickDelay / 150);

                // if the play went from playing to the results screen, continue, otherwise, return.
                if (!(lastScreen == OsuMemoryStatus.Playing && gameData.OsuStatus == OsuMemoryStatus.ResultsScreen)
                    && !(lastScreen == OsuMemoryStatus.Playing && gameData.OsuStatus == OsuMemoryStatus.MultiplayerResultsscreen)
                    && !(lastScreen == OsuMemoryStatus.Playing && gameData.OsuStatus == OsuMemoryStatus.MultiplayerRoom))
                {
                    lastScreen = gameData.OsuStatus;
                    return;
                }
                lastScreen = gameData.OsuStatus;
                Console.WriteLine("Importation begun.");
                await Task.Delay(2000); // wait a bit incase osu!servers are behind

                ApiScoreProcessor.ApiReqs += 1;
                var recent = OsuApi.GetUserRecent(SaveStorage.SaveData.PlayerID.ToString());
                if (recent == null)
                    return;

                var osuScore = recent[0];
                async Task<OsuApiBeatmap> mapRet() => await ApiScoreProcessor.OsuGetBeatmap(osuScore.MapID, osuScore.Mods, osuScore.Mode);

                await ApiScoreProcessor.SaveToStorageIfValid(osuScore, mapRet);
            }
            catch
            {
                lastScreen = OsuMemoryStatus.Unknown;
            }
            finally { }
        }

        public void ImportScore()
        {
        }
    }
}
