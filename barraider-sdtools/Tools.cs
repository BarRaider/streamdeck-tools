using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    public class Tools
    {
        private Tools() {}

        private const string HEADER_PREFIX = "data:image/png;base64,";

        public const int KEY_DEFAULT_HEIGHT = 72;
        public const int KEY_DEFAULT_WIDTH = 72;

        public static string FileToBase64(string fileName, bool addHeaderPrefix)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            using (Image image = Image.FromFile(fileName))
            {
                return ImageToBase64(image, addHeaderPrefix);
            }
        }

        public static string ImageToBase64(Image image, bool addHeaderPrefix)
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, ImageFormat.Png);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return addHeaderPrefix ? HEADER_PREFIX + base64String : base64String;
            }
        }

        public static Bitmap GenerateKeyImage(out Graphics graphics)
        {
            Bitmap bitmap = new Bitmap(KEY_DEFAULT_WIDTH, KEY_DEFAULT_HEIGHT);
            var brush = new SolidBrush(Color.Black);

            graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            //Fill background black
            graphics.FillRectangle(brush, 0, 0, KEY_DEFAULT_WIDTH, KEY_DEFAULT_HEIGHT);
            return bitmap;
        }
    }
}
