using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload received when the touchpad (above the dials) is pressed
    /// </summary>
    public class TouchpadPressPayload
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
        /// Boolean whether it was a long press or not
        /// </summary>
        [JsonProperty("hold")]
        public bool IsLongPress { get; private set; }

        /// <summary>
        /// Position on touchpad which was pressed
        /// </summary>
        [JsonProperty("tapPos")]
        public int[] TapPosition { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="settings"></param>
        /// <param name="controller"></param>
        /// <param name="isLongPress"></param>
        /// <param name="tapPosition"></param>
        public TouchpadPressPayload(KeyCoordinates coordinates, JObject settings, string controller, bool isLongPress, int[] tapPosition)
        {
            Coordinates = coordinates;
            Settings = settings;
            Controller = controller;
            IsLongPress = isLongPress;
            TapPosition = tapPosition;
        }

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public TouchpadPressPayload() { }
    }
}
