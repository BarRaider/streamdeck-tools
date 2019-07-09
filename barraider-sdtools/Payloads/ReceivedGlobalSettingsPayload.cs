using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Payload that holds all the settings in the ReceivedGlobalSettings event
    /// </summary>
    public class ReceivedGlobalSettingsPayload
    {
        /// <summary>
        /// Global settings object
        /// </summary>
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }
    }
}
