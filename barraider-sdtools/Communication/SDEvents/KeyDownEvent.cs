using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Communication.SDEvents
{
    /// <summary>
    /// Payload for KeyDown event
    /// </summary>
    public class KeyDownEvent : BaseEvent
    {
        /// <summary>
        /// Action Name
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// Unique Action UUID
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Device UUID key was pressed on
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Information on key
        /// </summary>
        [JsonProperty("payload")]
        public KeyPayload Payload { get; private set; }
    }
}
