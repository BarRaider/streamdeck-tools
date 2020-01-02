using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Events
{
    public class DeviceDidConnect
    {
        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("deviceInfo")]
        public StreamDeckDeviceInfo DeviceInfo { get; private set; }

        public DeviceDidConnect(StreamDeckDeviceInfo deviceInfo)
        {
            Device = deviceInfo?.Id;
            DeviceInfo = deviceInfo;
        }
    }
}
