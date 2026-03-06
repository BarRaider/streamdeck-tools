using System;

namespace BarRaider.SdTools.Internal
{
    internal static class ImageCodecProvider
    {
        private static readonly Lazy<IImageCodec> lazyInstance =
            new Lazy<IImageCodec>(() => new SystemDrawingImageCodec());

        internal static IImageCodec Instance => lazyInstance.Value;
    }
}
