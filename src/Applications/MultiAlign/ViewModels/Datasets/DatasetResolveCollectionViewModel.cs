using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MultiAlign.Commands;

namespace MultiAlign.ViewModels.Datasets
{
    public class DatasetResolveCollectionViewModel : ViewModelBase
    {
        public DatasetResolveCollectionViewModel(IEnumerable<DatasetResolveMatchViewModel> matches)
        {
            Datasets = new ObservableCollection<DatasetResolveMatchViewModel>(matches);

            CheckAllCommand = new BaseCommand(CheckAll, BaseCommand.AlwaysPass);
            UncheckAllCommand = new BaseCommand(UncheckAll, BaseCommand.AlwaysPass);
        }


        public ObservableCollection<DatasetResolveMatchViewModel> Datasets { get; private set; }
        public ICommand UncheckAllCommand { get; private set; }
        public ICommand CheckAllCommand { get; private set; }

        private void UncheckAll()
        {
            foreach (DatasetResolveMatchViewModel dataset in Datasets)
            {
                dataset.IsSelected = false;
            }
        }

        private void CheckAll()
        {
            foreach (DatasetResolveMatchViewModel dataset in Datasets)
            {
                dataset.IsSelected = true;
            }
        }
    }
}