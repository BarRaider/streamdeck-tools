using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for TitleParametersDidChange event
    /// </summary>
    public class TitleParametersDidChange
    {
        /// <summary>
        /// Action Id
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Context Id
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Device Guid
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Payload
        /// </summary>
        [JsonProperty("payload")]
        public TitleParametersPayload Payload { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="context"></param>
        /// <param name="device"></param>
        /// <param name="payload"></param>
        public TitleParametersDidChange(string action, string context, string device, TitleParametersPayload payload)
        {
            Action = action;
            Context = context;
            Device = device;
            Payload = payload;
        }
    }
}
