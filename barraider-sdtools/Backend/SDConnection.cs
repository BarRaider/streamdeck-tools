using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using BarRaider.SdTools.Wrappers;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Payloads;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Connection object which handles your communication with the Stream Deck app
    /// </summary>
    public class SDConnection : ISDConnection
    {
        #region Private Members

        private string previousImageHash = null;

        [JsonIgnore]
        private readonly string actionId;

        /// <summary>
        /// An opaque value identifying the plugin. Received as an argument when the executable was launched.
        /// </summary>
        [JsonIgnore]
        private readonly string pluginUUID;

        /// <summary>
        /// Holds information about the devices connected to the computer
        /// </summary>
        [JsonIgnore]
        private readonly StreamDeckInfo deviceInfo;

        #endregion

        #region Public Events

        /// <summary>
        /// Event received by the plugin when the Property Inspector uses the sendToPlugin event.
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<SendToPlugin>> OnSendToPlugin;
        /// <summary>
        /// Event received when the user changes the title or title parameters.
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<TitleParametersDidChange>> OnTitleParametersDidChange;
        /// <summary>
        /// Event received when a monitored application is terminated
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<ApplicationDidTerminate>> OnApplicationDidTerminate;
        /// <summary>
        /// Event received when a monitored application is launched
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<ApplicationDidLaunch>> OnApplicationDidLaunch;
        /// <summary>
        /// Event received when a device is unplugged from the computer
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<DeviceDidDisconnect>> OnDeviceDidDisconnect;
        /// <summary>
        /// Event received when a device is plugged to the computer.
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<DeviceDidConnect>> OnDeviceDidConnect;
        /// <summary>
        /// Event received when the Property Inspector appears in the Stream Deck software user interface, for example when selecting a new instance.
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<PropertyInspectorDidAppear>> OnPropertyInspectorDidAppear;
        /// <summary>
        /// Event received when the Property Inspector for an instance is removed from the Stream Deck software user interface, for example when selecting a different instance.
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<PropertyInspectorDidDisappear>> OnPropertyInspectorDidDisappear;

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
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="forceSendToStreamdeck">Should image be sent even if it is identical to the one sent previously. Default is false</param>
        /// <returns></returns>
        public async Task SetImageAsync(string base64Image, int? state = null, bool forceSendToStreamdeck = false)
        {
            string hash = Tools.StringToSHA512(base64Image);
            if (forceSendToStreamdeck || hash != previousImageHash)
            {
                previousImageHash = hash;
                await StreamDeckConnection.SetImageAsync(base64Image, ContextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware, state);
            }
        }

        /// <summary>
        /// Sets an image on the StreamDeck key
        /// </summary>
        /// <param name="image">Image object</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="forceSendToStreamdeck">Should image be sent even if it is identical to the one sent previously. Default is false</param>
        /// <returns></returns>
        public async Task SetImageAsync(Image image, int? state = null, bool forceSendToStreamdeck = false)
        {
            string hash = Tools.ImageToSHA512(image);
            if (forceSendToStreamdeck || hash != previousImageHash)
            {
                previousImageHash = hash;
                await StreamDeckConnection.SetImageAsync(image, ContextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware, state);
            }
        }

        /// <summary>
        /// Sets the default image for this state, as configured in the manifest
        /// </summary>
        /// <returns></returns>
        public async Task SetDefaultImageAsync()
        {
            await SetImageAsync((String)null);
        }

        /// <summary>
        /// Sets a title on the StreamDeck key
        /// </summary>
        /// <param name="title"></param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <returns></returns>
        public async Task SetTitleAsync(string title, int? state = null)
        {
            await StreamDeckConnection.SetTitleAsync(title, ContextId, streamdeck_client_csharp.SDKTarget.HardwareAndSoftware, state);
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

        /// <summary>
        /// Gets the Stream Deck device's info
        /// </summary>
        /// <returns></returns>
        public StreamDeckDeviceInfo DeviceInfo()
        {
            if (deviceInfo == null || string.IsNullOrEmpty(DeviceId))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"Could not get DeviceInfo for DeviceId: {DeviceId} Devices: {deviceInfo?.Devices?.Length}");
                return null;
            }

            return deviceInfo.Devices.Where(d => d.Id == DeviceId).FirstOrDefault();
        }

        /// <summary>
        /// Tells Stream Deck to return the current plugin settings via the ReceivedSettings function
        /// </summary>
        /// <returns></returns>
        public async Task GetSettingsAsync()
        {
            await StreamDeckConnection.GetSettingsAsync(ContextId);
        }

        /// <summary>
        /// Opens a URI in the user's browser
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task OpenUrlAsync(string uri)
        {
            await StreamDeckConnection.OpenUrlAsync(uri);
        }

        /// <summary>
        /// Opens a URI in the user's browser
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task OpenUrlAsync(Uri uri)
        {
            await StreamDeckConnection.OpenUrlAsync(uri);
        }

        /// <summary>
        /// Sets the plugin to a specific state which is pre-configured in the manifest file
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task SetStateAsync(uint state)
        {
            await StreamDeckConnection.SetStateAsync(state, ContextId);
        }

        #endregion

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
        /// Public constructor, a StreamDeckConnection object is required along with the current action and context IDs
        /// These will be used to correctly communicate with the StreamDeck App
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pluginUUID"></param>
        /// <param name="deviceInfo"></param>
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

            StreamDeckConnection.OnSendToPlugin += Connection_OnSendToPlugin;
            StreamDeckConnection.OnTitleParametersDidChange += Connection_OnTitleParametersDidChange;
            StreamDeckConnection.OnApplicationDidTerminate += Connection_OnApplicationDidTerminate;
            StreamDeckConnection.OnApplicationDidLaunch += Connection_OnApplicationDidLaunch;
            StreamDeckConnection.OnDeviceDidDisconnect += Connection_OnDeviceDidDisconnect;
            StreamDeckConnection.OnDeviceDidConnect += Connection_OnDeviceDidConnect;
            StreamDeckConnection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            StreamDeckConnection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
        }

        /// <summary>
        /// Dispose (Destructor) function
        /// </summary>
        public void Dispose()
        {
            StreamDeckConnection.OnSendToPlugin -= Connection_OnSendToPlugin;
            StreamDeckConnection.OnTitleParametersDidChange -= Connection_OnTitleParametersDidChange;
            StreamDeckConnection.OnApplicationDidTerminate -= Connection_OnApplicationDidTerminate;
            StreamDeckConnection.OnApplicationDidLaunch -= Connection_OnApplicationDidLaunch;
            StreamDeckConnection.OnDeviceDidDisconnect -= Connection_OnDeviceDidDisconnect;
            StreamDeckConnection.OnDeviceDidConnect -= Connection_OnDeviceDidConnect;
            StreamDeckConnection.OnPropertyInspectorDidAppear -= Connection_OnPropertyInspectorDidAppear;
            StreamDeckConnection.OnPropertyInspectorDidDisappear -= Connection_OnPropertyInspectorDidDisappear;
        }


        #region Event Wrappers

        private void Connection_OnPropertyInspectorDidDisappear(object sender, streamdeck_client_csharp.StreamDeckEventReceivedEventArgs<streamdeck_client_csharp.Events.PropertyInspectorDidDisappearEvent> e)
        {
            if (e.Event.Context == ContextId)
            {
                OnPropertyInspectorDidDisappear?.Invoke(this, new SDEventReceivedEventArgs<PropertyInspectorDidDisappear>(new PropertyInspectorDidDisappear(e.Event.Action, e.Event.Context, e.Event.Device)));
            }
        }

        private void Connection_OnPropertyInspectorDidAppear(object sender, streamdeck_client_csharp.StreamDeckEventReceivedEventArgs<streamdeck_client_csharp.Events.PropertyInspectorDidAppearEvent> e)
        {
            if (e.Event.Context == ContextId)
            {
                OnPropertyInspectorDidAppear?.Invoke(this, new SDEventReceivedEventArgs<PropertyInspectorDidAppear>(new PropertyInspectorDidAppear(e.Event.Action, e.Event.Context, e.Event.Device)));
            }
        }

        private void Connection_OnDeviceDidConnect(object sender, streamdeck_client_csharp.StreamDeckEventReceivedEventArgs<streamdeck_client_csharp.Events.DeviceDidConnectEvent> e)
        {
            OnDeviceDidConnect?.Invoke(this, new SDEventReceivedEventArgs<DeviceDidConnect>(new DeviceDidConnect(e.Event.DeviceInfo.ToStreamDeckDeviceInfo(e.Event.Device))));
        }

        private void Connection_OnDeviceDidDisconnect(object sender, streamdeck_client_csharp.StreamDeckEventReceivedEventArgs<streamdeck_client_csharp.Events.DeviceDidDisconnectEvent> e)
        {
            OnDeviceDidDisconnect?.Invoke(this, new SDEventReceivedEventArgs<DeviceDidDisconnect>(new DeviceDidDisconnect(e.Event.Device)));
        }

        private void Connection_OnApplicationDidTerminate(object sender, streamdeck_client_csharp.StreamDeckEventReceivedEventArgs<streamdeck_client_csharp.Events.ApplicationDidTerminateEvent> e)
        {
            OnApplicationDidTerminate?.Invoke(this, new SDEventReceivedEventArgs<ApplicationDidTerminate>(new ApplicationDidTerminate(new Payloads.ApplicationPayload(e.Event.Payload.Application))));
        }

        private void Connection_OnApplicationDidLaunch(object sender, streamdeck_client_csharp.StreamDeckEventReceivedEventArgs<streamdeck_client_csharp.Events.ApplicationDidLaunchEvent> e)
        {
            OnApplicationDidLaunch?.Invoke(this, new SDEventReceivedEventArgs<ApplicationDidLaunch>(new ApplicationDidLaunch(new Payloads.ApplicationPayload(e.Event.Payload.Application))));
        }

        private void Connection_OnTitleParametersDidChange(object sender, streamdeck_client_csharp.StreamDeckEventReceivedEventArgs<streamdeck_client_csharp.Events.TitleParametersDidChangeEvent> e)
        {
            if (e.Event.Context == ContextId)
            {
                var payload = e.Event.Payload;
                var newPayload = new TitleParametersPayload(payload.Settings, payload.Coordinates.ToKeyCoordinates(), payload.State, payload.Title, payload.TitleParameters.ToSDTitleParameters());
                OnTitleParametersDidChange?.Invoke(this, new SDEventReceivedEventArgs<TitleParametersDidChange>(new TitleParametersDidChange(e.Event.Action, e.Event.Context, e.Event.Device, newPayload)));
            }
        }

        private void Connection_OnSendToPlugin(object sender, streamdeck_client_csharp.StreamDeckEventReceivedEventArgs<streamdeck_client_csharp.Events.SendToPluginEvent> e)
        {
            if (e.Event.Context == ContextId)
            {
                OnSendToPlugin?.Invoke(this, new SDEventReceivedEventArgs<SendToPlugin>(new SendToPlugin(e.Event.Action, e.Event.Context, e.Event.Payload)));
            }
        }

        #endregion
    }
}
