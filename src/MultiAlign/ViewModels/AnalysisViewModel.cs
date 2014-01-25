using System;
using System.Collections.Generic;
using MultiAlign.ViewModels.TreeView;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlign.IO;
using System.Collections.ObjectModel;
using MultiAlign.Data;
using System.IO;
using System.Drawing;
using PNNLOmics.Data;
using System.Windows.Data;
using MultiAlign.ViewModels.Analysis;
using System.Windows.Forms.Integration;
using MultiAlignCustomControls.Charting;
using PNNLOmics.Data.Features;

namespace MultiAlign.ViewModels
{
    public class AnalysisViewModel: ViewModelBase
    {        
        private RectangleF                              m_viewport; 
        private bool                                    m_hasIdentifications;
        private UMCClusterCollectionTreeViewModel       m_clusterTreeModel;
        private IdentificationCollectionTreeViewModel   m_identificationTreeView;
        private bool                                    m_showDriftTime;
        private MultiAlignAnalysis                      m_analysis;
        private ctlClusterChart                         m_clusterChart;
        UMCClusterSpectraViewModel                      m_clusterSpectraViewModel;
        UMCClusterIdentificationViewModel               m_clusterIdentificationViewModel;
        private ObservableCollection<MassTagToCluster>  m_massTags;
        private Analysis.AnalysisOptionsViewModel       m_analysisOptionsViewModel;
        private WindowsFormsHost                        m_host;

        public AnalysisViewModel(MultiAlignAnalysis analysis)
        {
            m_showDriftTime = false;
            m_analysis      = analysis;


            LoadDatasets(analysis);

            /// Create matching clusters and AMT Matches.
            List<ClusterToMassTagMap> matches = m_analysis.DataProviders.MassTagMatches.FindAll();
            Tuple<List<UMCClusterLightMatched>, List<MassTagToCluster>> clusters =
                                analysis.Clusters.MapMassTagsToClusters(matches, m_analysis.MassTagDatabase);

            /// 
            /// Cache the clusters so that they can be readily accessible later on.
            /// This will help speed up performance, so that we dont have to hit the database
            /// when we want to find matching mass tags, and dont have to map clusters to tags multiple times.
            ///                     
            FeatureCacheManager<UMCClusterLightMatched>.SetFeatures(clusters.Item1);
            FeatureCacheManager<MassTagToCluster>.SetFeatures(clusters.Item2);
            SingletonDataProviders.Providers = m_analysis.DataProviders;

            MassTags           = new ObservableCollection<MassTagToCluster>(clusters.Item2);
            HasIdentifications = (MassTags.Count > 0);

            ClusterTree         = new UMCClusterCollectionTreeViewModel(clusters.Item1);           
            OnPropertyChanged("ClusterTree");
            
            // Create sub-view models
            ClusterSpectraViewModel         = new UMCClusterSpectraViewModel();
            ClusterIdentificationViewModel  = new UMCClusterIdentificationViewModel();
            AnalysisOptionsViewModel        = new AnalysisOptionsViewModel(analysis.Options);

            Binding spectraBinding               = new Binding();
            spectraBinding.Mode                  = BindingMode.TwoWay;
            spectraBinding.NotifyOnSourceUpdated = true;
            spectraBinding.UpdateSourceTrigger   = UpdateSourceTrigger.PropertyChanged;
            spectraBinding.Source = ClusterTree;

            m_clusterChart = new ctlClusterChart();
            m_clusterChart.Title = "";
            m_clusterChart.LegendVisible = false;
            m_clusterChart.YAxisShortHand = "ppm";
            m_clusterChart.XAxisShortHand = "NET";
            LoadClusters(clusters.Item1);

            ClusterChart = new WindowsFormsHost() { Child = m_clusterChart };            
        }

        #region Loading 
        private void LoadClusters(IEnumerable<UMCClusterLightMatched> clusters)
        {
            List<UMCClusterLight> clustersOnly = new List<UMCClusterLight>();
            foreach (var cluster in clusters)            
                clustersOnly.Add(cluster.Cluster);

            m_clusterChart.ClearData();
            m_clusterChart.AddClusters(clustersOnly);
        }
        private void LoadDatasets(MultiAlignAnalysis analysis)
        {
            List<DatasetInformation> datasets = analysis.MetaData.Datasets.ToList();

            // Sort the datasets for the view...
            datasets.Sort(delegate(DatasetInformation x, DatasetInformation y)
            {
                if (x.DatasetId == y.DatasetId)
                    return 0;

                if (x.IsBaseline)
                    return -1;

                return x.DatasetName.CompareTo(y.DatasetName);
            });

            // Make the dataset plots.                    
            string plotPath = Path.Combine(analysis.MetaData.AnalysisPath, "plots");
            if (Directory.Exists(plotPath))
            {
                DatasetPlotLoader loader = new Data.DatasetPlotLoader();
                loader.LoadDatasetPlots(plotPath, analysis.MetaData.Datasets.ToList());
            }

            Datasets = new DatasetCollectionViewModel(datasets);
        }
        #endregion

        #region Cluster Tree Event Handlers 
        /// <summary>
        /// Updates the global view of clusters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void value_ClustersFiltered(object sender, ClustersUpdatedEventArgs e)
        {
            List<UMCClusterLightMatched> clusters = new List<UMCClusterLightMatched>();
            foreach (var cluster in e.Clusters)
            {
                clusters.Add(cluster.Cluster);
            }
            LoadClusters(clusters);
        }
        /// <summary>
        /// Handles loading data when a cluster is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void value_ClusterSelected(object sender, ClusterSelectedEventArgs e)
        {
            if (e != null)
            {
                if (m_clusterSpectraViewModel != null)
                {
                    m_clusterSpectraViewModel.SelectedCluster = e.Cluster;
                }
                if (m_clusterIdentificationViewModel != null)
                {
                    m_clusterIdentificationViewModel.SelectedCluster = e.Cluster;
                }
            }
        }
        #endregion

        #region Properties
        public IdentificationCollectionTreeViewModel IdentificationTree
        {
            get
            {
                return m_identificationTreeView;
            }
            set{
                m_identificationTreeView = value;
            }
        }


        public UMCClusterCollectionTreeViewModel ClusterTree
        {
            get
            {
                return m_clusterTreeModel;
            }
            set
            {
                
                if (value != null && value != m_clusterTreeModel)
                {
                    m_clusterTreeModel = value;
                    value.ClustersFiltered += new EventHandler<ClustersUpdatedEventArgs>(value_ClustersFiltered);
                    value.ClusterSelected  += new EventHandler<ClusterSelectedEventArgs>(value_ClusterSelected);
                }
            }
        }


        void value_Selected(object sender, EventArgs e)
        {
            
        }

        public ObservableCollection<MassTagToCluster> MassTags
        {
            get
            {
                return m_massTags;
            }
            set
            {
                if (value != m_massTags)
                {
                    m_massTags = value;
                    OnPropertyChanged("MassTags");
                }
            }
        }

        public DatasetCollectionViewModel Datasets
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets whether the analysis used drift time.
        /// </summary>
        public bool UsesDriftTime
        {
            get
            {
                return m_showDriftTime;
            }
            set
            {
                if (value != m_showDriftTime)
                {
                    m_showDriftTime = value;
                    OnPropertyChanged("UsesDriftTime");
                }
            }
        }
        /// <summary>
        /// Gets or sets whether the analysis has identifications
        /// </summary>
        public bool HasIdentifications 
        {
            get
            {
                return m_hasIdentifications;
            }
            set
            {
                if (value != m_hasIdentifications)
                {
                    m_hasIdentifications = value;
                    OnPropertyChanged("HasIdentifications");
                }
            }
        }

        public RectangleF ViewPort 
        {
            get
            {
                return m_viewport;
            }
            set
            {
                if (value != m_viewport)
                {
                    m_viewport = value;
                    OnPropertyChanged("ViewPort");
                }
            }
        }

        public WindowsFormsHost ClusterChart
        {
            get
            {
                return m_host;
            }
            set
            {
                if (value != null && m_host != value)
                {
                    m_host = value;
                    OnPropertyChanged("ClusterChart");
                }
            }
        }
        #endregion

        #region ViewModels
        /// <summary>
        /// Gets or sets the view model for a selected cluster.
        /// </summary>
        public AnalysisOptionsViewModel AnalysisOptionsViewModel
        {
            get
            {
                return m_analysisOptionsViewModel;
            }
            set
            {
                if (m_analysisOptionsViewModel != value)
                {
                    m_analysisOptionsViewModel = value;
                    OnPropertyChanged("AnalysisOptionsViewModel");
                }
            }
        }
        /// <summary>
        /// Gets or sets the view model for a selected cluster.
        /// </summary>
        public UMCClusterSpectraViewModel ClusterSpectraViewModel
        {
            get
            {
                return m_clusterSpectraViewModel;
            }
            set
            {
                if (m_clusterSpectraViewModel != value)
                {
                    m_clusterSpectraViewModel = value;
                    OnPropertyChanged("ClusterSpectraViewModel");
                }
            }
        }
        /// <summary>
        /// Gets or sets the Cluster Identification View Model
        /// </summary>
        public UMCClusterIdentificationViewModel ClusterIdentificationViewModel
        {
            get
            {
                return m_clusterIdentificationViewModel;
            }
            set
            {
                if (m_clusterIdentificationViewModel != value)
                {
                    m_clusterIdentificationViewModel = value;
                    OnPropertyChanged("ClusterIdentificationViewModel");
                }
            }
        }
        #endregion
    }
}
