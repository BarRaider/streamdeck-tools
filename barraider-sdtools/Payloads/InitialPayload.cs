using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

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
        public InitialPayload(KeyCoordinates coordinates, JObject settings, uint state, bool isInMultiAction, StreamDeckInfo deviceInfo)
        {
            Coordinates = coordinates;
            Settings = settings;
            State = state;
            IsInMultiAction = isInMultiAction;
            DeviceInfo = deviceInfo;
        }
    }
}
