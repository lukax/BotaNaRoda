using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver.GeoJsonObjectModel.Serializers;

namespace BotaNaRoda.WebApi.Util
{
    public class ImageUtil
    {
        public static Stream CompressImage(Stream stream, int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException(nameof(quality));

            using (stream)
            {
                var original = Image.FromStream(stream);

                var ms = new MemoryStream();

                // Encoder parameter for image quality 
                EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
                // Jpeg image codec 
                ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders().First(t => t.MimeType == "image/jpeg");

                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;

                original.Save(ms, jpegCodec, encoderParams);

                ms.Position = 0;
                return ms;
            }
        }

        public static Stream CreateThumbnail(Stream stream, int width)
        {
            using (stream)
            {
                var ms = new MemoryStream();

                var original = Image.FromStream(stream);
                double ratio = (original.Width * 1.0) / original.Height;
                int proportionalHeight = (int)(width / ratio);

                var image = ResizeImage(original, width, proportionalHeight);
                image.Save(ms, ImageFormat.Jpeg);

                ms.Position = 0;
                return ms;
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
