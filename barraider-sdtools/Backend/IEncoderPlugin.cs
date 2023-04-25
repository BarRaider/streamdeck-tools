using BarRaider.SdTools.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Interface used to capture dial/encoder events
    /// </summary>
    public interface IEncoderPlugin : ICommonPluginFunctions
    {
        /// <summary>
        /// Called when the dial is rotated
        /// </summary>
        void DialRotate(DialRotatePayload payload);

        /// <summary>
        /// Called when the Dial is pressed
        /// </summary>
        void DialDown(DialPayload payload);

        /// <summary>
        /// Called when the Dial is released
        /// </summary>
        void DialUp(DialPayload payload);

        /// <summary>
        /// Called when the touchpad (above the dials) is pressed
        /// </summary>
        void TouchPress(TouchpadPressPayload payload);
    }
}
