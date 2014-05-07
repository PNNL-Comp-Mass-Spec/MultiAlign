
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.ViewModels.Features;
using MultiAlign.ViewModels.IO;
using MultiAlign.Windows.IO;
using MultiAlign.Windows.Viewers.Clusters;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace MultiAlign.ViewModels.TreeView
{
    /// <summary>
    /// Root tree object.
    /// </summary>
    public class UmcClusterCollectionTreeViewModel : TreeItemViewModel
    {
        public event EventHandler<ClusterSelectedEventArgs> ClusterSelected;
        public event EventHandler<ClustersUpdatedEventArgs> ClustersFiltered;

        private UMCLight                        m_selectedFeature;
        private UMCClusterLightMatched          m_selectedCluster;
        private MSSpectra                       m_selectedSpectrum;
        private readonly ICommand               m_exportCommand;

        readonly List<UMCClusterLightMatched>   m_clusters;
        private readonly ObservableCollection<UMCClusterTreeViewModel> m_filteredClusters;


        private readonly UMCClusterFilterViewModel           m_filterViewModel;
        private ClusterFilterWindow                 m_newWindow;  


        public UmcClusterCollectionTreeViewModel(List<UMCClusterLightMatched> clusters)
            : this(clusters, null)
        {
        }

        public UmcClusterCollectionTreeViewModel(List<UMCClusterLightMatched> clusters, UMCClusterTreeViewModel parent)
        {
            m_clusters          = clusters;
            m_filteredClusters  = new ObservableCollection<UMCClusterTreeViewModel>(
                (from cluster in clusters
                 select new UMCClusterTreeViewModel(cluster)).ToList());
            
            m_exportCommand   = new BaseCommand(ShowFilterWindow);            
            m_filterViewModel = new UMCClusterFilterViewModel(Clusters);    
            FilterCommand     = new BaseCommand(FilterWindow, BaseCommand.AlwaysPass);

            RegisterEvents();
        }
        private void ShowFilterWindow()
        {
            ClusterExportView clusterExport;
            clusterExport = new ClusterExportView();
            clusterExport.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            var viewModel = new ClusterExportViewModel(Clusters, FilteredClusters) {Status = ""};
            clusterExport.DataContext = viewModel;
            clusterExport.ShowDialog();
        }

        /// <summary>
        /// Makes sure that we have event handlers to know when a feature was selected below us....
        /// probably a better way through bubble-based command routing...
        /// </summary>
        void RegisterEvents()
        {
            foreach (var cluster in m_filteredClusters)
            {
                cluster.Selected         += cluster_Selected;
                cluster.FeatureSelected  += cluster_FeatureSelected;
                cluster.SpectrumSelected += cluster_SpectrumSelected;
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

        void cluster_Selected(object sender, EventArgs e)
        {
            var model   = sender as UMCClusterTreeViewModel;
            if (model == null) return;

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
        /// <summary>
        /// Filters the clusters based on the given mass and NET range
        /// </summary>
        public void Filter(FilterRange monoisotopicMass, FilterRange net)
        {
            m_filterViewModel.MassRange.Minimum = monoisotopicMass.Minimum;
            m_filterViewModel.MassRange.Maximum = monoisotopicMass.Maximum;
            m_filterViewModel.NetRange.Minimum  = net.Minimum;
            m_filterViewModel.NetRange.Maximum  = net.Maximum;

            m_filterViewModel.MassRange.ShouldUse   = true;
            m_filterViewModel.NetRange.ShouldUse    = true;

            Filter();
        }
        public ICommand FilterCommand { get; private set; }
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
                if (value == m_selectedSpectrum) return;

                m_selectedSpectrum = value;
                OnPropertyChanged("SelectedSpectrum");
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
                if (value == m_selectedFeature) return;
                m_selectedFeature = value;
                OnPropertyChanged("SelectedFeature");
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

        private void FilterWindow()
        {

            m_newWindow = new ClusterFilterWindow
            {
                DataContext = m_filterViewModel,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowState = WindowState.Normal,
                WindowStyle = WindowStyle.ToolWindow
            };

            var worked = m_newWindow.ShowDialog();
            if (worked == true)
            {
                Filter();
            }
        }

        private void Filter()
        {
            FilteredClusters.Clear();
            var clusters = m_filterViewModel.ApplyFilters();
            ResetClusters(clusters);
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
