using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Connection object which handles your communication with the Stream Deck app
    /// </summary>
    public class SDConnection
    {
        #region Private Methods

        private string previousImageHash = null;

        #endregion

        #region Public Implementations

        /// <summary>
        /// Send settings to the PropertyInspector
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task SendToPropertyInspectorAsync(JObject settings)
        {
            if (StreamDeckConnection != null && !String.IsNullOrEmpty(ContextId) && !String.IsNullOrEmpty(actionId))
            {
                await StreamDeckConnection.SendToPropertyInspectorAsync(actionId, settings, ContextId);
            }
        }

        /// <summary>
        /// Persists your plugin settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task SetSettingsAsync(JObject settings)
        {
            if (StreamDeckConnection != null && !String.IsNullOrEmpty(ContextId) && !String.IsNullOrEmpty(actionId))
            {
                await StreamDeckConnection.SetSettingsAsync(settings, ContextId);
            }
        }


        /// <summary>
        /// Persists your global plugin settings
        /// </summary>
        /// <param name="settings">Settings to save globally</param>
        /// <param name="triggerDidReceiveGlobalSettings">Boolean whether to also trigger a didReceiveGlobalSettings event. Default is true</param>
        /// <returns></returns>
        public async Task SetGlobalSettingsAsync(JObject settings, bool triggerDidReceiveGlobalSettings = true)
        {
            if (StreamDeckConnection != null)
            {
                await StreamDeckConnection.SetGlobalSettingsAsync(settings);

                if (triggerDidReceiveGlobalSettings)
                {
                    await GetGlobalSettingsAsync();
                }
            }
        }

        /// <summary>
        /// Persists your global plugin settings
        /// </summary>
        /// <returns></returns>
        public async Task GetGlobalSettingsAsync()
        {
            if (StreamDeckConnection != null)
            {
                await StreamDeckConnection.GetGlobalSettingsAsync();
            }
        }

        /// <summary>
        /// Sets an image on the StreamDeck key.
        /// </summary>
        /// <param name="base64Image">Base64 encoded image</param>
        /// <returns></returns>
        public async Task SetImageAsync(string base64Image, bool forceSendToStreamdeck = false)
        {
            string hash = Tools.StringToMD5(base64Image);
            if (forceSendToStreamdeck || hash != previousImageHash)
            {
                previousImageHash = hash;
                await StreamDeckConnection.SetImageAsync(base64Image, ContextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware);
            }
        }

        /// <summary>
        /// Sets an image on the StreamDeck key
        /// </summary>
        /// <param name="image">Image object</param>
        /// <returns></returns>
        public async Task SetImageAsync(Image image, bool forceSendToStreamdeck = false)
        {
            string hash = Tools.ImageToMD5(image);
            if (forceSendToStreamdeck || hash != previousImageHash)
            {
                previousImageHash = hash;
                await StreamDeckConnection.SetImageAsync(image, ContextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware);
            }
        }

        /// <summary>
        /// Sets a title on the StreamDeck key
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task SetTitleAsync(string title)
        {
            await StreamDeckConnection.SetTitleAsync(title, ContextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware);
        }

        /// <summary>
        /// Switches to one of the plugin's built-in profiles
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public async Task SwitchProfileAsync(string profileName)
        {
            await StreamDeckConnection.SwitchToProfileAsync(this.DeviceId, profileName, this.pluginUUID);
        }

        /// <summary>
        /// Switches to one of the plugin's built-in profiles. Allows to choose which device to switch it on.
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task SwitchProfileAsync(string profileName, string deviceId)
        {
            await StreamDeckConnection.SwitchToProfileAsync(deviceId, profileName, this.pluginUUID);
        }

        /// <summary>
        /// Shows the Alert (Yellow Triangle) on the StreamDeck key
        /// </summary>
        /// <returns></returns>
        public async Task ShowAlert()
        {
            await StreamDeckConnection.ShowAlertAsync(ContextId);
        }

        /// <summary>
        /// Shows the Success (Green checkmark) on the StreamDeck key
        /// </summary>
        /// <returns></returns>
        public async Task ShowOk()
        {
            await StreamDeckConnection.ShowOkAsync(ContextId);
        }

        /// <summary>
        /// Add a message to the Stream Deck log. This is the log located at: %appdata%\Elgato\StreamDeck\logs\StreamDeck0.log
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task LogSDMessage(string message)
        {
            await StreamDeckConnection.LogMessageAsync(message);
        }

        public StreamDeckDeviceInfo DeviceInfo()
        {
            if (deviceInfo == null || string.IsNullOrEmpty(DeviceId))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"Could not get DeviceInfo for DeviceId: {DeviceId} Devices: {deviceInfo?.Devices?.Length}");
                return null;
            }

            return deviceInfo.Devices.Where(d => d.Id == DeviceId).FirstOrDefault();
        }

        #endregion

        [JsonIgnore]
        private readonly string actionId;

        /// <summary>
        /// An opaque value identifying the plugin. Received as an argument when the executable was launched.
        /// </summary>
        [JsonIgnore]
        private readonly string pluginUUID;

        /// <summary>
        /// An opaque value identifying the plugin. This value is received during the Registration procedure
        /// </summary>
        [JsonIgnore]
        public String ContextId { get; private set; }

        /// <summary>
        /// An opaque value identifying the device the plugin is launched on.
        /// </summary>
        [JsonIgnore]
        public String DeviceId { get; private set; }

        /// <summary>
        /// StreamDeckConnection object, initialized based on the args received when launching the program
        /// </summary>
        [JsonIgnore]
        public streamdeck_client_csharp.StreamDeckConnection StreamDeckConnection { get; private set; }

        /// <summary>
        /// Holds information about the devices connected to the computer
        /// </summary>
        [JsonIgnore]
        private readonly StreamDeckInfo deviceInfo;

        /// <summary>
        /// Public constructor, a StreamDeckConnection object is required along with the current action and context IDs
        /// These will be used to correctly communicate with the StreamDeck App
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pluginUUID"></param>
        /// <param name="actionId"></param>
        /// <param name="contextId"></param>
        /// /// <param name="deviceId"></param>
        public SDConnection(streamdeck_client_csharp.StreamDeckConnection connection, string pluginUUID, StreamDeckInfo deviceInfo, string actionId, string contextId, string deviceId)
        {
            StreamDeckConnection = connection;
            this.pluginUUID = pluginUUID;
            this.deviceInfo = deviceInfo;
            this.actionId = actionId;
            this.ContextId = contextId;
            this.DeviceId = deviceId;
        }
    }
}
