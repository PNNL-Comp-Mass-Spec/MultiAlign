
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data.Features;
using System;
using MultiAlign.Commands.Clusters;
using System.Windows.Input;

namespace MultiAlign.ViewModels.TreeView
{
    /// <summary>
    /// Root tree object.
    /// </summary>
    public class UMCClusterCollectionTreeViewModel : TreeItemViewModel
    {
        private UMCLight                        m_selectedFeature;
        private UMCClusterLightMatched          m_selectedCluster;
        private ClusterTreeFilterCommand        m_command;

        readonly List<UMCClusterLightMatched>   m_clusters;
        private ObservableCollection<UMCClusterTreeViewModel> m_filteredClusters;

        public UMCClusterCollectionTreeViewModel(List<UMCClusterLightMatched> clusters)
            : this(clusters, null)
        {
        }

        public UMCClusterCollectionTreeViewModel(List<UMCClusterLightMatched> clusters, UMCClusterTreeViewModel parent)
        {
            m_clusters          = clusters;
            m_filteredClusters  = new ObservableCollection<UMCClusterTreeViewModel>(
                (from cluster in clusters
                 select new UMCClusterTreeViewModel(cluster)).ToList());

            m_command           = new ClusterTreeFilterCommand(this);

            RegisterEvents();
        }
        /// <summary>
        /// Makes sure that we have event handlers to know when a feature was selected below us....
        /// probably a better way through bubble-based command routing...
        /// </summary>
        void RegisterEvents()
        {
            foreach (UMCClusterTreeViewModel cluster in m_filteredClusters)
            {
                cluster.Selected += new System.EventHandler(cluster_Selected);
                cluster.FeatureSelected += new EventHandler<FeatureSelectedEventArgs>(cluster_FeatureSelected);
            }
        }

        void cluster_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            SelectedFeature = e.Feature;
        }

        void cluster_Selected(object sender, System.EventArgs e)
        {
            UMCClusterTreeViewModel model   = sender as UMCClusterTreeViewModel;
            model.LoadChildren();        
            SelectedCluster                 = model.Cluster;


            if (model.Cluster.Cluster.Features.Count > 0)
            {
                SelectedFeature = model.Cluster.Cluster.Features[0];
            }            
        }

        public ICommand FilterCommand 
        {
            get
            {
                return m_command;
            }

        }

        public List<UMCClusterLightMatched> Clusters
        {
            get
            {
                return m_clusters;
            }
        }

        public ObservableCollection<UMCClusterTreeViewModel> FilteredClusters
        {
            get
            {
                return m_filteredClusters;
            }
        }

        public UMCClusterLightMatched SelectedCluster 
        {
            get
            {
                return m_selectedCluster;
            }
            set
            {
                if (value != m_selectedCluster)
                {
                    m_selectedCluster = value;
                    OnPropertyChanged("SelectedCluster");
                }
            }            
        }
        public UMCLight SelectedFeature
        {
            get
            {
                return m_selectedFeature;
            }
            set
            {
                if (value != m_selectedFeature)
                {
                    m_selectedFeature = value;
                    OnPropertyChanged("SelectedFeature");
                }
            }   
        }

        public override void LoadChildren()
        {

        }

        public void ResetClusters(IEnumerable<UMCClusterLightMatched> clusters)
        {
            m_filteredClusters.Clear();

            foreach (var cluster in clusters)
            {
                m_filteredClusters.Add(new UMCClusterTreeViewModel(cluster));
            }
            RegisterEvents();
        }
    }
}
