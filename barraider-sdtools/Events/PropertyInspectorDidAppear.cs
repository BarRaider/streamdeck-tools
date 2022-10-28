using Newtonsoft.Json;

namespace BarRaider.SdTools.Events
{
    /// <summary>
    /// Payload for PropertyInspectorDidAppear event
    /// </summary>
    public class PropertyInspectorDidAppear
    {
        /// <summary>
        /// ActionId
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// ContextId
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Device Guid
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="context"></param>
        /// <param name="device"></param>
        public PropertyInspectorDidAppear(string action, string context, string device)
        {
            Action = action;
            Context = context;
            Device = device;
        }
    }
}
