using MultiAlignRogue.Utils;

namespace MultiAlignRogue.Alignment
{
    using System.Windows.Media.Imaging;

    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.Drawing;

    public class AlignmentViewModel
    {
        public BitmapImage HeatmapImage { get; private set; }
        public BitmapImage NetScanImage { get; private set; }
        public BitmapImage MassHistogram { get; private set; }
        public BitmapImage NetHistogram { get; private set; }
        public BitmapImage MassMzImage { get; private set; }
        public BitmapImage MassScanImage { get; private set; }
        public string WindowTitle { get; private set; }
        
        public AlignmentViewModel()
        {

        }

        public AlignmentViewModel(AlignmentData alignment)
        {
            this.WindowTitle = string.Format("{0} Alignment Data",alignment.AligneeDataset);

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
