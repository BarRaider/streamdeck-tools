using Newtonsoft.Json.Linq;
using streamdeck_client_csharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Helper class which allows fetching the GlobalSettings of a plugin
    /// </summary>
    public class GlobalSettingsManager
    {
        #region Private Static Members
        private static GlobalSettingsManager instance = null;
        private static readonly object objLock = new object();
        #endregion

        #region Constructor

        /// <summary>
        /// Returns singelton entry of GlobalSettingsManager
        /// </summary>
        public static GlobalSettingsManager Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new GlobalSettingsManager();
                    }
                    return instance;
                }
            }
        }

        private GlobalSettingsManager()
        {
        }

        #endregion

        #region Private Members

        StreamDeckConnection connection;

        #endregion

        #region Public Methods

        /// <summary>
        /// Event triggered when Global Settings are received
        /// </summary>
        public event EventHandler<ReceivedGlobalSettingsPayload> OnReceivedGlobalSettings;


        internal void Initialize(StreamDeckConnection connection)
        {
            this.connection = connection;
            this.connection.OnDidReceiveGlobalSettings += Connection_OnDidReceiveGlobalSettings;
            Logger.Instance.LogMessage(TracingLevel.INFO, "GlobalSettingsManager initialized");
        }

        /// <summary>
        /// Command to request the Global Settings. Use the OnDidReceiveGlobalSSettings callback function to receive the Global Settings.
        /// </summary>
        /// <returns></returns>
        public Task RequestGlobalSettings()
        {
            if (connection == null)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "GlobalSettingsManager::RequestGlobalSettings called while connection is null");
                return null;
            }

            Logger.Instance.LogMessage(TracingLevel.INFO, "GlobalSettingsManager::RequestGlobalSettings called");
            return connection.GetGlobalSettingsAsync();
        }

        /// <summary>
        /// Sets the Global Settings for the plugin
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="triggerDidReceiveGlobalSettings"></param>
        /// <returns></returns>
        public async Task SetGlobalSettings(JObject settings, bool triggerDidReceiveGlobalSettings = true)
        {
            if (connection == null)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "GlobalSettingsManager::SetGlobalSettings called while connection is null");
                return;
            }

            Logger.Instance.LogMessage(TracingLevel.INFO, "GlobalSettingsManager::SetGlobalSettings called");
            await connection.SetGlobalSettingsAsync(settings);

            if (triggerDidReceiveGlobalSettings)
            {
                await connection.GetGlobalSettingsAsync();
            }
        }


        #endregion

        #region Private Methods

        private void Connection_OnDidReceiveGlobalSettings(object sender, StreamDeckEventReceivedEventArgs<streamdeck_client_csharp.Events.DidReceiveGlobalSettingsEvent> e)
        {
            OnReceivedGlobalSettings?.Invoke(this, JObject.FromObject(e.Event.Payload).ToObject<ReceivedGlobalSettingsPayload>());
        }

        #endregion
    }
}
