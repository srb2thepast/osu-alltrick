using System;
using System.Collections.Generic;
using System.Text;
using osuAT.Game.Types.BeatmapParsers;
using Newtonsoft.Json;

namespace osuAT.Game.Types
{
    public class BeatmapDifficultyInfo
    {   
        public int HPDrainRate { get; set; }

        public int CircleSize { get; set; }

        public int OverallDifficulty { get; set; }
        
        public int ApproachRate { get; set; }

        public int SliderMultiplier { get; set; }

        public int SliderTickRate { get; set; }
    }
}
