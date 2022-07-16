using System;
using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Testing;
using osuAT.Game.Types;

#nullable enable

namespace osuAT.Game.Types
{
    public class SkillGoals
    {
        public int Learner { get; set; }
        public int Experienced { get; set; }
        public int Confident { get; set; }
        public int Proficient { get; set; }
        public int Mastery { get; set; }
        public int Chosen { get; set; }

        public SkillGoals(int learner, int experienced, int confident, int proficient, int mastery, int chosen)
        {
            Learner = learner;
            Confident = confident;
            Experienced = experienced;
            Proficient = proficient;
            Mastery = mastery;
            Chosen = chosen;
        }
    }
}
