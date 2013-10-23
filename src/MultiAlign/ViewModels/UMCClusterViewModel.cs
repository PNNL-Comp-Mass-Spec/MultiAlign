
using MultiAlign.IO;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using System.Collections.ObjectModel;

namespace MultiAlign.ViewModels
{    
    /// <summary>
    /// Cluster Tree View Model 
    /// </summary>
    public class UMCClusterViewModel : TreeItemViewModel
    {
        /// <summary>
        ///  The cluster in question
        /// </summary>
        UMCClusterLightMatched              m_cluster;
        ObservableCollection<UMCCollectionTreeViewModel> m_items;

        protected ObservableCollection<UMCCollectionTreeViewModel>  m_features;
        protected ObservableCollection<DatabaseSearchIdentificationsTreeViewModel>  m_identifications;
        protected ObservableCollection<MassTagCollectionMatchViewModel>       m_massTags;
        protected ObservableCollection<MsMsCollectionTreeViewModel>           m_msmsSpectra;

        public UMCClusterViewModel(UMCClusterLightMatched cluster):
            this(cluster, null)
        {
        }
        
        public UMCClusterViewModel(UMCClusterLightMatched cluster, TreeItemViewModel parent)
        {
            m_parent   = parent;
            m_cluster  = cluster;
            UMCCollectionTreeViewModel features = new UMCCollectionTreeViewModel(cluster.Cluster.Features);


            m_items = new ObservableCollection<UMCCollectionTreeViewModel>();
            m_items.Add(features);

            m_identifications   = new ObservableCollection<DatabaseSearchIdentificationsTreeViewModel>();
            m_massTags          = new ObservableCollection<MassTagCollectionMatchViewModel>();
            m_msmsSpectra       = new ObservableCollection<MsMsCollectionTreeViewModel>();
        }

        public ObservableCollection<UMCCollectionTreeViewModel> Features
        {
            get
            {
                return m_items; 
            }
        }
        public ObservableCollection<DatabaseSearchIdentificationsTreeViewModel> Identifications
        {
            get
            {
                return m_identifications;
            }
        }
        public ObservableCollection<MassTagCollectionMatchViewModel> MassTags
        {
            get
            {
                return m_massTags;
            }
        }

        public ObservableCollection<MsMsCollectionTreeViewModel> MsMsSpectra
        {
            get
            {
                return m_msmsSpectra;
            }
        }


        public int DatasetMemberCount
        {
            get
            {
                return m_cluster.Cluster.DatasetMemberCount;
            }
        }
        public int FeatureCount
        {
            get
            {
                return m_cluster.Cluster.MemberCount;
            }
        }

        public int MatchCount
        {
            get
            {
                return m_cluster.ClusterMatches.Count;
            }
        }

        
        /// <summary>
        /// Gets the ID of the cluster.
        /// </summary>
        public string ClusterId         
        { 
            get { return m_cluster.Cluster.ID.ToString();  } 
        }

        public override void LoadChildren()
        {
            m_cluster.Cluster.ReconstructUMCCluster(SingletonDataProviders.Providers, false, false);            
        }
    }


}
