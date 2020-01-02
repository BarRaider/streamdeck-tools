using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Events
{
    public class ApplicationDidTerminate
    {
        [JsonProperty("payload")]
        public ApplicationPayload Payload { get; private set; }

        public ApplicationDidTerminate(ApplicationPayload payload)
        {
            Payload = payload;
        }
    }
}
