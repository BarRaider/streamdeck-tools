using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// Raw payload for TitleParametersRawPayload event (without objects)
    /// </summary>
    public class TitleParametersRawPayload
    {
        /// <summary>
        /// Name of font family
        /// </summary>
        [JsonProperty("fontFamily")]
        public string FontFamily { get; private set; }

        /// <summary>
        /// Size of font
        /// </summary>
        [JsonProperty("fontSize")]
        public uint FontSize { get; private set; }

        /// <summary>
        /// Style of font (bold, italic)
        /// </summary>
        [JsonProperty("fontStyle")]
        public string FontStyle { get; private set; }

        /// <summary>
        /// Is there an underling
        /// </summary>
        [JsonProperty("fontUnderline")]
        public bool FontUnderline { get; private set; }

        /// <summary>
        /// Should title be shown
        /// </summary>
        [JsonProperty("showTitle")]
        public bool ShowTitle { get; private set; }

        /// <summary>
        /// Alignment of title (top, middle, bottom)
        /// </summary>
        [JsonProperty("titleAlignment")]
        public string TitleAlignment { get; private set; }

        /// <summary>
        /// Color of title
        /// </summary>
        [JsonProperty("titleColor")]
        public string TitleColor { get; private set; }
    }
}
