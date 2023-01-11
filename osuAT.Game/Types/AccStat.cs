using Newtonsoft.Json;

// [~] Maybe in the future we could finally have the location of misses in a beatmap avaliable based off of a replay! (https://discord.com/channels/546120878908506119/757615676310552598/1033540314914373642) <- PP Discord

namespace osuAT.Game.Types
{
    public class AccStat
    {
        [JsonProperty("count_300s")]
        public int Count300 { get; set; }

        [JsonProperty("count_100s")]
        public int Count100 { get; set; }

        [JsonProperty("count_50s")]
        public int Count50 { get; set; }

        [JsonProperty("count_miss")]
        public int CountMiss { get; set; }

        public AccStat(int count300, int count100, int count50, int countMiss)
        {
            Count300 = count300;
            Count100 = count100;
            Count50 = count50;
            CountMiss = countMiss;
        }
        public double CalcAcc()
        {
            return ((300 * Count300) + (100 * Count300) + (50 * Count50)) / (300 * (Count300 + Count100 + Count50 + CountMiss));
        }
    }

}
