using BarRaider.SdTools;
using streamdeck_client_csharp;
using streamdeck_client_csharp.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BarRaider.SdTools.Samples
{
    class PluginContainer
    {
        private StreamDeckConnection connection;
        private ManualResetEvent connectEvent = new ManualResetEvent(false);
        private ManualResetEvent disconnectEvent = new ManualResetEvent(false);
        private SemaphoreSlim instancesLock = new SemaphoreSlim(1);

        // Holds all instances of plugin
        private static Dictionary<string, IPluginable> instances = new Dictionary<string, IPluginable>();


        public void Run(StreamDeckOptions options)
        {
            connection = new StreamDeckConnection(options.Port, options.PluginUUID, options.RegisterEvent);

            // Register for events
            connection.OnConnected += Connection_OnConnected;
            connection.OnDisconnected += Connection_OnDisconnected;
            connection.OnKeyDown += Connection_OnKeyDown;
            connection.OnKeyUp += Connection_OnKeyUp;
            connection.OnWillAppear += Connection_OnWillAppear;
            connection.OnWillDisappear += Connection_OnWillDisappear;

            // Settings changed
            connection.OnSendToPlugin += Connection_OnSendToPlugin;

            // Start the connection
            connection.Run();

            // Wait for up to 10 seconds to connect
            if (connectEvent.WaitOne(TimeSpan.FromSeconds(10)))
            {
                // We connected, loop every second until we disconnect
                while (!disconnectEvent.WaitOne(TimeSpan.FromMilliseconds(1000)))
                {
                    RunTick();
                }
            }
        }

        // Button pressed
        private async void Connection_OnKeyDown(object sender, StreamDeckEventReceivedEventArgs<KeyDownEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
                if (instances.ContainsKey(e.Event.Context))
                {
                    instances[e.Event.Context].KeyPressed();
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Button released
        private async void Connection_OnKeyUp(object sender, StreamDeckEventReceivedEventArgs<KeyUpEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
                if (instances.ContainsKey(e.Event.Context))
                {
                    instances[e.Event.Context].KeyReleased();
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
                foreach (KeyValuePair<string, IPluginable> kvp in instances.ToArray())
                {
                    kvp.Value.OnTick();
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Stopwatch instance created
        private async void Connection_OnWillAppear(object sender, StreamDeckEventReceivedEventArgs<WillAppearEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
                instances[e.Event.Context] = new YOUR_CLASS_THAT_IMPLEMENTS_IPluginable(connection, e.Event.Action, e.Event.Context, e.Event.Payload.Settings);
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Stopwatch instance no longer shown
        private async void Connection_OnWillDisappear(object sender, StreamDeckEventReceivedEventArgs<WillDisappearEvent> e)
        {
            await instancesLock.WaitAsync();
            try
            {
                if (instances.ContainsKey(e.Event.Context))
                {
                    instances.Remove(e.Event.Context);
                }
            }
            finally
            {
                instancesLock.Release();
            }
        }

        // Settings updated
        private async void Connection_OnSendToPlugin(object sender, StreamDeckEventReceivedEventArgs<SendToPluginEvent> e)
        {

            await instancesLock.WaitAsync();
            try
            {
                if (instances.ContainsKey(e.Event.Context))
                {
                    instances[e.Event.Context].UpdateSettings(e.Event.Payload);
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
            disconnectEvent.Set();
        }
    }
}
