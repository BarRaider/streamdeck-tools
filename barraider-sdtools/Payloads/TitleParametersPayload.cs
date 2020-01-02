using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Payloads
{
    public class TitleParametersPayload
    {
        [JsonProperty("settings")]
        public JObject Settings { get; private set; }

        [JsonProperty("coordinates")]
        public KeyCoordinates Coordinates { get; private set; }

        [JsonProperty("state")]
        public uint State { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("titleParameters")]
        public TitleParameters TitleParameters { get; private set; }

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
