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
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;

namespace osuAT.Game.Skills
{
    /// <summary>
    /// An abstract class for skills whose calculations revolve around Strain.
    /// </summary>
    public abstract class StrainSkillCalc : SkillCalcuator
    {
        /// <summary>
        /// Changes decrease per second. The lower this number is, the slower the overall worth will decrease
        /// </summary>
        protected virtual double DecayFactor => 0.01;

        protected double TotalStrainWorth = 0;

        protected double TimeElapsed = 0;

        protected StrainSkillCalc(Score score) : base(score)
        {

        }

        // note: currently breaks just plunge the worth of value. there should be a scaling that
        // makes the power (obj.StartTime/1000 in the pow statment) fall back to 0 as the time
        // difference becomes too large.
        protected double GetAppliedStrain(DifficultyHitObject obj,double value)
        {
            return value * Math.Pow(1-DecayFactor, obj.StartTime/1000);
        }


    }
}
