using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    public interface ISDConnection : IDisposable
    {
        event EventHandler<SDEventReceivedEventArgs<SendToPlugin>> OnSendToPlugin;
        event EventHandler<SDEventReceivedEventArgs<TitleParametersDidChange>> OnTitleParametersDidChange;
        event EventHandler<SDEventReceivedEventArgs<ApplicationDidTerminate>> OnApplicationDidTerminate;
        event EventHandler<SDEventReceivedEventArgs<ApplicationDidLaunch>> OnApplicationDidLaunch;
        event EventHandler<SDEventReceivedEventArgs<DeviceDidDisconnect>> OnDeviceDidDisconnect;
        event EventHandler<SDEventReceivedEventArgs<DeviceDidConnect>> OnDeviceDidConnect;
        event EventHandler<SDEventReceivedEventArgs<PropertyInspectorDidAppear>> OnPropertyInspectorDidAppear;
        event EventHandler<SDEventReceivedEventArgs<PropertyInspectorDidDisappear>> OnPropertyInspectorDidDisappear;

        Task SendToPropertyInspectorAsync(JObject settings);
        Task SetSettingsAsync(JObject settings);
        Task SetGlobalSettingsAsync(JObject settings, bool triggerDidReceiveGlobalSettings = true);
        Task GetGlobalSettingsAsync();
        Task SetImageAsync(string base64Image, int? state = null, bool forceSendToStreamdeck = false);
        Task SetImageAsync(Image image, int? state = null, bool forceSendToStreamdeck = false);
        Task SetDefaultImageAsync();
        Task SetTitleAsync(string title, int? state = null);
        Task SwitchProfileAsync(string profileName);
        Task SwitchProfileAsync(string profileName, string deviceId);
        Task ShowAlert();
        Task ShowOk();
        Task LogSDMessage(string message);
        StreamDeckDeviceInfo DeviceInfo();
        Task GetSettingsAsync();
        Task OpenUrlAsync(string uri);
        Task OpenUrlAsync(Uri uri);
        Task SetStateAsync(uint state);
    }
}
