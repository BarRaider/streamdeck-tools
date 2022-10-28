using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetSettingsMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "setSettings"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public JObject Payload { get; private set; }

        public SetSettingsMessage(JObject settings, string context)
        {
            this.Context = context;
            this.Payload = settings;
        }
    }
}
