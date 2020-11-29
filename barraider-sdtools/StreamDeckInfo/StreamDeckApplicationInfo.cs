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
    public class StreamDeckApplicationInfo
    {
        /// <summary>
        /// Current language of the StreamDeck app
        /// </summary>
        [JsonProperty(PropertyName = "language")]
        public string Language { get; private set; }

        /// <summary>
        /// OS Platform
        /// </summary>
        [JsonProperty(PropertyName = "platform")]
        public string Platform { get; private set; }

        /// <summary>
        /// Current version of the StreamDeck app
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; private set; }

        /// <summary>
        /// Shows class information as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Language: {Language} Platform: {Platform} Version: {Version}";
        }
    }
}
