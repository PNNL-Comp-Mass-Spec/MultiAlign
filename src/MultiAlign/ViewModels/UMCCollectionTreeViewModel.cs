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

namespace MultiAlign.ViewModels
{

    public class UMCCollectionTreeViewModel: TreeItemViewModel
    {
        protected ObservableCollection<UMCViewModel> m_features;

        public UMCCollectionTreeViewModel(List<UMCLight> features):
            this(features, null)
        {
        }

        public UMCCollectionTreeViewModel(List<UMCLight> features, TreeItemViewModel parent)
        {
            m_parent = parent;

            m_features = new ObservableCollection<UMCViewModel>(
                (from feature in features
                 select new UMCViewModel(feature)).ToList());
        }

        public ObservableCollection<UMCViewModel> Features
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
 	        
        }
    }
}
