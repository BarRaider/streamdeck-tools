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
        private TitleParameters titleParameters = null;

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
        [JsonIgnore]
        public TitleParameters TitleParameters
        { 
            get
            {
                if (titleParameters != null)
                {
                    return titleParameters;
                }

                if (TitleParametersRaw != null)
                {
                    titleParameters = new TitleParameters(TitleParametersRaw.FontFamily, TitleParametersRaw.FontSize, TitleParametersRaw.FontStyle, TitleParametersRaw.FontUnderline, TitleParametersRaw.ShowTitle, TitleParametersRaw.TitleAlignment, TitleParametersRaw.TitleColor);
                }

                return titleParameters;
            }
            private set
            {
                titleParameters = value;
            }
        }

        /// <summary>
        /// Raw Title Parameters (not as proper object)
        /// </summary>
        [JsonProperty("titleParameters")]
        public TitleParametersRawPayload TitleParametersRaw { get; private set; }

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

        /// <summary>
        /// For Serilization
        /// </summary>
        public TitleParametersPayload() { }
    }
}
