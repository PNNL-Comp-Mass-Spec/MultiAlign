using System.Collections.ObjectModel;
using MultiAlign.IO;
using MultiAlign.ViewModels.Datasets;
using MultiAlignCore.Data.MetaData;
using PNNLOmics.Data;

namespace MultiAlign.ViewModels.Proteins
{
    public class PeptideViewModel : ViewModelBase
    {
        private readonly DatasetInformationViewModel m_dataset;
        private readonly Peptide m_peptide;

        public PeptideViewModel(Peptide peptide)
        {
            m_peptide = peptide;

            var info = SingletonDataProviders.GetDatasetInformation(peptide.GroupId);
            if (info != null)
            {
                m_dataset = new DatasetInformationViewModel(info);
            }


            MatchedProteins = new ObservableCollection<ProteinViewModel>();
            LoadProteinData(peptide);
        }

        /// <summary>
        ///     Gets or sets the matched proteins.
        /// </summary>
        public ObservableCollection<ProteinViewModel> MatchedProteins { get; set; }

        public DatasetInformationViewModel Dataset
        {
            get { return m_dataset; }
        }

        public string Sequence
        {
            get
            {
                if (m_peptide != null)
                    return m_peptide.Sequence;
                return "";
            }
        }

        public double Score
        {
            get
            {
                if (m_peptide == null)
                    return double.NaN;

                return m_peptide.Score;
            }
        }

        public double MassMonoisotopic
        {
            get
            {
                if (m_peptide == null)
                    return double.NaN;
                return m_peptide.MassMonoisotopic;
            }
        }

        public double Scan
        {
            get
            {
                if (m_peptide == null)
                    return double.NaN;
                return m_peptide.Scan;
            }
        }

        private void LoadProteinData(Peptide peptide)
        {
            MatchedProteins.Clear();
            peptide.ProteinList.ForEach(x => MatchedProteins.Add(new ProteinViewModel(x)));
        }
    }
}