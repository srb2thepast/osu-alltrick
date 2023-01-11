// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
/* License text:
Copyright (c) 2022 ppy Pty Ltd <contact@ppy.sh>.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using osu.Framework.Utils;

namespace osuAT.Game.Objects.LazerAssets.StarRating
{
    public readonly struct StarDifficulty
    {
        /// <summary>
        /// The star difficulty rating for the given beatmap.
        /// </summary>
        public readonly double Stars;

        public StarDifficulty(double starDifficulty)
        {
            Stars = starDifficulty;
        }
        public enum DifficultyRating
        {
            Easy,
            Normal,
            Hard,
            Insane,
            Expert,
            ExpertPlus
        }
        public static DifficultyRating GetDifficultyRating(double starRating)
        {
            if (Precision.AlmostBigger(starRating, 6.5, 0.005))
                return DifficultyRating.ExpertPlus;

            if (Precision.AlmostBigger(starRating, 5.3, 0.005))
                return DifficultyRating.Expert;

            if (Precision.AlmostBigger(starRating, 4.0, 0.005))
                return DifficultyRating.Insane;

            if (Precision.AlmostBigger(starRating, 2.7, 0.005))
                return DifficultyRating.Hard;

            if (Precision.AlmostBigger(starRating, 2.0, 0.005))
                return DifficultyRating.Normal;

            return DifficultyRating.Easy;
        }

        public DifficultyRating DifficultyRate => GetDifficultyRating(Stars);
    }
}
