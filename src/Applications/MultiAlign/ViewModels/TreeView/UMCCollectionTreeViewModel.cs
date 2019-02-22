using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MultiAlignCore.Data.Features;

namespace MultiAlign.ViewModels.TreeView
{
    public class UMCCollectionTreeViewModel : TreeItemViewModel
    {
        private readonly UMCClusterLight m_parentCluster;
        protected ObservableCollection<UMCTreeViewModel> m_features;

        public UMCCollectionTreeViewModel(UMCClusterLight parentCluster) :
            this(parentCluster, null)
        {
        }

        public UMCCollectionTreeViewModel(UMCClusterLight parentCluster, TreeItemViewModel parent)
        {
            m_parent = parent;
            m_parentCluster = parentCluster;
            m_features = new ObservableCollection<UMCTreeViewModel>();
        }

        public ObservableCollection<UMCTreeViewModel> Features
        {
            get { return m_features; }
        }

        /// <summary>
        /// Loads the children
        /// </summary>
        public override void LoadChildren()
        {
            if (m_loaded)
                return;

            var features = (from feature in m_parentCluster.Features
                select new UMCTreeViewModel(feature)).ToList();

            foreach (var model in features)
            {
                Features.Add(model);
                model.FeatureSelected += model_FeatureSelected;
            }
            m_loaded = true;
        }

        private void model_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            OnFeatureSelected(e.Feature);
        }

        /// <summary>
        /// Propagates the selected feature upwards.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void model_Selected(object sender, EventArgs e)
        {
        }
    }
}