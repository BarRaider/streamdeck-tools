using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BarRaider.SdTools.Internal
{
    /// <summary>
    /// Compatibility image codec implementation backed by System.Drawing.
    /// Image.FromStream requires the backing stream to remain open for the
    /// lifetime of the Image, so DecodeFromBytes copies pixel data into a
    /// new Bitmap and disposes the intermediate resources.
    /// </summary>
    internal sealed class SystemDrawingImageCodec : IImageCodec
    {
        public byte[] EncodeToPngBytes(Image image)
        {
            if (image == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }

        public Image DecodeFromBytes(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }

            var memoryStream = new MemoryStream(imageBytes);
            try
            {
                Image original = Image.FromStream(memoryStream);
                var copy = new Bitmap(original);
                original.Dispose();
                memoryStream.Dispose();
                return copy;
            }
            catch
            {
                memoryStream.Dispose();
                throw;
            }
        }

        public Image DecodeFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            using (Image original = Image.FromFile(filePath))
            {
                return new Bitmap(original);
            }
        }
    }
}
