using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BarRaider.SdTools.Wrappers
{
    /// <summary>
    /// Enum for the alignment of the Title text on the key
    /// </summary>
    public enum TitleVerticalAlignment
    {
        /// <summary>
        /// Top Alignment
        /// </summary>
        Top,

        /// <summary>
        /// Middle/Center Alignment
        /// </summary>
        Middle,

        /// <summary>
        /// Bottom Alignment
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Class holding all the Title Information set by a user in the Property Inspector
    /// </summary>
    public class TitleParameters
    {
        #region Private Members
        private const double POINTS_TO_PIXEL_CONVERT = 1.3;
        private const int DEFAULT_IMAGE_SIZE_FONT_SCALE = 3;
        private const string DEFAULT_FONT_FAMILY_NAME = "Verdana";
        #endregion

        /// <summary>
        /// Title Color
        /// </summary>
        [JsonProperty("titleColor")]
        public Color TitleColor { get; private set; } = Color.White;

        /// <summary>
        /// Font Size in Points
        /// </summary>
        [JsonProperty("fontSize")]
        public double FontSizeInPoints { get; private set; } = 10;

        /// <summary>
        /// Font Size in Pixels
        /// </summary>
        [JsonIgnore]
        public double FontSizeInPixels => Math.Round(FontSizeInPoints * POINTS_TO_PIXEL_CONVERT);

        /// <summary>
        /// Font Size Scaled to Image
        /// </summary>
        [JsonIgnore]
        public double FontSizeInPixelsScaledToDefaultImage => Math.Round(FontSizeInPixels * DEFAULT_IMAGE_SIZE_FONT_SCALE);

        /// <summary>
        /// Font Family
        /// </summary>
        [JsonProperty("fontFamily")]
        public FontFamily FontFamily { get; private set; } = new FontFamily(DEFAULT_FONT_FAMILY_NAME);

        /// <summary>
        /// Font Style
        /// </summary>
        [JsonProperty("fontStyle")]
        public FontStyle FontStyle { get; private set; } = FontStyle.Bold;

        /// <summary>
        /// Should Title be shown
        /// </summary>
        [JsonProperty("showTitle")]
        public bool ShowTitle { get; private set; }

        /// <summary>
        /// Alignment position of the Title text on the key
        /// </summary>
        [JsonProperty("titleAlignment")]
        public TitleVerticalAlignment VerticalAlignment { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fontFamily"></param>
        /// <param name="fontStyle"></param>
        /// <param name="fontSize"></param>
        /// <param name="titleColor"></param>
        /// <param name="showTitle"></param>
        /// <param name="verticalAlignment"></param>
        public TitleParameters(FontFamily fontFamily, FontStyle fontStyle, double fontSize, Color titleColor, bool showTitle, TitleVerticalAlignment verticalAlignment)
        {
            FontFamily = fontFamily;
            FontStyle = fontStyle;
            FontSizeInPoints = fontSize;
            TitleColor = titleColor;
            ShowTitle = showTitle;
            VerticalAlignment = verticalAlignment;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fontFamily"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontStyle"></param>
        /// <param name="fontUnderline"></param>
        /// <param name="showTitle"></param>
        /// <param name="titleAlignment"></param>
        /// <param name="titleColor"></param>
        public TitleParameters(string fontFamily, uint fontSize, string fontStyle, bool fontUnderline, bool showTitle, string titleAlignment, string titleColor)
        {
            ParsePayload(fontFamily, fontSize, fontStyle, fontUnderline, showTitle, titleAlignment, titleColor);
        }


        private void ParsePayload(string fontFamily, uint fontSize, string fontStyle, bool fontUnderline, bool showTitle, string titleAlignment, string titleColor)
        {
            try
            {
                ShowTitle = showTitle;

                // Color
                if (!String.IsNullOrEmpty(titleColor))
                {
                    TitleColor = ColorTranslator.FromHtml(titleColor);
                }

                if (!String.IsNullOrEmpty(fontFamily))
                {
                    FontFamily = new FontFamily(fontFamily);
                }

                FontSizeInPoints = fontSize;
                if (!String.IsNullOrEmpty(fontStyle))
                {
                    switch (fontStyle.ToLowerInvariant())
                    {
                        case "regular":
                            FontStyle = FontStyle.Regular;
                            break;
                        case "bold":
                            FontStyle = FontStyle.Bold;
                            break;
                        case "italic":
                            FontStyle = FontStyle.Italic;
                            break;
                        case "bold italic":
                            FontStyle = FontStyle.Bold | FontStyle.Italic;
                            break;
                        default:
                            Logger.Instance.LogMessage(TracingLevel.WARN, $"{this.GetType()} Cannot parse Font Style: {fontStyle}");
                            break;
                    }
                }
                if (fontUnderline)
                {
                    FontStyle |= FontStyle.Underline;
                }

                if (!string.IsNullOrEmpty(titleAlignment))
                {
                    switch (titleAlignment.ToLowerInvariant())
                    {
                        case "top":
                            VerticalAlignment = TitleVerticalAlignment.Top;
                            break;
                        case "bottom":
                            VerticalAlignment = TitleVerticalAlignment.Bottom;
                            break;
                        case "middle":
                            VerticalAlignment = TitleVerticalAlignment.Middle;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"TitleParameters failed to parse payload {ex}");
            }
        }
    }
}
