using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for SendToPlugin event
    /// </summary>
    public class SendToPlugin
    {
        /// <summary>
        /// ActionId
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// ContextId
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Payload
        /// </summary>
        [JsonProperty("payload")]
        public JObject Payload { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="context"></param>
        /// <param name="payload"></param>
        public SendToPlugin(string action, string context, JObject payload)
        {
            Action = action;
            Context = context;
            Payload = payload;
        }
    }
}
