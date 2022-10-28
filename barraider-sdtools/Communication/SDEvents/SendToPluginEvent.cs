using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for SendToPluginEvent event
    /// </summary>
    public class SendToPluginEvent : BaseEvent
    {
        /// <summary>
        /// Action Name
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Unique Action UUID
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Payload
        /// </summary>
        [JsonProperty("payload")]
        public JObject Payload { get; private set; }
    }
}
