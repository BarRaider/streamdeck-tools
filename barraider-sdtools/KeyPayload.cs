using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools
{
    public class KeyPayload
    {
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }

        [JsonProperty("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        [JsonProperty("state")]
        public uint State { get; private set; }

        [JsonProperty("userDesiredState")]
        public uint UserDesiredState { get; private set; }

        [JsonProperty("isInMultiAction")]
        public bool IsInMultiAction { get; private set; }

        public KeyPayload(KeyCoordinates coordinates, JObject settings, uint state, uint userDesiredState, bool isInMultiAction)
        {
            Coordinates = coordinates;
            Settings = settings;
            State = state;
            UserDesiredState = userDesiredState;
            IsInMultiAction = isInMultiAction;
        }
    }
}
