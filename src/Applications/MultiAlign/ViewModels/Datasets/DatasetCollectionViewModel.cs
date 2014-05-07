using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.Windows.Viewers.Datasets;
using MultiAlignCore.Data.MetaData;

namespace MultiAlign.ViewModels.Datasets
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

            var datasetViewModels = (from dataset in datasets
                select new DatasetInformationViewModel(dataset)).ToList();
            m_models = datasetViewModels;
            Datasets = new ObservableCollection<DatasetInformationViewModel>(datasetViewModels);
            FilteredDatasets = new ObservableCollection<DatasetInformationViewModel>(datasetViewModels);

            Action<string> action = ReconcilePaths;
            var command = new BrowseFolderCommand(action);
            ReconcilePathsCommand = command;
        }

        /// <summary>
        /// Reconciles the paths to the 
        /// </summary>
        /// <param name="path"></param>
        private void ReconcilePaths(string path)
        {
            var files = Directory.GetFiles(path);
            var nameMap = new Dictionary<string, string>();
            // Map the names of the files to the dictionary
            foreach (var file in files)
            {
                var filenameOnly = Path.GetFileName(file);
                if (filenameOnly == null) continue;                
                if (nameMap.ContainsKey(filenameOnly)) continue;                
                nameMap.Add(filenameOnly, file);
            }

            var newPaths = new List<DatasetResolveMatchViewModel>();
            foreach (var dataset in Datasets)
            {
                if (dataset.Dataset.RawPath == null)
                    continue;
                

                var filename = Path.GetFileName(dataset.Dataset.RawPath);
                if (nameMap.ContainsKey(filename))
                {
                    var newPath = nameMap[filename];
                    var model = new DatasetResolveMatchViewModel(dataset, newPath);   
                    newPaths.Add(model);
                }                 
            }

            if (newPaths.Count > 0)
            {
                var view            = new DatasetResolveView();
                var viewModel       = new DatasetResolveCollectionViewModel(newPaths);
                view.DataContext    = viewModel;

                var result = view.ShowDialog();

                if (result == true)
                {
                    foreach (var match in newPaths)
                    {
                        if (match.IsSelected)
                        {
                            match.Dataset.Dataset.RawPath = match.NewPath;
                        }
                    }
                }
            }
        }


        private void FilterDatasets()
        {
            var filter                           = m_filter.ToLower();
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
                if (value == m_expandImagesall) return;
                m_expandImagesall = value;

                foreach (var model in Datasets)
                {
                    model.ShouldExpand = value;
                }
                OnPropertyChanged("IsExpandAllImages");
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

        public ICommand ReconcilePathsCommand { get; private set; }
    }
}
