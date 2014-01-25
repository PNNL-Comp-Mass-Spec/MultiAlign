using System.Collections.Generic;
using System.Collections.ObjectModel;
using MultiAlignCore.Data;
using System.Linq;

namespace MultiAlign.ViewModels
{
    public class DatasetCollectionViewModel: ViewModelBase 
    {
        private string m_filter;
        private IEnumerable<DatasetInformationViewModel> m_models;

        public DatasetCollectionViewModel():
            this(new List<DatasetInformation>())
        {
        }

        public DatasetCollectionViewModel(IEnumerable<DatasetInformation> datasets)
        {
            
            List<DatasetInformationViewModel> datasetViewModels =  (from dataset in datasets
                                                                    select new DatasetInformationViewModel(dataset)).ToList(); 
            m_models         = datasetViewModels;
            Datasets         = new ObservableCollection<DatasetInformationViewModel>(datasetViewModels);
            FilteredDatasets = new ObservableCollection<DatasetInformationViewModel>(datasetViewModels);
        }


        private void FilterDatasets()
        {
            string filter                           = m_filter.ToLower();
            IEnumerable<DatasetInformationViewModel> filtered = new List<DatasetInformationViewModel>(m_models);           
            filtered                                = filtered.Where(x => x.Name.ToLower().Contains(filter));
            FilteredDatasets.Clear();
            filtered.ToList().ForEach(x => FilteredDatasets.Add(x));
        }

        public ObservableCollection<DatasetInformationViewModel> Datasets
        {
            get;
            private set;
        }

        public ObservableCollection<DatasetInformationViewModel> FilteredDatasets
        {
            get;
            private set;
        }

        private bool m_expandImagesall;

        public bool IsExpandAllImages
        {
            get
            {
                return m_expandImagesall;
            }
            set
            {
                if (value != m_expandImagesall)
                {
                    if (value != null)
                    {
                        m_expandImagesall = value;

                        foreach (DatasetInformationViewModel model in Datasets)
                        {
                            model.ShouldExpand = value;
                        }
                        OnPropertyChanged("IsExpandAllImages");
                    }
                }
            }
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
                    if (value != null)
                    {
                        m_filter = value;                  
                        OnPropertyChanged("Filter");
                        FilterDatasets();
                    }
                }
            }
        }
    }
}
