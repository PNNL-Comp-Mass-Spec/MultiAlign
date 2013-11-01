using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data.Features;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MultiAlign.ViewModels;
using MultiAlignCore.Data;
using MultiAlignCore.Extensions;
using MultiAlign.IO;

namespace MultiAlign.ViewModels.TreeView
{

    public class UMCCollectionTreeViewModel: TreeItemViewModel
    {
        public event EventHandler<FeatureSelectedEventArgs> FeatureSelected;
        private UMCClusterLight     m_parentCluster;
        protected ObservableCollection<UMCTreeViewModel> m_features;

        public UMCCollectionTreeViewModel(UMCClusterLight parentCluster):
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
            get
            {
                return m_features;
            }
        }
                 
        /// <summary>
        /// Loads the children 
        /// </summary>
        public override void  LoadChildren()
        {
            if (m_loaded)
                return;

            List<UMCTreeViewModel> features = (from feature in m_parentCluster.Features
                                            select new UMCTreeViewModel(feature)).ToList();

            foreach (UMCTreeViewModel model in features)
            {
                Features.Add(model);
                model.Selected += new EventHandler(model_Selected);
            }
            m_loaded = true;
        }

        /// <summary>
        /// Propagates the selected feature upwards.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void model_Selected(object sender, EventArgs e)
        {
            if (FeatureSelected != null)
            {
                UMCTreeViewModel model = sender as UMCTreeViewModel;
                FeatureSelected(this, new FeatureSelectedEventArgs(model.Feature));
            }
        }
    }


    public class FeatureSelectedEventArgs : EventArgs
    {
        public FeatureSelectedEventArgs(UMCLight feature)
        {
            Feature = feature;
        }
        public UMCLight Feature { get; private set; }
    }
}
