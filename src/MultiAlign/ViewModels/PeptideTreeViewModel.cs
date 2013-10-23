using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace MultiAlign.ViewModels
{
    public class PeptideTreeViewModel: TreeItemViewModel
    {
        Peptide m_peptide;

        public PeptideTreeViewModel(Peptide peptide)
        {
            m_peptide = peptide;
        }

        public override void LoadChildren()
        {
            
        }
    }
}
