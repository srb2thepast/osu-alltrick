using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osuAT.Game.Skills.Resources;
using osuAT.Game.Types;
using osuTK;

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

        protected double StrainPosition = 1;

        protected double PercentMin = 0.50;

        protected double Peak = 0;

        protected double UncappedVal = 0;

        protected StrainSkillCalc(Score score) : base(score) { }

        // note: currently breaks just plunge the worth of value. there should be a scaling that
        // makes the power (obj.StartTime/1000 in the pow statment) fall back to 0 as the time
        // difference becomes too large.
        protected double GetTimeAppliedStrain(double value, DifficultyHitObject obj)
        {
            return value * Math.Pow(1 - DecayFactor, obj.StartTime / 1000);
        }

        protected double GetPositionAppliedStrain(double value, bool capAtMin = true)
        {
            var newval = value * Math.Pow(1 - DecayFactor, StrainPosition);
            UncappedVal = newval;
            if (newval > Peak) Peak = newval;
            return capAtMin ? Math.Max(Peak, newval) : newval;
        }
    }
}
