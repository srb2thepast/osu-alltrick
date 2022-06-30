using System;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Testing;
using osuAT.Game.Types;

#nullable enable

namespace osuAT.Game.Types
{
    public class IScore {
        public int ID { get; set; }

        public uint BeatmapID { get; set; }

        public uint BeatmapsetID { get; set; }

        public string? BeatmapLocation{ get; set; }

        public long TotalScore { get; set; }

        public double Accuracy { get; set; }

        public int Combo { get; set; }

        public double? AlltrickPP { get; set; }

        public bool IsLazer { get; set; }

        public RulesetInfo? Ruleset { get; set; }

        public ModInfo[]? Mods { get; set; }
    }
    public class OsuScore : IScore {


        public OsuScore(int iD, uint beatmap, uint beatmapset, long totalScore, double accuracy, int combo, double? alltrickPP, bool isLazer, ModInfo[]? mods)
        {
            ID = iD;
            BeatmapID = beatmap;
            BeatmapsetID = beatmapset;
            TotalScore = totalScore;
            Accuracy = accuracy;
            Combo = combo;
            AlltrickPP = alltrickPP;
            IsLazer = isLazer;
            Mods = mods;
        } // thanks intellisense for typing that all for me lmao
    }
}
