using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using System.Collections.ObjectModel;

namespace MultiAlign.ViewModels
{
    public class MsMsCollectionTreeViewModel : TreeItemViewModel
    {
        protected ObservableCollection<MsMsTreeViewModel> m_spectra;

        public MsMsCollectionTreeViewModel(List<MSSpectra> spectra):
            this(spectra, null)
        {
        }

        public MsMsCollectionTreeViewModel(List<MSSpectra> spectra, UMCClusterViewModel parent)
        {            
            m_parent  = parent;
            m_spectra = new ObservableCollection<MsMsTreeViewModel>(
                    (from spectrum in spectra
                     select new MsMsTreeViewModel(spectrum)).ToList());
        
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
        }
    }
}
