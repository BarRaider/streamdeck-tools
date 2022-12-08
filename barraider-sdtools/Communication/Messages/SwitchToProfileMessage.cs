using Newtonsoft.Json;

namespace BarRaider.SdTools.Communication.Messages
{
    internal class SwitchToProfileMessage : IMessage
    {
        [JsonProperty("event")]
        public string Event { get { return "switchToProfile"; } }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("payload")]
        public IPayload Payload { get; private set; }

        public SwitchToProfileMessage(string device, string profileName, string pluginUUID)
        {
            this.Context = pluginUUID;
            this.Device = device;
            if (!string.IsNullOrEmpty(profileName))
            {
                this.Payload = new PayloadClass(profileName);
            }
            else
            {
                this.Payload = new EmptyPayload();
            }
        }

        private class PayloadClass : IPayload
        {
            [JsonProperty("profile")]
            public string Profile { get; private set; }

            public PayloadClass(string profile)
            {
                this.Profile = profile;
            }
        }
    }
}
