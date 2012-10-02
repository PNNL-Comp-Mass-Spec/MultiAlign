using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Controls;
using MultiAlignCore.Data;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.MetaData;
using System.IO;
using System.Windows.Data;
using System.Windows;

namespace Manassa.Windows
{
    /// <summary>
    /// Interaction logic for AllClustersControl.xaml
    /// </summary>
    public partial class AnalysisView : UserControl
    {
        private MultiAlignAnalysis m_analysis;
        
        public AnalysisView()
        {
            InitializeComponent();

            m_analysis = null;


            Binding binding = new Binding("Viewport");
            binding.Source = m_clusterControl;
            SetBinding(ViewportProperty, binding);            
        }

        /// <summary>
        /// Gets or sets the Analysis
        /// </summary>
        public MultiAlignAnalysis Analysis
        {
            get
            {
                return m_analysis;
            }
            set
            {
                m_analysis = value;
                if (value != null)
                {
                    /// 
                    /// replace this with bindings!!!!
                    /// 
                    m_clusterGrid.Clusters      = value.Clusters;                   
                    m_clusterPlot.SetClusters(value.Clusters);

                    m_clusterControl.Providers  = m_analysis.DataProviders;
                    Dictionary<int, int> map    = value.Clusters.CreateChargeMap<UMCClusterLight>();

                    if (value.MassTagDatabase != null)
                    {
                        m_massTagViewer.Database = value.MassTagDatabase;
                    }
                    m_msmsViewer.Analysis       = value;
                    m_msmsViewer.ExtractMsMsData(m_analysis.DataProviders);

                    // Make the dataset plots.
                    string directoryPath        = Path.GetDirectoryName(value.MetaData.AnalysisPath);
                    string plotPath             = Path.Combine(directoryPath, "plots");
                    if (Directory.Exists(plotPath))
                    {
                        Manassa.Data.DatasetPlotLoader loader = new Data.DatasetPlotLoader();                      
                        loader.LoadDatasetPlots(plotPath, value.MetaData.Datasets);
                    }

                    // Sort the datasets for the view...
                    value.MetaData.Datasets.Sort(delegate(DatasetInformation x, DatasetInformation y)
                    {
                        if (x.DatasetId == y.DatasetId)
                            return 0;

                        if (x.IsBaseline)
                            return -1;

                        return x.DatasetName.CompareTo(y.DatasetName);
                    });

                    m_datasetsName.Datasets = value.MetaData.Datasets;

                    // Setup the histogram data.
                    m_chargeStates.ConstructHistogram(map);
                    m_chargeStates.AutoViewPort();
                }
            }
        }

        private static void SynchronizeViewport(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisSender = (AnalysisView)sender;

            if (e.NewValue != null)
            {
                RectangleF viewport = (RectangleF) e.NewValue;
                thisSender.m_clusterPlot.UpdateHighlightArea(viewport);                
            }
        }

        public RectangleF Viewport
        {
            get { return (RectangleF)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register("Viewport", typeof(RectangleF), typeof(AnalysisView),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(SynchronizeViewport)));
    }
}
