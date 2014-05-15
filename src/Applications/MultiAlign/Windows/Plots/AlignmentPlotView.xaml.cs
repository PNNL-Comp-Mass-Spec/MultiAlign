using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MultiAlign.Data;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Drawing;
using MultiAlignEngine.Alignment;
using PNNLOmicsViz.Drawing;

namespace MultiAlign.Windows.Plots
{
    /// <summary>
    ///     Interaction logic for AlignmentPlotView.xaml
    /// </summary>
    public partial class AlignmentPlotView : UserControl
    {
        public static readonly DependencyProperty AlignmentDataProperty =
            DependencyProperty.Register("AlignmentData",
                typeof (FeaturesAlignedEventArgs),
                typeof (AlignmentPlotView),
                new PropertyMetadata(SetData));

        public static readonly DependencyProperty PlotWidthProperty =
            DependencyProperty.Register("PlotWidth", typeof (int), typeof (AlignmentPlotView),
                new UIPropertyMetadata(256));

        public static readonly DependencyProperty PlotHeightProperty =
            DependencyProperty.Register("PlotHeight", typeof (int), typeof (AlignmentPlotView),
                new UIPropertyMetadata(256));

        public static readonly DependencyProperty FeaturePlotImageProperty =
            DependencyProperty.Register("FeaturePlotImage", typeof (BitmapImage), typeof (AlignmentPlotView));

        public static readonly DependencyProperty HeatmapImageProperty =
            DependencyProperty.Register("HeatmapImage", typeof (BitmapImage), typeof (AlignmentPlotView));

        public static readonly DependencyProperty MassHistogramProperty =
            DependencyProperty.Register("MassHistogram", typeof (BitmapImage), typeof (AlignmentPlotView));

        public static readonly DependencyProperty NetHistogramProperty =
            DependencyProperty.Register("NetHistogram", typeof (BitmapImage), typeof (AlignmentPlotView));

        public static readonly DependencyProperty PlotNameProperty =
            DependencyProperty.Register("PlotName", typeof (string), typeof (AlignmentPlotView));

        public static readonly DependencyProperty NetScanImageProperty =
            DependencyProperty.Register("NetScanImage", typeof (BitmapImage), typeof (AlignmentPlotView));

        public static readonly DependencyProperty MassScanImageProperty =
            DependencyProperty.Register("MassScanImage", typeof (BitmapImage), typeof (AlignmentPlotView));

        public static readonly DependencyProperty MassMzImageProperty =
            DependencyProperty.Register("MassMzImage", typeof (BitmapImage), typeof (AlignmentPlotView));

        public AlignmentPlotView()
        {
            InitializeComponent();

            DataContext = this;
        }

        public FeaturesAlignedEventArgs AlignmentData
        {
            get { return (FeaturesAlignedEventArgs) GetValue(AlignmentDataProperty); }
            set { SetValue(AlignmentDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlignmentData.  This enables animation, styling, binding, etc...


        public int PlotWidth
        {
            get { return (int) GetValue(PlotWidthProperty); }
            set { SetValue(PlotWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...


        public int PlotHeight
        {
            get { return (int) GetValue(PlotHeightProperty); }
            set { SetValue(PlotHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...


        public BitmapImage FeaturePlotImage
        {
            get { return (BitmapImage) GetValue(FeaturePlotImageProperty); }
            set { SetValue(FeaturePlotImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FeaturePlotImage.  This enables animation, styling, binding, etc...

        public BitmapImage HeatmapImage
        {
            get { return (BitmapImage) GetValue(HeatmapImageProperty); }
            set { SetValue(HeatmapImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeatmapImage.  This enables animation, styling, binding, etc...


        public BitmapImage MassHistogram
        {
            get { return (BitmapImage) GetValue(MassHistogramProperty); }
            set { SetValue(MassHistogramProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MassHistogram.  This enables animation, styling, binding, etc...


        public BitmapImage NetHistogram
        {
            get { return (BitmapImage) GetValue(NetHistogramProperty); }
            set { SetValue(NetHistogramProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NetHistogram.  This enables animation, styling, binding, etc...

        public string PlotName
        {
            get { return (string) GetValue(PlotNameProperty); }
            set { SetValue(PlotNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...

        public BitmapImage NetScanImage
        {
            get { return (BitmapImage) GetValue(NetScanImageProperty); }
            set { SetValue(NetScanImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NetScanImage.  This enables animation, styling, binding, etc...

        public BitmapImage MassScanImage
        {
            get { return (BitmapImage) GetValue(MassScanImageProperty); }
            set { SetValue(MassScanImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MassScanImage.  This enables animation, styling, binding, etc...

        public BitmapImage MassMzImage
        {
            get { return (BitmapImage) GetValue(MassMzImageProperty); }
            set { SetValue(MassMzImageProperty, value); }
        }

        private static void SetData(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var x = sender as AlignmentPlotView;
            x.CreatePlots();
        }

        // Using a DependencyProperty as the backing store for MassMzImage.  This enables animation, styling, binding, etc...


        private void CreatePlots()
        {
            if (AlignmentData == null) return;
            var alignmentData = AlignmentData.AlignmentData;

            if (alignmentData == null) return;

            var heatmap = HeatmapFactory.CreateAlignedHeatmap(alignmentData.heatScores);
            var feature = ScatterPlotFactory.CreateFeatureMassScatterPlot(AlignmentData.AligneeFeatures);
            var netHistomgram = HistogramFactory.CreateHistogram(alignmentData.netErrorHistogram, "NET Error", "NET Error");
            var massHistomgram = HistogramFactory.CreateHistogram(alignmentData.netErrorHistogram, "Mass Error", "Mass Error (ppm)");

            var residuals = alignmentData.ResidualData;
            var netResidual = ScatterPlotFactory.CreateResidualPlot(residuals.scans, residuals.linearCustomNet,
                residuals.linearNet, "NET Residuals", "Scans", "NET");
            var massMzResidual = ScatterPlotFactory.CreateResidualPlot(residuals.mz, residuals.mzMassError,
                residuals.mzMassErrorCorrected, "Mass Residuals", "m/z", "Mass Errors");
            var massScanResidual = ScatterPlotFactory.CreateResidualPlot(residuals.scans, residuals.mzMassError,
                residuals.mzMassErrorCorrected, "Mass Residuals", "Scan", "Mass Errors");

            NetScanImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(netResidual));
            MassHistogram = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(massHistomgram));
            NetHistogram = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(netHistomgram));
            HeatmapImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(heatmap));
            MassMzImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(massMzResidual));
            MassScanImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(massScanResidual));
            FeaturePlotImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(feature));
        }
    }
}