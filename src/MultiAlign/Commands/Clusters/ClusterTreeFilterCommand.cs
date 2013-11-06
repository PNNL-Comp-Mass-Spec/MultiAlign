using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using MultiAlign.ViewModels.TreeView;
using System.Windows;
using MultiAlign.Windows.Viewers.Clusters;
using MultiAlign.ViewModels;
using MultiAlignCore.Data.Features;

namespace MultiAlign.Commands.Clusters
{
    class ClusterTreeFilterCommand: ICommand
    {
        public event EventHandler                   CanExecuteChanged;
        private UMCClusterFilterViewModel           m_filterViewModel;
        private UMCClusterCollectionTreeViewModel   m_viewModel;
        private ClusterFilterWindow                 m_newWindow;  

        public ClusterTreeFilterCommand(UMCClusterCollectionTreeViewModel viewModel)
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

            m_newWindow = new ClusterFilterWindow();
            m_newWindow.DataContext = m_filterViewModel;
            m_newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            m_newWindow.WindowState = WindowState.Normal;
            m_newWindow.WindowStyle = WindowStyle.ToolWindow;

            bool? worked = m_newWindow.ShowDialog();
            if (worked == true)
            {
                m_viewModel.FilteredClusters.Clear();
                IEnumerable<UMCClusterLightMatched> clusters = m_filterViewModel.ApplyFilters();
                m_viewModel.ResetClusters(clusters);
            }
        }
    }
}
