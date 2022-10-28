using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SetImageMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "setImage"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public SetImageMessage(string base64Image, string context, SDKTarget target, int? state)
        {
            this.Context = context;
            this.Payload = new PayloadClass(base64Image, target, state);
        }

        private class PayloadClass : IPayload
        {
            [JsonProperty("image")]
            public string Image { get; private set; }

            [JsonProperty("target")]
            public SDKTarget Target { get; private set; }

            [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
            public int? State { get; private set; }

            public PayloadClass(string image, SDKTarget target, int? state)
            {
                this.Image = image;
                this.Target = target;
                this.State = state;
            }
        }
    }
}
