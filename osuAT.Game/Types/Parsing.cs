using System;
using System.Globalization;


namespace osuAT.Game.Types
{
    // from osu!
    public static class Parsing
    {
        public const int MAX_COORDINATE_VALUE = 131072;

        public const double MAX_PARSE_VALUE = int.MaxValue;

        public static float ParseFloat(string input, float parseLimit = (float)MAX_PARSE_VALUE)
        {
            float output = float.Parse(input, CultureInfo.InvariantCulture);

            if (output < -parseLimit) throw new OverflowException("Value is too low");
            if (output > parseLimit) throw new OverflowException("Value is too high");

            if (float.IsNaN(output)) throw new FormatException("Not a number");

            return output;
        }

        public static double ParseDouble(string input, double parseLimit = MAX_PARSE_VALUE)
        {
            double output = double.Parse(input, CultureInfo.InvariantCulture);

            if (output < -parseLimit) throw new OverflowException("Value is too low");
            if (output > parseLimit) throw new OverflowException("Value is too high");

            if (double.IsNaN(output)) throw new FormatException("Not a number");

            return output;
        }

        public static int ParseInt(string input, int parseLimit = (int)MAX_PARSE_VALUE)
        {
            int output = int.Parse(input, CultureInfo.InvariantCulture);

            if (output < -parseLimit) throw new OverflowException("Value is too low");
            if (output > parseLimit) throw new OverflowException("Value is too high");

            return output;
        }
    }
}
