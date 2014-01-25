using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using System.Collections.ObjectModel;
using MultiAlignCore.Data;
using MultiAlign.IO;

namespace MultiAlign.ViewModels
{
    public class PeptideViewModel: ViewModelBase
    {
        DatasetInformationViewModel    m_dataset;
        private Peptide     m_peptide;

        public PeptideViewModel(Peptide peptide)
        {
            m_peptide = peptide;

            DatasetInformation info = SingletonDataProviders.GetDatasetInformation(peptide.GroupId);
            if (info != null)
            {
                m_dataset = new DatasetInformationViewModel(info);
            }


            MatchedProteins = new ObservableCollection<ProteinViewModel>();
            LoadProteinData(peptide);
        }

        private void LoadProteinData(Peptide peptide)
        {
            MatchedProteins.Clear();
            peptide.ProteinList.ForEach(x => MatchedProteins.Add(new ProteinViewModel(x)));
        }

        /// <summary>
        /// Gets or sets the matched proteins.
        /// </summary>
        public ObservableCollection<ProteinViewModel> MatchedProteins { get; set; }

        public DatasetInformationViewModel Dataset
        {
            get
            {
                return m_dataset;
            }
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
        
    }

}
