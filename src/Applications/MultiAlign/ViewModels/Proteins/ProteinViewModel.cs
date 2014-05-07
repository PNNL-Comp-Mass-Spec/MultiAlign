using PNNLOmics.Data;

namespace MultiAlign.ViewModels.Proteins
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
