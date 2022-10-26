// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;

namespace osuAT.Game.Types;

public interface IExtendedDifficultyCalculator
{
    DifficultyHitObject[] GetDifficultyHitObjects(IBeatmap beatmap, double clockRate);
}
