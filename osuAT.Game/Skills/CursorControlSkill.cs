using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osuAT.Game.Types;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using static osuAT.Game.Skills.AimSkill;

namespace osuAT.Game.Skills
{
    public class CursorControlSkill : ISkill
    {
        #region
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
        #endregion


        public class CursorControlCalculator : SkillCalcuator
        {
            public CursorControlCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            public override void CalcNext(DifficultyHitObject diffHitObj)
            {
                CurTotalPP = new Random().Next(1200);
            }
        }

        public SkillCalcuator GetSkillCalc(Score score) => new CursorControlCalculator(score);
    }
}
