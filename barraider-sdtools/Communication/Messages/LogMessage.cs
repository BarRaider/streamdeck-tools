using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class LogMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "logMessage"; } }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public LogMessage(string message)
        {
            this.Payload = new PayloadClass(message);
        }

        private class PayloadClass : IPayload
        {
            [JsonProperty("message")]
            public string Message { get; private set; }

            public PayloadClass(string message)
            {
                this.Message = message;
            }
        }
    }
}
