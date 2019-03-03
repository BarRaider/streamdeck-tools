using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Layout of the keys on the StreamDeck hardware device
    /// </summary>
    public class StreamDeckDeviceSize
    {
        /// <summary>
        /// Number of key rows on the StreamDeck hardware device
        /// </summary>
        [JsonProperty(PropertyName = "rows")]
        public int Rows { get; private set; }

        /// <summary>
        /// Number of key columns on the StreamDeck hardware device
        /// </summary>
        [JsonProperty(PropertyName = "columns")]
        public int Cols { get; private set; }
    }
}
