using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace osuAT.Game.Skill
{

    /// <summary>
    /// A class to store the top plays of each skill. Created primarly for <see cref="SaveStorage"/>.
    /// </summary>
    public class SkillTopScores
    {
        [JsonProperty("flowaim")]
        public List<int> FlowAim;
    }
}
