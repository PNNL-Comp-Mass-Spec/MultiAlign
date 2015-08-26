using MultiAlign.IO;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO;

namespace MultiAlign.ViewModels.TreeView
{
    public class PeptideTreeViewModel : GenericCollectionTreeViewModel
    {
        private readonly Peptide m_peptide;

        public PeptideTreeViewModel(Peptide peptide) :
            this(peptide, null)
        {
        }

        public PeptideTreeViewModel(Peptide peptide, TreeItemViewModel parent)
        {
            m_parent = parent;
            m_peptide = peptide;

            var information = SingletonDataProviders.GetDatasetInformation(m_peptide.GroupId);

            if (information != null)
            {
                Name = information.DatasetName;
            }
            else
            {
                Name = string.Format("Dataset {0}", m_peptide.GroupId);
            }

            AddStatistic("Id", m_peptide.Id);
            AddStatistic("Dataset Id", m_peptide.GroupId);

            AddStatistic("Precursor m/z", m_peptide.Spectrum.PrecursorMz);
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
            get { return base.IsSelected; }
            set
            {
                base.IsSelected = value;
                if (m_peptide != null)
                {
                    var feature = m_peptide.GetParentUmc();
                    OnFeatureSelected(feature);
                }
            }
        }

        /// <summary>
        ///     Gets the sequence.
        /// </summary>
        public string Sequence
        {
            get { return m_peptide.Sequence; }
        }

        /// <summary>
        ///     Loads the children
        /// </summary>
        public override void LoadChildren()
        {
        }
    }
}