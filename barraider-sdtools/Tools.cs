using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static string FileToBase64(string fileName, bool addHeaderPrefix)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            using (Image image = Image.FromFile(fileName))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return addHeaderPrefix ? HEADER_PREFIX + base64String : base64String;
                }
            }
        }
    }
}
