using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuAT.Game.Types.BeatmapParsers;
using osuAT.Game.Types;
using osuAT.Game.Objects;
using osuTK;

namespace osuAT.Game
{
    /// <summary>
    /// Converts a .osu map to a <see cref="Beatmap"/>.
    /// </summary>
    public static class BeatmapFileParser
    {

        public static KeyValuePair<string, string> SplitKeyVal(string line, char separator = ':')
        {
            string[] split = line.Split(separator, 2);

            return new KeyValuePair<string, string>
            (
                split[0].Trim(),
                split.Length > 1 ? split[1].Trim() : string.Empty
            );
        }

        private static void handleMetadata(Beatmap beatmap, string line)
        {
            var pair = SplitKeyVal(line);

            switch (pair.Key)
            {
                case @"Title":
                    // Unicode is preferred.
                    break;

                case @"TitleUnicode":
                    beatmap.SongName = pair.Value;
                    break;

                case @"Artist":
                    // Unicode is preferred.
                    break;

                case @"ArtistUnicode":
                    beatmap.SongArtist = pair.Value;
                    break;

                case @"Creator":
                    beatmap.MapsetCreator = pair.Value;
                    break;

                case @"Version":
                    beatmap.DifficultyName = pair.Value;
                    break;

                case @"BeatmapID":
                    beatmap.MapID = int.Parse(pair.Value);
                    break;

                case @"BeatmapSetID":
                    beatmap.MapsetID = int.Parse(pair.Value);
                    break;
            }
        }

        private static HitObject handleHitObject(RulesetInfo ruleset, string line)
        {
            IParser parser = ruleset.MapParser;

            var obj = parser.ParseHitObject(line);

            if (obj != null)
            {
                return obj;
            }
        }

        private static void handleDifficulty(BeatmapDifficultyInfo difficulty, string line) {
            var pair = SplitKeyVal(line);
            var difficulty = new BeatmapDifficultyInfo;

            switch (pair.Key)
            {
                case @"HPDrainRate":
                    difficulty.HPDrainRate = Parsing.ParseFloat(pair.Value);
                    break;

                case @"CircleSize":
                    difficulty.CircleSize = Parsing.ParseFloat(pair.Value);
                    break;

                case @"OverallDifficulty":
                    difficulty.OverallDifficulty = Parsing.ParseFloat(pair.Value);
                    if (!hasApproachRate)
                        difficulty.ApproachRate = difficulty.OverallDifficulty;
                    break;

                case @"ApproachRate":
                    difficulty.ApproachRate = Parsing.ParseFloat(pair.Value);
                    hasApproachRate = true;
                    break;

                case @"SliderMultiplier":
                    difficulty.SliderMultiplier = Parsing.ParseDouble(pair.Value);
                    break;

                case @"SliderTickRate":
                    difficulty.SliderTickRate = Parsing.ParseDouble(pair.Value);
                    break;
            }
        }

        private static bool shouldSkipLine(string line) => string.IsNullOrWhiteSpace(line) || line.AsSpan().TrimStart().StartsWith("//".AsSpan(), StringComparison.Ordinal);

        private static string stripComments(string line)
        {
            int index = line.AsSpan().IndexOf("//".AsSpan());
            if (index > 0)
                return line.Substring(0, index);

            return line;
        }

        /// <summary>
        /// Converts a the contents of a .osu file into an Instance of the <see cref="Beatmap"/> class.
        /// </summary>
        /// <param name="location">The location of the file.</param>
        /// <param name="map">The map to parse to.</param>
        /// <param name="requestedSections">The sections in the .osu to parse to the Beatmap.</param>
        /// <param name="ruleset">The target ruleset. Can be null if the HitObjects section is not requested.</param>
        public static void ParseOsuFile(string location, Beatmap map, List<Section> requestedSections, RulesetInfo? ruleset)
        {
            BeatmapDifficultyInfo diffinfo = new BeatmapDifficultyInfo();
            
            
            Section section = Section.General;
            foreach (string line in File.ReadLines(location))
            {
                if (shouldSkipLine(line))
                {
                    continue;
                }

                string lineStrip = stripComments(line);
                
                if (lineStrip.StartsWith('[') && line.EndsWith(']'))
                {
                    if (!Enum.TryParse(lineStrip[1..^1], out section)) Console.WriteLine ($"Unknown section \"{lineStrip}\" in ");

                    continue;
                }

                // ParseLine
                // Goal: Asking for all 3 of these sections would fill every single variable
                // of a Beatmap class.
                switch (section)
                {
                    case Section.Metadata:
                        if (requestedSections.Contains(section)) {
                            handleMetadata(map, line);
                        }
                        return;

                    case Section.HitObjects:
                        if (requestedSections.Contains(section)) {
                            map.HitObjects.Add(handleHitObject(beatmap, ruleset, line));
                        }
                        return;
                    case Section.Difficulty:
                        if (requestedSections.Contains(section)) {
                            handleDifficulty(diffInfo, line);
                        }
                        return;
                }
            }
            // note to self: get rid of any processing related to diffInfo if the beatmap already has one
            // because all of the work we did putting things inside diffInfo would be discarded
            // at the end anyways.
            map.BeatmapDifficultyInfo ??= diffInfo
        }
    }
}
