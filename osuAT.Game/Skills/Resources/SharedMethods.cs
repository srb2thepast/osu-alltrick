using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Diagnostics.Runtime.Windows;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Objects;
using osuAT.Game.Types;

namespace osuAT.Game.Skills.Resources
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
        public static double MissPenalty(int Misses, int MaxCombo)
        {
            return .97 * Math.Pow(1 - Math.Pow(((double)Misses) / MaxCombo, .775), Misses);
        }

        public static double StandardDeviation(IEnumerable<double> NumList, double sub, bool Sample = false)
        {
            double total = 0;
            foreach (var num in NumList)
            {
                total += Math.Pow(num - sub, 2);
            }
            total /= Math.Max(1, NumList.Count() - (Sample ? 1 : 0));
            return total;
        }

        /// <summary>
        /// Converts the BPM inputted into it's ms value with <paramref name="divisor"/>.
        /// </summary>
        /// <param name="bpm">The BPM to convert</param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static double BPMToMS(double bpm,int divisor=4) {
            return (60000/bpm)/divisor;
        }

        /// <summary>
        /// Converts the MS inputted into it's bpm value with <paramref name="divisor"/>.
        /// </summary>
        /// <param name="ms">The BPM to convert</param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static double MSToBPM(double ms, int divisor = 4)
        {
            return  (60000/ms)/divisor;
        }

        /// <summary>
        /// Returns an integer ranging from 0-1, where 1 means the score is an FC and 0 means the score has 0 combo.
        /// </summary>
        /// <returns></returns>
        public static double LinearComboScaling(int combo, int maxcombo)
        {
            return ((double)combo) / maxcombo;
        }
    }

    /// <summary>
    /// Methods that return data based on a difficulty hit object for
    /// a specific ruleset (ex. OsuDifficiultyHitObject.GetAngle())
    /// </summary>
    public static class DiffHitObjectExtensions
    {

    }
}
