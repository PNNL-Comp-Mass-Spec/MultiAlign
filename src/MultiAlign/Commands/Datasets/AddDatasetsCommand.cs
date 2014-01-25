using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using MultiAlignCore.Data;
using MultiAlign.Commands.Datasets;
using MultiAlign.ViewModels;

namespace MultiAlign.Commands.Wizard
{
    public class AddDatasetsCommand : ICommand
    {
        private ObservableCollection<DatasetInformationViewModel> m_datasets;

        public AddDatasetsCommand(ObservableCollection<DatasetInformationViewModel> datasets)
        {
            m_datasets = datasets;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            
        }
    }
}
