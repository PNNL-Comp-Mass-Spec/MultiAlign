
using MultiAlign.IO;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using System.Collections.ObjectModel;
using PNNLOmics.Data.Features;
using System;

namespace MultiAlign.ViewModels.TreeView
{    
    /// <summary>
    /// Cluster Tree View Model 
    /// </summary>
    public class UMCClusterTreeViewModel : TreeItemViewModel
    {
        public event EventHandler<FeatureSelectedEventArgs> FeatureSelected;

        /// <summary>
        ///  The cluster in question
        /// </summary>
        UMCClusterLightMatched                  m_cluster;
        MsMsCollectionTreeViewModel             m_spectra;
        ObservableCollection<TreeItemViewModel> m_items;

        public UMCClusterTreeViewModel(UMCClusterLightMatched cluster):
            this(cluster, null)
        {
        }
        
        public UMCClusterTreeViewModel(UMCClusterLightMatched matchedCluster, TreeItemViewModel parent)
        {
            m_items                                         = new ObservableCollection<TreeItemViewModel>();            
            m_parent                                        = parent;
            m_cluster                                       = matchedCluster;
            UMCCollectionTreeViewModel features             = new UMCCollectionTreeViewModel(matchedCluster.Cluster);
            features.FeatureSelected += new EventHandler<FeatureSelectedEventArgs>(features_FeatureSelected);
            features.Name                                   = "Features";

            // Cluster level statistics
            UMCClusterLight cluster = matchedCluster.Cluster;
            AddStatistics("Mass",       cluster.MassMonoisotopic);
            AddStatistics("NET",        cluster.NET);

            if (cluster.DriftTime > 0)
            {
                AddStatistics("Drift Time", cluster.DriftTime);
            }
            AddStatistics("Datasets",   cluster.DatasetMemberCount);
            AddStatistics("Total",      cluster.MemberCount);

            // Items to display the base childen.
            PeptideCollectionTreeViewModel identifications  = new PeptideCollectionTreeViewModel(cluster);
            identifications.Name                            = "Search Results";

            MsMsCollectionTreeViewModel spectra             = new MsMsCollectionTreeViewModel(cluster);
            spectra.Name                                    = "MS/MS";
            m_spectra                                       = spectra;

            MassTagCollectionMatchTreeViewModel matches     = new MassTagCollectionMatchTreeViewModel(matchedCluster.ClusterMatches);
            matches.Name                                    = "AMT Tags";

            m_items.Add(features);
            m_items.Add(identifications);
            m_items.Add(spectra);
            m_items.Add(matches);            
        }

        void features_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            if (FeatureSelected != null)
            {
                FeatureSelected(sender, e);
            }
        }

        private void AddStatistics(string name, double value)
        {
            StatisticTreeViewItem item = new StatisticTreeViewItem(value, name);
            m_items.Add(item);
        }


        public ObservableCollection<TreeItemViewModel> Items
        {
            get
            {
                return m_items; 
            }
        }
        
        public int DatasetMemberCount
        {
            get
            {
                return m_cluster.Cluster.DatasetMemberCount;
            }
        }

        public UMCClusterLightMatched Cluster
        {
            get
            {
                return m_cluster;
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
            if (m_loaded)
                return;

            m_cluster.Cluster.ReconstructUMCCluster(SingletonDataProviders.Providers, true, true);

            foreach (TreeItemViewModel treeModel in m_items)
            {
                treeModel.LoadChildren();
            }   
        }
    }
}
