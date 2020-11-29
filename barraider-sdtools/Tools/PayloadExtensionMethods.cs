using streamdeck_client_csharp.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools
{
    internal static class PayloadExtensionMethods
    {
        internal static string ToStringEx(this ReceiveSettingsPayload rsp)
        {
            if (rsp == null)
            {
                return "ReceiveSettingsPayload is null!";
            }
            return $"IsInMultiAction: {rsp.IsInMultiAction} Coordinates: ({rsp.Coordinates?.Rows},{rsp.Coordinates?.Columns}) Settings: {rsp.Settings}";
        }

        internal static string ToStringEx(this AppearancePayload ap)
        {
            if (ap == null)
            {
                return "AppearancePayload is null!";
            }
            return $"State: {ap.State} IsInMultiAction: {ap.IsInMultiAction} Coordinates: ({ap.Coordinates?.Rows},{ap.Coordinates?.Columns}) Settings: {ap.Settings}";
        }

        internal static string ToStringEx(this streamdeck_client_csharp.Events.KeyPayload kp)
        {
            if (kp == null)
            {
                return "KeyPayload is null!";
            }
            return $"State: {kp.State} IsInMultiAction: {kp.IsInMultiAction} DesiredState: {kp.UserDesiredState} Coordinates: ({kp.Coordinates?.Rows},{kp.Coordinates?.Columns}) Settings: {kp.Settings}";
        }

        internal static string ToStringEx(this ReceiveGlobalSettingsPayload gsp)
        {
            if (gsp == null)
            {
                return "ReceiveGlobalSettingsPayload is null!";
            }
            return $"Settings: {gsp.Settings}";
        }
    }
}
