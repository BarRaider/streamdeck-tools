using BarRaider.SdTools.Communication.SDEvents;
using BarRaider.SdTools.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools
{
    internal static class PayloadExtensionMethods
    {
        internal static string ToStringEx(this ReceivedSettingsPayload rsp)
        {
            if (rsp == null)
            {
                return "ReceiveSettingsPayload is null!";
            }
            return $"IsInMultiAction: {rsp.IsInMultiAction} Coordinates: ({rsp.Coordinates?.Row},{rsp.Coordinates?.Column}) Settings: {rsp.Settings}";
        }

        internal static string ToStringEx(this AppearancePayload ap)
        {
            if (ap == null)
            {
                return "AppearancePayload is null!";
            }
            return $"State: {ap.State} IsInMultiAction: {ap.IsInMultiAction} Coordinates: ({ap.Coordinates?.Row},{ap.Coordinates?.Column}) Settings: {ap.Settings}";
        }

        internal static string ToStringEx(this KeyPayload kp)
        {
            if (kp == null)
            {
                return "KeyPayload is null!";
            }
            return $"State: {kp.State} IsInMultiAction: {kp.IsInMultiAction} DesiredState: {kp.UserDesiredState} Coordinates: ({kp.Coordinates?.Row},{kp.Coordinates?.Column}) Settings: {kp.Settings}";
        }

        internal static string ToStringEx(this ReceivedGlobalSettingsPayload gsp)
        {
            if (gsp == null)
            {
                return "ReceiveGlobalSettingsPayload is null!";
            }
            return $"Settings: {gsp.Settings}";
        }
    }
}
