using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;


namespace osuAT.Game.Skills
{
    public class FlowAimSkill : ISkill
    {

        public string Name => "Flow Aim";

        public string Identifier => "flowaim";

        public string Version => "0.002";

        public string Summary => "Ability to move your cursor \n in a fluid motion.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#99FF69");

        public Colour4 SecondaryColor => Colour4.FromHex("#00FFF0");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize => (new Vector2(-20, 60), new Vector2(700, 440));

        public float MiniHeight => 224;

        public int BoxNameSize => 83;

        public Vector2 BoxPosition => new Vector2(00, 400);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500,3000, 6000, 9000, 10000);

        public RulesetInfo[] SupportedRulesets => new RulesetInfo[]{ RulesetStore.Osu };

        public double SkillCalc(Score score)
        {
            if (!SupportedRulesets.Contains(score.ScoreRuleset)) return -1;
            if (score.BeatmapInfo.FolderName == default) return -1;

            Console.WriteLine(score.BeatmapInfo.FolderName);

            var text = File.ReadAllLinesAsync(SaveStorage.SaveData.OsuPath + @"\" + score.BeatmapInfo.FolderName).Result;
            string filemapid;
                
            foreach (string line in text)
            {
                if (line.StartsWith("BeatmapID:")) filemapid = line.Split("BeatmapID:")[1];
                if (line == "[Difficulty]") break;
            }
            
            return new Random().Next(15000);
        }
    }
}
