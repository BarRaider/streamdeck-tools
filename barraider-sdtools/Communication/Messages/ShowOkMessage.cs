using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class ShowOkMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "showOk"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        public ShowOkMessage(string context)
        {
            this.Context = context;
        }
    }
}
