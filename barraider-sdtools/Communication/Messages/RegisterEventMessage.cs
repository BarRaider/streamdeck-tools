using Newtonsoft.Json;
using System;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class RegisterEventMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get; private set; }

        [JsonProperty("uuid")]
        public string UUID { get; private set; }

        public RegisterEventMessage(string eventName, string uuid)
        {
            this.Event = eventName;
            this.UUID = uuid;
        }
    }
}
