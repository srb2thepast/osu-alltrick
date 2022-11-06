using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OsuApiHelper;

namespace osuAT.Game.API
{
    public class OsuApiBeatmap : OsuBeatmap
    {
        [JsonProperty("file_md5")]
        public string FileMd5 { get; set; }
    }

}
