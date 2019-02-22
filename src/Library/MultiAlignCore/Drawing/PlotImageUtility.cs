#region

using System.IO;
using System.Windows.Media.Imaging;

#endregion

namespace MultiAlignCore.Drawing
{
    /// <summary>
    /// Adapter / Factory class for creating LC-MS/MS based plots and images.
    /// </summary>
    public static class PlotImageUtility
    {
        /// <summary>
        /// Converts a plot model into a System.Drawing.Image.
        /// </summary>
        /// <param name="plot"></param>
        /// <returns></returns>
        public static BitmapSource CreateImage(PlotBase plot)
        {
            var encoder = new PngPlotModelEncoder();
            return encoder.CreateImage(plot.Model);
        }

        /// <summary>
        /// Saves the plot model to the path provided.
        /// </summary>
        /// <param name="plot"></param>
        /// <param name="path"></param>
        /// <param name="encoder"></param>
        public static void SaveImage<T>(PlotBase plot, string path, IPlotModelEncoder<T> encoder)
        {
            encoder.SaveImage(plot.Model, path);
        }

        public static void SaveImage(PlotBase plot, string path)
        {
            SaveImage(plot, path, new PngPlotModelEncoder());
        }

        /// <summary>
        /// Saves the plot model to the path provided.
        /// </summary>
        public static void SaveImage(BitmapSource image, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var encoder = new PngBitmapEncoder {Interlace = PngInterlaceOption.On};
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
            }
        }
    }
}