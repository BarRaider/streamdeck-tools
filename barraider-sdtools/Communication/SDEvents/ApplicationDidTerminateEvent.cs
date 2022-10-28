using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for ApplicationDidTerminate Event
    /// </summary>
    public class ApplicationDidTerminateEvent : BaseEvent
    {
        /// <summary>
        /// Application payload
        /// </summary>
        [JsonProperty("payload")]
        public ApplicationPayload Payload { get; private set; }
    }
}
