using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using System.Collections.ObjectModel;

namespace MultiAlign.ViewModels.TreeView
{
    public class PeptideTreeViewModel: TreeItemViewModel
    {
        private Peptide m_peptide;
        
        public PeptideTreeViewModel(Peptide peptide):
            this(peptide, null)
        {
        }

        public PeptideTreeViewModel(Peptide peptide, TreeItemViewModel parent)
        {
            m_parent  = parent;
            m_peptide = peptide;    
        }

        /// <summary>
        /// Gets the sequence.
        /// </summary>
        public string Sequence
        {
            get
            {
                return m_peptide.Sequence;
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
