using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for DidReceiveSettings Event
    /// </summary>
    public class DidReceiveSettingsEvent : BaseEvent
    {
        /// <summary>
        /// Action Name
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Context (unique action UUID)
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Device UUID action is on
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Settings for action
        /// </summary>
        [JsonProperty("payload")]
        public ReceivedSettingsPayload Payload { get; private set; }
    }
}
