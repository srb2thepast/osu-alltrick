using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Diagnostics.Runtime.Windows;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables.Cards;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Difficulty.Preprocessing;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Scoring;
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
        ///

        public enum Angle
        {
            Triangle = 60,
            Square = 90,
            Pentagon = 108,
            Hexagon = 120,
            Octagon = 135,
            Line = 180,
        }

        public enum JumpDistances
        {
            Corners = 640, // Corner Jumps
        }

        public enum StreamLengths
        {
            Burst = 3,
            Stream = 10,
            DeathStream = 32
        }

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
        public static double BPMToMS(double bpm, int divisor = 4)
        {
            return (60000 / bpm) / divisor;
        }

        /// <summary>
        /// Converts the MS inputted into it's bpm value with <paramref name="divisor"/>.
        /// </summary>
        /// <param name="ms">The BPM to convert</param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static double MSToBPM(double ms, int divisor = 4)
        {
            return (60000 / ms) / divisor;
        }

        public static double ARToMS(double AR)
        {
            if (AR < 5) return (-60 * AR) + 1800;
            else return (-75 * AR) + 1875;
        }

        /// <summary>
        /// Returns an integer ranging from 0-1, where 1 means the score is an FC and 0 means the score has 0 combo.
        /// </summary>
        /// <returns></returns>
        public static double LinearComboScaling(int combo, int maxcombo)
        {
            return ((double)combo) / maxcombo;
        }

        /// <summary>
        /// Returns a number to penalize or buff score worth depending on acc. Named "Simple" as it doesn't really consider beatmap length or anything besides the accuracy.
        /// </summary>
        /// <returns></returns>
        public static double SimpleAccNerf(double accuracy)
        {
            return (Math.Pow(7, accuracy) - 1) / 6;
        }

        /// <summary>
        /// The ComputeAccuracyValue method ported from osu!. Currently TODO.
        /// </summary>
        /// <returns></returns>
        // from https://github.com/ppy/osu/blob/master/osu.Game.Rulesets.Osu/Difficulty/OsuPerformanceCalculator.cs ([!] to be documented)
        [Obsolete]
        public static double ComputeAccuracyValue(BeatmapContents mapCont, AccStat Accuracy, List<ModInfo> mods)
        {
            if (mods.Any(h => h == ModStore.Relax))
                return 0.0;

            // This percentage only considers HitCircles of any value - in this part of the calculation we focus on hitting the timing hit window.
            double betterAccuracyPercentage;
            int amountHitObjectsWithAccuracy = mapCont.HitObjects.Count;
            int totalHits = Accuracy.Count300 + Accuracy.Count100 + Accuracy.Count50 + Accuracy.CountMiss;

            if (amountHitObjectsWithAccuracy > 0)
                betterAccuracyPercentage = ((Accuracy.Count300 - (totalHits - amountHitObjectsWithAccuracy)) * 6 + Accuracy.Count100 * 2 + Accuracy.Count50) / (double)(amountHitObjectsWithAccuracy * 6);
            else
                betterAccuracyPercentage = 0;

            // It is possible to reach a negative accuracy with this formula. Cap it at zero - zero points.
            if (betterAccuracyPercentage < 0)
                betterAccuracyPercentage = 0;

            // Lots of arbitrary values from testing.
            // Considering to use derivation from perfect accuracy in a probabilistic manner - assume normal distribution.
            double accuracyValue = Math.Pow(1.52163, mapCont.DifficultyInfo.OverallDifficulty) * Math.Pow(betterAccuracyPercentage, 24) * 2.83;

            // Bonus for many hitcircles - it's harder to keep good accuracy up for longer.
            accuracyValue *= Math.Min(1.15, Math.Pow(amountHitObjectsWithAccuracy / 1000.0, 0.3));

            if (mods.Any(m => m == ModStore.Hidden))
                accuracyValue *= 1.08;

            if (mods.Any(m => m == ModStore.Flashlight))
                accuracyValue *= 1.02;
            Console.WriteLine(accuracyValue + " | ACC");
            return accuracyValue;
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
