using MultiAlignCore.Data;

namespace MultiAlign.ViewModels.TreeView
{
    public class MassTagToClusterMatch : GenericCollectionTreeViewModel
    {
        private MassTagToCluster m_match;

        public MassTagToClusterMatch(MassTagToCluster match)
            : this(match, null)
        {
        }

        public MassTagToClusterMatch(MassTagToCluster match, MassTagToClusterMatch parent)
        {
            m_match = match;
            m_parent = parent;
            Name = "Peptide: " + match.MassTag.PeptideSequence;

            AddString("Sequence", match.MassTag.PeptideSequence);
            AddStatistic("Proteins", match.MatchingProteins.Count);
        }

        public override void LoadChildren()
        {
        }
    }
}