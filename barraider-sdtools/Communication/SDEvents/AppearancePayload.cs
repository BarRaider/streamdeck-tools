using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for Apperance settings
    /// </summary>
    public class AppearancePayload
    {
        /// <summary>
        /// Additional settings
        /// </summary>
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }

        /// <summary>
        /// Coordinates of key pressed
        /// </summary>
        [JsonProperty("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        /// <summary>
        /// State of key
        /// </summary>
        [JsonProperty("state")]
        public uint State { get; private set; }

        /// <summary>
        /// Is action in MultiAction
        /// </summary>
        [JsonProperty("isInMultiAction")]
        public bool IsInMultiAction { get; private set; }
    }
}
