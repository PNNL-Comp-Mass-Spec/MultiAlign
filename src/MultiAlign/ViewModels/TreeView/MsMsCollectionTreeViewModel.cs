using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using System.Collections.ObjectModel;
using MultiAlignCore.Extensions;
using PNNLOmics.Data.Features;
using MultiAlign.IO;

namespace MultiAlign.ViewModels.TreeView
{
    public class MsMsCollectionTreeViewModel : TreeItemViewModel
    {
        private UMCClusterLight m_cluster;
        protected ObservableCollection<MsMsTreeViewModel> m_spectra;

        public MsMsCollectionTreeViewModel(UMCClusterLight cluster) :
            this(cluster, null)
        {
        }

        public MsMsCollectionTreeViewModel(UMCClusterLight cluster, UMCClusterTreeViewModel parent)
        {
            m_cluster = cluster;
            m_parent  = parent;        
            m_spectra = new ObservableCollection<MsMsTreeViewModel>();            
        }

        public ObservableCollection<MsMsTreeViewModel> Spectra
        {
            get
            {
                return m_spectra;
            }
        }

        public override void LoadChildren()
        {
            List <MSSpectra> spectra = m_cluster.FindSpectra();

            List<MsMsTreeViewModel> peptideModels =
                (from spectrum in spectra
                 select new MsMsTreeViewModel(spectrum)).ToList();

            peptideModels.ForEach(x => m_spectra.Add(x));
        }
    }
}
