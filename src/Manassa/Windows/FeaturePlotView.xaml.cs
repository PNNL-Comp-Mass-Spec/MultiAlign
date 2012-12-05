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
using MultiAlignCustomControls.Charting;
using MultiAlignCustomControls.Drawing;
using Manassa.Data;
using MultiAlignCore.Data.Imaging;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for FeaturePlotView.xaml
    /// </summary>
    public partial class FeaturePlotView : UserControl
    {
        public FeaturePlotView()
        {
            InitializeComponent();
        }


        public int PlotWidth
        {
            get { return (int)GetValue(PlotWidthProperty); }
            set { SetValue(PlotWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotWidthProperty =
            DependencyProperty.Register("PlotWidth", typeof(int), typeof(FeaturePlotView), new UIPropertyMetadata(256));


        public int PlotHeight
        {
            get { return (int)GetValue(PlotHeightProperty); }
            set { SetValue(PlotHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotHeightProperty =
            DependencyProperty.Register("PlotHeight", typeof(int), typeof(FeaturePlotView), new UIPropertyMetadata(256));



        public string PlotName
        {
            get { return (string)GetValue(PlotNameProperty); }
            set { SetValue(PlotNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotNameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(FeaturePlotView));


        public BitmapImage FeaturesImage
        {
            get { return (BitmapImage)GetValue(NetScanImageProperty); }
            set { SetValue(NetScanImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NetScanImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NetScanImageProperty =
            DependencyProperty.Register("FeaturesImage", typeof(BitmapImage), typeof(FeaturePlotView));


        public BaselineFeaturesLoadedEventArgs BaselineData
        {
            get { return (BaselineFeaturesLoadedEventArgs)GetValue(BaselineDataProperty); }
            set { SetValue(BaselineDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlignmentData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BaselineDataProperty =
            DependencyProperty.Register("BaselineData",
            typeof(BaselineFeaturesLoadedEventArgs),
            typeof(FeaturePlotView),
            new PropertyMetadata(new PropertyChangedCallback(SetData)));

        private static void SetData(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var x = sender as FeaturePlotView;
            x.CreatePlots();
        }

        private void CreatePlots()
        {
            BaselineFeaturesLoadedEventArgs  data = BaselineData;

            string name                 = data.DatasetInformation.DatasetName;
            PlotName                    = string.Format("Baseline: {0}", name);
            FeatureImageData imageData  = AnalysisImageCreator.CreateBaselinePlots(data, PlotWidth, PlotHeight, false);
            FeaturesImage = ImageConverter.ConvertImage(imageData.FeatureImage);            
        }
    }
}
