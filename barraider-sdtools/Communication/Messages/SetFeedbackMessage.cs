using Newtonsoft.Json;
using System.Collections.Generic;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetFeedbackMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "setFeedback"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public Dictionary<string, string> DictKeyValues { get; private set; }

        public SetFeedbackMessage(Dictionary<string, string> dictKeyValues, string pluginUUID)
        {
            this.Context = pluginUUID;
            DictKeyValues = dictKeyValues;
        }
    }
}
