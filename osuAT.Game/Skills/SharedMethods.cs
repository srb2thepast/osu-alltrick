using System;
using System.Collections.Generic;
using System.Text;

namespace osuAT.Game.Skills
{
    /// <summary>
    /// Methods that multiple calcs would use (ex. Combo Multiplier) go here.
    /// </summary>
    public static class SharedMethods
    {
        // https://osu.ppy.sh/home/news/2021-01-14-performance-points-updates
        public static double MissPenalty(int Misses,int MaxCombo) {
            return .97 * Math.Pow((1 - Math.Pow(Misses / MaxCombo, .775)), Misses);
        }
    }
}
