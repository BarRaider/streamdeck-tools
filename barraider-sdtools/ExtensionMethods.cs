using streamdeck_client_csharp.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BarRaider.SdTools
{
    public static class ExtensionMethods
    {
        #region Coordinates
        public static bool IsCoordinatesSame(this KeyCoordinates coordinates, KeyCoordinates secondCoordinates)
        {
            if (secondCoordinates == null)
            {
                return false;
            }

            return coordinates.Row == secondCoordinates.Row && coordinates.Column == secondCoordinates.Column;
        }

        public static KeyCoordinates ToKeyCoordinates(this streamdeck_client_csharp.Events.Coordinates coordinates)
        {
            if (coordinates == null)
            {
                return null;
            }

            return new KeyCoordinates() { Column = coordinates.Columns, Row = coordinates.Rows };
        }

        #endregion

        #region Devices

        public static StreamDeckDeviceInfo ToStreamDeckDeviceInfo(this DeviceInfo deviceInfo, string deviceId)
        {
            if (deviceInfo == null)
            {
                return null;
            }

            return new StreamDeckDeviceInfo(new StreamDeckDeviceSize(deviceInfo.Size.Rows, deviceInfo.Size.Columns), (StreamDeckDeviceType) ((int) deviceInfo.Type), deviceId);
        }

        #endregion

        #region Brushes/Colors

        public static Wrappers.TitleParameters ToSDTitleParameters(this streamdeck_client_csharp.Events.TitleParameters titleParameters)
        {
            if (titleParameters == null)
            {
                return null;
            }

            return new Wrappers.TitleParameters(titleParameters.FontFamily, titleParameters.FontSize, titleParameters.FontStyle, titleParameters.FontUnderline, titleParameters.ShowTitle, titleParameters.TitleAlignment, titleParameters.TitleColor);
        }

        public static string ToHex(this Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public static string ToHex(this Brush brush)
        {
            if (brush is SolidBrush solidBrush)
            {
                return solidBrush.Color.ToHex();
            }
            return null;
        }

        #endregion
    }
}
