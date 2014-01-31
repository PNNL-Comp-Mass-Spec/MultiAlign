
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data.Features;
using System;
using MultiAlign.Commands.Clusters;
using System.Windows.Input;
using PNNLOmics.Data;

namespace MultiAlign.ViewModels.TreeView
{
    /// <summary>
    /// Root tree object.
    /// </summary>
    public class UMCClusterCollectionTreeViewModel : TreeItemViewModel
    {
        public event EventHandler<ClusterSelectedEventArgs> ClusterSelected;
        public event EventHandler<ClustersUpdatedEventArgs> ClustersFiltered;

        private UMCLight                        m_selectedFeature;
        private UMCClusterLightMatched          m_selectedCluster;
        private MSSpectra                       m_selectedSpectrum;
        private ClusterTreeFilterCommand        m_command;
        private ClusterExportCommand            m_exportCommand;

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
            m_exportCommand     = new ClusterExportCommand(this);

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
                cluster.Selected         += new System.EventHandler(cluster_Selected);
                cluster.FeatureSelected  += new EventHandler<FeatureSelectedEventArgs>(cluster_FeatureSelected);
                cluster.SpectrumSelected += new EventHandler<IdentificationFeatureSelectedEventArgs>(cluster_SpectrumSelected);
            }
        }

        void cluster_SpectrumSelected(object sender, IdentificationFeatureSelectedEventArgs e)
        {
            SelectedSpectrum = e.Spectrum;
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

            if (ClusterSelected != null)
            {
                ClusterSelected(this, new ClusterSelectedEventArgs(model));
            }

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
        public ICommand ExportCommand
        {
            get
            {
                return m_exportCommand;
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
        public MSSpectra SelectedSpectrum
        {
            get
            {
                return m_selectedSpectrum;
            }
            set
            {
                if (value != m_selectedSpectrum)
                {
                    m_selectedSpectrum = value;
                    OnPropertyChanged("SelectedSpectrum");
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
            m_filteredClusters.OrderBy(x => x.Cluster.Cluster.MemberCount);
            RegisterEvents();

            if (ClustersFiltered != null)
            {
                ClustersFiltered(this, new ClustersUpdatedEventArgs(m_filteredClusters));
            }
        }
    }

    public class ClustersUpdatedEventArgs: EventArgs
    {
        public ClustersUpdatedEventArgs(ObservableCollection<UMCClusterTreeViewModel> clusters)
        {
            Clusters = clusters;
        }

        public ObservableCollection<UMCClusterTreeViewModel> Clusters { get;  private set; }
    }
}
