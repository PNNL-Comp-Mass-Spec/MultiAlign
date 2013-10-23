using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PNNLOmics.Data.Features;
using PNNLOmics.Data;

namespace MultiAlign.ViewModels
{
    public class DatabaseSearchIdentificationsTreeViewModel: TreeItemViewModel
    {
        protected ObservableCollection<PeptideTreeViewModel> m_peptides;


        public DatabaseSearchIdentificationsTreeViewModel(List<Peptide> peptides):
            this(peptides, null)
        {
        }
            
        public DatabaseSearchIdentificationsTreeViewModel(List<Peptide> peptides, TreeItemViewModel parent)
        {
            m_parent   = parent;
            m_peptides = new ObservableCollection<PeptideTreeViewModel>(
                (from peptide in peptides
                 select new PeptideTreeViewModel(peptide)).ToList());
        }

        public ObservableCollection<PeptideTreeViewModel> Peptides
        {
            get
            {
                return m_peptides;
            }
        }

        public override void LoadChildren()
        {
            
        }
    }


}
