using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.IO.Network;
using Newtonsoft.Json;

namespace osuAT.Game.API
{
    public class NullableJsonWebRequest<T> : JsonWebRequest<T>
    {
        public NullableJsonWebRequest(string url = null, params object[] args)
            : base(url, args)
        { }

        protected override void ProcessResponse()
        {
            if (ResponseStream == null)
                return;

            string response = GetResponseString();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            if (response != null)
                ResponseObject = JsonConvert.DeserializeObject<T>(response, settings);
        }

        public new T ResponseObject { get; private set; }
    }
}
