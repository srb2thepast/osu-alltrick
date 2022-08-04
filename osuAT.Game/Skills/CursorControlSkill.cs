using System;
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
    public class CursorControlSkill : ISkill
    {

        public string Name => "Cursor Control";

        public string Identifier => "cursorcontrol";

        public string Version => "0.001";

        public string Summary => "Ability to control the movement \n of your aim.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#F7E65D");

        public Colour4 SecondaryColor => Colour4.FromHex("#F0A1B7");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize { get; }

        public float MiniHeight => 224;

        public int BoxNameSize => 63;

        public Vector2 BoxPosition => new Vector2(2000, -1300);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500,3000, 6000, 9000, 10000);

        public RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

        public double SkillCalc(Score score)
        {
            if (!SupportedRulesets.Contains(score.ScoreRuleset)) return -1;

            return new Random().Next(1200);
        }
    }
}
