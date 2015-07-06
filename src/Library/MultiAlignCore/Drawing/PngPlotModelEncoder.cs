#region

using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using OxyPlot;
using OxyPlot.Wpf;
using PNNLOmics.Annotations;

#endregion

namespace MultiAlignCore.Drawing
{
    /// <summary>
    /// </summary>
    [UsedImplicitly]
    public sealed class PngPlotModelEncoder : IPlotModelEncoder<BitmapSource>
    {
        [UsedImplicitly] private const int WIDTH = 800;
        [UsedImplicitly] private const int HEIGHT = 600;

        /// <summary>
        ///     Converts the plot model into an SVG string.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public BitmapSource CreateImage(PlotModel model, Size size)
        {
            var pngExporter = new PngExporter
            {
                Width = size.Width,
                Height = size.Height
            };

            return pngExporter.ExportToBitmap(model);
        }

        /// <summary>
        ///     Creates the image.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public BitmapSource CreateImage(PlotModel model)
        {
            return CreateImage(model, new Size(WIDTH, HEIGHT));
        }

        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="path"></param>
        public void SaveImage(PlotModel model, string path)
        {
            SaveImage(model, path, new Size(WIDTH, HEIGHT));
        }

        /// <summary>
        ///     Saves an image of the given size for the model provided at the path suggested.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="path"></param>
        /// <param name="size"></param>
        public void SaveImage(PlotModel model, string path, Size size)
        {
            using (var stream = File.Create(path))
            {
                var pngExporter = new PngExporter
                {
                    Width = size.Width,
                    Height = size.Height
                };
                pngExporter.Export(model, stream);
            }
        }
    }
}