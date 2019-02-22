using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MultiAlign.Data;
using MultiAlignCore.Data;
using MultiAlignCore.Drawing;

namespace MultiAlign.Windows.Plots
{
    /// <summary>
    /// Interaction logic for FeaturePlotView.xaml
    /// </summary>
    public partial class FeaturePlotView : UserControl
    {
        public static readonly DependencyProperty PlotWidthProperty =
            DependencyProperty.Register("PlotWidth", typeof (int), typeof (FeaturePlotView), new UIPropertyMetadata(256));

        public static readonly DependencyProperty PlotHeightProperty =
            DependencyProperty.Register("PlotHeight", typeof (int), typeof (FeaturePlotView),
                new UIPropertyMetadata(256));

        public static readonly DependencyProperty PlotNameProperty =
            DependencyProperty.Register("Name", typeof (string), typeof (FeaturePlotView));

        public static readonly DependencyProperty NetScanImageProperty =
            DependencyProperty.Register("FeaturesImage", typeof (BitmapImage), typeof (FeaturePlotView));

        public static readonly DependencyProperty MassTagsDataProperty =
            DependencyProperty.Register("MassTagsData", typeof (MassTagsLoadedEventArgs), typeof (FeaturePlotView),
                new PropertyMetadata(SetMassTagData));

        public static readonly DependencyProperty BaselineDataProperty =
            DependencyProperty.Register("BaselineData",
                typeof (BaselineFeaturesLoadedEventArgs),
                typeof (FeaturePlotView),
                new PropertyMetadata(SetData));

        public FeaturePlotView()
        {
            InitializeComponent();
        }


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


        public string PlotName
        {
            get { return (string) GetValue(PlotNameProperty); }
            set { SetValue(PlotNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...


        public BitmapImage FeaturesImage
        {
            get { return (BitmapImage) GetValue(NetScanImageProperty); }
            set { SetValue(NetScanImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NetScanImage.  This enables animation, styling, binding, etc...


        public MassTagsLoadedEventArgs MassTagsData
        {
            get { return (MassTagsLoadedEventArgs) GetValue(MassTagsDataProperty); }
            set { SetValue(MassTagsDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MassTagsData.  This enables animation, styling, binding, etc...

        public BaselineFeaturesLoadedEventArgs BaselineData
        {
            get { return (BaselineFeaturesLoadedEventArgs) GetValue(BaselineDataProperty); }
            set { SetValue(BaselineDataProperty, value); }
        }

        private static void SetMassTagData(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var x = sender as FeaturePlotView;
            x.CreateMassTagPlots();
        }

        // Using a DependencyProperty as the backing store for AlignmentData.  This enables animation, styling, binding, etc...

        private static void SetData(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var x = sender as FeaturePlotView;
            x.CreatePlots();
        }

        ///
        private void CreatePlots()
        {
            var data = BaselineData;
            var name = "";
            if (data.DatasetInformation == null)
            {
                if (data.Database != null)
                {
                    name = data.Database.Name;
                }
            }
            else
            {
                name = data.DatasetInformation.DatasetName;
            }

            PlotName = string.Format("Baseline: {0}", name);
            var plot = ScatterPlotFactory.CreateFeatureMassScatterPlot(data.Features);
            FeaturesImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(plot));
        }


        private void CreateMassTagPlots()
        {
            var data = MassTagsData;
            PlotName = string.Format("Mass Tag Database");
            var plot = ScatterPlotFactory.CreateFeatureMassScatterPlot(data.MassTags);
            plot.Model.Title = PlotName;
            FeaturesImage = ImageConverter.ConvertImage(PlotImageUtility.CreateImage(plot));
        }
    }
}