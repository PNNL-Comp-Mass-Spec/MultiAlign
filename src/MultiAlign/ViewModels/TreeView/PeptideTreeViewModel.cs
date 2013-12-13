using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using System.Collections.ObjectModel;
using MultiAlignCore.Data;
using MultiAlign.IO;
using MultiAlignCore.Extensions;

namespace MultiAlign.ViewModels.TreeView
{
    public class PeptideTreeViewModel : GenericCollectionTreeViewModel
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

            DatasetInformation information = SingletonDataProviders.GetDatasetInformation(m_peptide.GroupId);

            if (information != null)
            {
                Name = information.DatasetName;
            }
            else
            {
                Name = string.Format("Dataset {0}", m_peptide.GroupId);
            } 
            
            AddStatistic("Id",          m_peptide.ID);
            AddStatistic("Dataset Id",  m_peptide.GroupId);

            AddStatistic("Precursor m/z", m_peptide.Spectrum.PrecursorMZ);
            if (m_peptide.Spectrum.ParentFeature != null)
            {
                AddStatistic("Charge", m_peptide.Spectrum.ParentFeature.ChargeState);
            }
            else
            {
                AddStatistic("Charge", m_peptide.Spectrum.PrecursorChargeState);
            }

            AddString("Sequence", peptide.Sequence);
            AddStatistic("Score", peptide.Score);
            AddStatistic("Scan", peptide.Scan);
        }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                base.IsSelected = value;
                if (m_peptide != null)
                {
                    UMCLight feature = m_peptide.GetParentUmc();
                    OnFeatureSelected(feature);
                }
            }
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
