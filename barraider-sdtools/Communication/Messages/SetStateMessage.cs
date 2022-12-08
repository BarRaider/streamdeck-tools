using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetStateMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "setState"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public SetStateMessage(uint state, string context)
        {
            this.Context = context;
            this.Payload = new PayloadClass(state);
        }

        private class PayloadClass : IPayload
        {
            [JsonProperty("state")]
            public uint State { get; private set; }

            public PayloadClass(uint state)
            {
                this.State = state;
            }
        }
    }
}
