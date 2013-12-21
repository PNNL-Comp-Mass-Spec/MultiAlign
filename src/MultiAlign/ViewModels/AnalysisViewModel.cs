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

namespace MultiAlign.ViewModels
{
    public class AnalysisViewModel: ViewModelBase
    {        
        private RectangleF                          m_viewport; 
        private bool                                m_hasIdentifications;
        private UMCClusterCollectionTreeViewModel   m_clusterTreeModel;
        private IdentificationCollectionTreeViewModel m_identificationTreeView;
        private bool                                m_showDriftTime;
        private MultiAlignAnalysis                  m_analysis;

        UMCClusterSpectraViewModel m_clusterSpectraViewModel;
        UMCClusterIdentificationViewModel m_clusterIdentificationViewModel;


        private ObservableCollection<MassTagToCluster> m_massTags;

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

            Binding spectraBinding  = new Binding();
            spectraBinding.Mode = BindingMode.TwoWay;
            spectraBinding.NotifyOnSourceUpdated = true;
            spectraBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            spectraBinding.Source = ClusterTree;
            

            // Make the dataset plots.                    
            string plotPath = Path.Combine(analysis.MetaData.AnalysisPath, "plots");
            if (Directory.Exists(plotPath))
            {
                DatasetPlotLoader loader = new Data.DatasetPlotLoader();
                loader.LoadDatasetPlots(plotPath, analysis.MetaData.Datasets.ToList());
            }
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

            Datasets = new DatasetCollectionViewModel(datasets);
        }

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
                    value.ClusterSelected += new EventHandler<ClusterSelectedEventArgs>(value_ClusterSelected);
                }
            }
        }

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
    }
}
