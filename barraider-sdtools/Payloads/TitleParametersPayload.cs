using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Payload for TitleParametersDidChange Event
    /// </summary>
    public class TitleParametersPayload
    {
        /// <summary>
        /// Settings JSON Object
        /// </summary>
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }

        /// <summary>
        /// Key Coordinates
        /// </summary>
        [JsonProperty("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        /// <summary>
        /// Key State
        /// </summary>
        [JsonProperty("state")]
        public uint State { get; private set; }

        /// <summary>
        /// Title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; private set; }

        /// <summary>
        /// Title Parameters
        /// </summary>
        [JsonProperty("titleParameters")]
        public TitleParameters TitleParameters { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="coordinates"></param>
        /// <param name="state"></param>
        /// <param name="title"></param>
        /// <param name="titleParameters"></param>
        public TitleParametersPayload(JObject settings, KeyCoordinates coordinates, uint state, string title, TitleParameters titleParameters)
        {
            Settings = settings;
            Coordinates = coordinates;
            State = state;
            Title = title;
            TitleParameters = titleParameters;
        }
    }
}
