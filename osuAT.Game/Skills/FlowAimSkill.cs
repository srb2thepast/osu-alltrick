using System;
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

        public string Name => "Flow aim";

        public string Identifier => "flowaim";

        public string Version => "0.002";

        public string Summary => "The ability to move your cursor \n as smoothly and linearly as\n possible.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#99FF69");

        public Colour4 SecondaryColor => Colour4.FromHex("#00FFF0");

        public string Background => "SkillBG/Flowaim1";

        public string Cover => "SkillBG/Flowaim2";

        public double MiniHeight => 100;

        public int BoxNameSize => 83;

        public Vector2 BoxPosition => new Vector2(00, 400);

        public SkillGoals Benchmarks => new SkillGoals(200, 500, 1000, 3000, 5000, 10000);

        public List<RulesetInfo> SupportedRulesets => new List<RulesetInfo> { RulesetStore.Osu };

        public double SkillCalc(Score score)
        {
            if (!SupportedRulesets.Contains(score.ScoreRuleset)) return -1;

            return new Random().Next(1200);
        }
    }
}
