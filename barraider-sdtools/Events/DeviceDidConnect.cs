using Newtonsoft.Json;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for DeviceDidConnect event
    /// </summary>
    public class DeviceDidConnect
    {
        /// <summary>
        /// Device GUID
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Device Info
        /// </summary>
        [JsonProperty("deviceInfo")]
        public StreamDeckDeviceInfo DeviceInfo { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceInfo"></param>
        public DeviceDidConnect(StreamDeckDeviceInfo deviceInfo)
        {
            Device = deviceInfo?.Id;
            DeviceInfo = deviceInfo;
        }
    }
}
