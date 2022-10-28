using BarRaider.SdTools.Communication;
using BarRaider.SdTools.Communication.SDEvents;
using BarRaider.SdTools.Payloads;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json.Linq;
using System;
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

        #region Private Members

        private const int GET_GLOBAL_SETTINGS_DELAY_MS = 300;

        private Communication.StreamDeckConnection connection;
        private readonly System.Timers.Timer tmrGetGlobalSettings = new System.Timers.Timer();

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
            tmrGetGlobalSettings.Interval = GET_GLOBAL_SETTINGS_DELAY_MS;
            tmrGetGlobalSettings.Elapsed += TmrGetGlobalSettings_Elapsed;
            tmrGetGlobalSettings.AutoReset = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Event triggered when Global Settings are received
        /// </summary>
        public event EventHandler<ReceivedGlobalSettingsPayload> OnReceivedGlobalSettings;


        internal void Initialize(StreamDeckConnection connection, int getGlobalSettingsDelayMs = GET_GLOBAL_SETTINGS_DELAY_MS)
        {
            this.connection = connection;
            this.connection.OnDidReceiveGlobalSettings += Connection_OnDidReceiveGlobalSettings;

            tmrGetGlobalSettings.Stop();
            tmrGetGlobalSettings.Interval = getGlobalSettingsDelayMs;
            Logger.Instance.LogMessage(TracingLevel.INFO, "GlobalSettingsManager initialized");
        }

        /// <summary>
        /// Command to request the Global Settings. Use the OnDidReceiveGlobalSSettings callback function to receive the Global Settings.
        /// </summary>
        /// <returns></returns>
        public void RequestGlobalSettings()
        {
            if (connection == null)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "GlobalSettingsManager::RequestGlobalSettings called while connection is null");
                return;
            }

            Logger.Instance.LogMessage(TracingLevel.INFO, "GlobalSettingsManager::RequestGlobalSettings called");
            tmrGetGlobalSettings.Start();
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
                tmrGetGlobalSettings.Start();
            }
        }


        #endregion

        #region Private Methods

        private void Connection_OnDidReceiveGlobalSettings(object sender, SDEventReceivedEventArgs<DidReceiveGlobalSettingsEvent> e)
        {
            OnReceivedGlobalSettings?.Invoke(this, JObject.FromObject(e.Event.Payload).ToObject<ReceivedGlobalSettingsPayload>());
        }

        private async void TmrGetGlobalSettings_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmrGetGlobalSettings.Stop();

            Logger.Instance.LogMessage(TracingLevel.INFO, "GlobalSettingsManager::GetGlobalSettingsAsync triggered");
            await connection.GetGlobalSettingsAsync();
        }

        #endregion
    }
}
