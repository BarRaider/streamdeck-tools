using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Holds general information on the StreamDeck App we're communicating with
    /// </summary>
    public class StreamDeckPluginInfo
    {
        /// <summary>
        /// Current version of the plugin
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; private set; }
    }
}
