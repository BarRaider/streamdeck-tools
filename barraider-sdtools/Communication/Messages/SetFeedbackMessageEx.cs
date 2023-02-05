using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetFeedbackMessageEx : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "setFeedback"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public JObject Payload { get; private set; }

        public SetFeedbackMessageEx(JObject payload, string pluginUUID)
        {
            this.Context = pluginUUID;
            Payload = payload;
        }
    }
}
