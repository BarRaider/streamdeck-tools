using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetGlobalSettingsMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "setGlobalSettings"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public JObject Payload { get; private set; }

        public SetGlobalSettingsMessage(JObject settings, string pluginUUID)
        {
            this.Context = pluginUUID;
            this.Payload = settings;
        }
    }
}
