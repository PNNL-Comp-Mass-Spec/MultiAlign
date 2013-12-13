using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using MultiAlign.IO;
using MultiAlign.ViewModels;
using MultiAlign.ViewModels.TreeView;
using MultiAlign.Windows.Viewers.Clusters;
using MultiAlignCore.Data;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlign.Commands.Clusters
{
    class ClusterExportCommand: ICommand
    {
        public event EventHandler                   CanExecuteChanged;
        private UMCClusterFilterViewModel           m_filterViewModel;
        private UMCClusterCollectionTreeViewModel   m_viewModel;
        private ClusterFilterWindow                 m_newWindow;

        public ClusterExportCommand(UMCClusterCollectionTreeViewModel viewModel)
        {
            m_viewModel         = viewModel;
            m_filterViewModel   = new UMCClusterFilterViewModel(viewModel.Clusters);
            
        }

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;   
        }        
        public void Execute(object parameter)
        {
            FilterWindow();
        }
        #endregion

        private void FilterWindow()
        {
            SaveFileDialog saveDialog   = new SaveFileDialog();
            DialogResult result         =  saveDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                List<UMCClusterLight> clusters      = new List<UMCClusterLight>();
                List<DatasetInformation> datasets   = SingletonDataProviders.GetAllInformation().ToList();
                IFeatureClusterWriter writer        = new UMCClusterAbundanceCrossTabWriter(saveDialog.FileName);

                Dictionary<int, List<ClusterToMassTagMap>> matches  = new Dictionary<int, List<ClusterToMassTagMap>>();
                Dictionary<string, MassTagLight> massTags           = new Dictionary<string,MassTagLight>();

                foreach (var x in m_viewModel.FilteredClusters)
                {
                    UMCClusterLight cluster = x.Cluster.Cluster;
                    List<ClusterToMassTagMap> maps = new List<ClusterToMassTagMap>();
                    matches.Add(cluster.ID, x.Cluster.ClusterMatches);

                    foreach (var match in x.Cluster.ClusterMatches)
                    {
                        MassTagLight tag = match.MassTag.MassTag;
                        string sequence  = tag.PeptideSequence;
                        if (!massTags.ContainsKey(sequence))
                        {
                            massTags.Add(sequence, tag);
                        }
                    }

                }

                writer.WriteClusters(clusters, matches, datasets, massTags);
            }            
        }
    }
}
