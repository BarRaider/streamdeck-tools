using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Interface used to capture key events
    /// </summary>
    public interface IKeypadPlugin : ICommonPluginFunctions
    {
        void KeyPressed(KeyPayload payload);

        /// <summary>
        /// Called when a Stream Deck key is released
        /// </summary>
        void KeyReleased(KeyPayload payload);
    }
}
