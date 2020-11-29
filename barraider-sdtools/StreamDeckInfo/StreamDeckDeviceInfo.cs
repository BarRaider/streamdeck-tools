using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Type of StreamDeck hardware device, currently two are supported (classic and mini)
    /// </summary>
    public enum StreamDeckDeviceType
    {
        /// <summary>
        /// StreamDeck classic with 15 keys
        /// </summary>
        StreamDeckClassic = 0,

        /// <summary>
        /// StreamDeck mini with 6 keys
        /// </summary>
        StreamDeckMini = 1,

        /// <summary>
        /// StreamDeck XL with 32 keys
        /// </summary>
        StreamDeckXL = 2,

        /// <summary>
        /// StreamDeck Mobile version
        /// </summary>
        StreamDeckMobile = 3,

        /// <summary>
        /// Corsair G-Keys version
        /// </summary>
        CorsairGKeys = 4
    }

    /// <summary>
    /// Class which holds information on the StreamDeck hardware device
    /// </summary>
    public class StreamDeckDeviceInfo
    {
        /// <summary>
        /// Details on number of keys of the StreamDeck hardware device
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public StreamDeckDeviceSize Size { get; private set; }

        /// <summary>
        /// Type of StreamDeck hardware device
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public StreamDeckDeviceType Type { get; private set; }

        /// <summary>
        /// Id of the StreamDeck hardware device
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <param name="deviceId"></param>
        public StreamDeckDeviceInfo(StreamDeckDeviceSize size, StreamDeckDeviceType type, string deviceId)
        {
            Size = size;
            Type = type;
            Id = deviceId;
        }

        /// <summary>
        /// Shows class information as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Id: {Id} Type: {Type} Size: {Size?.ToString()}";
        }
    }
}
