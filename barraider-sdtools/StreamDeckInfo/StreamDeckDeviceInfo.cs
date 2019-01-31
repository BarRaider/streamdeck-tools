using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    public enum StreamDeckDeviceType
    {
        StreamDeckClassic = 0,
        StreamDeckMini = 1
    }

    public class StreamDeckDeviceInfo
    {
        [JsonProperty(PropertyName = "size")]
        public StreamDeckDeviceSize Size { get; set; }

        [JsonProperty(PropertyName = "type")]
        public StreamDeckDeviceType Type { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
