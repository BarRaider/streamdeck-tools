using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for ApplicationDidTerminate event
    /// </summary>
    public class ApplicationDidTerminate
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
        public ApplicationDidTerminate(ApplicationPayload payload)
        {
            Payload = payload;
        }
    }
}
