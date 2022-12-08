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

        internal static string ToStringEx(this DialRotatePayload drp)
        {
            if (drp == null)
            {
                return "DialRotatePayload is null!";
            }
            return $"Controller: {drp.Controller} Ticks: {drp.Ticks} Coordinates: ({drp.Coordinates?.Row},{drp.Coordinates?.Column}) Settings: {drp.Settings}";
        }

        internal static string ToStringEx(this DialPressPayload dpp)
        {
            if (dpp == null)
            {
                return "DialPressPayload is null!";
            }
            return $"Controller: {dpp.Controller} IsDialPressed: {dpp.IsDialPressed} Coordinates: ({dpp.Coordinates?.Row},{dpp.Coordinates?.Column}) Settings: {dpp.Settings}";
        }

        internal static string ToStringEx(this TouchpadPressPayload tpp)
        {
            if (tpp == null)
            {
                return "KeyPayload is null!";
            }
            return $"Controller: {tpp.Controller} LongPress: {tpp.IsLongPress} Position: {(tpp.TapPosition?.Length == 2 ? tpp.TapPosition[0].ToString() + "," + tpp.TapPosition[1] : "Invalid")} Coordinates: ({tpp.Coordinates?.Row},{tpp.Coordinates?.Column}) Settings: {tpp.Settings}";
        }
    }
}
