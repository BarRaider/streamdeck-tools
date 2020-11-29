using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Class which holds information on the StreamDeck app and StreamDeck hardware device that the plugin is communicating with
    /// </summary>
    public class StreamDeckInfo
    {
        /// <summary>
        /// Information on the StreamDeck App which we're communicating with
        /// </summary>
        [JsonProperty(PropertyName = "application")]
        public StreamDeckApplicationInfo Application { get; private set; }

        /// <summary>
        /// Information on the StreamDeck hardware device that the plugin is running on
        /// </summary>
        [JsonProperty(PropertyName = "devices")]
        public StreamDeckDeviceInfo[] Devices { get; private set; }

        /// <summary>
        /// Information on the Plugin we're currently running
        /// </summary>
        [JsonProperty(PropertyName = "plugin")]
        public StreamDeckPluginInfo Plugin { get; private set; }

        /// <summary>
        /// Device pixel ratio
        /// </summary>
        [JsonProperty(PropertyName = "devicePixelRatio")]
        public int DevicePixelRatio { get; private set; }

        /// <summary>
        /// Shows class information as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Devices != null)
            {
                sb.Append("Devices:\n");
                for (int device = 0; device < Devices.Length; device++)
                {
                    if (Devices[device] != null)
                    {
                        sb.Append($"[{Devices[device]}]\n");
                    }
                }
            }

            if (Application != null)
            {
                sb.Append($"ApplicationInfo: {Application}\n");
            }

            if (Plugin != null)
            {
                sb.Append($"PluginInfo: {Plugin}\n");
            }
            return sb.ToString();
        }
    }
}
