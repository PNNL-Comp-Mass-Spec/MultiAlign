using System;
using FeatureAlignment.Data.Features;
using PNNLOmics.Annotations;

namespace MultiAlign.ViewModels.TreeView
{
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

        protected bool m_loaded;

        /// <summary>
        /// Parent Item
        /// </summary>
        protected TreeItemViewModel m_parent;

        public TreeItemViewModel()
        {
            m_loaded = false;
        }

        /// <summary>
        /// Gets or sets the name of the tree view item model
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is expanded.
        /// </summary>
        public virtual bool IsExpanded
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
                {
                    m_parent.IsExpanded = true;
                }
                if (m_isExpanded && ! m_loaded)
                {
                    LoadChildren();
                    m_loaded = true;
                }

                if (m_isExpanded)
                {
                    IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is selected.
        /// </summary>
        [UsedImplicitly]
        public virtual bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (value != m_isSelected)
                {
                    m_isSelected = value;
                    OnPropertyChanged("IsSelected");
                    if (value)
                    {
                        if (Selected != null)
                        {
                            Selected(this, null);
                        }
                    }
                }
            }
        }

        public event EventHandler<FeatureSelectedEventArgs> FeatureSelected;

        /// <summary>
        /// Fired if the item was selected
        /// </summary>
        public event EventHandler Selected;

        protected void OnFeatureSelected(UMCLight feature)
        {
            if (FeatureSelected != null)
            {
                if (feature == null)
                    return;

                FeatureSelected(this, new FeatureSelectedEventArgs(feature));
            }
        }

        /// <summary>
        /// Forces items to lazy load their children
        /// </summary>
        public abstract void LoadChildren();
    }
}