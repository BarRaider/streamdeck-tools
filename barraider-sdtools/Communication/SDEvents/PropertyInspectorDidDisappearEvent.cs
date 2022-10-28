using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for PropertyInspectorDidDisappearEvent event
    /// </summary>
    public class PropertyInspectorDidDisappearEvent : BaseEvent
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
        /// Stream Deck device UUID
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }
    }
}
