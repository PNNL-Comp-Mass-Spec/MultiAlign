using System.Windows;
using System.Windows.Media.Imaging;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Drawing;

namespace MultiAlignRogue.Alignment
{
    public class AlignmentPlotCreator
    {
        public PlotBase Heatmap { get; private set; }
        public PlotBase NetResidual { get; private set; }
        public PlotBase MassHistogram { get; private set; }
        public PlotBase NetHistogram { get; private set; }
        public PlotBase MassMzResidual { get; private set; }
        public PlotBase MassScanResidual { get; private set; }

        public AlignmentPlotCreator(AlignmentData alignment)
        {
            var residuals = alignment.ResidualData;

            Heatmap = HeatmapFactory.CreateAlignedHeatmap(alignment.HeatScores, alignment.BaselineIsAmtDB);
            NetResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Net, residuals.LinearCustomNet,
                residuals.LinearNet, "NET Residuals", "Scans", "NET");

            MassHistogram = HistogramFactory.CreateHistogram(alignment.MassErrorHistogram, "Mass Error", "Mass Error (ppm)");
            NetHistogram = HistogramFactory.CreateHistogram(alignment.NetErrorHistogram, "NET Error", "NET Error");

            MassMzResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Mz, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "m/z", "Mass Errors");

            MassScanResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Net, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "Scan", "Mass Errors");
        }

        /// <summary>
        ///     Save the plots to the specified path, with a specified width, height, and dpi
        /// </summary>
        /// <remarks>If the DPI is not 96, the width and height will be scaled so that the image 
        /// provides the needed pixel size for the specified size and DPI</remarks>
        /// <param name="pathAndPrefix">Path to save the images to, with a trailing '\' or name prefix</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dpi"></param>
        public void SavePlots(string pathAndPrefix, double width, double height, double dpi)
        {
            var encoder = new PngPlotModelEncoder();
            var size = new Size(width, height);

            if (dpi.Equals(96))
            {
                encoder.SaveImage(this.Heatmap.Model, pathAndPrefix + "Heatmap.png", size);
                encoder.SaveImage(this.NetResidual.Model, pathAndPrefix + "NetScan.png", size);
                encoder.SaveImage(this.MassHistogram.Model, pathAndPrefix + "MassHistogram.png", size);
                encoder.SaveImage(this.NetHistogram.Model, pathAndPrefix + "NetHistogram.png", size);
                encoder.SaveImage(this.MassMzResidual.Model, pathAndPrefix + "MassMz.png", size);
                encoder.SaveImage(this.MassScanResidual.Model, pathAndPrefix + "MassScan.png", size);
            }
            else
            {
                encoder.SaveImageHighRes(this.Heatmap.Model, pathAndPrefix + "Heatmap.png", size, dpi);
                encoder.SaveImageHighRes(this.NetResidual.Model, pathAndPrefix + "NetScan.png", size, dpi);
                encoder.SaveImageHighRes(this.MassHistogram.Model, pathAndPrefix + "MassHistogram.png", size, dpi);
                encoder.SaveImageHighRes(this.NetHistogram.Model, pathAndPrefix + "NetHistogram.png", size, dpi);
                encoder.SaveImageHighRes(this.MassMzResidual.Model, pathAndPrefix + "MassMz.png", size, dpi);
                encoder.SaveImageHighRes(this.MassScanResidual.Model, pathAndPrefix + "MassScan.png", size, dpi);
            }
        }
    }
}
