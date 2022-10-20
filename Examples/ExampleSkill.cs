using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types.BeatmapParsers;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;

/// Refer to FlowAimSkill for a legitmate calc example.
namespace osuAT.Game.Skills
{
    public class ExampleSkill : ISkill
    {

        public string Name => "Example";

        public string Identifier => "example";

        public string Version => "0.002";

        public string Summary => "This is an example Skill! Normal users shouldn't see this.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#99FF69");

        public Colour4 SecondaryColor => Colour4.FromHex("#00FFF0");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize => (new Vector2(-20, 60), new Vector2(700, 440));

        public float MiniHeight => 224;

        public int BoxNameSize => 83;

        public Vector2 BoxPosition => new Vector2(00, 00);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500,3000, 6000, 9000, 10000);

        public RulesetInfo[] SupportedRulesets => new RulesetInfo[]{ RulesetStore.Osu };



        /// "-" = Something wrong
        /// "~" = Something wrong but headed in the right direction
        /// "?" = Unsure if it's wrong
        /// <summary>
        /// Returns a pp value based off of the amount of circles, slider heads, and slider repeats in a map.
        /// </summary>
        /// <remarks> 1 Circle = 1pp, 1 slider = 2pp, 1 slider repeating 1 time = 3pp
        /// Current faults:
        /// - Example
        /// ~ Example
        /// ? Example
        /// </remarks>
        public double SkillCalc(Score score)
        {
            if (!SupportedRulesets.Contains(score.ScoreRuleset)) return -1;
            if (score.BeatmapInfo.FolderName == default) return -2;
            if (score.BeatmapInfo.HitObjects == default) return -3;

            int totalCount = 0;

            for (int i = 0; i < score.BeatmapInfo.HitObjects.Count; i++) {
                HitObject HitObj = score.BeatmapInfo.HitObjects[i];

                
                if (HitObj is OsuParser.Circle)
                {
                    totalCount++
                }

                if (HitObj is OsuParser.Slider)
                    // SpanCount is the amount of times a slider repeats added by 1.
                    totalCount += HitObj.SpanCount
                }
            }
            return totalCount;
        }
    }
}
