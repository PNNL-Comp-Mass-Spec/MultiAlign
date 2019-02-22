#region

using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OxyPlot;
using OxyPlot.Wpf;
using PNNLOmics.Annotations;

#endregion

namespace MultiAlignCore.Drawing
{
    // TODO: Static or non-static?
    /// <summary>
    /// </summary>
    [UsedImplicitly]
    public sealed class PngPlotModelEncoder : IPlotModelEncoder<BitmapSource>
    {
        [UsedImplicitly] private const int WIDTH = 800;
        [UsedImplicitly] private const int HEIGHT = 600;

        /// <summary>
        /// Converts the plot model into an SVG string.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public BitmapSource CreateImage(PlotModel model, Size size)
        {
            var pngExporter = new PngExporter
            {
                Width = (int)size.Width,
                Height = (int)size.Height,
                Resolution = 120
            };
            return pngExporter.ExportToBitmap(model);
        }

        /// <summary>
        /// Creates the image.
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
        /// Saves an image of the given size for the model provided at the path suggested.
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
                    Width = (int)size.Width,
                    Height = (int)size.Height
                };
                pngExporter.Export(model, stream);
            }
        }

        /// <summary>
        /// Saves an image of the given size and resolution for the model provided at the path suggested.
        /// </summary>
        /// <remarks>Image will not be saved at the specified size, but instead will be saved at a size that will permit resampling to the specified resolution</remarks>
        /// <param name="model"></param>
        /// <param name="path"></param>
        /// <param name="size"></param>
        /// <param name="resolution"></param>
        public void SaveImageHighRes(PlotModel model, string path, Size size, double resolution)
        {
            using (var stream = File.Create(path))
            {
                var multiplier = resolution / 96.0;
                var width = Math.Ceiling(size.Width * multiplier);
                var height = Math.Ceiling(size.Height * multiplier);

                var pngExporter = new PngExporter
                {
                    Width = (int)width,
                    Height = (int)height,
                    Resolution = (int)resolution
                };
                pngExporter.Export(model, stream);
            }
        }

        /// <summary>
        /// Converts the plot model into a BitmapSource with the given size and resolution.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="size"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public BitmapSource CreateImageHighRes(PlotModel model, Size size, double resolution)
        {
            var multiplier = (resolution / 96.0);
            var width = Math.Ceiling(size.Width * multiplier);
            var height = Math.Ceiling(size.Height * multiplier);
            var pngExporter = new PngExporter
            {
                Width = (int)width,
                Height = (int)height,
                Resolution = (int)resolution
            };
            return pngExporter.ExportToBitmap(model);
        }
    }
}