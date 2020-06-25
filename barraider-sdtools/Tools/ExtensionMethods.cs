using BarRaider.SdTools.Wrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Extension methods for various objects
    /// </summary>
    public static class ExtensionMethods
    {
        #region Coordinates
        /// <summary>
        /// Checks if too KeyCoordinates match to the same key
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="secondCoordinates"></param>
        /// <returns></returns>
        public static bool IsCoordinatesSame(this KeyCoordinates coordinates, KeyCoordinates secondCoordinates)
        {
            if (secondCoordinates == null)
            {
                return false;
            }

            return coordinates.Row == secondCoordinates.Row && coordinates.Column == secondCoordinates.Column;
        }

        /// <summary>
        /// Converts to a SDTools.KeyCoordinates
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts to a SDTools StreamDeckDeviceInfo object
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static StreamDeckDeviceInfo ToStreamDeckDeviceInfo(this streamdeck_client_csharp.Events.DeviceInfo deviceInfo, string deviceId)
        {
            if (deviceInfo == null)
            {
                return null;
            }

            return new StreamDeckDeviceInfo(new StreamDeckDeviceSize(deviceInfo.Size.Rows, deviceInfo.Size.Columns), (StreamDeckDeviceType) ((int) deviceInfo.Type), deviceId);
        }

        #endregion

        #region Brushes/Colors

        /// <summary>
        /// Converts to an SDTools TitleParameters
        /// </summary>
        /// <param name="titleParameters"></param>
        /// <returns></returns>
        public static Wrappers.TitleParameters ToSDTitleParameters(this streamdeck_client_csharp.Events.TitleParameters titleParameters)
        {
            if (titleParameters == null)
            {
                return null;
            }

            return new Wrappers.TitleParameters(titleParameters.FontFamily, titleParameters.FontSize, titleParameters.FontStyle, titleParameters.FontUnderline, titleParameters.ShowTitle, titleParameters.TitleAlignment, titleParameters.TitleColor);
        }

        /// <summary>
        /// Shows Color In Hex Format
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToHex(this Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        /// <summary>
        /// Shows Color in Hex format
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static string ToHex(this Brush brush)
        {
            if (brush is SolidBrush solidBrush)
            {
                return solidBrush.Color.ToHex();
            }
            return null;
        }

        #endregion

        #region Image/Graphics

        /// <summary>
        /// Converts an Image into a Byte Array
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Draws a string on a Graphics object and returns the ending Y position of the string
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static float DrawAndMeasureString(this Graphics graphics, string text, Font font, Brush brush, PointF position)
        {
            SizeF stringSize = graphics.MeasureString(text, font);
            graphics.DrawString(text, font, brush, position);

            return position.Y + stringSize.Height;
        }

        /// <summary>
        /// Returns the center X position of a string, given the image's max Width and Font information
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="imageWidth"></param>
        /// <param name="font"></param>
        /// <param name="minIndentation"></param>
        /// <returns></returns>
        public static float GetTextCenter(this Graphics graphics, string text, int imageWidth, Font font, int minIndentation = 0)
        {
            SizeF stringSize = graphics.MeasureString(text, font);
            float stringWidth = minIndentation;
            if (stringSize.Width < imageWidth)
            {
                stringWidth = Math.Abs((imageWidth - stringSize.Width)) / 2;
            }
            return stringWidth;
        }

        /// <summary>
        /// Adds a text path to an existing Graphics object. Uses TitleParameters to emulate the Text settings in the Property Inspector
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="titleParameters"></param>
        /// <param name="imageHeight"></param>
        /// <param name="imageWidth"></param>
        /// <param name="text"></param>
        /// <param name="pixelsAlignment"></param>
        public static void AddTextPath(this Graphics graphics, TitleParameters titleParameters, int imageHeight, int imageWidth, string text, int pixelsAlignment = 15)
        {
            AddTextPath(graphics, titleParameters, imageHeight, imageWidth, text, Color.Black, 1, pixelsAlignment);
        }

        /// <summary>
        /// Adds a text path to an existing Graphics object. Uses TitleParameters to emulate the Text settings in the Property Inspector
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="titleParameters"></param>
        /// <param name="imageHeight"></param>
        /// <param name="imageWidth"></param>
        /// <param name="text"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="pixelsAlignment"></param>
        public static void AddTextPath(this Graphics graphics, TitleParameters titleParameters, int imageHeight, int imageWidth, string text, Color strokeColor, float strokeThickness, int pixelsAlignment = 15)
        {
            try
            {
                if (titleParameters == null)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"AddTextPath: titleParameters is null");
                    return;
                }

                Font font = new Font(titleParameters.FontFamily, (float)titleParameters.FontSizeInPixelsScaledToDefaultImage, titleParameters.FontStyle, GraphicsUnit.Pixel);
                Color color = titleParameters.TitleColor;
                graphics.PageUnit = GraphicsUnit.Pixel;
                float ratio = graphics.DpiY / imageWidth;
                SizeF stringSize = graphics.MeasureString(text, font);
                float textWidth = stringSize.Width * (1 - ratio);
                float textHeight = stringSize.Height * (1 - ratio);
                int stringWidth = 0;
                if (textWidth < imageWidth)
                {
                    stringWidth = (int)(Math.Abs((imageWidth - textWidth)) / 2) - pixelsAlignment;
                }

                int stringHeight = pixelsAlignment; // Top
                if (titleParameters.VerticalAlignment == TitleVerticalAlignment.Middle)
                {
                    stringHeight = (imageHeight / 2) - pixelsAlignment;
                }
                else if (titleParameters.VerticalAlignment == TitleVerticalAlignment.Bottom)
                {
                    stringHeight = (int)(Math.Abs((imageHeight - textHeight)) - pixelsAlignment);
                }

                Pen stroke = new Pen(strokeColor, strokeThickness);
                GraphicsPath gpath = new GraphicsPath();
                gpath.AddString(text,
                                    font.FontFamily,
                                    (int)font.Style,
                                    graphics.DpiY * font.SizeInPoints / imageWidth,
                                    new Point(stringWidth, stringHeight),
                                    new StringFormat());
                graphics.DrawPath(stroke, gpath);
                graphics.FillPath(new SolidBrush(color), gpath);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"AddTextPath Exception {ex}");
            }
        }

        #endregion
    }
}
