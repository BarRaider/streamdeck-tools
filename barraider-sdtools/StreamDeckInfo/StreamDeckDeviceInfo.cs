using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
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
        public DeviceType Type { get; private set; }

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
        public StreamDeckDeviceInfo(StreamDeckDeviceSize size, DeviceType type, string deviceId)
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
