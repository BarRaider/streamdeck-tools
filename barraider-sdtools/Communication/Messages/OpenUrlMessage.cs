using Newtonsoft.Json;
using System;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class OpenUrlMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "openUrl"; } }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public OpenUrlMessage(Uri uri)
        {
            this.Payload = new PayloadClass(uri);
        }

        private class PayloadClass : IPayload
        {
            [JsonProperty("url")]
            public string Url { get; private set; }

            public PayloadClass(Uri uri)
            {
                this.Url = uri.ToString();
            }
        }
    }
}
