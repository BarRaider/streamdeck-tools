using BarRaider.SdTools.Internal;
using BarRaider.SdTools.Wrappers;
using SkiaSharp;
using System;

namespace BarRaider.SdTools
{
    /// <summary>
    /// SkiaSharp extension methods for SKBitmap and SKCanvas.
    /// These are the cross-platform equivalents of the System.Drawing extension methods in <see cref="ExtensionMethods"/>.
    /// </summary>
    public static class SkiaExtensionMethods
    {
        private static readonly Lazy<SkiaSharpImageCodec> codec =
            new Lazy<SkiaSharpImageCodec>(() => new SkiaSharpImageCodec());

        #region SKBitmap Extensions

        /// <summary>
        /// Converts an SKBitmap to a PNG byte array.
        /// </summary>
        /// <param name="bitmap">The bitmap to encode.</param>
        /// <returns>PNG-encoded byte array.</returns>
        public static byte[] ToPngByteArray(this SKBitmap bitmap)
        {
            return codec.Value.EncodeToPngBytes(bitmap);
        }

        /// <summary>
        /// Converts an SKBitmap to a Base64 PNG string.
        /// </summary>
        /// <param name="bitmap">The bitmap to encode.</param>
        /// <param name="addHeaderPrefix">Whether to prepend the data URI header.</param>
        /// <returns>Base64-encoded PNG string.</returns>
        public static string ToBase64(this SKBitmap bitmap, bool addHeaderPrefix)
        {
            return SkiaTools.ImageToBase64(bitmap, addHeaderPrefix);
        }

        #endregion

        #region SKCanvas Extensions

        /// <summary>
        /// Draws a line of text treating the Y coordinate as the top of the text box
        /// (matching System.Drawing.Graphics.DrawString behavior) and returns the Y
        /// position for the next line, based on the font's recommended line spacing.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="font">The font for text rendering.</param>
        /// <param name="paint">The paint for color/style.</param>
        /// <param name="position">The position to draw at (X = left edge, Y = top of text).</param>
        /// <returns>The Y coordinate for the top of the next line.</returns>
        public static float DrawTextLine(this SKCanvas canvas, string text, SKFont font, SKPaint paint, SKPoint position)
        {
            float baseline = position.Y + Math.Abs(font.Metrics.Ascent);
            canvas.DrawText(text, position.X, baseline, font, paint);
            return position.Y + font.Spacing;
        }

        /// <summary>
        /// Draws a string on an SKCanvas and returns the Y position for the next line.
        /// Equivalent to <see cref="DrawTextLine"/>. Position Y is the top of the text, not the baseline.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="font">The font for text rendering.</param>
        /// <param name="paint">The paint for color/style.</param>
        /// <param name="position">The position to draw at (X = left edge, Y = top of text).</param>
        /// <returns>The Y coordinate for the top of the next line.</returns>
        public static float DrawAndMeasureString(this SKCanvas canvas, string text, SKFont font, SKPaint paint, SKPoint position)
        {
            return canvas.DrawTextLine(text, font, paint, position);
        }

        /// <summary>
        /// Returns the center X position of a string given the image width and font.
        /// </summary>
        /// <param name="canvas">The canvas (used for consistency with the System.Drawing extension signature).</param>
        /// <param name="text">The text to measure.</param>
        /// <param name="imageWidth">The total image width.</param>
        /// <param name="font">The font for measurement.</param>
        /// <param name="textFitsImage">Set to true if the text fits within the image width.</param>
        /// <param name="minIndentation">Minimum indentation when text overflows.</param>
        /// <returns>The X coordinate to center the text.</returns>
        public static float GetTextCenter(this SKCanvas canvas, string text, int imageWidth, SKFont font, out bool textFitsImage, int minIndentation = 0)
        {
            float textWidth = font.MeasureText(text);
            float stringWidth = minIndentation;
            textFitsImage = false;
            if (textWidth < imageWidth)
            {
                textFitsImage = true;
                stringWidth = Math.Abs(imageWidth - textWidth) / 2;
            }
            return stringWidth;
        }

        /// <summary>
        /// Returns the center X position of a string given the image width and font.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="text">The text to measure.</param>
        /// <param name="imageWidth">The total image width.</param>
        /// <param name="font">The font for measurement.</param>
        /// <param name="minIndentation">Minimum indentation when text overflows.</param>
        /// <returns>The X coordinate to center the text.</returns>
        public static float GetTextCenter(this SKCanvas canvas, string text, int imageWidth, SKFont font, int minIndentation = 0)
        {
            return canvas.GetTextCenter(text, imageWidth, font, out _, minIndentation);
        }

        /// <summary>
        /// Returns the largest font size at which the text fits within the given image width.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="text">The text to measure.</param>
        /// <param name="imageWidth">Maximum width in pixels.</param>
        /// <param name="font">The base font (used for typeface and initial size).</param>
        /// <param name="minimalFontSize">The smallest acceptable font size.</param>
        /// <returns>The computed font size in the same units as font.Size.</returns>
        public static float GetFontSizeWhereTextFitsImage(this SKCanvas canvas, string text, int imageWidth, SKFont font, int minimalFontSize = 6)
        {
            bool textFitsImage;
            float size = font.Size;
            using (var variableFont = new SKFont(font.Typeface, size))
            {
                do
                {
                    canvas.GetTextCenter(text, imageWidth, variableFont, out textFitsImage);
                    if (!textFitsImage)
                    {
                        size -= 0.5f;
                        variableFont.Size = size;
                    }
                }
                while (!textFitsImage && size > minimalFontSize);
            }
            return size;
        }

        /// <summary>
        /// Adds styled text to an SKCanvas using TitleParameters for layout.
        /// Emulates the text rendering settings from the Stream Deck Property Inspector.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="titleParameters">Title parameters from the Stream Deck event.</param>
        /// <param name="imageHeight">Height of the key image.</param>
        /// <param name="imageWidth">Width of the key image.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="pixelsAlignment">Alignment offset in pixels.</param>
        public static void AddTextPath(this SKCanvas canvas, TitleParameters titleParameters, int imageHeight, int imageWidth, string text, int pixelsAlignment = 15)
        {
            AddTextPath(canvas, titleParameters, imageHeight, imageWidth, text, SKColors.Black, 1, pixelsAlignment);
        }

        /// <summary>
        /// Adds styled text to an SKCanvas using TitleParameters for layout, with stroke settings.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="titleParameters">Title parameters from the Stream Deck event.</param>
        /// <param name="imageHeight">Height of the key image.</param>
        /// <param name="imageWidth">Width of the key image.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="strokeColor">Color for the text stroke.</param>
        /// <param name="strokeThickness">Thickness of the text stroke.</param>
        /// <param name="pixelsAlignment">Alignment offset in pixels.</param>
        public static void AddTextPath(this SKCanvas canvas, TitleParameters titleParameters, int imageHeight, int imageWidth, string text, SKColor strokeColor, float strokeThickness, int pixelsAlignment = 15)
        {
            try
            {
                if (titleParameters == null)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, "SkiaExtensionMethods.AddTextPath: titleParameters is null");
                    return;
                }

                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                float fontSize = (float)titleParameters.FontSizeInPixelsScaledToDefaultImage;
                // Match System.Drawing's GraphicsPath.AddString scaling: emSize = DpiY * SizeInPoints / imageWidth
                // which simplifies to fontSize * 72 / imageWidth (since SizeInPoints = fontSize * 72 / DpiY).
                float scaledFontSize = fontSize * 72.0f / imageWidth;
                var typeface = titleParameters.TitleTypeface;
                var color = titleParameters.TitleSKColor;

                using (var font = new SKFont(typeface, scaledFontSize))
                {
                    // SKCanvas.DrawText uses the text baseline for Y, while the old
                    // GraphicsPath.AddString used the top-left corner.  Shift Y down
                    // by the ascent so the visual position matches the old behavior.
                    float ascent = -font.Metrics.Ascent;

                    string[] lines = text.Split('\n');
                    float lineSpacing = font.Spacing;
                    float totalTextHeight = ascent + font.Metrics.Descent + (lines.Length - 1) * lineSpacing;

                    float startY;
                    if (titleParameters.VerticalAlignment == TitleVerticalAlignment.Bottom)
                    {
                        startY = imageHeight - pixelsAlignment - totalTextHeight + ascent;
                    }
                    else if (titleParameters.VerticalAlignment == TitleVerticalAlignment.Middle)
                    {
                        startY = (imageHeight - totalTextHeight) / 2f + ascent;
                    }
                    else
                    {
                        startY = pixelsAlignment + ascent;
                    }

                    using (var strokePaint = new SKPaint
                    {
                        Color = strokeColor,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = strokeThickness,
                        IsAntialias = true
                    })
                    using (var fillPaint = new SKPaint
                    {
                        Color = color,
                        Style = SKPaintStyle.Fill,
                        IsAntialias = true
                    })
                    {
                        float y = startY;
                        foreach (string line in lines)
                        {
                            float textWidth = font.MeasureText(line);
                            float stringWidth = 0;
                            if (textWidth < imageWidth)
                            {
                                stringWidth = (imageWidth - textWidth) / 2f;
                            }

                            canvas.DrawText(line, stringWidth, y, font, strokePaint);
                            canvas.DrawText(line, stringWidth, y, font, fillPaint);
                            y += lineSpacing;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SkiaExtensionMethods.AddTextPath Exception: {ex}");
            }
        }

        /// <summary>
        /// Adds line breaks to a string so it fits within the key image when using SetTitleAsync().
        /// SkiaSharp equivalent of <see cref="ExtensionMethods.SplitToFitKey"/>.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="titleParameters">Title parameters for font information.</param>
        /// <param name="font">The SKFont used for text measurement.</param>
        /// <param name="leftPaddingPixels">Left padding in pixels.</param>
        /// <param name="rightPaddingPixels">Right padding in pixels.</param>
        /// <param name="imageWidthPixels">Width of the key image in pixels.</param>
        /// <returns>The string with line breaks inserted.</returns>
        public static string SplitToFitKey(this string str, TitleParameters titleParameters, SKFont font, int leftPaddingPixels = 3, int rightPaddingPixels = 3, int imageWidthPixels = 72)
        {
            try
            {
                if (titleParameters == null || font == null)
                {
                    return str;
                }

                int padding = leftPaddingPixels + rightPaddingPixels;
                var finalString = new System.Text.StringBuilder();
                var currentLine = new System.Text.StringBuilder();

                for (int idx = 0; idx < str.Length; idx++)
                {
                    currentLine.Append(str[idx]);
                    float width = font.MeasureText(currentLine.ToString());
                    if (width <= imageWidthPixels - padding)
                    {
                        finalString.Append(str[idx]);
                    }
                    else
                    {
                        finalString.Append("\n" + str[idx]);
                        currentLine = new System.Text.StringBuilder(str[idx].ToString());
                    }
                }

                return finalString.ToString();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SkiaExtensionMethods.SplitToFitKey Exception: {ex}");
                return str;
            }
        }

        #endregion
    }
}
