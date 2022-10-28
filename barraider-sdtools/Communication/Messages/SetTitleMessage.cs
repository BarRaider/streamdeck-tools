using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetTitleMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "setTitle"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public SetTitleMessage(string title, string context, SDKTarget target, int? state)
        {
            this.Context = context;
            this.Payload = new PayloadClass(title, target, state);
        }

        private class PayloadClass : IPayload
        {
            [JsonProperty("title")]
            public string Title { get; private set; }

            [JsonProperty("target")]
            public SDKTarget Target { get; private set; }

            [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
            public int? State { get; private set; }

            public PayloadClass(string title, SDKTarget target, int? state)
            {
                this.Title = title;
                this.Target = target;
                this.State = state;
            }
        }
    }
}
