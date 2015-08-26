using System.Collections.Generic;
using System.Collections.ObjectModel;
using MultiAlign.ViewModels.Proteins;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;

namespace MultiAlign.ViewModels.Features
{
    public class UMCClusterIdentificationViewModel : ViewModelBase
    {
        private UMCClusterLightMatched m_selectedCluster;

        public UMCClusterIdentificationViewModel()
        {
            DatabaseResults = new ObservableCollection<PeptideViewModel>();
            MassTagResults = new ObservableCollection<MassTagMatchedViewModel>();
        }


        public UMCClusterLightMatched SelectedCluster
        {
            get { return m_selectedCluster; }
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

        private void LoadCluster(UMCClusterLightMatched cluster)
        {
            m_selectedCluster = cluster;

            MassTagResults.Clear();
            DatabaseResults.Clear();

            var peptides = cluster.Cluster.FindPeptides();
            peptides.ForEach(x => DatabaseResults.Add(new PeptideViewModel(x)));
            cluster.ClusterMatches.ForEach(x => MassTagResults.Add(new MassTagMatchedViewModel(x)));
        }
    }
}