// Expanded From Josh Smith's tree example.
// http://www.codeproject.com/Articles/26288/Simplifying-the-WPF-TreeView-by-Using-the-ViewMode
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.Features;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Manassa.ViewModels;
using MultiAlignCore.Data;

namespace Manassa.ViewModels
{
    /// <summary>
    /// Root tree object.
    /// </summary>
    public class UMCClusterTreeRootViewModel : TreeItemViewModel
    {
        readonly ReadOnlyCollection<UMCClusterTreeViewItemModel> m_clusters;
        
        public UMCClusterTreeRootViewModel(List<UMCClusterLightMatched> clusters)
            : this(clusters, null)
        {
        }

        public UMCClusterTreeRootViewModel(List<UMCClusterLightMatched> clusters, UMCClusterTreeViewItemModel parent)
        {
            m_clusters = new ReadOnlyCollection<UMCClusterTreeViewItemModel>(
                (from cluster in clusters
                select new UMCClusterTreeViewItemModel(cluster)).ToList());
        }

        public ReadOnlyCollection<UMCClusterTreeViewItemModel> Clusters
        {
            get
            {
                return m_clusters;
            }
        }
    }
    /// <summary>
    /// Cluster Tree View Model 
    /// </summary>
    public class UMCClusterTreeViewItemModel : TreeItemViewModel
    {
        /// <summary>
        ///  The cluster in question
        /// </summary>
        UMCClusterLightMatched   m_cluster;        
        /// <summary>
        /// Features
        /// </summary>
        protected readonly ReadOnlyCollection<UMCTreeViewViewModel> m_features;
        /// <summary>
        /// Matches
        /// </summary>
        protected readonly ReadOnlyCollection<MassTagMatchTreeViewItemModel> m_matches;

        public UMCClusterTreeViewItemModel(UMCClusterLightMatched cluster)
            : this(cluster, null)
        {
        }

        public UMCClusterTreeViewItemModel(UMCClusterLightMatched cluster, UMCClusterTreeViewItemModel parent)
        {
            m_cluster = cluster;
            m_parent  = parent;

            m_features = new ReadOnlyCollection<UMCTreeViewViewModel>(
                (from feature in cluster.Cluster.Features
                 select new UMCTreeViewViewModel(feature)).ToList());

            m_matches = new ReadOnlyCollection<MassTagMatchTreeViewItemModel>(
                (from match in cluster.ClusterMatches
                     select new MassTagMatchTreeViewItemModel(match)
                     ).ToList()
                );

            
        }
        
        /// <summary>
        /// Gets the ID of the cluster.
        /// </summary>
        public string ClusterId         
        { 
            get { return m_cluster.Cluster.ID.ToString();  } 
        }

        public ReadOnlyCollection<UMCTreeViewViewModel> Features
        {
            get
            {
                return m_features;
            }
        }
        public ReadOnlyCollection<MassTagMatchTreeViewItemModel> Matches
        {
            get
            {
                return m_matches;
            }
        }
    }
    /// <summary>
    /// Feature tree view model.
    /// </summary>
    public class UMCTreeViewViewModel : TreeItemViewModel
    {
        private UMCLight m_feature;

        public UMCTreeViewViewModel(UMCLight cluster)
            : this(cluster, null)
        {
        }

        public UMCTreeViewViewModel(UMCLight feature, UMCClusterTreeViewItemModel parent)
        {
            m_feature = feature;
            m_parent  = parent;
        }

        public string Id
        {
            get
            {
                return m_feature.ID.ToString();
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class MassTagMatchTreeViewItemModel : TreeItemViewModel
    {
        private ClusterToMassTagMap m_match;

        public MassTagMatchTreeViewItemModel(ClusterToMassTagMap match)
                    : this(match, null)
        {

        }

        public MassTagMatchTreeViewItemModel(ClusterToMassTagMap match, UMCClusterTreeViewItemModel  parent)
        {
            m_match     = match;
            m_parent    = parent;
        }

        public string Name
        {
            get
            {
                return m_match.MassTag.MassTag.PeptideSequence;
            }
        }
        public double Stac
        {
            get
            {
                return m_match.StacScore;
            }
        }
        public double StacUp
        {
            get
            {
                return m_match.StacUP;
            }
        }
    }
    /// <summary>
    /// Base class for tree view items.
    /// </summary>
    public abstract class TreeItemViewModel : ViewModelBase
    {
        /// <summary>
        /// Flag indicating the model was expanded.
        /// </summary>
        protected bool m_isExpanded;
        /// <summary>
        /// Flag indicating the model was selected.
        /// </summary>
        protected bool m_isSelected;
        /// <summary>
        /// Parent Item
        /// </summary>
        protected TreeItemViewModel m_parent;

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return m_isExpanded; }
            set
            {
                if (value != m_isExpanded)
                {
                    m_isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (m_isExpanded && m_parent != null)
                    m_parent.IsExpanded = true;
            }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (value != m_isSelected)
                {
                    m_isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }
    }
}
