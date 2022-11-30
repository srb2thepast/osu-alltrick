using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osuAT.Game.Types;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osuAT.Game.Skills.Resources;

namespace osuAT.Game.Skills
{
    public class AimSkill : ISkill
    {

        #region
        public string Name => "Aim";

        public string Identifier => "overallaim";

        public string Version => "0.001";

        public string Summary => "Absolutely Broken"; // "The ability to move your cursor \n to any circle\n.";

        public int SummarySize => 9;

        public Colour4 PrimaryColor => Colour4.FromHex("#F7E65D");

        public Colour4 SecondaryColor => Colour4.FromHex("#F0A1B7");

        public string Background => "SkillBG/Flowaim2";

        public string Cover => "SkillBG/Flowaim1";

        public string BadgeBG => "SkillBG/Flowaim1";

        public (Vector2, Vector2) BadgePosSize { get; }

        public float MiniHeight => 264;

        public int BoxNameSize => 150;

        public Vector2 BoxPosition => new Vector2(2690, -500);

        public SkillGoals Benchmarks => new SkillGoals(600, 1500, 3000, 6000, 9000, 10000);
        #endregion

        public class AimCalculator : SkillCalcuator
        {
            public AimCalculator(Score score) : base(score)
            {
            }

            public override RulesetInfo[] SupportedRulesets => new RulesetInfo[] { RulesetStore.Osu };

            public override void CalcNext(OsuDifficultyHitObject diffHitObj)
            {
                CurTotalPP = new Random().Next(120000);
            }
        }

        public Type SkillCalcType => typeof(AimCalculator);

        public SkillCalcuator GetSkillCalc(Score score) => new AimCalculator(score);
    }
}
