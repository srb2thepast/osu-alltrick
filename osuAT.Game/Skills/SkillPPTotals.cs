using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace osuAT.Game.Skills
{

    /// <summary>
    /// A class to store the PPTotals of each skill. Created primarly for <see cref="SaveStorage"/>.
    /// </summary>
    public class SkillPPTotals
    {
        [JsonProperty("flowaim")]
        public int FlowAim = 0;
    }
}
