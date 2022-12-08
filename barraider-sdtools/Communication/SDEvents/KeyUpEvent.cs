using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for KeyUp event
    /// </summary>
    public class KeyUpEvent : BaseEvent
    {
        /// <summary>
        /// Action name
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Unique action UUID
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Stream Deck device UUID
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Key settings
        /// </summary>
        [JsonProperty("payload")]
        public KeyPayload Payload { get; private set; }
    }
}
