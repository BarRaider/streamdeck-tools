using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarRaider.SdTools.Payloads;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools
{

    /// <summary>
    /// Obsolete! Use `KeypadBase` moving forward, or choose one of the other options: `EncoderBase`, `KeyAndEncoderBase`
    /// </summary>
    [Obsolete("PluginBase will be removed in next version. Use either 'KeypadBase' (if you don't support dials), 'EncoderBase' (for only dials), 'KeyAndEncoderBase' (for both keys and dials) instead")]
    public abstract class PluginBase : IKeypadPlugin
    {
        /// <summary>
        /// Called when a Stream Deck key is pressed
        /// </summary>
        public abstract void KeyPressed(KeyPayload payload);

        /// <summary>
        /// Called when a Stream Deck key is released
        /// </summary>
        public abstract void KeyReleased(KeyPayload payload);

        /// <summary>
        /// Called when the PropertyInspector has new settings
        /// </summary>
        /// <param name="payload"></param>
        public abstract void ReceivedSettings(ReceivedSettingsPayload payload);

        /// <summary>
        /// Called when GetGlobalSettings is called.
        /// </summary>
        /// <param name="payload"></param>
        public abstract void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload);

        /// <summary>
        /// Called every second
        /// Logic for displaying title/image can go here
        /// </summary>
        public abstract void OnTick();

        /// <summary>
        /// Abstract method Called when the plugin is disposed
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Main iDisposable Dispose function
        /// </summary>
        public void Destroy()
        {
            Dispose();
            if (Connection != null)
            {
                Connection.Dispose();
            }
        }

        /// <summary>
        /// Connection object which handles your communication with the Stream Deck app
        /// </summary>
        protected ISDConnection Connection { get; private set; }

        /// <summary>
        /// Constructor for PluginBase. Receives the communication and plugin settings 
        /// Note that the settings object is not used by the base and should be consumed by the deriving class.
        /// Usually, a private class inside the deriving class is created which stores the settings
        /// Example for settings usage:
        /// * if (payload.Settings == null || payload.Settings.Count == 0)
        /// * {
        /// *         // Create default settings
        /// * }
        /// * else
        /// * {
        ///     this.settings = payload.Settings.ToObject();
        /// * }
        /// 
        /// </summary>
        /// <param name="connection">Communication module with Stream Deck</param>
        /// <param name="payload">Plugin settings - NOTE: Not used in base class, should be consumed by deriving class</param>
#pragma warning disable IDE0060 // Remove unused parameter
        public PluginBase(ISDConnection connection, InitialPayload payload)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            Connection = connection;
        }
    }
}