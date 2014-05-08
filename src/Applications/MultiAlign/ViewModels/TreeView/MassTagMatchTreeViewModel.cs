using MultiAlignCore.Data;

namespace MultiAlign.ViewModels.TreeView
{
    /// <summary>
    /// </summary>
    public sealed class MassTagMatchTreeViewModel : TreeItemViewModel
    {
        private readonly ClusterToMassTagMap m_match;

        public MassTagMatchTreeViewModel(ClusterToMassTagMap match, UMCClusterTreeViewModel parent)
        {
            m_match = match;
            m_parent = parent;
        }

        public override string Name
        {
            get { return m_match.MassTag.MassTag.PeptideSequence; }
        }

        public double Stac
        {
            get { return m_match.StacScore; }
        }

        public double StacUp
        {
            get { return m_match.StacUP; }
        }

        public override void LoadChildren()
        {
        }
    }
}