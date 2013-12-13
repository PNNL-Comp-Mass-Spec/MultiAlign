using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.Features;
using System.Collections.ObjectModel;
using PNNLOmics.Data;
using MultiAlignCore.Extensions;
using System.Windows.Documents;

namespace MultiAlign.ViewModels
{
    public class UMCClusterIdentificationViewModel: ViewModelBase
    {
        private UMCClusterLightMatched m_selectedCluster;

        public UMCClusterIdentificationViewModel()
        {
            DatabaseResults = new ObservableCollection<PeptideViewModel>();
            MassTagResults  = new ObservableCollection<MassTagMatchedViewModel>();
        }

        private void LoadCluster(UMCClusterLightMatched cluster)
        {
            m_selectedCluster = cluster;

            MassTagResults.Clear();
            DatabaseResults.Clear();

            List<Peptide> peptides = cluster.Cluster.FindPeptides();
            peptides.ForEach(x => DatabaseResults.Add(new PeptideViewModel(x)));            
            cluster.ClusterMatches.ForEach(x => MassTagResults.Add(new MassTagMatchedViewModel(x)));
        }


        public UMCClusterLightMatched SelectedCluster
        {
            get
            {
                return m_selectedCluster;
            }
            set
            {

                if (m_selectedCluster != value)
                {
                    m_selectedCluster = value;
                    LoadCluster(value);
                    OnPropertyChanged("SelectedCluster");
                }
            }
        }

        public ObservableCollection<PeptideViewModel> DatabaseResults { get; set; }
        public ObservableCollection<MassTagMatchedViewModel> MassTagResults { get; set; }

    }
}
