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
using MultiAlign.Data;
using MultiAlignCore.Data.Imaging;

namespace MultiAlign.Windows
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


        public int PlotWidth
        {
            get { return (int)GetValue(PlotWidthProperty); }
            set { SetValue(PlotWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotWidthProperty =
            DependencyProperty.Register("PlotWidth", typeof(int), typeof(AlignmentPlotView), new UIPropertyMetadata(256));


        public int PlotHeight
        {
            get { return (int)GetValue(PlotHeightProperty); }
            set { SetValue(PlotHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotHeightProperty =
            DependencyProperty.Register("PlotHeight", typeof(int), typeof(AlignmentPlotView), new UIPropertyMetadata(256));


        public BitmapImage FeaturePlotImage
        {
            get { return (BitmapImage)GetValue(FeaturePlotImageProperty); }
            set { SetValue(FeaturePlotImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FeaturePlotImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FeaturePlotImageProperty =
            DependencyProperty.Register("FeaturePlotImage", typeof(BitmapImage), typeof(AlignmentPlotView));

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

        public string PlotName
        {
            get { return (string)GetValue(PlotNameProperty); }
            set { SetValue(PlotNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotNameProperty =
            DependencyProperty.Register("PlotName", typeof(string), typeof(AlignmentPlotView));
       
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
            PlotName                        = data.AligneeDatasetInformation.DatasetName;
            AlignmentImageData imageData    = AnalysisImageCreator.CreateAlignmentPlots(data, PlotWidth, PlotHeight, false);

            if (imageData == null)
                return;

            if (imageData.NetResidualsHistogramImage != null)
                NetScanImage    = ImageConverter.ConvertImage(imageData.NetResidualsHistogramImage);
            if (imageData.MassHistogramImage != null) 
                MassHistogram = ImageConverter.ConvertImage(imageData.MassHistogramImage);
            if (imageData.NetHistogramImage != null)
                NetHistogram = ImageConverter.ConvertImage(imageData.NetHistogramImage);
            if (imageData.HeatmapImage != null)
                HeatmapImage = ImageConverter.ConvertImage(imageData.HeatmapImage);
            if (imageData.MassMzImage != null)
                MassMzImage = ImageConverter.ConvertImage(imageData.MassMzImage);
            if (imageData.MassScanImage != null)
                MassScanImage = ImageConverter.ConvertImage(imageData.MassScanImage);
            if (imageData.FeaturePlotImage != null)
                FeaturePlotImage = ImageConverter.ConvertImage(imageData.FeaturePlotImage);
        }
    }
}
