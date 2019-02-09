using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools
{

    /// <summary>
    /// Main abstract class your plugin should derive from
    /// Holds implementation for all the basic functions
    /// If you're missing an event, you can register to it from the Connection.StreamDeckConnection object
    /// </summary>
    public abstract class PluginBase
    {
        /// <summary>
        /// Called when a Stream Deck key is pressed
        /// </summary>
        public abstract void KeyPressed();

        /// <summary>
        /// Called when a Stream Deck key is released
        /// </summary>
        public abstract void KeyReleased();

        /// <summary>
        /// Called when the PropertyInspector has new settings
        /// </summary>
        /// <param name="payload"></param>
        public abstract void UpdateSettings(JObject payload);

        /// <summary>
        /// Called every second
        /// Logic for displaying title/image can go here
        /// </summary>
        public abstract void OnTick();

        /// <summary>
        /// Called when the plugin is disposed
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Connection object which handles your communication with the Stream Deck app
        /// </summary>
        protected SDConnection Connection { get; private set; }

        /// <summary>
        /// Constructor for PluginBase. Receives the communication and plugin settings 
        /// Note that the settings object is not used by the base and should be consumed by the deriving class.
        /// Usually, a private class inside the deriving class is created which stores the settings
        /// Example for settings usage:
        /// * if (settings == null || settings.Count == 0)
        /// * {
        /// *         // Create default settings
        /// * }
        /// * else
        /// * {
        ///     this.settings = settings.ToObject();
        /// * }
        /// 
        /// </summary>
        /// <param name="connection">Communication module with Stream Deck</param>
        /// <param name="settings">Plugin settings - NOTE: Not used in base class, should be consumed by deriving class</param>
        public PluginBase(SDConnection connection, JObject settings)
        {
            Connection = connection;
            /*
            if (settings == null || settings.Count == 0)
            {
                this.settings = PluginableSettings.CreateDefaultSettings();
            }
            else
            {
                this.settings = settings.ToObject<PluginableSettings>();
            }*/

        }
    }
}