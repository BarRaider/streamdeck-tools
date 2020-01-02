using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BarRaider.SdTools.Wrappers
{
    public enum TitleVerticalAlignment
    {
        Top,
        Middle,
        Bottom
    }

    public class TitleParameters
    {
        #region Private Members
        private const double POINTS_TO_PIXEL_CONVERT = 1.3;
        private const int DEFAULT_IMAGE_SIZE_FONT_SCALE = 3;
        #endregion


        [JsonProperty("titleColor")]
        public Color TitleColor { get; private set; } = Color.White;

        [JsonProperty("fontSize")]
        public double FontSizeInPoints { get; private set; } = 10;

        [JsonIgnore]
        public double FontSizeInPixels => Math.Round(FontSizeInPoints * POINTS_TO_PIXEL_CONVERT);

        [JsonIgnore]
        public double FontSizeInPixelsScaledToDefaultImage => Math.Round(FontSizeInPixels * DEFAULT_IMAGE_SIZE_FONT_SCALE);
        
        [JsonProperty("fontFamily")]
        public FontFamily FontFamily { get; private set; } = new FontFamily("Verdana");

        [JsonProperty("fontStyle")]
        public FontStyle FontStyle { get; private set; } = FontStyle.Bold;

        [JsonProperty("showTitle")]
        public bool ShowTitle { get; private set; }

        [JsonProperty("titleAlignment")]
        public TitleVerticalAlignment VerticalAlignment { get; private set; }

        public TitleParameters(FontFamily fontFamily, FontStyle fontStyle, double fontSize, Color titleColor, bool showTitle, TitleVerticalAlignment verticalAlignment)
        {
            FontFamily = fontFamily;
            FontStyle = fontStyle;
            FontSizeInPoints = fontSize;
            TitleColor = titleColor;
            ShowTitle = showTitle;
            VerticalAlignment = verticalAlignment;
        }

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
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"TitleParser failed to parse payload {ex}");
            }
        }
    }
}
