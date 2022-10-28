using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for ApplicationDidLaunch event
    /// </summary>
    public class ApplicationDidLaunchEvent : BaseEvent
    {
        /// <summary>
        /// Application information
        /// </summary>
        [JsonProperty("payload")]
        public ApplicationPayload Payload { get; private set; }
    }
}
