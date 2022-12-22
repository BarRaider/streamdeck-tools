using BarRaider.SdTools.Wrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Library of tools used to manipulate graphics
    /// </summary>
    public static class GraphicsTools
    {
        /// <summary>
        /// Return a Color object based on the hex value
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static Color ColorFromHex(string hexColor)
        {
            return System.Drawing.ColorTranslator.FromHtml(hexColor);
        }

        /// <summary>
        /// Generates multiple shades based on an initial color, and number of stages/shades you want
        /// </summary>
        /// <param name="initialColor"></param>
        /// <param name="currentShade"></param>
        /// <param name="totalAmountOfShades"></param>
        /// <returns></returns>
        public static Color GenerateColorShades(string initialColor, int currentShade, int totalAmountOfShades)
        {
            Color color = ColorFromHex(initialColor);
            int a = color.A;
            double r = color.R;
            double g = color.G;
            double b = color.B;

            // Try and increase the color in the last stage;
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

            return Color.FromArgb(a, (int)r, (int)g, (int)b);
        }

        /// <summary>
        /// Resizes an image while scaling
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Image ResizeImage(Image original, int newWidth, int newHeight)
        {
            if (original == null)
            {
                return null;
            }

            int originalWidth = original.Width;
            int originalHeight = original.Height;

            Image canvas = new Bitmap(newWidth, newHeight);
            Graphics graphic = Graphics.FromImage(canvas);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            // Figure out the ratio
            double ratioX = (double)newWidth / (double)originalWidth;
            double ratioY = (double)newHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            //int imgHeight = Convert.ToInt32(originalHeight * ratio);
            //int imgWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((newWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((newHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.Black); // Padding
            graphic.DrawImage(original, posX, posY, newWidth, newHeight);

            return canvas;
        }

        /// <summary>
        /// Extract a part of an Image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap ExtractRectangle(Image image, int startX, int startY, int width, int height)
        {
            Rectangle rec = new Rectangle(startX, startY, width, height);
            using (Bitmap src = new Bitmap(image))
            {
                return src.Clone(rec, src.PixelFormat);
            }
        }

        /// <summary>
        /// Creates a new image with different opacity
        /// </summary>
        /// <param name="image"></param>
        /// <param name="opacity"></param>
        /// <returns></returns>
        public static Image CreateOpacityImage(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix
                    {
                        //set the opacity  
                        Matrix33 = opacity
                    };

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SetImageOpacity exception {ex}");
                return null;
            }
        }

        /// <summary>
        /// Generates one (or more) images where each one has a few letters drawn on them based on the parameters. You can set number of letters and number of lines per key. 
        /// Use expandToNextImage to decide if you want only one Image returned or multiple if text is too long for one key
        /// </summary>
        /// <param name="text"></param>
        /// <param name="currentTextPosition"></param>
        /// <param name="lettersPerLine"></param>
        /// <param name="numberOfLines"></param>
        /// <param name="font"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="textColor"></param>
        /// <param name="expandToNextImage"></param>
        /// <param name="keyDrawStartingPosition"></param>
        /// <returns></returns>
        public static Image[] DrawMultiLinedText(string text, int currentTextPosition, int lettersPerLine, int numberOfLines, Font font, Color backgroundColor, Color textColor, bool expandToNextImage, PointF keyDrawStartingPosition)
        {
            float currentWidth = keyDrawStartingPosition.X;
            float currentHeight = keyDrawStartingPosition.Y;
            int currentLine = 0;
            List<Image> images = new List<Image>();
            Bitmap img = Tools.GenerateGenericKeyImage(out Graphics graphics);
            images.Add(img);

            // Draw Background
            var bgBrush = new SolidBrush(backgroundColor);
            graphics.FillRectangle(bgBrush, 0, 0, img.Width, img.Height);

            float lineHeight = img.Height / numberOfLines;
            if (numberOfLines == 1)
            {
                currentHeight = img.Height / 2; // Align to middle
            }

            float widthIncrement = img.Width / lettersPerLine;
            for (int letter = currentTextPosition; letter < text.Length; letter++)
            {
                // Check if I need to move back to the beginning of the key, but on a new line
                if (letter > currentTextPosition && letter % lettersPerLine == 0)
                {
                    currentLine++;
                    if (currentLine >= numberOfLines)
                    {
                        if (expandToNextImage)
                        {
                            images.AddRange(DrawMultiLinedText(text, letter, lettersPerLine, numberOfLines, font, backgroundColor, textColor, expandToNextImage, keyDrawStartingPosition));
                        }
                        break;
                    }

                    currentHeight += lineHeight;
                    currentWidth = keyDrawStartingPosition.X;
                }

                graphics.DrawString(text[letter].ToString(), font, new SolidBrush(textColor), new PointF(currentWidth, currentHeight));
                currentWidth += widthIncrement;
            }
            graphics.Dispose();
            return images.ToArray();
        }

        /// <summary>
        /// Adds line breaks ('\n') to the string every time the text would overflow the image
        /// </summary>
        /// <param name="str"></param>
        /// <param name="titleParameters"></param>
        /// <param name="leftPaddingPixels"></param>
        /// <param name="rightPaddingPixels"></param>
        /// <param name="imageWidthPixels"></param>
        /// <returns></returns>
        public static string WrapStringToFitImage(string str, TitleParameters titleParameters, int leftPaddingPixels = 5, int rightPaddingPixels = 5, int imageWidthPixels = 72)
        {
            try
            {
                if (titleParameters == null)
                {
                    return str;
                }

                int padding = leftPaddingPixels + rightPaddingPixels;
                Font font = new Font(titleParameters.FontFamily, (float)titleParameters.FontSizeInPixels, titleParameters.FontStyle, GraphicsUnit.Pixel);
                StringBuilder finalString = new StringBuilder();
                StringBuilder currentLine = new StringBuilder();
                SizeF currentLineSize;

                using (Bitmap img = new Bitmap(imageWidthPixels, imageWidthPixels))
                {
                    using (Graphics graphics = Graphics.FromImage(img))
                    {
                        for (int idx = 0; idx < str.Length; idx++)
                        {
                            currentLine.Append(str[idx]);
                            currentLineSize = graphics.MeasureString(currentLine.ToString(), font);
                            if (currentLineSize.Width <= img.Width - padding)
                            {
                                finalString.Append(str[idx]);
                            }
                            else // Overflow
                            {
                                finalString.Append("\n" + str[idx]);
                                currentLine = new StringBuilder(str[idx].ToString());
                            }
                        }
                    }
                }

                return finalString.ToString();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"WrapStringToFitImage Exception: {ex}");
                return str;
            }
        }
    }
}
