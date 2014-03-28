using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MultiAlign.Commands;

namespace MultiAlign.ViewModels.Datasets
{
    public class DatasetResolveCollectionViewModel : ViewModelBase 
    {
        public DatasetResolveCollectionViewModel(IEnumerable<DatasetResolveMatchViewModel> matches)
        {
            Datasets = new ObservableCollection<DatasetResolveMatchViewModel>(matches);

            CheckAllCommand = new BaseCommandBridge(CheckAll);
            UncheckAllCommand = new BaseCommandBridge(UncheckAll);        
        }


        private void UncheckAll(object parameter)
        {
            foreach (var dataset in Datasets)
            {
                dataset.IsSelected = false;
            }
        }

        private void CheckAll(object parameter)
        {
            foreach (var dataset in Datasets)
            {
                dataset.IsSelected = true;
            }
        }

        public ObservableCollection<DatasetResolveMatchViewModel> Datasets { get; private set; }
        public ICommand UncheckAllCommand { get; private set; }
        public ICommand CheckAllCommand { get; private set; }
    }
}
