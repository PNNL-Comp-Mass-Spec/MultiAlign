using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MultiAlignCore.Data;
using MultiAlignCustomControls.Drawing;
using MultiAlignCustomControls.Charting;
using Manassa.Data;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for AlignmentPlotView.xaml
    /// </summary>
    public partial class AlignmentPlotView : UserControl
    {
        public AlignmentPlotView()
        {
            InitializeComponent();

            DataContext = this;
        }

        public FeaturesAlignedEventArgs AlignmentData
        {
            get { return (FeaturesAlignedEventArgs)GetValue(AlignmentDataProperty); }
            set { SetValue(AlignmentDataProperty, value); }
        }



        public int PlotWidth
        {
            get { return (int)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("PlotWidth", typeof(int), typeof(AlignmentPlotView), new UIPropertyMetadata(128));


        public int PlotHeight
        {
            get { return (int)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("PlotHeight", typeof(int), typeof(AlignmentPlotView), new UIPropertyMetadata(128));


        // Using a DependencyProperty as the backing store for AlignmentData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlignmentDataProperty =
            DependencyProperty.Register("AlignmentData",
            typeof(FeaturesAlignedEventArgs),
            typeof(AlignmentPlotView),
            new PropertyMetadata(new PropertyChangedCallback(SetData)));

        private static void SetData(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var x = sender as AlignmentPlotView;
            x.CreatePlots();            
        }

        public BitmapImage HeatmapImage
        {
            get { return (BitmapImage)GetValue(HeatmapImageProperty); }
            set { SetValue(HeatmapImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeatmapImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeatmapImageProperty =
            DependencyProperty.Register("HeatmapImage", typeof(BitmapImage), typeof(AlignmentPlotView));


        public BitmapImage MassHistogram
        {
            get { return (BitmapImage)GetValue(MassHistogramProperty); }
            set { SetValue(MassHistogramProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MassHistogram.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MassHistogramProperty =
            DependencyProperty.Register("MassHistogram", typeof(BitmapImage), typeof(AlignmentPlotView));


        public BitmapImage NetHistogram
        {
            get { return (BitmapImage)GetValue(NetHistogramProperty); }
            set { SetValue(NetHistogramProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NetHistogram.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NetHistogramProperty =
            DependencyProperty.Register("NetHistogram", typeof(BitmapImage), typeof(AlignmentPlotView));




        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(AlignmentPlotView));

        

        public BitmapImage NetScanImage
        {
            get { return (BitmapImage)GetValue(NetScanImageProperty); }
            set { SetValue(NetScanImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NetScanImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NetScanImageProperty =
            DependencyProperty.Register("NetScanImage", typeof(BitmapImage), typeof(AlignmentPlotView));

        public BitmapImage MassScanImage
        {
            get { return (BitmapImage)GetValue(MassScanImageProperty); }
            set { SetValue(MassScanImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MassScanImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MassScanImageProperty =
            DependencyProperty.Register("MassScanImage", typeof(BitmapImage), typeof(AlignmentPlotView));

        public BitmapImage MassMzImage
        {
            get { return (BitmapImage)GetValue(MassMzImageProperty); }
            set { SetValue(MassMzImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MassMzImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MassMzImageProperty =
            DependencyProperty.Register("MassMzImage", typeof(BitmapImage), typeof(AlignmentPlotView));


        private void CreatePlots()
        {
            FeaturesAlignedEventArgs data   = AlignmentData;
            string name                     = data.AligneeDatasetInformation.DatasetName;
            Name                            = name;
            ChartDisplayOptions options     = new ChartDisplayOptions(false, true, true, true);
            options.DisplayTitle            = false;
            options.DisplayLegend           = false;

            options.MarginMin               = 1;
            options.MarginMax               = 100;
            options.Title                   = "Alignment Heatmap ";
            options.XAxisLabel              = "Baseline";
            options.YAxisLabel              = "Alignee";            
            options.Width                   = PlotWidth;
            options.Height                  = PlotHeight;
            System.Drawing.Image heatmapImage = RenderDatasetInfo.AlignmentHeatmap_Thumbnail(data.AlignmentData, PlotWidth, PlotHeight);
            heatmapImage.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

            options.Title                   = "NET Error Histogram " + name;
            options.XAxisLabel              = "NET Error (%)";
            options.YAxisLabel              = "Count";
            System.Drawing.Image netHistogramImage = RenderDatasetInfo.ErrorHistogram_Thumbnail(data.AlignmentData.netErrorHistogram, options);
            
            options.Title                   = "Net vs. Scan Residuals" + name;
            System.Drawing.Image netResidualsHistogramImage = RenderDatasetInfo.NETResiduals_Thumbnail(data.AlignmentData.ResidualData, options);
            
            options.Title                   = "Mass Error Histogram " + name;
            options.XAxisLabel              = "Mass Error (PPM)";
            System.Drawing.Image massHistogramImage = RenderDatasetInfo.ErrorHistogram_Thumbnail(data.AlignmentData.massErrorHistogram, options);
            
            options.Title = "Mass vs. Scan Residuals" + name;
            System.Drawing.Image massScanImage = RenderDatasetInfo.MassVsScanResiduals_Thumbnail(data.AlignmentData.ResidualData, options);
            
            options.Title = "Mass vs. m/z Residuals" + name;            
            System.Drawing.Image massMzImage = RenderDatasetInfo.ClusterMassVsMZResidual_Thumbnail(data.AlignmentData.ResidualData, options);            

            NetScanImage  = ImageConverter.ConvertImage(netResidualsHistogramImage);
            MassHistogram = ImageConverter.ConvertImage(massHistogramImage);
            NetHistogram  = ImageConverter.ConvertImage(netHistogramImage);
            HeatmapImage  = ImageConverter.ConvertImage(heatmapImage);
            MassMzImage   = ImageConverter.ConvertImage(massMzImage);
            MassScanImage = ImageConverter.ConvertImage(massScanImage);            
        }
    }
}
