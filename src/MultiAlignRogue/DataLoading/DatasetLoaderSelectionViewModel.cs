namespace MultiAlignRogue.DataLoading
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using GalaSoft.MvvmLight;

    using MultiAlignCore.Data;
    using MultiAlignCore.IO.DatasetLoaders;

    /// <summary>
    /// View model for selecting which dataset loader settings should be displayed on the GUI.
    /// </summary>
    public class DatasetLoaderSelectionViewModel : ViewModelBase
    {
        /// <summary>
        /// Maps the supported dataset type to the view model for the loader for that dataset type.
        /// </summary>
        private readonly Dictionary<DatasetLoader.SupportedDatasetTypes, DatasetLoaderViewModelBase> datasetTypeToDatasetLoader;

        /// <summary>
        /// A value indicating whether or not there are dataset loader selection options.
        /// </summary>
        private bool shouldShowSelection;

        /// <summary>
        /// The selected dataset loader.
        /// </summary>
        private DatasetLoaderViewModelBase selectedDatasetLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetLoaderSelectionViewModel" /> class. 
        /// </summary>
        public DatasetLoaderSelectionViewModel()
        {
            this.datasetTypeToDatasetLoader = new Dictionary<DatasetLoader.SupportedDatasetTypes, DatasetLoaderViewModelBase>();
            this.DatasetLoaderTypes = new ObservableCollection<DatasetLoader.SupportedDatasetTypes>();

            // When datasets are selected or unselected, update 
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not there are dataset loader selection options.
        /// </summary>
        public bool ShouldShowSelection
        {
            get { return this.shouldShowSelection; }
            private set
            {
                if (this.shouldShowSelection != value)
                {
                    this.shouldShowSelection = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the list of available dataset loaders.
        /// </summary>
        public ReadOnlyCollection<IDatasetLoader> DatasetLoaders { get; private set; }

        /// <summary>
        /// Gets the types of dataset loaders that can be selected.
        /// </summary>
        public ObservableCollection<DatasetLoader.SupportedDatasetTypes> DatasetLoaderTypes { get; private set; }

        /// <summary>
        /// Gets the selected dataset loader.
        /// </summary>
        public DatasetLoaderViewModelBase SelectedDatasetLoader
        {
            get { return this.selectedDatasetLoader; }
            private set
            {
                if (this.selectedDatasetLoader != value)
                {
                    this.selectedDatasetLoader = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// The type of supported dataset type that is selected.
        /// </summary>
        private DatasetLoader.SupportedDatasetTypes selectedDatasetLoaderType;

        /// <summary>
        /// Gets or sets the type of supported dataset type that is selected.
        /// </summary>
        public DatasetLoader.SupportedDatasetTypes SelectedDatasetLoaderType
        {
            get { return this.selectedDatasetLoaderType; }
            set
            {
                if (this.selectedDatasetLoaderType != value)
                {
                    this.selectedDatasetLoaderType = value;
                    this.SelectedDatasetLoader = this.datasetTypeToDatasetLoader[value];
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Sets a new set of dataset loaders that can be selected from the GUI.
        /// </summary>
        /// <param name="datasetLoaders">The new dataset loaders.</param>
        public void SetDatasetLoaders(IList<IDatasetLoader> datasetLoaders)
        {
            this.datasetTypeToDatasetLoader.Clear();
            this.DatasetLoaderTypes.Clear();
            this.DatasetLoaders = new ReadOnlyCollection<IDatasetLoader>(datasetLoaders);

            foreach (var loader in datasetLoaders)
            {
                var loaderViewModel = DatasetLoaderViewModelFactory.GetDatasetLoaderViewModel(loader);
                this.datasetTypeToDatasetLoader.Add(loaderViewModel.SupportedDatasetType, loaderViewModel);
                this.DatasetLoaderTypes.Add(loaderViewModel.SupportedDatasetType);
            }

            this.ShouldShowSelection = datasetLoaders.Count > 1;

            if (datasetLoaders.Count > 0)
            {
                this.SelectedDatasetLoader = this.datasetTypeToDatasetLoader.First().Value;
            }
        }

        /// <summary>
        /// Restore defaults on all available dataset loaders.
        /// </summary>
        public void RestoreDefaults()
        {
            foreach (var datasetLoader in this.DatasetLoaders)
            {
                datasetLoader.RestoreDefaults();
            }
        }
    }
}
