using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received when a dial is down or up
    /// </summary>
    public class DialPayload
    {
        /// <summary>
        /// Controller which issued the event
        /// </summary>
        [JsonProperty("controller")]
        public string Controller { get; private set; }

        /// <summary>
        /// Current event settings
        /// </summary>
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }

        /// <summary>
        /// Coordinates of key on the stream deck
        /// </summary>
        [JsonProperty("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="settings"></param>
        /// <param name="controller"></param>
        public DialPayload(KeyCoordinates coordinates, JObject settings, string controller)
        {
            Coordinates = coordinates;
            Settings = settings;
            Controller = controller;
        }

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public DialPayload() { }
    }
}
