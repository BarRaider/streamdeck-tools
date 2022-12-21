using BarRaider.SdTools.Communication;
using BarRaider.SdTools.Communication.SDEvents;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    class PluginContainer
    {
        private const int STREAMDECK_INITIAL_CONNECTION_TIMEOUT_SECONDS = 60;
        private StreamDeckConnection connection;
        private readonly ManualResetEvent connectEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent disconnectEvent = new ManualResetEvent(false);
        private string pluginUUID = null;
        private StreamDeckInfo deviceInfo;

        private static readonly Dictionary<string, Type> supportedActions = new Dictionary<string, Type>();

        // Holds all instances of plugin
        private static readonly ConcurrentDictionary<string, ICommonPluginFunctions> instances = new ConcurrentDictionary<string, ICommonPluginFunctions>();

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
            connection.OnDialPress += Connection_OnDialPress;
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
        private void Connection_OnKeyDown(object sender, SDEventReceivedEventArgs<KeyDownEvent> e)
        {
                try
                {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin Keydown: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                    if (instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions value))
                    {
                        KeyPayload payload = new KeyPayload(e.Event.Payload.Coordinates,
                                                            e.Event.Payload.Settings, e.Event.Payload.State, e.Event.Payload.UserDesiredState, e.Event.Payload.IsInMultiAction);
                        if (value is IKeypadPlugin plugin)
                        {
                            plugin.KeyPressed(payload);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.ERROR, $"Keydown General Error: Could not convert {e.Event.Context} to IKeypadPlugin");
                        }
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"General Error: Keydown Cache miss for {e.Event.Context}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"Keydown General Exception for {e.Event.Context}: {ex}");
                }
        }

        // Button released
        private void Connection_OnKeyUp(object sender, SDEventReceivedEventArgs<KeyUpEvent> e)
        {
                try
                {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin Keyup: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                    if (instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions value))
                    {
                        KeyPayload payload = new KeyPayload(e.Event.Payload.Coordinates,
                                                            e.Event.Payload.Settings, e.Event.Payload.State, e.Event.Payload.UserDesiredState, e.Event.Payload.IsInMultiAction);
                        if (value is IKeypadPlugin plugin)
                        {
                            plugin.KeyReleased(payload);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.ERROR, $"Keyup General Error: Could not convert {e.Event.Context} to IKeypadPlugin");
                        }
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"General Error: Keyup Cache miss for {e.Event.Context}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"Keyup General Exception for {e.Event.Context}: {ex}");
                }
        }

        // Function runs every second, used to update UI
        private void RunTick()
        {
            try
            {
                foreach (KeyValuePair<string, ICommonPluginFunctions> kvp in instances.ToArray())
                {
                    Task.Run(() => kvp.Value.OnTick());
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"RunTick General Exception: {ex}");
            }
        }

        // Action is loaded in the Stream Deck
        private void Connection_OnWillAppear(object sender, SDEventReceivedEventArgs<WillAppearEvent> e)
        {
            SDConnection conn = new SDConnection(connection, pluginUUID, deviceInfo, e.Event.Action, e.Event.Context, e.Event.Device);
                try
                {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin OnWillAppear: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                    if (supportedActions.ContainsKey(e.Event.Action))
                    {
                        try
                        {
                            if (instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions value) && value != null)
                            {
                                Logger.Instance.LogMessage(TracingLevel.INFO, $"WillAppear called for already existing context {e.Event.Context} (might be inside a multi-action)");
                                return;
                            }
                            InitialPayload payload = new InitialPayload(e.Event.Payload.Coordinates,
                                                                        e.Event.Payload.Settings, e.Event.Payload.State, e.Event.Payload.IsInMultiAction, deviceInfo);
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
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"WillAppear General Exception for {e.Event.Context}: {ex}");
                }
        }

        private void Connection_OnWillDisappear(object sender, SDEventReceivedEventArgs<WillDisappearEvent> e)
        {
                try
                {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin OnWillDisappear: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                    if (instances.TryRemove(e.Event.Context, out ICommonPluginFunctions plugin))
                    {
                        plugin.Destroy();
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"General Error: WillDisappear Cache miss for {e.Event.Context}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"WillDisappear General Exception for {e.Event.Context}: {ex}");
                }
        }

        // Settings updated
        private void Connection_OnDidReceiveSettings(object sender, SDEventReceivedEventArgs<DidReceiveSettingsEvent> e)
        {
                try
                {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin OnDidReceiveSettings: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                    if (instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions value))
                    {
                        value.ReceivedSettings(JObject.FromObject(e.Event.Payload).ToObject<ReceivedSettingsPayload>());
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"General Error: ReceiveSettings Cache miss for {e.Event.Context}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"DidReceiveSettings General Exception for {e.Event.Context}: {ex}");
                }
        }

        // Global settings updated
        private void Connection_OnDidReceiveGlobalSettings(object sender, SDEventReceivedEventArgs<DidReceiveGlobalSettingsEvent> e)
        {
            try
            {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"Plugin OnDidReceiveGlobalSettings: Settings: {e.Event.Payload?.ToStringEx()}");
#endif

                var globalSettings = JObject.FromObject(e.Event.Payload).ToObject<ReceivedGlobalSettingsPayload>();
                foreach (KeyValuePair<string, ICommonPluginFunctions> kvp in instances.ToArray())
                {
                    Task.Run(() => kvp.Value.ReceivedGlobalSettings(globalSettings));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"DidReceiveGlobalSettings General Exception: {ex}");
            }
        }

        private void Connection_OnTouchpadPress(object sender, SDEventReceivedEventArgs<TouchpadPress> e)
        {
                try
                {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"TouchpadPress: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                    if (instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions value))
                    {
                        TouchpadPressPayload payload = new TouchpadPressPayload(e.Event.Payload.Coordinates, e.Event.Payload.Settings, e.Event.Payload.Controller, e.Event.Payload.IsLongPress, e.Event.Payload.TapPosition);
                        if (value is IEncoderPlugin plugin)
                        {
                            plugin.TouchPress(payload);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.ERROR, $"TouchpadPress General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"TouchpadPress General Exception for {e.Event.Context}: {ex}");
                }
        }

        private void Connection_OnDialPress(object sender, SDEventReceivedEventArgs<DialPressEvent> e)
        {
                try
                {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"DialPress: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                    if (instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions value))
                    {
                        DialPressPayload payload = new DialPressPayload(e.Event.Payload.Coordinates, e.Event.Payload.Settings, e.Event.Payload.Controller, e.Event.Payload.IsDialPressed);
                        if (value is IEncoderPlugin plugin)
                        {
                            plugin.DialPress(payload);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.ERROR, $"DialPress General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"DialPress General Exception for {e.Event.Context}: {ex}");
                }
        }

        private void Connection_OnDialRotate(object sender, SDEventReceivedEventArgs<DialRotateEvent> e)
        {

                try
                {
#if DEBUG
                Logger.Instance.LogMessage(TracingLevel.DEBUG, $"DialRotate: Context: {e.Event.Context} Action: {e.Event.Action} Payload: {e.Event.Payload?.ToStringEx()}");
#endif

                    if (instances.TryGetValue(e.Event.Context, out ICommonPluginFunctions value))
                    {
                        DialRotatePayload payload = new DialRotatePayload(e.Event.Payload.Coordinates, e.Event.Payload.Settings, e.Event.Payload.Controller, e.Event.Payload.Ticks, e.Event.Payload.IsDialPressed);
                        if (value is IEncoderPlugin plugin)
                        {
                            plugin.DialRotate(payload);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.ERROR, $"DialRotate General Error: Could not convert {e.Event.Context} to IEncoderPlugin");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"DialRotate General Exception for {e.Event.Context}: {ex}");
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
