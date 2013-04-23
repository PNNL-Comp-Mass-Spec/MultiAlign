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

namespace Manassa.Windows.ViewModels
{
    /// <summary>
    /// Cluster Tree View Model 
    /// </summary>
    public class ClusterTreeViewModel: ViewModelBase
    {
        UMCClusterLightMatched m_cluster;
        ClusterTreeViewModel   m_parent;
        bool m_isExpanded;
        bool m_isSelected;

        public ClusterTreeViewModel(UMCClusterLightMatched person)
            : this(person, null)
        {
        }

        private ClusterTreeViewModel(UMCClusterLightMatched cluster, ClusterTreeViewModel parent)
        {
            m_cluster = cluster;
            m_parent  = parent;

        }

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

        /// <summary>
        /// Gets the ID of the cluster.
        /// </summary>
        public int ClusterId         
        { 
            get { return m_cluster.Cluster.ID;  } 
        }
    }

    public abstract class ViewModelBase: INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler  PropertyChanged;
        #endregion

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
