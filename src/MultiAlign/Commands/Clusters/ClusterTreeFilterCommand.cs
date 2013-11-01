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
        public event EventHandler CanExecuteChanged;

        UMCClusterCollectionTreeViewModel m_viewModel;

        public ClusterTreeFilterCommand(UMCClusterCollectionTreeViewModel viewModel)
        {
            m_viewModel = viewModel;
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
            ClusterFilterWindow newWindow               = new ClusterFilterWindow();            
            UMCClusterFilterViewModel filterViewModel   = new UMCClusterFilterViewModel(m_viewModel.Clusters);
            newWindow.DataContext                       = filterViewModel;            
            newWindow.WindowStartupLocation             = WindowStartupLocation.CenterScreen;
            newWindow.WindowState                       = WindowState.Normal;
            newWindow.WindowStyle                       = WindowStyle.ToolWindow;
            
            bool? worked = newWindow.ShowDialog();
            if (worked == true)
            {
                m_viewModel.FilteredClusters.Clear();
                IEnumerable<UMCClusterLightMatched> clusters = filterViewModel.ApplyFilters();
                m_viewModel.ResetClusters(clusters);
            }
        }
    }
}
