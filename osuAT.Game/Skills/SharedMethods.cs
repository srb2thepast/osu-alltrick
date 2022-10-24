using System;
using System.Collections.Generic;
using System.Text;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;

namespace osuAT.Game.Skills
{
    /// <summary>
    /// Methods that multiple calcs would use (ex. Combo Multiplier) go here.
    /// </summary>
    public static class SharedMethods
    {
        // from https://osu.ppy.sh/home/news/2021-01-14-performance-points-updates
        /// <summary>
        /// Returns a double from a degrading exponentional curve.
        /// </summary>
        /// <param name="Misses">The amount of misses on the map.</param>
        /// <param name="MaxCombo">The maximum amount of combo possible from the map.</param>
        /// <returns></returns>
        public static double MissPenalty(int Misses,int MaxCombo) {
            return .97 * Math.Pow((1 - Math.Pow(Misses / MaxCombo, .775)), Misses);
        }
    }
}
