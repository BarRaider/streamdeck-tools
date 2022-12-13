using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Interface for a Stream Deck connection
    /// </summary>
    public interface ISDConnection : IDisposable
    {
        #region Events

        /// <summary>
        /// Event received by the plugin when the Property Inspector uses the sendToPlugin event.
        /// </summary>
        event EventHandler<SDEventReceivedEventArgs<SendToPlugin>> OnSendToPlugin;
        /// <summary>
        /// Event received when the user changes the title or title parameters.
        /// </summary>
        event EventHandler<SDEventReceivedEventArgs<TitleParametersDidChange>> OnTitleParametersDidChange;
        /// <summary>
        /// Event received when a monitored application is terminated
        /// </summary>
        event EventHandler<SDEventReceivedEventArgs<ApplicationDidTerminate>> OnApplicationDidTerminate;
        /// <summary>
        /// Event received when a monitored application is launched
        /// </summary>
        event EventHandler<SDEventReceivedEventArgs<ApplicationDidLaunch>> OnApplicationDidLaunch;
        /// <summary>
        /// Event received when a device is unplugged from the computer
        /// </summary>
        event EventHandler<SDEventReceivedEventArgs<DeviceDidDisconnect>> OnDeviceDidDisconnect;
        /// <summary>
        /// Event received when a device is plugged to the computer.
        /// </summary>
        event EventHandler<SDEventReceivedEventArgs<DeviceDidConnect>> OnDeviceDidConnect;
        /// <summary>
        /// Event received when the Property Inspector appears in the Stream Deck software user interface, for example when selecting a new instance.
        /// </summary>
        event EventHandler<SDEventReceivedEventArgs<PropertyInspectorDidAppear>> OnPropertyInspectorDidAppear;
        /// <summary>
        /// Event received when the Property Inspector for an instance is removed from the Stream Deck software user interface, for example when selecting a different instance.
        /// </summary>
        event EventHandler<SDEventReceivedEventArgs<PropertyInspectorDidDisappear>> OnPropertyInspectorDidDisappear;
        /// <summary>
        /// Event received when the computer wakes up
        /// </summary>
        event EventHandler<SDEventReceivedEventArgs<SystemDidWakeUp>> OnSystemDidWakeUp;

        #endregion

        #region Methods

        /// <summary>
        /// Send settings to the PropertyInspector
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task SendToPropertyInspectorAsync(JObject settings);

        /// <summary>
        /// Persists your plugin settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task SetSettingsAsync(JObject settings);

        /// <summary>
        /// Persists your global plugin settings
        /// </summary>
        /// <param name="settings">Settings to save globally</param>
        /// <param name="triggerDidReceiveGlobalSettings">Boolean whether to also trigger a didReceiveGlobalSettings event. Default is true</param>
        /// <returns></returns>
        Task SetGlobalSettingsAsync(JObject settings, bool triggerDidReceiveGlobalSettings = true);

        /// <summary>
        /// Persists your global plugin settings
        /// </summary>
        /// <returns></returns>
        Task GetGlobalSettingsAsync();

        /// <summary>
        /// Sets an image on the StreamDeck key.
        /// </summary>
        /// <param name="base64Image">Base64 encoded image</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="forceSendToStreamdeck">Should image be sent even if it is identical to the one sent previously. Default is false</param>
        /// <returns></returns>
        Task SetImageAsync(string base64Image, int? state = null, bool forceSendToStreamdeck = false);

        /// <summary>
        /// Sets an image on the StreamDeck key
        /// </summary>
        /// <param name="image">Image object</param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <param name="forceSendToStreamdeck">Should image be sent even if it is identical to the one sent previously. Default is false</param>
        /// <returns></returns>
        Task SetImageAsync(Image image, int? state = null, bool forceSendToStreamdeck = false);

        /// <summary>
        /// Sets the default image for this state, as configured in the manifest
        /// </summary>
        /// <returns></returns>
        Task SetDefaultImageAsync();

        /// <summary>
        /// Sets a title on the StreamDeck key
        /// </summary>
        /// <param name="title"></param>
        /// <param name="state">A 0-based integer value representing the state of an action with multiple states. This is an optional parameter. If not specified, the title is set to all states.</param>
        /// <returns></returns>
        Task SetTitleAsync(string title, int? state = null);

        /// <summary>
        /// Switches to one of the plugin's built-in profiles
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        Task SwitchProfileAsync(string profileName);

        /// <summary>
        /// Switches to one of the plugin's built-in profiles. Allows to choose which device to switch it on.
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task SwitchProfileAsync(string profileName, string deviceId);

        /// <summary>
        /// Shows the Alert (Yellow Triangle) on the StreamDeck key
        /// </summary>
        /// <returns></returns>
        Task ShowAlert();

        /// <summary>
        /// Shows the Success (Green checkmark) on the StreamDeck key
        /// </summary>
        /// <returns></returns>
        Task ShowOk();

        /// <summary>
        /// Add a message to the Stream Deck log. This is the log located at: %appdata%\Elgato\StreamDeck\logs\StreamDeck0.log
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task LogSDMessage(string message);

        /// <summary>
        /// Gets the Stream Deck device's info
        /// </summary>
        /// <returns></returns>
        StreamDeckDeviceInfo DeviceInfo();

        /// <summary>
        /// Tells Stream Deck to return the current plugin settings via the ReceivedSettings function
        /// </summary>
        /// <returns></returns>
        Task GetSettingsAsync();

        /// <summary>
        /// Opens a URI in the user's browser
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task OpenUrlAsync(string uri);

        /// <summary>
        /// Opens a URI in the user's browser
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task OpenUrlAsync(Uri uri);

        /// <summary>
        /// Sets the plugin to a specific state which is pre-configured in the manifest file
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task SetStateAsync(uint state);

        /// <summary>
        /// Sets the values of touchpad layouts items
        /// </summary>
        /// <param name="dictKeyValue">Dictionary holding the layout item keys and values you want to change</param>
        /// <returns></returns>
        Task SetFeedbackAsync(Dictionary<string, string> dictKeyValue);

        /// <summary>
        /// Sets the value of a single touchpad layout item
        /// </summary>
        /// <returns></returns>
        Task SetFeedbackAsync(string layoutItemKey, string value);

        /// <summary>
        /// Changes the current Stream Deck+ touch display layout
        /// </summary>
        /// <returns></returns>
        Task SetFeedbackLayoutAsync(string layout);

        #endregion

        /// <summary>
        /// An opaque value identifying the plugin. This value is received during the Registration procedure
        /// </summary>
        [JsonIgnore]
        String ContextId { get; }

        /// <summary>
        /// An opaque value identifying the device the plugin is launched on.
        /// </summary>
        [JsonIgnore]
        String DeviceId { get; }

        /// <summary>
        /// StreamDeckConnection object, initialized based on the args received when launching the program
        /// </summary>
        [JsonIgnore]
        Communication.StreamDeckConnection StreamDeckConnection { get; }
    }
}
