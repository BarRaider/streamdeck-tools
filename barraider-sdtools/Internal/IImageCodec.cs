using System.Drawing;
using System.IO;

namespace BarRaider.SdTools.Internal
{
    /// <summary>
    /// Internal image codec abstraction used by compatibility adapters.
    /// </summary>
    internal interface IImageCodec
    {
        byte[] EncodeToPngBytes(Image image);
        Image DecodeFromBytes(byte[] imageBytes);
        Image DecodeFromFile(string filePath);
        Image DecodeFromStream(Stream stream);
    }
}
