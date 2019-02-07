using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    public class SDConnection
    {
        #region Public Implementations

        public async Task SendToPropertyInspectorAsync(JObject settings)
        {
            if (StreamDeckConnection != null && !String.IsNullOrEmpty(contextId) && !String.IsNullOrEmpty(actionId))
            {
                await StreamDeckConnection.SendToPropertyInspectorAsync(actionId, settings, contextId);
            }
        }

        public async Task SetSettingsAsync(JObject settings)
        {
            if (StreamDeckConnection != null && !String.IsNullOrEmpty(contextId) && !String.IsNullOrEmpty(actionId))
            {
                await StreamDeckConnection.SetSettingsAsync(settings, contextId);
            }
        }

        public async Task SetImageAsync(string base64Image)
        {
            await StreamDeckConnection.SetImageAsync(base64Image, contextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware);
        }

        public async Task SetTitleAsync(string title)
        {
            await StreamDeckConnection.SetTitleAsync(title, contextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware);
        }

        public async Task ShowAlert()
        {
            await StreamDeckConnection.ShowAlertAsync(contextId);
        }

        public async Task ShowOk()
        {
            await StreamDeckConnection.ShowOkAsync(contextId);
        }

        #endregion

        [JsonIgnore]
        private readonly string actionId;

        [JsonIgnore]
        private readonly string contextId;

        [JsonIgnore]
        public streamdeck_client_csharp.StreamDeckConnection StreamDeckConnection { get; private set; }

        public SDConnection(streamdeck_client_csharp.StreamDeckConnection connection, string actionId, string contextId)
        {
            StreamDeckConnection = connection;
            this.actionId = actionId;
            this.contextId = contextId;
        }
    }
}
