using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for ApplicationDidLaunch event
    /// </summary>
    public class ApplicationDidLaunch
    {
        /// <summary>
        /// Payload
        /// </summary>
        [JsonProperty("payload")]
        public ApplicationPayload Payload { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload"></param>
        public ApplicationDidLaunch(ApplicationPayload payload)
        {
            Payload = payload;
        }
    }
}
