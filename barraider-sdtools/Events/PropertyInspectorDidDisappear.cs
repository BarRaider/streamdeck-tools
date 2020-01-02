using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Events
{
    public class PropertyInspectorDidDisappear
    {
        [JsonProperty("action")]
        public string Action { get; private set; }

        [JsonProperty("context")]
        public string Context { get; private set; }

        [JsonProperty("device")]
        public string Device { get; private set; }

        public PropertyInspectorDidDisappear(string action, string context, string device)
        {
            Action = action;
            Context = context;
            Device = device;
        }
    }
}
