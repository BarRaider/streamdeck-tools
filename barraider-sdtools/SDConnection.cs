using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Connection object which handles your communication with the Stream Deck app
    /// </summary>
    public class SDConnection
    {
        #region Public Implementations

        /// <summary>
        /// Send settings to the PropertyInspector
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task SendToPropertyInspectorAsync(JObject settings)
        {
            if (StreamDeckConnection != null && !String.IsNullOrEmpty(contextId) && !String.IsNullOrEmpty(actionId))
            {
                await StreamDeckConnection.SendToPropertyInspectorAsync(actionId, settings, contextId);
            }
        }

        /// <summary>
        /// Persists your plugin settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task SetSettingsAsync(JObject settings)
        {
            if (StreamDeckConnection != null && !String.IsNullOrEmpty(contextId) && !String.IsNullOrEmpty(actionId))
            {
                await StreamDeckConnection.SetSettingsAsync(settings, contextId);
            }
        }

        /// <summary>
        /// Sets an image on the StreamDeck key
        /// </summary>
        /// <param name="base64Image"></param>
        /// <returns></returns>
        public async Task SetImageAsync(string base64Image)
        {
            await StreamDeckConnection.SetImageAsync(base64Image, contextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware);
        }

        /// <summary>
        /// Sets a title on the StreamDeck key
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task SetTitleAsync(string title)
        {
            await StreamDeckConnection.SetTitleAsync(title, contextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware);
        }

        /// <summary>
        /// Shows the Alert (Yellow Triangle) on the StreamDeck key
        /// </summary>
        /// <returns></returns>
        public async Task ShowAlert()
        {
            await StreamDeckConnection.ShowAlertAsync(contextId);
        }

        /// <summary>
        /// Shows the Success (Green checkmark) on the StreamDeck key
        /// </summary>
        /// <returns></returns>
        public async Task ShowOk()
        {
            await StreamDeckConnection.ShowOkAsync(contextId);
        }

        #endregion

        [JsonIgnore]
        private readonly string actionId;

        [JsonIgnore]
        private readonly string contextId;

        /// <summary>
        /// StreamDeckConnection object, initialized based on the args received when launching the program
        /// </summary>
        [JsonIgnore]
        public streamdeck_client_csharp.StreamDeckConnection StreamDeckConnection { get; private set; }

        /// <summary>
        /// Public constructor, a StreamDeckConnection object is required along with the current action and context IDs
        /// These will be used to correctly communicate with the StreamDeck App
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="actionId"></param>
        /// <param name="contextId"></param>
        public SDConnection(streamdeck_client_csharp.StreamDeckConnection connection, string actionId, string contextId)
        {
            StreamDeckConnection = connection;
            this.actionId = actionId;
            this.contextId = contextId;
        }
    }
}
