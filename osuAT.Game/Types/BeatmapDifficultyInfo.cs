using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace osuAT.Game.Types
{
    public class BeatmapDifficultyInfo
    {   
        public float HPDrainRate { get; set; }

        public float CircleSize { get; set; }

        public float OverallDifficulty { get; set; }
        
        public float ApproachRate { get; set; }

        public double SliderMultiplier { get; set; }

        public double SliderTickRate { get; set; }
    }
}
