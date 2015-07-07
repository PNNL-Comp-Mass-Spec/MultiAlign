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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows.Controls;
using MultiAlignCore.Data;


namespace MultiAlignRogue
{
    class AlignmentViewModel: DependencyObject
    {

        public static readonly DependencyProperty PlotWidthProperty =
    DependencyProperty.Register("PlotWidth", typeof(int), typeof(AlignmentView),
        new UIPropertyMetadata(256));

        public static readonly DependencyProperty PlotHeightProperty =
            DependencyProperty.Register("PlotHeight", typeof(int), typeof(AlignmentView),
                new UIPropertyMetadata(256));

        public static readonly DependencyProperty FeaturePlotImageProperty =
            DependencyProperty.Register("FeaturePlotImage", typeof(BitmapImage), typeof(AlignmentView));

        public static readonly DependencyProperty HeatmapImageProperty =
            DependencyProperty.Register("HeatmapImage", typeof(BitmapImage), typeof(AlignmentView));

        public static readonly DependencyProperty MassHistogramProperty =
            DependencyProperty.Register("MassHistogram", typeof(BitmapImage), typeof(AlignmentView));

        public static readonly DependencyProperty NetHistogramProperty =
            DependencyProperty.Register("NetHistogram", typeof(BitmapImage), typeof(AlignmentView));

        public static readonly DependencyProperty PlotNameProperty =
            DependencyProperty.Register("PlotName", typeof(string), typeof(AlignmentView));

        public static readonly DependencyProperty NetScanImageProperty =
            DependencyProperty.Register("NetScanImage", typeof(BitmapImage), typeof(AlignmentView));

        public static readonly DependencyProperty MassScanImageProperty =
            DependencyProperty.Register("MassScanImage", typeof(BitmapImage), typeof(AlignmentView));

        public static readonly DependencyProperty MassMzImageProperty =
            DependencyProperty.Register("MassMzImage", typeof(BitmapImage), typeof(AlignmentView));

        
        
        public BitmapImage FeaturePlotImage
        {
            get { return (BitmapImage)GetValue(FeaturePlotImageProperty); }
            set { SetValue(FeaturePlotImageProperty, value); }
        }
        
        public BitmapImage HeatmapImage
        {
            get { return (BitmapImage)GetValue(HeatmapImageProperty); }
            set { SetValue(HeatmapImageProperty, value); }
        }

        public BitmapImage Heatmap2 { get; private set; }
        

        public AlignmentViewModel()
        {

        }

        public AlignmentViewModel(classAlignmentData alignment)
        {
            var heatmap = HeatmapFactory.CreateAlignedHeatmap(alignment.heatScores);

            Heatmap2 = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(heatmap));
            
        }

    }
}
