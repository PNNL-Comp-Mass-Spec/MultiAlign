using System.IO;
using System.Windows.Media.Imaging;

namespace MultiAlignRogue.Utils
{
    public static class ImageConverter
    {
        public static BitmapImage ConvertImage(BitmapSource bitmapSource)
        {
            var encoder = new PngBitmapEncoder();
            using (var stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);

                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(stream.ToArray());
                image.EndInit();

                return image;
            }
        }
    }
}