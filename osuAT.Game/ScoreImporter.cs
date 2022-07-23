using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using osu.Framework;
using osuAT.Game.Types;
using osuAT.Game.Objects.LazerAssets;
using OsuApiHelper;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;

namespace osuAT.Game
{
    public class ScoreImporter
    {
        // looks like timers dont go to well with hot reload in o!f. alternative is needed  
        private Timer scoreSetTimer = new Timer(1000); 
        private StructuredOsuMemoryReader osuReader;
        private string osuLocation = "C:\\Users\\alexh\\AppData\\Local\\osu!"; // dont forget to set this as something saved in SaveStorage
        private OsuMemoryStatus lastScreen;


        public ScoreImporter()
        {
            Console.WriteLine("initalised scoreimpoter");
            osuReader = StructuredOsuMemoryReader.Instance.GetInstanceForWindowTitleHint("");

            scoreSetTimer.Elapsed += scoreSetTimer_Elapsed;
            scoreSetTimer.Enabled = true;
            scoreSetTimer.AutoReset = true;
            scoreSetTimer.Start();

        }

        private async void scoreSetTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            /*
            {
                ResultsScreen playerResults = new ResultsScreen();
                GeneralData gameData = new GeneralData();

                osuReader.TryRead(playerResults);
                osuReader.TryRead(gameData);
                Console.WriteLine(gameData.OsuStatus);
                Console.WriteLine(playerResults.Username);
                Console.WriteLine(playerResults.MaxCombo.ToString());
                Console.WriteLine(RulesetStore.GetByNum(playerResults.Mode).Name);
                if (lastScreen == OsuMemoryStatus.Playing && gameData.OsuStatus == OsuMemoryStatus.ResultsScreen)
                {
                    Console.WriteLine("results reached");
                    if (playerResults.Username != "osu!") 
                    {
                        var osuScore = OsuApi.GetUserRecent("srb2thepast")[0];
                        var splay = new OsuPlay();
                        if (splay.Mode != OsuMode.Standard) return;
                        var smap = new OsuBeatmap();
                        
                        string maplocation = osuLocation + "\\Songs\\" + mapPlayed.FolderName + "\\" + mapPlayed.OsuFileName;
                        string[] text = await File.ReadAllLinesAsync(maplocation);
                        string title = default;
                        string artist = default;
                        string creator = default;
                        string diffname = default;


                        foreach (string line in text) {
                            if (line.StartsWith("Title:")) title = line.Split("Title:")[1];
                            if (line.StartsWith("Artist:")) artist = line.Split("Artist:")[1];
                            if (line.StartsWith("Creator:")) creator = line.Split("Creator:")[1];
                            if (line.StartsWith("Version:")) diffname = line.Split("Version:")[1];
                            if (line == "[Difficulty]") break;
                        }

                        System.Console.WriteLine(title + " by " + artist);
                        System.Console.WriteLine(creator + " (maybe) made the diff " + diffname);
                        Console.WriteLine((OsuMod)playerResults.Mods.Value);
                        
                        AccStat accstats = new AccStat(playerResults.Hit300, playerResults.Hit100, playerResults.Hit50, playerResults.HitMiss);
                        Score score = new Score
                        {
                            ScoreRuleset = RulesetStore.GetByNum(playerResults.Mode),
                            IsLazer = false,
                            // osu
                            BeatmapInfo = new Beatmap
                            {
                                MapID = mapPlayed.id,
                                MapsetID = mapPlayed.SetId,
                                SongArtist = artist,
                                SongName = title,
                                DifficultyName = diffname,
                                MapsetCreator = creator,
                                FolderName = mapPlayed.FolderName,
                                MaxCombo = 30000, // Although i'd like to include MaxCombo, there appears to be no way for me to get it unless i make an entire method to get all circles, slider heads, slider tails, and slider repeat count. Leaving all scores as 30,000 until this system is created.
                                StarRating = 8
                            },
                            Accuracy = accstats.CalcAcc(),
                            AccuracyStats = accstats,
                            Combo = playerResults.Combo,
                            TotalScore = playerResults.Score,
                            ModsString = new List<string> { "Nightcore", "Hidden" },
                            Grade = "SS"
                            
                            
                        };
                        score.Register();
                        SaveStorage.AddScore(score);
                    }
                }


                lastScreen = (OsuMemoryStatus)gameData.RawStatus;
            }*/
        }

        

        public void CheckScoreSet_Tick()
        {
            //Console.WriteLine(osuReader.GetPlayingMods().ToString());
        }

        public void CheckScore_Timer()
        {


        }

        public void ImportScore() {
        }
    }
}
