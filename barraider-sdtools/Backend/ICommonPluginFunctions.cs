using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Common functions used by both keypad and dial plugins
    /// </summary>
    public interface ICommonPluginFunctions : IDisposable
    {
        /// <summary>
        /// Called when the PropertyInspector has new settings
        /// </summary>
        /// <param name="payload"></param>
        void ReceivedSettings(ReceivedSettingsPayload payload);

        /// <summary>
        /// Called when GetGlobalSettings is called.
        /// </summary>
        /// <param name="payload"></param>
        void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload);

        /// <summary>
        /// Called every second
        /// Logic for displaying title/image can go here
        /// </summary>
        void OnTick();

        /// <summary>
        /// Internal function used by StreamDeckTools to prevent memory leaks
        /// </summary>
        void Destroy();
    }
}
