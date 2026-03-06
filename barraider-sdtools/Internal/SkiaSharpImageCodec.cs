using SkiaSharp;
using System.IO;

namespace BarRaider.SdTools.Internal
{
    internal sealed class SkiaSharpImageCodec
    {
        public byte[] EncodeToPngBytes(SKBitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            using (SKImage image = SKImage.FromBitmap(bitmap))
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                return data.ToArray();
            }
        }

        public SKBitmap DecodeFromBytes(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }

            return SKBitmap.Decode(imageBytes);
        }

        public SKBitmap DecodeFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            return SKBitmap.Decode(filePath);
        }

        public SKBitmap DecodeFromStream(Stream stream)
        {
            if (stream == null)
            {
                return null;
            }

            return SKBitmap.Decode(stream);
        }
    }
}
