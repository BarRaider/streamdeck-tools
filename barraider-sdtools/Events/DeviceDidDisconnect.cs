using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for DeviceDidDisconnect event
    /// </summary>
    public class DeviceDidDisconnect
    {
        /// <summary>
        /// Device GUID
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device"></param>
        public DeviceDidDisconnect(String device)
        {
            Device = device;
        }
    }
}
