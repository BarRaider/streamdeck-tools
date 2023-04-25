using BarRaider.SdTools.Communication;
using BarRaider.SdTools.Communication.SDEvents;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BarRaider.SdTools
{
    class PluginContainer
    {
        private const int STREAMDECK_INITIAL_CONNECTION_TIMEOUT_SECONDS = 60;
        private StreamDeckConnection connection;
        private readonly ManualResetEvent connectEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent disconnectEvent = new ManualResetEvent(false);
        private readonly SemaphoreSlim instancesLock = new SemaphoreSlim(1);
        private string pluginUUID = null;
        private StreamDeckInfo deviceInfo;

        private static readonly Dictionary<string, Type> supportedActions = new Dictionary<string, Type>();

        // Holds all instances of plugin
        private static readonly Dictionary<string, ICommonPluginFunctions> instances = new Dictionary<string, ICommonPluginFunctions>();

        public PluginContainer(PluginActionId[] supportedActionIds)
        {
            foreach (PluginActionId action in supportedActionIds)
            {
                supportedActions[action.ActionId] = action.PluginBaseType;
            }
        }

        public void Run(StreamDeckOptions options)
        {
            pluginUUID = options.PluginUUID;
            deviceInfo = options.DeviceInfo;
            connection = new StreamDeckConnection(options.Port, options.PluginUUID, options.RegisterEvent);

            // Register for events
            connection.OnConnected += Connection_OnConnected;
            connection.OnDisconnected += Connection_OnDisconnected;
            connection.OnKeyDown += Connection_OnKeyDown;
            connection.OnKeyUp += Connection_OnKeyUp;
            connection.OnWillAppear += Connection_OnWillAppear;
            connection.OnWillDisappear += Connection_OnWillDisappear;
            connection.OnDialRotate += Connection_OnDialRotate;
            connection.OnDialDown += Connection_OnDialDown;
            connection.OnDialUp += Connection_OnDialUp;
            connection.OnTouchpadPress += Connection_OnTouchpadPress;

            // Settings changed
            connection.OnDidReceiveSettings += Connection_OnDidReceiveSettings;
            connection.OnDidReceiveGlobalSettings += Connection_OnDidReceiveGlobalSettings;

            // Start the connection
            connection.Run();
            Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin Loaded: UUID: {pluginUUID} Device Info: {deviceInfo}");
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Plugin version: {deviceInfo.Plugin.Version}");
            Logger.Instance.LogMessage(TracingLevel.INFO, "Connecting to Stream Deck...");

            // Time to wait for initial connection
            if (connectEvent.WaitOne(TimeSpan.FromSeconds(STREAMDECK_INITIAL_CONNECTION_TIMEOUT_SECONDS)))
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "Connected to Stream Deck");

                // Initialize GlobalSettings manager
                GlobalSettingsManager.Instance.Initialize(connection);

                // We connected, loop every second until we disconnect
                while (!disconnectEvent.WaitOne(TimeSpan.FromMilliseconds(1000)))
                {
                    RunTick();
                }
            }
            Logger.Instance.LogMessage(TracingLevel.INFO, "Plugin Disconnected - Exiting");
        }

        // Button pressed
        private async void Connection_OnKeyDown(object sender, SDEventReceivedEventArgs<KeyDownEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin Keydown: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                if (instances.ContainsKey(e.Event.Context))
                {
                    KeyPayload payload = new KeyPayload(e.Event.Payload.Coordinates,
                                                        e.Event.Payload.Settings, e.Event.Payload.State, e.Event.Payload.UserDesiredState, e.Event.Payload.IsInMultiAction);
                    if (instances[e.Event.Context] is IKeypadPlugin plugin)
                    {
                        plugin.KeyPressed(payload);
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"Keydown General Error: Could not convert {e.Event.Context} to IKeypadPlugin");
                    }
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Button released
        private async void Connection_OnKeyUp(object sender, SDEventReceivedEventArgs<KeyUpEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin Keyup: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                if (instances.ContainsKey(e.Event.Context))
                {
                    KeyPayload payload = new KeyPayload(e.Event.Payload.Coordinates,
                                                        e.Event.Payload.Settings, e.Event.Payload.State, e.Event.Payload.UserDesiredState, e.Event.Payload.IsInMultiAction);
                    if (instances[e.Event.Context] is IKeypadPlugin plugin)
                    {
                        plugin.KeyReleased(payload);
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"Keyup General Error: Could not convert {e.Event.Context} to IKeypadPlugin");
                    }
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Function runs every second, used to update UI
        private async void RunTick()
        {
            await instancesLock.WaitAsync();
            try
            {
                foreach (KeyValuePair<string, ICommonPluginFunctions> kvp in instances.ToArray())
                {
                    kvp.Value.OnTick();
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Action is loaded in the Stream Deck
        private async void Connection_OnWillAppear(object sender, SDEventReceivedEventArgs<WillAppearEvent> e)
        {
            SDConnection conn = new SDConnection(connection, pluginUUID, deviceInfo, e.Event.Action, e.Event.Context, e.Event.Device);
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin OnWillAppear: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                if (supportedActions.ContainsKey(e.Event.Action))
                {
                    try
                    {
                        if (instances.ContainsKey(e.Event.Context) && instances[e.Event.Context] != null)
                        {
                            Logger.Instance.LogMessage(TracingLevel.INFO, $"WillAppear called for already existing context {e.Event.Context} (might be inside a multi-action)");
                            return;
                        }
                        InitialPayload payload = new InitialPayload(e.Event.Payload, deviceInfo);
                        instances[e.Event.Context] = (ICommonPluginFunctions)Activator.CreateInstance(supportedActions[e.Event.Action], conn, payload);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogMessage(TracingLevel.FATAL, $"Could not create instance of {supportedActions[e.Event.Action]} with context {e.Event.Context} - This may be due to an Exception raised in the constructor, or the class does not inherit PluginBase with the same constructor {ex}");
                    }
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"No plugin found that matches action: {e.Event.Action}");
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        private async void Connection_OnWillDisappear(object sender, SDEventReceivedEventArgs<WillDisappearEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin OnWillDisappear: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                if (instances.ContainsKey(e.Event.Context))
                {
                    instances[e.Event.Context].Destroy();
                    instances.Remove(e.Event.Context);
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Settings updated
        private async void Connection_OnDidReceiveSettings(object sender, SDEventReceivedEventArgs<DidReceiveSettingsEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin OnDidReceiveSettings: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                if (instances.ContainsKey(e.Event.Context))
                {
                    instances[e.Event.Context].ReceivedSettings(JObject.FromObject(e.Event.Payload).ToObject<ReceivedSettingsPayload>());
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Global settings updated
        private async void Connection_OnDidReceiveGlobalSettings(object sender, SDEventReceivedEventArgs<DidReceiveGlobalSettingsEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin OnDidReceiveGlobalSettings: Settings: {e.Event.Payload?.ToStringEx()}");
#endif

                var globalSettings = JObject.FromObject(e.Event.Payload).ToObject<ReceivedGlobalSettingsPayload>();
                foreach (string key in instances.Keys)
                {
                    instances[key].ReceivedGlobalSettings(globalSettings);
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        private async void Connection_OnTouchpadPress(object sender, SDEventReceivedEventArgs<TouchpadPressEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"TouchpadPress: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                if (instances.ContainsKey(e.Event.Context))
                {
                    TouchpadPressPayload payload = new TouchpadPressPayload(e.Event.Payload.Coordinates,e.Event.Payload.Settings, e.Event.Payload.Controller, e.Event.Payload.IsLongPress, e.Event.Payload.TapPosition);
                    if (instances[e.Event.Context] is IEncoderPlugin plugin)
                    {
                        plugin.TouchPress(payload);
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"TouchpadPress General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                    }
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Dial Up

        private async void Connection_OnDialUp(object sender, SDEventReceivedEventArgs<DialUpEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"DialPress: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                if (instances.ContainsKey(e.Event.Context))
                {
                    DialPayload payload = new DialPayload(e.Event.Payload.Coordinates, e.Event.Payload.Settings, e.Event.Payload.Controller);
                    if (instances[e.Event.Context] is IEncoderPlugin plugin)
                    {
                        plugin.DialUp(payload);
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"DialDown General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                    }
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Dial Down
        private async void Connection_OnDialDown(object sender, SDEventReceivedEventArgs<DialDownEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"DialPress: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                if (instances.ContainsKey(e.Event.Context))
                {
                    DialPayload payload = new DialPayload(e.Event.Payload.Coordinates, e.Event.Payload.Settings, e.Event.Payload.Controller);
                    if (instances[e.Event.Context] is IEncoderPlugin plugin)
                    {
                        plugin.DialDown(payload);
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"DialDown General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                    }
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        private async void Connection_OnDialRotate(object sender, SDEventReceivedEventArgs<DialRotateEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"DialRotate: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                if (instances.ContainsKey(e.Event.Context))
                {
                    DialRotatePayload payload = new DialRotatePayload(e.Event.Payload.Coordinates, e.Event.Payload.Settings, e.Event.Payload.Controller, e.Event.Payload.Ticks, e.Event.Payload.IsDialPressed);
                    if (instances[e.Event.Context] is IEncoderPlugin plugin)
                    {
                        plugin.DialRotate(payload);
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"DialRotate General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                    }
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }



        private void Connection_OnConnected(object sender, EventArgs e)
        {
            connectEvent.Set();
        }

        private void Connection_OnDisconnected(object sender, EventArgs e)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Disconnect event received");
            disconnectEvent.Set();
        }
    }
}
