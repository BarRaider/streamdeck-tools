using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    public class StreamDeckDeviceSize
    {
        [JsonProperty(PropertyName = "rows")]
        public int Rows { get; set; }

        [JsonProperty(PropertyName = "columns")]
        public int Cols { get; set; }
    }
}
