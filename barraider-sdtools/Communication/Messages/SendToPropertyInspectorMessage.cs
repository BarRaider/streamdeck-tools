using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SendToPropertyInspectorMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "sendToPropertyInspector"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public JObject Payload { get; private set; }

        [JsonProperty("action")]
        public string Action { get; private set; }

        public SendToPropertyInspectorMessage(string action, JObject data, string context)
        {
            this.Context = context;
            this.Payload = data;
            this.Action = action;
        }
    }
}
