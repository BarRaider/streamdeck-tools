using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    public class StreamDeckApplicationInfo
    {
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "platform")]
        public string Platform { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }
}
