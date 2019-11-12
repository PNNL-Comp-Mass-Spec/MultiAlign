using FeatureAlignment.Data.Alignment;
using MultiAlignRogue.Utils;

namespace MultiAlignRogue.Alignment
{
    using System.Windows.Media.Imaging;
    using MultiAlignCore.Drawing;

    public class AlignmentViewModel
    {
        public BitmapImage HeatmapImage { get; }
        public BitmapImage NetScanImage { get; }
        public BitmapImage MassHistogram { get; }
        public BitmapImage NetHistogram { get; }
        public BitmapImage MassMzImage { get; }
        public BitmapImage MassScanImage { get; }
        public string WindowTitle { get; }

        public AlignmentViewModel(AlignmentData alignment)
        {
            this.WindowTitle = string.Format("{0} Alignment Data", alignment.AligneeDataset);

            var plots = new AlignmentPlotCreator(alignment);

            this.HeatmapImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(plots.Heatmap));
            this.NetScanImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(plots.NetResidual));
            this.MassHistogram = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(plots.MassHistogram));
            this.NetHistogram = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(plots.NetHistogram));
            this.MassMzImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(plots.MassMzResidual));
            this.MassScanImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(plots.MassScanResidual));
        }

    }
}
