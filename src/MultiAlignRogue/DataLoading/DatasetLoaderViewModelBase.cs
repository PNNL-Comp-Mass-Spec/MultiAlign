namespace MultiAlignRogue.DataLoading
{
    using System;
    using System.Threading.Tasks;

    using System.Windows.Input;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.MetaData;
    using MultiAlignCore.IO.DatasetLoaders;

    /// <summary>
    /// This class serves as the base class for all dataset loader view models.
    /// This has the basic functionality to start the loading process and report progress.
    /// </summary>
    public class DatasetLoaderViewModelBase : ViewModelBase
    {
        /// <summary>
        /// A value indicating whether the loading progress bar should be shown.
        /// </summary>
        private bool shouldShowProgress;

        /// <summary>
        /// The loading progress percentage.
        /// </summary>
        private double loadingProgress;

        /// <summary>
        /// The dataset loader to load data for.
        /// </summary>
        protected IDatasetLoader DatasetLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetLoaderViewModelBase" /> class. 
        /// </summary>
        public DatasetLoaderViewModelBase()
        {
            this.LoadDatasetCommand = new RelayCommand(async () => await this.Load());
            this.ShouldShowProgress = false;
        }

        /// <summary>
        /// Gets or sets the dataset to load.
        /// </summary>
        public DatasetInformation SelectedDataset { get; set; }

        /// <summary>
        /// Gets or sets the supported dataset type that the dataset loader loads.
        /// </summary>
        public DatasetLoader.SupportedDatasetTypes SupportedDatasetType { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the loading progress bar should be shown.
        /// </summary>
        public bool ShouldShowProgress
        {
            get { return this.shouldShowProgress; }
            protected set
            {
                if (!this.shouldShowProgress != value)
                {
                    this.shouldShowProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the loading progress percentage.
        /// </summary>
        public double LoadingProgress
        {
            get { return this.loadingProgress; }
            protected set
            {
                if (!this.loadingProgress.Equals(value))
                {
                    this.loadingProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a command that loads the dataset when executed.
        /// </summary>
        public ICommand LoadDatasetCommand { get; private set; }

        /// <summary>
        /// Loads the dataset.
        /// </summary>
        protected virtual async Task Load()
        {
            this.ShouldShowProgress = true;
            var progressReporter = new Progress<ProgressData>(pd => this.LoadingProgress = pd.Percent);
            await Task.Run(() => this.DatasetLoader.Load(this.SelectedDataset, progressReporter));
            this.ShouldShowProgress = false;
        }
    }
}
