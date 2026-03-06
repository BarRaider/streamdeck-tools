using BarRaider.SdTools.Internal;
using SkiaSharp;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Cross-platform image utilities using SkiaSharp. These methods are the SkiaSharp equivalents
    /// of the System.Drawing methods in <see cref="Tools"/> and work on Windows, macOS, and Linux.
    /// </summary>
    public static class SkiaTools
    {
        private const string HEADER_PREFIX = "data:image/png;base64,";
        private const int CLASSIC_KEY_DEFAULT_HEIGHT = 72;
        private const int CLASSIC_KEY_DEFAULT_WIDTH = 72;
        private const int PLUS_KEY_DEFAULT_HEIGHT = 144;
        private const int PLUS_KEY_DEFAULT_WIDTH = 144;
        private const int XL_KEY_DEFAULT_HEIGHT = 96;
        private const int XL_KEY_DEFAULT_WIDTH = 96;
        private const int GENERIC_KEY_IMAGE_SIZE = 144;

        private static readonly Lazy<SkiaSharpImageCodec> codec =
            new Lazy<SkiaSharpImageCodec>(() => new SkiaSharpImageCodec());

        #region Image Related

        /// <summary>
        /// Generates an empty key bitmap with the default dimensions for the given Stream Deck device type.
        /// The returned SKCanvas is ready for drawing and must be disposed by the caller.
        /// </summary>
        /// <param name="streamDeckType">The Stream Deck device type.</param>
        /// <param name="canvas">The SKCanvas for drawing on the bitmap. Caller must dispose.</param>
        /// <returns>An SKBitmap sized for the device. Caller must dispose.</returns>
        public static SKBitmap GenerateKeyImage(DeviceType streamDeckType, out SKCanvas canvas)
        {
            int height = Tools.GetKeyDefaultHeight(streamDeckType);
            int width = Tools.GetKeyDefaultWidth(streamDeckType);
            return GenerateKeyImage(height, width, out canvas);
        }

        /// <summary>
        /// Creates a key image that fits all Stream Deck models (144x144).
        /// The returned SKCanvas is ready for drawing and must be disposed by the caller.
        /// </summary>
        /// <param name="canvas">The SKCanvas for drawing on the bitmap. Caller must dispose.</param>
        /// <returns>An SKBitmap. Caller must dispose.</returns>
        public static SKBitmap GenerateGenericKeyImage(out SKCanvas canvas)
        {
            return GenerateKeyImage(GENERIC_KEY_IMAGE_SIZE, GENERIC_KEY_IMAGE_SIZE, out canvas);
        }

        /// <summary>
        /// Convert an SKBitmap to a Base64 PNG string.
        /// Set addHeaderPrefix to true if sending to SetImageAsync.
        /// </summary>
        /// <param name="bitmap">The bitmap to encode.</param>
        /// <param name="addHeaderPrefix">Whether to prepend the data URI header.</param>
        /// <returns>Base64-encoded PNG string, or null if bitmap is null.</returns>
        public static string ImageToBase64(SKBitmap bitmap, bool addHeaderPrefix)
        {
            if (bitmap == null)
            {
                return null;
            }

            byte[] imageBytes = codec.Value.EncodeToPngBytes(bitmap);
            if (imageBytes == null)
            {
                return null;
            }

            string base64String = Convert.ToBase64String(imageBytes);
            return addHeaderPrefix ? HEADER_PREFIX + base64String : base64String;
        }

        /// <summary>
        /// Convert a base64 image string to an SKBitmap.
        /// </summary>
        /// <param name="base64String">Base64-encoded image, optionally with a data URI header.</param>
        /// <returns>An SKBitmap, or null on failure. Caller must dispose.</returns>
        public static SKBitmap Base64StringToImage(string base64String)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String))
                {
                    return null;
                }

                if (base64String.StartsWith(HEADER_PREFIX, StringComparison.Ordinal))
                {
                    base64String = base64String.Substring(HEADER_PREFIX.Length);
                }

                byte[] imageBytes = Convert.FromBase64String(base64String);
                return codec.Value.DecodeFromBytes(imageBytes);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SkiaTools.Base64StringToImage Exception: {ex}");
            }
            return null;
        }

        /// <summary>
        /// Convert an image file to a Base64 PNG string.
        /// Set addHeaderPrefix to true if sending to SetImageAsync.
        /// </summary>
        /// <param name="fileName">Path to the image file.</param>
        /// <param name="addHeaderPrefix">Whether to prepend the data URI header.</param>
        /// <returns>Base64-encoded PNG string, or null if file doesn't exist.</returns>
        public static string FileToBase64(string fileName, bool addHeaderPrefix)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            using (SKBitmap bitmap = codec.Value.DecodeFromFile(fileName))
            {
                return ImageToBase64(bitmap, addHeaderPrefix);
            }
        }

        /// <summary>
        /// Loads an image from a file path. Returns an independent SKBitmap that does not lock the file.
        /// </summary>
        /// <param name="filePath">Path to the image file.</param>
        /// <returns>An SKBitmap, or null if the path is invalid or file doesn't exist. Caller must dispose.</returns>
        public static SKBitmap LoadImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            return codec.Value.DecodeFromFile(filePath);
        }

        /// <summary>
        /// Loads an image from a stream. The caller may close the stream after this returns.
        /// </summary>
        /// <param name="stream">The stream containing image data.</param>
        /// <returns>An SKBitmap, or null if the stream is null. Caller must dispose.</returns>
        public static SKBitmap LoadImage(Stream stream)
        {
            return codec.Value.DecodeFromStream(stream);
        }

        /// <summary>
        /// Returns a SHA512 hash of the PNG-encoded bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to hash.</param>
        /// <returns>Hex-encoded SHA512 hash, or null on failure.</returns>
        public static string ImageToSHA512(SKBitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            try
            {
                byte[] imageBytes = codec.Value.EncodeToPngBytes(bitmap);
                return imageBytes == null ? null : Tools.BytesToSHA512(imageBytes);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SkiaTools.ImageToSHA512 Exception: {ex}");
            }
            return null;
        }

        /// <summary>
        /// Creates an SKFont from a font family name, size in points, and optional style.
        /// The caller is responsible for disposing the returned SKFont.
        /// </summary>
        /// <param name="familyName">Font family name (e.g. "Arial", "Verdana").</param>
        /// <param name="sizeInPoints">Font size in points.</param>
        /// <param name="style">SkiaSharp font style. Defaults to Normal.</param>
        /// <returns>A new SKFont. Caller must dispose.</returns>
        public static SKFont CreateFont(string familyName, float sizeInPoints, SKFontStyle style = null)
        {
            var typeface = SKTypeface.FromFamilyName(familyName, style ?? SKFontStyle.Normal);
            return new SKFont(typeface, sizeInPoints);
        }

        /// <summary>
        /// Parses a hex color string (e.g. "#FF0000" or "#AAFF0000") into an SKColor.
        /// </summary>
        /// <param name="hexColor">Hex color string with leading '#'.</param>
        /// <returns>The parsed SKColor.</returns>
        public static SKColor ColorFromHex(string hexColor)
        {
            if (!string.IsNullOrEmpty(hexColor) && SKColor.TryParse(hexColor, out SKColor color))
            {
                return color;
            }

            Logger.Instance.LogMessage(TracingLevel.WARN, $"SkiaTools.ColorFromHex: Failed to parse '{hexColor}', returning black");
            return SKColors.Black;
        }

        #endregion

        #region Private Methods

        private static SKBitmap GenerateKeyImage(int height, int width, out SKCanvas canvas)
        {
            try
            {
                var bitmap = new SKBitmap(width, height);
                canvas = new SKCanvas(bitmap);
                canvas.Clear(SKColors.Black);
                return bitmap;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SkiaTools.GenerateKeyImage exception: {ex} Height: {height} Width: {width}");
            }
            canvas = null;
            return null;
        }

        #endregion
    }
}
