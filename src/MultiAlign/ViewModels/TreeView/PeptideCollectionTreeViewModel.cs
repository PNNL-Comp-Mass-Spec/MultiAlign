using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PNNLOmics.Data.Features;
using MultiAlignCore.Extensions;
using PNNLOmics.Data;
using MultiAlign.IO;

namespace MultiAlign.ViewModels.TreeView
{
    public class PeptideCollectionTreeViewModel: TreeItemViewModel
    {
        private UMCClusterLight m_parentCluster;

        protected ObservableCollection<PeptideTreeViewModel> m_features;

        public PeptideCollectionTreeViewModel(UMCClusterLight parentCluster):
            this(parentCluster, null)
        {
        }

        public PeptideCollectionTreeViewModel(UMCClusterLight parentCluster, TreeItemViewModel parent)
        {
            m_parent        = parent;
            m_parentCluster = parentCluster;
            m_features      = new ObservableCollection<PeptideTreeViewModel>();
        }

        public ObservableCollection<PeptideTreeViewModel> Peptides
        {
            get
            {
                return m_features;
            }
        }

        public override void LoadChildren()
        {
            List<Peptide> peptides = m_parentCluster.FindPeptides(SingletonDataProviders.Providers);

            List<PeptideTreeViewModel> peptideModels = 
                (from peptide in peptides
                 select new PeptideTreeViewModel(peptide)).ToList();
            peptideModels.ForEach(x => m_features.Add(x));
        }
    }
}
