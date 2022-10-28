using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for DeviceDidConnect Event
    /// </summary>
    public class DeviceDidConnectEvent : BaseEvent
    {
        /// <summary>
        /// UUID of device
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Information on the device connected
        /// </summary>
        [JsonProperty("deviceInfo")]
        public StreamDeckDeviceInfo DeviceInfo { get; private set; }
    }
}
