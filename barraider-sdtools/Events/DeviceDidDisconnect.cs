using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Events
{
    public class DeviceDidDisconnect
    {
        [JsonProperty("device")]
        public string Device { get; private set; }

        public DeviceDidDisconnect(String device)
        {
            Device = device;
        }
    }
}
