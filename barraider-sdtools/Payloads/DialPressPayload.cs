using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received when a dial is pressed or unpressed
    /// </summary>
    public class DialPressPayload
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
        /// Boolean whether the dial is currently pressed or not
        /// </summary>
        [JsonProperty("pressed")]
        public bool IsDialPressed { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="settings"></param>
        /// <param name="controller"></param>
        /// <param name="isDialPressed"></param>
        public DialPressPayload(KeyCoordinates coordinates, JObject settings, string controller, bool isDialPressed)
        {
            Coordinates = coordinates;
            Settings = settings;
            Controller = controller;
            IsDialPressed = isDialPressed;
        }

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public DialPressPayload() { }
    }
}
