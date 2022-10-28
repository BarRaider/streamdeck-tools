using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for DeviceDidDisconnect Event
    /// </summary>
    public class DeviceDidDisconnectEvent : BaseEvent
    {
        /// <summary>
        /// UUID of device that was disconnected
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }
    }
}
