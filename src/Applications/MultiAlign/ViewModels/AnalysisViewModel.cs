using MultiAlign.Commands;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.ViewModels.Spectra;
using MultiAlign.ViewModels.TreeView;
using MultiAlign.ViewModels.Viewers;
using MultiAlign.ViewModels.Wizard;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Extensions;
using MultiAlignCustomControls.Charting;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace MultiAlign.ViewModels
{
    public class AnalysisViewModel: ViewModelBase
    {        
        private RectangleF                              m_viewport; 
        private bool                                    m_hasIdentifications;
        private UmcClusterCollectionTreeViewModel       m_clusterTreeModel;
        private IdentificationCollectionTreeViewModel   m_identificationTreeView;
        private bool                                    m_showDriftTime;
        private MultiAlignAnalysis                      m_analysis;
        private ctlClusterChart                         m_clusterChart;
        UmcClusterSpectraViewModel                      m_clusterSpectraViewModel;
        UMCClusterIdentificationViewModel               m_clusterIdentificationViewModel;
        private ObservableCollection<MassTagToCluster>  m_massTags;
        private AnalysisOptionsViewModel       m_analysisOptionsViewModel;
        private WindowsFormsHost                        m_host;
        private ClusterDetailViewModel                        m_clusterViewModel;
        private RectangleF m_savedClusterViewPort;
        private string m_selectedClusterName;
        private Viewers.GlobalStatisticsViewModel m_globalStatistics;

        public AnalysisViewModel(MultiAlignAnalysis analysis)
        {
            m_showDriftTime = false;
            m_analysis      = analysis;

            LoadDatasets(analysis);

            /// Create matching clusters and AMT Matches.
            List<ClusterToMassTagMap> matches = m_analysis.DataProviders.MassTagMatches.FindAll();
            Tuple<List<UMCClusterLightMatched>, List<MassTagToCluster>> clusters =
                                analysis.Clusters.MapMassTagsToClusters(matches, m_analysis.MassTagDatabase);

            // Cache the clusters so that they can be readily accessible later on.
            // This will help speed up performance, so that we dont have to hit the database
            // when we want to find matching mass tags, and dont have to map clusters to tags multiple times.
            FeatureCacheManager<UMCClusterLightMatched>.SetFeatures(clusters.Item1);
            FeatureCacheManager<MassTagToCluster>.SetFeatures(clusters.Item2);
            SingletonDataProviders.Providers    = m_analysis.DataProviders;

            
            // Create sub-view models
            MassTags                            = new ObservableCollection<MassTagToCluster>(clusters.Item2);
            ClusterTree                         = new UmcClusterCollectionTreeViewModel(clusters.Item1);   
            ClusterSpectraViewModel             = new UmcClusterSpectraViewModel();
            ClusterIdentificationViewModel      = new UMCClusterIdentificationViewModel();
            AnalysisOptionsViewModel            = new AnalysisOptionsViewModel(analysis.Options);
            ClusterViewModel                    = new ClusterDetailViewModel();

            var charges = SingletonDataProviders.Providers.FeatureCache.RetrieveChargeStates();            

            GlobalStatisticsViewModel           = new GlobalStatisticsViewModel(clusters.Item1, charges);
            HasIdentifications                  = (MassTags.Count > 0);

            m_clusterChart                      = new ctlClusterChart
            {
                Title = "",
                LegendVisible = false,
                YAxisShortHand = "ppm",
                XAxisShortHand = "NET"
            };
            SelectedClusterName                 = "Cluster Details:";
            LoadClusters(clusters.Item1);
            m_clusterChart.AutoViewPort();

            ApplyViewAsFilter = new BaseCommandBridge(FilterFromView);

            ClusterChart = new WindowsFormsHost() { Child = m_clusterChart };            
        }

        private void FilterFromView(object parameter)
        {
            var viewport = m_clusterChart.ViewPort;
            var mass     = new FilterRange(viewport.Y, viewport.Bottom);
            var net      = new FilterRange(viewport.X, viewport.X + viewport.Right);
            m_clusterTreeModel.Filter(mass, net);
        }

        #region Loading 
        private void LoadClusters(IEnumerable<UMCClusterLightMatched> clusters)
        {
            List<UMCClusterLight> clustersOnly = new List<UMCClusterLight>();
            foreach (var cluster in clusters)            
                clustersOnly.Add(cluster.Cluster);

            m_clusterChart.ClearData();
            m_clusterChart.AddClusters(clustersOnly, false);
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
                m_clusterSpectraViewModel.SelectedCluster        = e.Cluster.Cluster;                
                m_clusterIdentificationViewModel.SelectedCluster = e.Cluster.Cluster;
                m_clusterViewModel.SelectedCluster               = e.Cluster;
                SelectedClusterName                              = string.Format("Cluster Details: {0}", e.Cluster.ClusterId);
            }
        }

        public string SelectedClusterName
        {
            get { return m_selectedClusterName; }
            set
            {
                m_selectedClusterName = value;
                OnPropertyChanged("SelectedClusterName");
            }
        }

        /// <summary>
        /// Handles updating the cluster detail with the appropriate data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void value_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            
            if (e != null)
            {
                m_clusterViewModel.SelectedFeature = new UMCTreeViewModel(e.Feature);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a list of the mass tags loaded.
        /// </summary>
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
        /// <summary>
        /// Gets a list of the datasets loaded
        /// </summary>
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

        /// <summary>
        /// Gets the control used for displaying a global view of the clusters
        /// </summary>
        public WindowsFormsHost ClusterChart
        {
            get
            {
                return m_host;
            }
            private set
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
        /// Gets the cluster detail view model
        /// </summary>
        public ClusterDetailViewModel ClusterViewModel
        {
            get
            {
                return m_clusterViewModel;
            }
            private set
            {
                m_clusterViewModel = value;
            }
        }
        /// <summary>
        /// Gets the identification tree view model
        /// </summary>
        public IdentificationCollectionTreeViewModel IdentificationTree
        {
            get
            {
                return m_identificationTreeView;
            }
            set
            {
                m_identificationTreeView = value;
            }
        }

        public GlobalStatisticsViewModel GlobalStatisticsViewModel
        {
            get
            {
                return m_globalStatistics;
            }
            set 
            { 
                if (m_globalStatistics == value) 
                    return;

                m_globalStatistics = value;
                OnPropertyChanged("GlobalStatisticsViewModel");
            }
        }
        /// <summary>
        /// Gets the cluster tree view model
        /// </summary>
        public UmcClusterCollectionTreeViewModel ClusterTree
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
                    value.FeatureSelected += value_FeatureSelected;
                    value.ClustersFiltered += new EventHandler<ClustersUpdatedEventArgs>(value_ClustersFiltered);
                    value.ClusterSelected += new EventHandler<ClusterSelectedEventArgs>(value_ClusterSelected);
                }
            }
        }        
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
        public UmcClusterSpectraViewModel ClusterSpectraViewModel
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


        public ICommand ApplyViewAsFilter { get; set; }

    }
}
