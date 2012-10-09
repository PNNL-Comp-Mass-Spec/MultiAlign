using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.Features;

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
                    //TODO: replace this with bindings!!!!
                    
                    List<ClusterToMassTagMap> matches = m_analysis.DataProviders.MassTagMatches.FindAll();
                    Tuple<List<UMCClusterLightMatched>, List<MassTagToCluster>> clusters = 
                        value.Clusters.MapMassTagsToClusters(matches, m_analysis.MassTagDatabase);

                    m_clusterGrid.Clusters = clusters.Item1;                   
                    m_clusterPlot.SetClusters(value.Clusters);
                    m_massTagViewer.MatchedTags = new System.Collections.ObjectModel.ObservableCollection<MassTagToCluster>(clusters.Item2);

                    /// 
                    /// Cache the clusters so that they can be readily accessible later on.
                    /// This will help speed up performance, so that we dont have to hit the database
                    /// when we want to find matching mass tags, and dont have to map clusters to tags multiple times.
                    ///                     
                    FeatureCacheManager<UMCClusterLightMatched>.SetFeatures(clusters.Item1);
                    FeatureCacheManager<MassTagToCluster>.SetFeatures(clusters.Item2);

                    m_clusterControl.Providers  = m_analysis.DataProviders;
                    m_massTagViewer.Providers   = m_analysis.DataProviders;

                    if (value.MassTagDatabase != null)
                    {
                        m_massTagViewer.Database = value.MassTagDatabase;
                    }
                    m_msmsViewer.Analysis       = value;
                    m_clusterControl.Analysis   = value;
                    m_datasetBox.ItemsSource    = new System.Collections.ObjectModel.ObservableCollection<DatasetInformation>(value.MetaData.Datasets);

                    //m_msmsViewer.ExtractMsMsData(m_analysis.DataProviders);

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
                    Dictionary<int, int> map    = value.Clusters.CreateChargeMap<UMCClusterLight>();
                    m_chargeStates.ConstructHistogram(map);
                    m_chargeStates.AutoViewPort();

                    Dictionary<int, int> datasetMap = value.Clusters.CreateClusterDatasetMemeberSizeHistogram();
                    m_datasetSizeHistogram.ConstructHistogram(datasetMap);
                    m_datasetSizeHistogram.AutoViewPort();

                    Dictionary<int, int> sizeMap = value.Clusters.CreateClusterSizeHistogram();
                    m_clusterSizeHistogram.ConstructHistogram(sizeMap);
                    m_clusterSizeHistogram.AutoViewPort();

                    Dictionary<int, int> massTagMap = clusters.Item2.CreateMassTagClusterSizeHistogram();
                    m_massTagHistogram.ConstructHistogram(massTagMap);
                    m_massTagHistogram.AutoViewPort();

                    m_clusterRatioPlot.AddClusters(value.Clusters);
                    m_clusterRatioPlot.UpdateCharts(true);
                    m_clusterRatioRawPlot.AddClusters(value.Clusters);
                    m_clusterRatioRawPlot.UpdateCharts(true);
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
