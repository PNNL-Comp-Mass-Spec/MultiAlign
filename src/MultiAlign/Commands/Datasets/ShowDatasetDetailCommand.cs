using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;
using System.Windows.Input;
using System.Windows;
using MultiAlign.Windows.Viewers.Datasets;
using MultiAlign.ViewModels;

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
            DatasetInformation information = parameter as DatasetInformation;

            if (information != null)
            {
                Window newWindow    = new Window();
                newWindow.Width     = 650;
                newWindow.Height    = 350;

                DatasetInputFileEditor viewer   = new DatasetInputFileEditor();
                viewer.DataContext              = new DatasetInformationViewModel(information);
                newWindow.Content               = viewer;
                newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                newWindow.ShowDialog();
            }
        }
    }
}
