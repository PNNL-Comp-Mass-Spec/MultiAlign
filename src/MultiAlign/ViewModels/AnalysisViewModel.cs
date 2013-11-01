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

namespace MultiAlign.ViewModels
{
    public class AnalysisViewModel: ViewModelBase
    {        
        private RectangleF                          m_viewport; 
        private bool                                m_hasIdentifications;
        private UMCClusterCollectionTreeViewModel   m_clusterTreeModel;
        private bool                                m_showDriftTime;
        private MultiAlignAnalysis                  m_analysis;


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

            ClusterTree   = new UMCClusterCollectionTreeViewModel(clusters.Item1);
            OnPropertyChanged("ClusterTree");


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

            Datasets = new ObservableCollection<DatasetInformation>(datasets);
        }

        public UMCClusterCollectionTreeViewModel ClusterTree
        {
            get
            {
                return m_clusterTreeModel;
            }
            set
            {
                m_clusterTreeModel = value;
            }
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

        public ObservableCollection<DatasetInformation> Datasets
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

    }
}
