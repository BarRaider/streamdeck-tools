using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BarRaider.SdTools.Payloads;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Payload received during the plugin's constructor
    /// </summary>
    public class InitialPayload
    {
        /// <summary>
        /// Plugin instance's settings (set through Property Inspector)
        /// </summary>
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }

        /// <summary>
        /// Plugin's physical location on the Stream Deck device
        /// </summary>
        [JsonProperty("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        /// <summary>
        /// Current plugin state
        /// </summary>
        [JsonProperty("state")]
        public uint State { get; private set; }

        /// <summary>
        /// Is it in a Multiaction
        /// </summary>
        [JsonProperty("isInMultiAction")]
        public bool IsInMultiAction { get; private set; }

        /// <summary>
        /// The controller of the current action. Values include "Keypad" and "Encoder".
        /// </summary>
        [JsonProperty("controller")]
        public string Controller { get; private set; }

        /// <summary>
        /// Information regarding the Stream Deck hardware device
        /// </summary>
        [JsonProperty("deviceInfo", Required = Required.AllowNull)]
        public StreamDeckInfo DeviceInfo { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="settings"></param>
        /// <param name="state"></param>
        /// <param name="isInMultiAction"></param>
        /// <param name="deviceInfo"></param>
        public InitialPayload(AppearancePayload appearancePayload, StreamDeckInfo deviceInfo)
        {
            Coordinates = appearancePayload.Coordinates;
            Settings = appearancePayload.Settings;
            State = appearancePayload.State;
            IsInMultiAction = appearancePayload.IsInMultiAction;
            Controller = appearancePayload.Controller;
            DeviceInfo = deviceInfo;
        }
    }
}
