using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Events
{
    public class ApplicationDidLaunch
    {
        [JsonProperty("payload")]
        public ApplicationPayload Payload { get; private set; }

        public ApplicationDidLaunch(ApplicationPayload payload)
        {
            Payload = payload;
        }
    }
}
