using BarRaider.SdTools.Wrappers;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Cross-platform graphics manipulation utilities using SkiaSharp.
    /// These methods are the SkiaSharp equivalents of the System.Drawing methods in <see cref="GraphicsTools"/>.
    /// </summary>
    public static class SkiaGraphicsTools
    {
        /// <summary>
        /// Parse a hex color string into an SKColor.
        /// </summary>
        /// <param name="hexColor">Hex string such as "#FF0000" or "#80FF0000".</param>
        /// <returns>The parsed SKColor.</returns>
        public static SKColor ColorFromHex(string hexColor)
        {
            return SkiaTools.ColorFromHex(hexColor);
        }

        /// <summary>
        /// Generates multiple shades based on an initial color and the number of stages.
        /// </summary>
        /// <param name="initialColor">Initial hex color string.</param>
        /// <param name="currentShade">The current shade index (0-based).</param>
        /// <param name="totalAmountOfShades">Total number of shade levels.</param>
        /// <returns>A darker shade of the initial color.</returns>
        public static SKColor GenerateColorShades(string initialColor, int currentShade, int totalAmountOfShades)
        {
            SKColor color = ColorFromHex(initialColor);
            double r = color.Red;
            double g = color.Green;
            double b = color.Blue;

            if (currentShade == totalAmountOfShades - 1)
            {
                currentShade = 1;
            }

            for (int idx = 0; idx < currentShade; idx++)
            {
                r /= 2;
                g /= 2;
                b /= 2;
            }

            return new SKColor((byte)r, (byte)g, (byte)b, color.Alpha);
        }

        /// <summary>
        /// Resizes an image while maintaining aspect ratio and centering on a black background.
        /// </summary>
        /// <param name="original">The source bitmap.</param>
        /// <param name="newWidth">Target width.</param>
        /// <param name="newHeight">Target height.</param>
        /// <returns>A new resized SKBitmap. Caller must dispose. Returns null if original is null.</returns>
        public static SKBitmap ResizeImage(SKBitmap original, int newWidth, int newHeight)
        {
            if (original == null)
            {
                return null;
            }

            int originalWidth = original.Width;
            int originalHeight = original.Height;

            if (originalWidth == 0 || originalHeight == 0)
            {
                return null;
            }

            double ratioX = (double)newWidth / originalWidth;
            double ratioY = (double)newHeight / originalHeight;
            double ratio = Math.Min(ratioX, ratioY);

            float scaledWidth = (float)(originalWidth * ratio);
            float scaledHeight = (float)(originalHeight * ratio);
            int posX = (int)((newWidth - scaledWidth) / 2);
            int posY = (int)((newHeight - scaledHeight) / 2);

            var result = new SKBitmap(newWidth, newHeight);
            try
            {
                using (var canvas = new SKCanvas(result))
                using (var paint = new SKPaint { IsAntialias = true })
                {
                    canvas.Clear(SKColors.Black);
                    var destRect = SKRect.Create(posX, posY, scaledWidth, scaledHeight);
                    canvas.DrawBitmap(original, destRect, paint);
                }
            }
            catch
            {
                result.Dispose();
                throw;
            }
            return result;
        }

        /// <summary>
        /// Extracts a rectangular region from an image.
        /// </summary>
        /// <param name="bitmap">The source bitmap.</param>
        /// <param name="startX">Left edge of the rectangle.</param>
        /// <param name="startY">Top edge of the rectangle.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <returns>A new SKBitmap with the extracted region. Caller must dispose.</returns>
        public static SKBitmap ExtractRectangle(SKBitmap bitmap, int startX, int startY, int width, int height)
        {
            if (bitmap == null)
            {
                return null;
            }

            var subset = new SKRectI(startX, startY, startX + width, startY + height);
            var result = new SKBitmap();
            if (bitmap.ExtractSubset(result, subset))
            {
                return result;
            }

            result.Dispose();
            return null;
        }

        /// <summary>
        /// Creates a copy of the image with modified opacity.
        /// </summary>
        /// <param name="bitmap">The source bitmap.</param>
        /// <param name="opacity">Opacity value from 0.0 (transparent) to 1.0 (opaque).</param>
        /// <returns>A new SKBitmap with the applied opacity. Caller must dispose.</returns>
        public static SKBitmap CreateOpacityImage(SKBitmap bitmap, float opacity)
        {
            if (bitmap == null)
            {
                return null;
            }

            try
            {
                var result = new SKBitmap(bitmap.Width, bitmap.Height);
                try
                {
                    using (var canvas = new SKCanvas(result))
                    using (var paint = new SKPaint())
                    {
                        byte alpha = (byte)(opacity * 255);
                        var colorFilter = SKColorFilter.CreateBlendMode(
                            new SKColor(255, 255, 255, alpha), SKBlendMode.DstIn);
                        paint.ColorFilter = colorFilter;
                        canvas.Clear(SKColors.Transparent);
                        canvas.DrawBitmap(bitmap, 0, 0, paint);
                    }
                }
                catch
                {
                    result.Dispose();
                    throw;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SkiaGraphicsTools.CreateOpacityImage exception: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Draws multi-lined text onto key images. Generates one or more images where each has
        /// text drawn based on the given parameters.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="currentTextPosition">Starting character position.</param>
        /// <param name="lettersPerLine">Maximum characters per line.</param>
        /// <param name="numberOfLines">Maximum lines per image.</param>
        /// <param name="font">The SKFont for text rendering.</param>
        /// <param name="backgroundColor">Background fill color.</param>
        /// <param name="textColor">Text color.</param>
        /// <param name="expandToNextImage">If true, overflow text creates additional images.</param>
        /// <param name="keyDrawStartingPosition">Starting draw position on the key.</param>
        /// <returns>Array of SKBitmaps with drawn text. Caller must dispose each bitmap.</returns>
        public static SKBitmap[] DrawMultiLinedText(string text, int currentTextPosition, int lettersPerLine, int numberOfLines,
            SKFont font, SKColor backgroundColor, SKColor textColor, bool expandToNextImage, SKPoint keyDrawStartingPosition)
        {
            if (string.IsNullOrEmpty(text) || font == null)
            {
                return Array.Empty<SKBitmap>();
            }

            float currentWidth = keyDrawStartingPosition.X;
            float currentHeight = keyDrawStartingPosition.Y;
            int currentLine = 0;
            var images = new List<SKBitmap>();

            SKBitmap img = SkiaTools.GenerateGenericKeyImage(out SKCanvas canvas);
            if (img == null)
            {
                return Array.Empty<SKBitmap>();
            }
            images.Add(img);

            canvas.Clear(backgroundColor);

            float lineHeight = img.Height / numberOfLines;
            if (numberOfLines == 1)
            {
                currentHeight = img.Height / 2f;
            }

            float widthIncrement = img.Width / (float)lettersPerLine;

            using (var paint = new SKPaint { Color = textColor, IsAntialias = true })
            {
                for (int letter = currentTextPosition; letter < text.Length; letter++)
                {
                    if (letter > currentTextPosition && letter % lettersPerLine == 0)
                    {
                        currentLine++;
                        if (currentLine >= numberOfLines)
                        {
                            if (expandToNextImage)
                            {
                                var overflow = DrawMultiLinedText(text, letter, lettersPerLine, numberOfLines,
                                    font, backgroundColor, textColor, expandToNextImage, keyDrawStartingPosition);
                                images.AddRange(overflow);
                            }
                            break;
                        }

                        currentHeight += lineHeight;
                        currentWidth = keyDrawStartingPosition.X;
                    }

                    canvas.DrawText(text[letter].ToString(), currentWidth, currentHeight, font, paint);
                    currentWidth += widthIncrement;
                }
            }

            canvas.Dispose();
            return images.ToArray();
        }

        /// <summary>
        /// Adds line breaks to a string so that it fits within the given image width
        /// when rendered with the specified font.
        /// </summary>
        /// <param name="str">The string to wrap.</param>
        /// <param name="font">The SKFont used for measurement.</param>
        /// <param name="imageWidthPixels">The maximum width in pixels.</param>
        /// <param name="leftPaddingPixels">Left padding in pixels.</param>
        /// <param name="rightPaddingPixels">Right padding in pixels.</param>
        /// <returns>The string with line breaks inserted.</returns>
        public static string WrapStringToFitImage(string str, SKFont font, int imageWidthPixels = 72, int leftPaddingPixels = 5, int rightPaddingPixels = 5)
        {
            try
            {
                if (font == null)
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
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SkiaGraphicsTools.WrapStringToFitImage Exception: {ex}");
                return str;
            }
        }
    }
}
