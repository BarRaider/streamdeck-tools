using BarRaider.SdTools.Communication.Messages;
using BarRaider.SdTools.Communication.SDEvents;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarRaider.SdTools.Communication
{
    /// <summary>
    /// Underlying object that communicates with the stream deck app
    /// </summary>
    public class StreamDeckConnection
    {
        private const int BufferSize = 1024 * 1024;

        private ClientWebSocket webSocket;
        private readonly SemaphoreSlim sendSocketSemaphore = new SemaphoreSlim(1);
        private readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private readonly string registerEvent;

        /// <summary>
        /// The port used to connect to the StreamDeck websocket
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// This is the unique identifier used to communicate with the register StreamDeck plugin.
        /// </summary>
        public string UUID { get; private set; }

        #region Public Events

        /// <summary>
        /// Raised when plugin is connected to stream deck app
        /// </summary>
        public event EventHandler<EventArgs> OnConnected;

        /// <summary>
        /// /// Raised when plugin is disconnected from stream deck app
        /// </summary>
        public event EventHandler<EventArgs> OnDisconnected;

        /// <summary>
        /// Raised when key is pushed down
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<KeyDownEvent>> OnKeyDown;

        /// <summary>
        /// Raised when key is released
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<KeyUpEvent>> OnKeyUp;

        /// <summary>
        /// Raised when the action is shown, main trigger for a PluginAction
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<WillAppearEvent>> OnWillAppear;

        /// <summary>
        /// Raised when the action is no longer shown, main trigger for Dispose of PluginAction
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<WillDisappearEvent>> OnWillDisappear;

        /// <summary>
        /// Contains information on the Title and its style
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<TitleParametersDidChangeEvent>> OnTitleParametersDidChange;

        /// <summary>
        /// Raised when a Stream Deck device is connected to the PC
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<DeviceDidConnectEvent>> OnDeviceDidConnect;

        /// <summary>
        /// Raised when a Stream Deck device has disconnected from the PC
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<DeviceDidDisconnectEvent>> OnDeviceDidDisconnect;

        /// <summary>
        /// Raised when a monitored app is launched/active
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<ApplicationDidLaunchEvent>> OnApplicationDidLaunch;

        /// <summary>
        /// Raised when a monitored app is terminated
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<ApplicationDidTerminateEvent>> OnApplicationDidTerminate;

        /// <summary>
        /// Raised after the PC wakes up from sleep
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<SystemDidWakeUpEvent>> OnSystemDidWakeUp;

        /// <summary>
        /// Raised when settings for the action are received
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<DidReceiveSettingsEvent>> OnDidReceiveSettings;

        /// <summary>
        /// Raised when global settings for the entire plugin are received
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<DidReceiveGlobalSettingsEvent>> OnDidReceiveGlobalSettings;

        /// <summary>
        /// Raised when the user is viewing the settings in the Stream Deck app
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<PropertyInspectorDidAppearEvent>> OnPropertyInspectorDidAppear;

        /// <summary>
        /// Raised when the user stops viewing the settings in the Stream Deck app
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<PropertyInspectorDidDisappearEvent>> OnPropertyInspectorDidDisappear;

        /// <summary>
        /// Raised when a payload is sent to the plugin from the PI
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<SendToPluginEvent>> OnSendToPlugin;

        /// <summary>
        /// Raised when a dial is rotated
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<DialRotateEvent>> OnDialRotate;

        /// <summary>
        /// Raised when a dial is pressed or unpressed
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<DialPressEvent>> OnDialPress;

        /// <summary>
        /// Raised when the tochpad is pressed
        /// </summary>
        public event EventHandler<SDEventReceivedEventArgs<TouchpadPress>> OnTouchpadPress;

        #endregion

        internal StreamDeckConnection(int port, string uuid, string registerEvent)
        {
            this.Port = port;
            this.UUID = uuid;
            this.registerEvent = registerEvent;
        }

        internal void Run()
        {
            if (webSocket == null)
            {
                webSocket = new ClientWebSocket();
                _ = this.RunAsync();
            }
        }

        internal void Stop()
        {
            cancelTokenSource.Cancel();
        }

        internal Task SendAsync(IMessage message)
        {
            return SendAsync(JsonConvert.SerializeObject(message));
        }

        #region Requests

        internal Task SetTitleAsync(string title, string context, SDKTarget target, int? state)
        {
            return SendAsync(new SetTitleMessage(title, context, target, state));
        }

        internal Task LogMessageAsync(string message)
        {
            return SendAsync(new LogMessage(message));
        }

        internal Task SetImageAsync(Image image, string context, SDKTarget target, int? state)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                byte[] imageBytes = memoryStream.ToArray();

                // Convert byte[] to Base64 String
                string base64String = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
                return SetImageAsync(base64String, context, target, state);
            }
        }

        internal Task SetImageAsync(string base64Image, string context, SDKTarget target, int? state)
        {
            return SendAsync(new SetImageMessage(base64Image, context, target, state));
        }

        internal Task ShowAlertAsync(string context)
        {
            return SendAsync(new ShowAlertMessage(context));
        }

        internal Task ShowOkAsync(string context)
        {
            return SendAsync(new ShowOkMessage(context));
        }

        internal Task SetGlobalSettingsAsync(JObject settings)
        {
            return SendAsync(new SetGlobalSettingsMessage(settings, this.UUID));
        }

        internal Task GetGlobalSettingsAsync()
        {
            return SendAsync(new GetGlobalSettingsMessage(this.UUID));
        }

        internal Task SetSettingsAsync(JObject settings, string context)
        {
            return SendAsync(new SetSettingsMessage(settings, context));
        }

        internal Task GetSettingsAsync(string context)
        {
            return SendAsync(new GetSettingsMessage(context));
        }

        internal Task SetStateAsync(uint state, string context)
        {
            return SendAsync(new SetStateMessage(state, context));
        }

        internal Task SendToPropertyInspectorAsync(string action, JObject data, string context)
        {
            return SendAsync(new SendToPropertyInspectorMessage(action, data, context));
        }

        internal Task SwitchToProfileAsync(string device, string profileName, string context)
        {
            return SendAsync(new SwitchToProfileMessage(device, profileName, context));
        }
        internal Task OpenUrlAsync(string uri)
        {
            return OpenUrlAsync(new Uri(uri));
        }

        internal Task OpenUrlAsync(Uri uri)
        {
            return SendAsync(new OpenUrlMessage(uri));
        }

        internal Task SetFeedbackAsync(Dictionary<string,string> dictKeyValues, string context)
        {
            return SendAsync(new SetFeedbackMessage(dictKeyValues, context));
        }

        internal Task SetFeedbackLayoutAsync(string layout, string context)
        {
            return SendAsync(new SetFeedbackLayoutMessage(layout,context));
        }

        #endregion

        #region Private Methods

        private async Task SendAsync(string text)
        {
            try
            {
                if (webSocket != null)
                {
                    try
                    {
                        await sendSocketSemaphore.WaitAsync();
                        byte[] buffer = Encoding.UTF8.GetBytes(text);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancelTokenSource.Token);
                    }
                    finally
                    {
                        sendSocketSemaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.FATAL, $"{this.GetType()} SendAsync Exception: {ex}");
                await DisconnectAsync();
            }

        }

        private async Task RunAsync()
        {
            try
            {
                await webSocket.ConnectAsync(new Uri($"ws://localhost:{this.Port}"), cancelTokenSource.Token);
                if (webSocket.State != WebSocketState.Open)
                {

                    Logger.Instance.LogMessage(TracingLevel.FATAL, $"{this.GetType()} RunAsync failed - Websocket not open {webSocket.State}");
                    await DisconnectAsync();
                    return;
                }

                await SendAsync(new RegisterEventMessage(registerEvent, this.UUID));

                OnConnected?.Invoke(this, new EventArgs());
                await ReceiveAsync();
            }
            finally
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, $"{this.GetType()} RunAsync completed, shutting down");
                await DisconnectAsync();
            }
        }

        private async Task<WebSocketCloseStatus> ReceiveAsync()
        {
            byte[] buffer = new byte[BufferSize];
            ArraySegment<byte> arrayBuffer = new ArraySegment<byte>(buffer);
            StringBuilder textBuffer = new StringBuilder(BufferSize);

            try
            {
                while (!cancelTokenSource.IsCancellationRequested && webSocket != null)
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(arrayBuffer, cancelTokenSource.Token);

                    if (result != null)
                    {
                        if (result.MessageType == WebSocketMessageType.Close ||
                            (result.CloseStatus != null && result.CloseStatus.HasValue && result.CloseStatus.Value != WebSocketCloseStatus.Empty))
                        {
                            return result.CloseStatus.GetValueOrDefault();
                        }
                        else if (result.MessageType == WebSocketMessageType.Text)
                        {
                            textBuffer.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                            if (result.EndOfMessage)
                            {
#if DEBUG
                                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Incoming Message: {textBuffer}");
#endif

                                string strBuffer = textBuffer.ToString();
                                textBuffer.Clear();
                                BaseEvent evt = BaseEvent.Parse(strBuffer);
                                if (evt == null)
                                {
                                    Logger.Instance.LogMessage(TracingLevel.FATAL, $"{this.GetType()} Unknown event received from Stream Deck: {strBuffer}");
                                    continue;
                                }

                                try
                                {
                                    switch (evt.Event)
                                    {
                                        case EventTypes.KeyDown: OnKeyDown?.Invoke(this, new SDEventReceivedEventArgs<KeyDownEvent>(evt as KeyDownEvent)); break;
                                        case EventTypes.KeyUp: OnKeyUp?.Invoke(this, new SDEventReceivedEventArgs<KeyUpEvent>(evt as KeyUpEvent)); break;
                                        case EventTypes.WillAppear: OnWillAppear?.Invoke(this, new SDEventReceivedEventArgs<WillAppearEvent>(evt as WillAppearEvent)); break;
                                        case EventTypes.WillDisappear: OnWillDisappear?.Invoke(this, new SDEventReceivedEventArgs<WillDisappearEvent>(evt as WillDisappearEvent)); break;
                                        case EventTypes.TitleParametersDidChange: OnTitleParametersDidChange?.Invoke(this, new SDEventReceivedEventArgs<TitleParametersDidChangeEvent>(evt as TitleParametersDidChangeEvent)); break;
                                        case EventTypes.DeviceDidConnect: OnDeviceDidConnect?.Invoke(this, new SDEventReceivedEventArgs<DeviceDidConnectEvent>(evt as DeviceDidConnectEvent)); break;
                                        case EventTypes.DeviceDidDisconnect: OnDeviceDidDisconnect?.Invoke(this, new SDEventReceivedEventArgs<DeviceDidDisconnectEvent>(evt as DeviceDidDisconnectEvent)); break;
                                        case EventTypes.ApplicationDidLaunch: OnApplicationDidLaunch?.Invoke(this, new SDEventReceivedEventArgs<ApplicationDidLaunchEvent>(evt as ApplicationDidLaunchEvent)); break;
                                        case EventTypes.ApplicationDidTerminate: OnApplicationDidTerminate?.Invoke(this, new SDEventReceivedEventArgs<ApplicationDidTerminateEvent>(evt as ApplicationDidTerminateEvent)); break;
                                        case EventTypes.SystemDidWakeUp: OnSystemDidWakeUp?.Invoke(this, new SDEventReceivedEventArgs<SystemDidWakeUpEvent>(evt as SystemDidWakeUpEvent)); break;
                                        case EventTypes.DidReceiveSettings: OnDidReceiveSettings?.Invoke(this, new SDEventReceivedEventArgs<DidReceiveSettingsEvent>(evt as DidReceiveSettingsEvent)); break;
                                        case EventTypes.DidReceiveGlobalSettings: OnDidReceiveGlobalSettings?.Invoke(this, new SDEventReceivedEventArgs<DidReceiveGlobalSettingsEvent>(evt as DidReceiveGlobalSettingsEvent)); break;
                                        case EventTypes.PropertyInspectorDidAppear: OnPropertyInspectorDidAppear?.Invoke(this, new SDEventReceivedEventArgs<PropertyInspectorDidAppearEvent>(evt as PropertyInspectorDidAppearEvent)); break;
                                        case EventTypes.PropertyInspectorDidDisappear: OnPropertyInspectorDidDisappear?.Invoke(this, new SDEventReceivedEventArgs<PropertyInspectorDidDisappearEvent>(evt as PropertyInspectorDidDisappearEvent)); break;
                                        case EventTypes.SendToPlugin: OnSendToPlugin?.Invoke(this, new SDEventReceivedEventArgs<SendToPluginEvent>(evt as SendToPluginEvent)); break;
                                        case EventTypes.DialRotate: OnDialRotate?.Invoke(this, new SDEventReceivedEventArgs<DialRotateEvent>(evt as DialRotateEvent)); break;
                                        case EventTypes.DialPress: OnDialPress?.Invoke(this, new SDEventReceivedEventArgs<DialPressEvent>(evt as DialPressEvent)); break;
                                        case EventTypes.TouchpadPress: OnTouchpadPress?.Invoke(this, new SDEventReceivedEventArgs<TouchpadPress>(evt as TouchpadPress)); break;
                                        default:
                                            Logger.Instance.LogMessage(TracingLevel.WARN, $"{this.GetType()} Unsupported Stream Deck event: {strBuffer}");
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"{this.GetType()} Unhandled 3rd party exception when triggering {evt.Event} event. Exception: {ex}");
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.FATAL, $"{this.GetType()} ReceiveAsync Exception: {ex}");
            }

            return WebSocketCloseStatus.NormalClosure;
        }

        private async Task DisconnectAsync()
        {
            if (webSocket != null)
            {
                ClientWebSocket socket = webSocket;
                webSocket = null;

                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", cancelTokenSource.Token);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"{this.GetType()} DisconnectAsync failed to close connection. Exception: {ex}");
                }


                try
                {
                    socket.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"{this.GetType()} DisconnectAsync failed to dispose websocket. Exception: {ex}");
                }

                OnDisconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
