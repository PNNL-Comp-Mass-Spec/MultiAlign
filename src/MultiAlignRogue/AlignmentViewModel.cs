using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Drawing;
using System.Windows.Media.Imaging;
using MultiAlign.Data;
using MultiAlignCore.Data;


namespace MultiAlignRogue
{
    class AlignmentViewModel
    {
        public BitmapImage HeatmapImage { get; private set; }
        public BitmapImage NetScanImage { get; private set; }
        public BitmapImage MassHistogram { get; private set; }
        public BitmapImage NetHistogram { get; private set; }        
        public BitmapImage MassMzImage { get; private set; }
        public BitmapImage MassScanImage { get; private set; }
        public String WindowTitle { get; private set; }
        
        public AlignmentViewModel()
        {

        }

        public AlignmentViewModel(classAlignmentData alignment)
        {
            WindowTitle = String.Format("{0} Alignment Data",alignment.aligneeDataset);
            var residuals = alignment.ResidualData;

            var heatmap = HeatmapFactory.CreateAlignedHeatmap(alignment.heatScores);
            var netResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Scan, residuals.LinearCustomNet,
                residuals.LinearNet, "NET Residuals", "Scans", "NET");
            var massHistogram = HistogramFactory.CreateHistogram(alignment.netErrorHistogram, "Mass Error", "Mass Error (ppm)");
            var netHistogram = HistogramFactory.CreateHistogram(alignment.netErrorHistogram, "NET Error", "NET Error");            
            var massMzResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Mz, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "m/z", "Mass Errors");
            var massScanResidual = ScatterPlotFactory.CreateResidualPlot(residuals.Scan, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "Scan", "Mass Errors");
            
            HeatmapImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(heatmap));
            NetScanImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(netResidual));
            MassHistogram = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(massHistogram));
            NetHistogram = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(netHistogram));         
            MassMzImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(massMzResidual));
            MassScanImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(massScanResidual));          
        }

    }
}
