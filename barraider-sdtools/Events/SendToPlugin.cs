using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Events
{
    public class SendToPlugin
    {
        [JsonProperty("action")]
        public string Action { get; private set; }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public JObject Payload { get; private set; }

        public SendToPlugin(string action, string context, JObject payload)
        {
            Action = action;
            Context = context;
            Payload = payload;
        }
    }
}
