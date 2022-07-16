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
    public class AimSkill : ISkill
    {

        public string Name => "Aim";

        public string Identifier => "overallaim";

        public string Version => "0.001";

        public string Summary => "The ability to move your cursor \n to any circle\n.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#F7E65D");

        public Colour4 SecondaryColor => Colour4.FromHex("#F0A1B7");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public float MiniHeight => 264;

        public int BoxNameSize => 150;

        public Vector2 BoxPosition => new Vector2(2800, -1300);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500, 3000, 6000, 9000, 10000);

        public List<RulesetInfo> SupportedRulesets => new List<RulesetInfo> { RulesetStore.Osu };

        public double SkillCalc(Score score)
        {
            if (!SupportedRulesets.Contains(score.ScoreRuleset)) return -1;

            return new Random().Next(1200);
        }
    }
}
