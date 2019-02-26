
using FeatureAlignment.Data.MassTags;

namespace MultiAlign.ViewModels.Proteins
{
    public sealed class ProteinViewModel : ViewModelBase
    {
        private readonly Protein m_protein;

        public ProteinViewModel(Protein p)
        {
            m_protein = p;
        }

        public Protein Protein
        {
            get { return m_protein; }
        }
    }
}