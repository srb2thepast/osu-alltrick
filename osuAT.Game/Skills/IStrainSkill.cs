using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Sprites;
using osuAT.Game.Types;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osuTK;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Colour;

namespace osuAT.Game.Skills
{ 
    public abstract class StrainSkillCalculator : SkillCalcuator
    {

        protected double DecayWeight = 0.5;

        protected StrainSkillCalculator(Score score) : base(score)
        {
        }
    }
}
