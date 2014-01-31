using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace MultiAlign.ViewModels
{
    public class ProteinViewModel: ViewModelBase 
    {
        private Protein m_protein;

        public ProteinViewModel(Protein p)
        {
            m_protein = p;
        }

        public Protein Protein
        {
            get
            {
                return m_protein;
            }
        }
    }
}
