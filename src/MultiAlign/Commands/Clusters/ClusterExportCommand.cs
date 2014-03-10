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
using MultiAlign.Windows.IO;
using System.Windows;
using MultiAlign.ViewModels.IO;

namespace MultiAlign.Commands.Clusters
{
    class ClusterExportCommand: ICommand
    {
        public event EventHandler                   CanExecuteChanged;
        private UmcClusterCollectionTreeViewModel m_clustersViewModel;
        private ClusterExportViewModel m_viewModel;
        

        public ClusterExportCommand(UmcClusterCollectionTreeViewModel viewModel)
        {
            m_clustersViewModel     = viewModel;
            m_viewModel = new ClusterExportViewModel(m_clustersViewModel.Clusters, m_clustersViewModel.FilteredClusters);
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
            ClusterExportView clusterExport;
            clusterExport = new ClusterExportView();
            clusterExport.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            m_viewModel.Status                  = "";
            clusterExport.DataContext           = m_viewModel;            
            clusterExport.ShowDialog();
        }
    }
}
