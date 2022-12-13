using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetFeedbackLayoutMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "setFeedbackLayout"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public SetFeedbackLayoutMessage(string layout, string context)
        {
            this.Context = context;
            this.Payload = new PayloadClass(layout);
        }

        private class PayloadClass : IPayload
        {
            [JsonProperty("layout")]
            public string Layout { get; private set; }
            public PayloadClass(string layout)
            {
                this.Layout = layout;
            }
        }
    }
}
