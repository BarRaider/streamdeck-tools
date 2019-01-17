using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BarRaider.SdTools
{
    public abstract class SettingsBase
    {
        #region Public Implementations

        public async void SendToPropertyInspectorAsync()
        {
            if (StreamDeckConnection != null && !String.IsNullOrEmpty(ContextId) && !String.IsNullOrEmpty(ActionId))
            {
                await StreamDeckConnection.SendToPropertyInspectorAsync(ActionId, JObject.FromObject(this), this.ContextId);
            }
        }

        public async void SetSettingsAsync()
        {
            if (StreamDeckConnection != null && !String.IsNullOrEmpty(ContextId) && !String.IsNullOrEmpty(ActionId))
            {
                await StreamDeckConnection.SetSettingsAsync(JObject.FromObject(this), this.ContextId);
            }
        }

        public async void SetImageAsync(string base64Image)
        {
            await StreamDeckConnection.SetImageAsync(base64Image, this.ContextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware);
        }

        public async void SetTitleAsync(string title)
        {
            await StreamDeckConnection.SetTitleAsync(title, this.ContextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware);
        }

        public async void ShowAlert()
        {
            await StreamDeckConnection.ShowAlertAsync(this.ContextId);
        }

        public async void ShowOk()
        {
            await StreamDeckConnection.ShowOkAsync(this.ContextId);
        }



        #endregion

        [JsonIgnore]
        public string ActionId { private get; set; }

        [JsonIgnore]
        public string ContextId { private get; set; }

        [JsonIgnore]
        public streamdeck_client_csharp.StreamDeckConnection StreamDeckConnection { private get; set; }

    }
}
