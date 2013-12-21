using System.Collections.Generic;
using System.Collections.ObjectModel;
using MultiAlignCore.Data;
using System.Linq;

namespace MultiAlign.ViewModels
{
    public class DatasetCollectionViewModel: ViewModelBase 
    {
        private string m_filter;
        private IEnumerable<DatasetViewModel> m_models;

        public DatasetCollectionViewModel():
            this(new List<DatasetInformation>())
        {
        }

        public DatasetCollectionViewModel(IEnumerable<DatasetInformation> datasets)
        {
            
            List<DatasetViewModel> datasetViewModels =  (from dataset in datasets
                                                                select new DatasetViewModel(dataset)).ToList(); 
            m_models         = datasetViewModels;
            Datasets         = new ObservableCollection<DatasetViewModel>(datasetViewModels);            
            FilteredDatasets = new ObservableCollection<DatasetViewModel>(datasetViewModels);
        }


        private void FilterDatasets()
        {
            IEnumerable<DatasetViewModel> filtered  = new List<DatasetViewModel>(m_models);           
            filtered                                = filtered.Where(x => x.Name.Contains(m_filter));

            FilteredDatasets.Clear();
            filtered.ToList().ForEach(x => FilteredDatasets.Add(x));
        }

        public ObservableCollection<DatasetViewModel> Datasets
        {
            get;
            private set;
        }

        public ObservableCollection<DatasetViewModel> FilteredDatasets
        {
            get;
            private set;
        }



        public string Filter
        {
            get
            {
                return m_filter;
            }
            set
            {
                if (value != m_filter)
                {
                    m_filter = value;
                    OnPropertyChanged("Filter");
                    FilterDatasets();
                }
            }
        }
    }
}
