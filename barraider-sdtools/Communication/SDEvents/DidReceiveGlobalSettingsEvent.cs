using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for DidReceiveGlobalSettings Event
    /// </summary>
    public class DidReceiveGlobalSettingsEvent : BaseEvent
    {
        /// <summary>
        /// Global Settings payload
        /// </summary>
        [JsonProperty("payload")]
        public ReceivedGlobalSettingsPayload Payload { get; private set; }
    }
}
