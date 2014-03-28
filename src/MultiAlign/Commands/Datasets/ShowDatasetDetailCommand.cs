using MultiAlign.ViewModels;
using MultiAlign.Windows.Viewers.Datasets;
using MultiAlignCore.Data.MetaData;
using System;
using System.Windows;
using System.Windows.Input;

namespace MultiAlign.Commands.Datasets
{
    /// <summary>
    /// Handles showing the details of a particular dataset.
    /// </summary>
    public class ShowDatasetDetailCommand: ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        /// <summary>
        /// Displays a window for modifying information about a dataset.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            var information = parameter as DatasetInformationViewModel;

            if (information == null) return;
            var newWindow    = new Window {Width = 650, Height = 350};

            var viewer   = new DatasetInputFileEditor {DataContext = information};
            newWindow.Content               = viewer;
            newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            newWindow.ShowDialog();
        }
    }
}
