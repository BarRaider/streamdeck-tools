using Newtonsoft.Json;
namespace BarRaider.SdTools
{
    /// <summary>
    /// Coordinates of the current key
    /// </summary>
    public class KeyCoordinates
    {
        /// <summary>
        /// Column of the current key
        /// </summary>
        [JsonProperty("column")]
        public int Column { get; set; }

        /// <summary>
        /// Row of the current key
        /// </summary>
        [JsonProperty("row")]
        public int Row { get; set; }
    }
}

