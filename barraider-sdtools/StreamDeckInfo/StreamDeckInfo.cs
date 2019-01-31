using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    public class StreamDeckInfo
    {
        [JsonProperty(PropertyName = "application")]
        public StreamDeckApplicationInfo Application { get; set; }

        [JsonProperty(PropertyName = "devices")]
        public StreamDeckDeviceInfo[] Devices { get; set; }
    }
}
