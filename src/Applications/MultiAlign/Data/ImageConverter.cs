using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;

namespace MultiAlign.Data
{
    public class ImageConverter
    {
        public static BitmapImage ConvertImage(System.Drawing.Image bitmapImage)
        {            
            var image       = bitmapImage;
            
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            
            MemoryStream memoryStream = new MemoryStream();
            // Save to a memory stream...
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            // Rewind the stream...
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();

            return bitmap;
        }
    }
}
