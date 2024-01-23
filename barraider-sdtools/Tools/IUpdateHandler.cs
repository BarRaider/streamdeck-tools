using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Interface for a plugin update handler
    /// </summary>
    public interface IUpdateHandler : IDisposable
    {
        #region Events

        /// <summary>
        /// Event received by the infrastructure after an upgrade check
        /// </summary>
        event EventHandler<PluginUpdateInfo> OnUpdateStatusChanged;

        #endregion

        #region Properties
        /// <summary>
        /// Is the current update a blocking one
        /// </summary>
        bool IsBlockingUpdate { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the information about the plugin
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="pluginVersion"></param>
        void SetPluginConfiguration(string pluginName, string pluginVersion);

        /// <summary>
        /// Sets the plugin global settings
        /// </summary>
        /// <param name="settings"></param>
        void SetGlobalSettings(object settings);

        /// <summary>
        /// Checks for update
        /// </summary>
        void CheckForUpdate();

        #endregion

    }
}
