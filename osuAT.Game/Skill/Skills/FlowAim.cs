using System;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;


namespace osuAT.Game.Skill.Skills
{
    public class FlowAim : ISkill
    {
        public string Name => "Flow aim";

        public Colour4 PrimaryColor => Colour4.FromHex("#99FF69");

        public Colour4 SecondaryColor => Colour4.FromHex("#00FFF0");

        public Texture Background => throw new NotImplementedException();

        public double MiniHeight => 100;

        public int BoxNameSize => 83;

        public Vector2 BoxPosition => new Vector2(700,400);

        public SkillGoals Benchmarks => new SkillGoals(200, 500, 1000, 3000, 5000, 10000);

        public RulesetInfo[] SupportedRulesets => new RulesetInfo[] {RulesetStore.Osu};

        public double SkillCalc(Score score)
        {
            throw new NotImplementedException();
        }
    }
}
