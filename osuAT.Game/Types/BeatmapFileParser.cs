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

namespace osuAT.Game.Types
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

        private static void handleMetadata(Beatmap beatmap, RulesetInfo ruleset, string line)
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

        private static void handleHitObject(Beatmap beatmap, RulesetInfo ruleset, string line)
        {
            IParser parser = ruleset.MapParser;

            var obj = parser.ParseHitObject(line);

            if (obj != null)
            {
                beatmap.HitObjects.Add(obj);
            }
        }

        public static void ParseLine(Beatmap beatmap, RulesetInfo ruleset, Section section, string line,handleMeta = false)
        {

            switch (section)
            {
                case Section.Metadata:
                    if (handleMeta) {
                        handleMetadata(beatmap, ruleset, line);
                    }
                    return;

                case Section.HitObjects:
                    handleHitObject(beatmap, ruleset, line);
                    return;
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
        /// Converts a the contents of a .osu file  into an Instance of the  <see cref="Beatmap"/> class.
        /// </summary>
        public static Beatmap ParseOsuFile(string location, RulesetInfo ruleset)
        {
            Beatmap map = new Beatmap { };
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

                ParseLine(map, ruleset, section, line);
            }
            return map
        }

        /// <summary>
        /// Same as above, but it only returns the list of HitObjects.
        /// </summary>
        /// <param name="location">The location of the file.</param>
        /// <param name="ruleset">The ruleset to use for parsing the file.</param>
        public static List<HitObject> ParseOsuFileHitObjects(string location, RulesetInfo ruleset)
        {
            List<HitObject> hitObjects = new List<HitObject>();
            Section section = Section.General;
            IParser parser = ruleset.MapParser;
            if (File.Exists(location)) {
                
            }
            foreach (string line in File.ReadLines(location))
            {
                if (section == Section.HitObjects)
                {
                    if (shouldSkipLine(line))
                    {
                        continue;
                    }

                    string lineStrip = stripComments(line);

                    if (lineStrip.StartsWith('[') && line.EndsWith(']'))
                    {
                        if (!Enum.TryParse(lineStrip[1..^1], out section))
                            Console.WriteLine($"Unknown section \"{lineStrip}\" in ");

                        continue;
                    }

                    // only different code vs the original ParseOsuFile
                    var obj = parser.ParseHitObject(lineStrip);
                    if (obj != null)
                    {
                        hitObjects.Add(obj);
                    }
                }
            }
            return hitObjects;

        }
    }
}
