using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.ViewModels.Datasets;
using MultiAlign.ViewModels.Features;
using MultiAlign.ViewModels.Spectra;
using MultiAlign.ViewModels.TreeView;
using MultiAlign.ViewModels.Viewers;
using MultiAlign.ViewModels.Wizard;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO;
using OxyPlot;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmicsViz.Drawing;

namespace MultiAlign.ViewModels
{
    public class AnalysisViewModel : ViewModelBase
    {
        private AnalysisOptionsViewModel m_analysisOptionsViewModel;
        private UMCClusterIdentificationViewModel m_clusterIdentificationViewModel;
        private UmcClusterSpectraViewModel m_clusterSpectraViewModel;
        private UmcClusterCollectionTreeViewModel m_clusterTreeModel;
        private ClusterDetailViewModel m_clusterViewModel;
        private PlotBase m_clustersPlotModel;
        private GlobalStatisticsViewModel m_globalStatistics;
        private bool m_hasIdentifications;        
        private ObservableCollection<MassTagToCluster> m_massTags;
        private string m_selectedClusterName;
        private bool m_showDriftTime;
        private RectangleF m_viewport;

        public AnalysisViewModel(MultiAlignAnalysis analysis)
        {
            m_showDriftTime = false;

            LoadDatasets(analysis);

            // Create matching clusters and AMT Matches.
            var matches = analysis.DataProviders.MassTagMatches.FindAll();
            var clusters =
                analysis.Clusters.MapMassTagsToClusters(matches, analysis.MassTagDatabase);

            // Cache the clusters so that they can be readily accessible later on.
            // This will help speed up performance, so that we dont have to hit the database
            // when we want to find matching mass tags, and dont have to map clusters to tags multiple times.
            FeatureCacheManager<UMCClusterLightMatched>.SetFeatures(clusters.Item1);
            FeatureCacheManager<MassTagToCluster>.SetFeatures(clusters.Item2);
            SingletonDataProviders.Providers = analysis.DataProviders;


            // Create sub-view models
            MassTags = new ObservableCollection<MassTagToCluster>(clusters.Item2);
            ClusterTree = new UmcClusterCollectionTreeViewModel(clusters.Item1);
            ClusterSpectraViewModel = new UmcClusterSpectraViewModel();
            ClusterIdentificationViewModel = new UMCClusterIdentificationViewModel();
            AnalysisOptionsViewModel = new AnalysisOptionsViewModel(analysis.Options);
            ClusterViewModel = new ClusterDetailViewModel();

            var charges = SingletonDataProviders.Providers.FeatureCache.RetrieveChargeStates();

            GlobalStatisticsViewModel = new GlobalStatisticsViewModel(clusters.Item1, charges);
            HasIdentifications = (MassTags.Count > 0);


            SelectedClusterName = "Cluster Details:";
            LoadClusters(clusters.Item1);
            ApplyViewAsFilter = new BaseCommand(FilterFromView);
        }

        public ICommand ApplyViewAsFilter { get; set; }


        public PlotBase ClustersPlotModel
        {
            get { return m_clustersPlotModel; }
            set
            {
                if (value == null || value == m_clustersPlotModel)
                    return;

                m_clustersPlotModel = value;
                OnPropertyChanged("ClustersPlotModel");
            }
        }

        private void FilterFromView()
        {
            if (m_clustersPlotModel == null)
                return;


            var model = m_clustersPlotModel.Model;
            //var viewport = m_clusterChart.ViewPort;
            var mass = new FilterRange(model.DefaultYAxis.ActualMinimum, model.DefaultYAxis.ActualMaximum);
            var net = new FilterRange(model.DefaultXAxis.ActualMinimum, model.DefaultXAxis.ActualMaximum);
            m_clusterTreeModel.Filter(mass, net);
        }

        #region Loading 

        private void LoadClusters(IEnumerable<UMCClusterLightMatched> clusters)
        {
            var clustersOnly = clusters.Select(cluster => cluster.Cluster).ToList();
            ClustersPlotModel = ScatterPlotFactory.CreateClusterMassScatterPlot(clustersOnly);
        }

        private void LoadDatasets(MultiAlignAnalysis analysis)
        {
            var datasets = analysis.MetaData.Datasets.ToList();

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
            var plotPath = Path.Combine(analysis.MetaData.AnalysisPath, "plots");
            if (Directory.Exists(plotPath))
            {
                var loader = new DatasetPlotLoader();
                loader.LoadDatasetPlots(plotPath, analysis.MetaData.Datasets.ToList());
            }

            Datasets = new DatasetCollectionViewModel(datasets);
        }

        #endregion

        #region Cluster Tree Event Handlers 

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
        ///     Updates the global view of clusters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void value_ClustersFiltered(object sender, ClustersUpdatedEventArgs e)
        {
            var clusters = new List<UMCClusterLightMatched>();
            foreach (var cluster in e.Clusters)
            {
                clusters.Add(cluster.Cluster);
            }
            LoadClusters(clusters);
        }

        /// <summary>
        ///     Handles loading data when a cluster is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void value_ClusterSelected(object sender, ClusterSelectedEventArgs e)
        {
            if (e != null)
            {
                m_clusterSpectraViewModel.SelectedCluster = e.Cluster.Cluster;
                m_clusterIdentificationViewModel.SelectedCluster = e.Cluster.Cluster;
                m_clusterViewModel.SelectedCluster = e.Cluster;
                SelectedClusterName = string.Format("Cluster Details: {0}", e.Cluster.ClusterId);
            }
        }

        /// <summary>
        ///     Handles updating the cluster detail with the appropriate data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void value_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            if (e != null)
            {
                m_clusterViewModel.SelectedFeature = new UMCTreeViewModel(e.Feature);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a list of the mass tags loaded.
        /// </summary>
        public ObservableCollection<MassTagToCluster> MassTags
        {
            get { return m_massTags; }
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
        ///     Gets a list of the datasets loaded
        /// </summary>
        public DatasetCollectionViewModel Datasets { get; private set; }

        /// <summary>
        ///     Gets or sets whether the analysis used drift time.
        /// </summary>
        public bool UsesDriftTime
        {
            get { return m_showDriftTime; }
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
        ///     Gets or sets whether the analysis has identifications
        /// </summary>
        public bool HasIdentifications
        {
            get { return m_hasIdentifications; }
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
            get { return m_viewport; }
            set
            {
                if (value != m_viewport)
                {
                    m_viewport = value;
                    OnPropertyChanged("ViewPort");
                }
            }
        }

        #endregion

        #region ViewModels

        /// <summary>
        ///     Gets the cluster detail view model
        /// </summary>
        public ClusterDetailViewModel ClusterViewModel
        {
            get { return m_clusterViewModel; }
            private set { m_clusterViewModel = value; }
        }

        /// <summary>
        ///     Gets the identification tree view model
        /// </summary>
        public IdentificationCollectionTreeViewModel IdentificationTree { get; set; }

        public GlobalStatisticsViewModel GlobalStatisticsViewModel
        {
            get { return m_globalStatistics; }
            set
            {
                if (m_globalStatistics == value)
                    return;

                m_globalStatistics = value;
                OnPropertyChanged("GlobalStatisticsViewModel");
            }
        }

        /// <summary>
        ///     Gets the cluster tree view model
        /// </summary>
        public UmcClusterCollectionTreeViewModel ClusterTree
        {
            get { return m_clusterTreeModel; }
            set
            {
                if (value != null && value != m_clusterTreeModel)
                {
                    m_clusterTreeModel = value;
                    value.FeatureSelected += value_FeatureSelected;
                    value.ClustersFiltered += value_ClustersFiltered;
                    value.ClusterSelected += value_ClusterSelected;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the view model for a selected cluster.
        /// </summary>
        public AnalysisOptionsViewModel AnalysisOptionsViewModel
        {
            get { return m_analysisOptionsViewModel; }
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
        ///     Gets or sets the view model for a selected cluster.
        /// </summary>
        public UmcClusterSpectraViewModel ClusterSpectraViewModel
        {
            get { return m_clusterSpectraViewModel; }
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
        ///     Gets or sets the Cluster Identification View Model
        /// </summary>
        public UMCClusterIdentificationViewModel ClusterIdentificationViewModel
        {
            get { return m_clusterIdentificationViewModel; }
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