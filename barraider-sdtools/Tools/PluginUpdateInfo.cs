using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Status of update request
    /// </summary>
    public enum PluginUpdateStatus
    {
        /// <summary>
        /// Unknown reply
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Plugin is up to date
        /// </summary>
        UpToDate = 1,

        /// <summary>
        /// Minor version update available
        /// </summary>
        MinorUpgrade = 2,

        /// <summary>
        /// Major version update available
        /// </summary>
        MajorUpgrade = 3,

        /// <summary>
        /// Critical update needed
        /// </summary>
        CriticalUpgrade = 4
    }

    /// <summary>
    /// Response to an update request
    /// </summary>
    public class PluginUpdateInfo
    {
        /// <summary>
        /// Status
        /// </summary>
        [JsonProperty("status")]
        public PluginUpdateStatus Status { get; private set; }

        /// <summary>
        /// Update URL
        /// </summary>
        [JsonProperty("updateURL")]
        public string UpdateURL { get; private set; }

        /// <summary>
        /// Update URL
        /// </summary>
        [JsonProperty("updateImage")]
        public string UpdateImage { get; private set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PluginUpdateInfo()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="updateURL"></param>
        /// /// <param name="updateImage"></param>
        public PluginUpdateInfo(PluginUpdateStatus status, string updateURL, string updateImage)
        {
            Status = status;
            UpdateURL = updateURL;
            UpdateImage = updateImage;
        }
    }
}
