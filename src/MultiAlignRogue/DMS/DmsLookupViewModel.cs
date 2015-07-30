// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DmsLookupViewModel.cs" company="Pacific Northwest National Laboratory">
//   2015 Pacific Northwest National Laboratory
// </copyright>
// <author>Christopher Wilkins</author>
// <summary>
//   A view model for searching DMS for datasets and jobs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Ookii.Dialogs.Wpf;

namespace MultiAlignRogue.DMS
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using MultiAlign.Data;

    using Ookii.Dialogs;

    /// <summary>
    /// A view model for searching DMS for datasets and jobs.
    /// </summary>
    public class DmsLookupViewModel : ViewModelBase
    {
        /// <summary>
        /// The name of the the previous results file
        /// </summary>
        private const string PreviousResultsFile = "dmsSearches.txt";

        /// <summary>
        /// Utility for querying DMS.
        /// </summary>
        private readonly DmsLookupUtility dmsLookupUtility;

        /// <summary>
        /// A value indicating whether or not the No Results alert should be shown.
        /// Set to true when a search has been performed that yielded 0 results.
        /// </summary>
        private bool isNoResultsShown;
        
        /// <summary>
        /// The selected DMS dataset.
        /// </summary>
        private DmsDatasetViewModel selectedDataset;

        /// <summary>
        /// The number of weeks in the past to search for datasets.
        /// </summary>
        private int numberOfWeeks;

        /// <summary>
        /// The name of the dataset to search for.
        /// </summary>
        private string dataSetFilter;

        /// <summary>
        /// Path for directory to copy DMS data set to.
        /// </summary>
        private string outputDirectory;

        /// <summary>
        /// The name of the file being copied.
        /// </summary>
        private string copyStatusText;

        /// <summary>
        /// A value indicating whether files are currently being copied.
        /// </summary>
        private bool isCopying;

        /// <summary>
        /// A value indicating whether files should be copied to the output directory upon opening.
        /// </summary>
        private bool shouldCopyFiles;

        /// <summary>
        /// The progress of the file copy.
        /// </summary>
        private double progress;

        /// <summary>
        /// A cancellation token for cancelling the file copy.
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DmsLookupViewModel"/> class.
        /// </summary>
        public DmsLookupViewModel()
        {
            this.SearchCommand = new RelayCommand(this.SearchImpl, () => !string.IsNullOrWhiteSpace(this.dataSetFilter) && this.numberOfWeeks > 0);
            this.OpenCommand = new RelayCommand(
                                                async () => await this.OpenImpl(), 
                                                () => this.SelectedDataset != null &&
                                                      !string.IsNullOrEmpty(this.SelectedDataset.DatasetFolderPath) &&
                                                      !string.IsNullOrEmpty(this.OutputDirectory) &&
                                                      Directory.Exists(this.OutputDirectory) && !this.IsCopying);
            this.CancelCommand = new RelayCommand(this.CancelImpl);
            this.BrowseOutputDirectoriesCommand = new RelayCommand(this.BrowseOutputDirectoriesImpl);

            this.ShouldCopyFiles = true;
            this.Status = false;
            this.NumberOfWeeks = 10;
            this.Datasets = new ObservableCollection<DmsDatasetViewModel>();
            this.AvailableFiles = new ObservableCollection<string>();
            this.dmsLookupUtility = new DmsLookupUtility();
            this.SelectedDataset = new DmsDatasetViewModel();
            this.OutputDirectory = string.Empty;
            this.CopyStatusText = string.Empty;
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// An event indicated that a dataset has been selected and copied, or cancelled.
        /// </summary>
        public event EventHandler DatasetSelected;

        /// <summary>
        /// Gets the list of data sets found through searching DMS.
        /// </summary>
        public ObservableCollection<DmsDatasetViewModel> Datasets { get; private set; }

        /// <summary>
        /// Gets the list of available files for a certain dataset.
        /// </summary>
        public ObservableCollection<string> AvailableFiles { get; private set; } 

        /// <summary>
        /// Gets a command that searches DMS for data sets.
        /// </summary>
        public RelayCommand SearchCommand { get; private set; }

        /// <summary>
        /// Gets a command that opens the selected data set and job.
        /// </summary>
        public RelayCommand OpenCommand { get; private set; }

        /// <summary>
        /// Gets a command that is executed when Cancel is clicked.
        /// </summary>
        public RelayCommand CancelCommand { get; private set; }

        /// <summary>
        /// Gets a command for browsing output directories.
        /// </summary>
        public RelayCommand BrowseOutputDirectoriesCommand { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a valid data set has been selected.
        /// </summary>
        public bool Status { get; private set; }

        /// <summary>
        /// Gets or sets the progress of the file copy.
        /// </summary>
        public double Progress
        {
            get { return this.progress; }
            private set
            {
                if (this.progress != value)
                {
                    this.progress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected DMS dataset.
        /// </summary>
        public DmsDatasetViewModel SelectedDataset
        {
            get { return this.selectedDataset; }
            set
            {
                if (this.selectedDataset != value)
                {
                    this.selectedDataset = value;
                    this.RaisePropertyChanged();
                    this.OpenCommand.RaiseCanExecuteChanged();

                    this.AvailableFiles.Clear();
                    var availableFiles = this.selectedDataset.GetAvailableFiles();
                    foreach (var file in availableFiles)
                    {
                        this.AvailableFiles.Add(Path.GetFileName(file));
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the No Results alert should be shown.
        /// Set to true when a search has been performed that yielded 0 results.
        /// </summary>
        public bool IsNoResultsShown
        {
            get { return this.isNoResultsShown; }
            private set
            {
                if (this.isNoResultsShown != value)
                {
                    this.isNoResultsShown = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of weeks in the past to search for datasets.
        /// </summary>
        public int NumberOfWeeks
        {
            get { return this.numberOfWeeks; }
            set
            {
                if (this.numberOfWeeks != value)
                {
                    this.numberOfWeeks = value;
                    this.RaisePropertyChanged();
                    this.SearchCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the dataset to search for.
        /// </summary>
        public string DatasetFilter
        {
            get { return this.dataSetFilter; }
            set
            {
                if (this.dataSetFilter != value)
                {
                    this.dataSetFilter = value;
                    this.RaisePropertyChanged();
                    this.SearchCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string OutputDirectory
        {
            get { return this.outputDirectory; }
            set
            {
                if (this.outputDirectory != value)
                {
                    this.outputDirectory = value;
                    this.RaisePropertyChanged();
                    this.OpenCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets the name of the file being copied.
        /// </summary>
        public string CopyStatusText
        {
            get { return this.copyStatusText; }
            private set
            {
                if (this.copyStatusText != value)
                {
                    this.copyStatusText = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether files are currently being copied.
        /// </summary>
        public bool IsCopying
        {
            get { return this.isCopying; }
            private set
            {
                if (this.isCopying != value)
                {
                    this.isCopying = value;
                    this.RaisePropertyChanged();
                    this.OpenCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether files should be copied to the output directory upon opening.
        /// </summary>
        public bool ShouldCopyFiles
        {
            get { return this.shouldCopyFiles; }
            set
            {
                if (this.shouldCopyFiles != value)
                {
                    this.shouldCopyFiles = value;
                    this.RaisePropertyChanged();
                    this.BrowseOutputDirectoriesCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Checks to see if the data set selected is a valid data set.
        /// </summary>
        /// <returns>A value indicating whether the data set selected is valid.</returns>
        public bool ValidateDataSet()
        {
            return this.SelectedDataset != null && !string.IsNullOrEmpty(this.SelectedDataset.DatasetFolderPath)
                    && Directory.Exists(this.SelectedDataset.DatasetFolderPath);
        }

        /// <summary>
        /// Get a list of all raw files associated with the selected data set.
        /// </summary>
        /// <returns>List containing full paths associated with the selected data set.</returns>
        public List<string> GetRawFileNames()
        {
            if (!this.ValidateDataSet())
            {
                return new List<string>();
            }

            var dataSetDirFiles = Directory.GetFiles(this.SelectedDataset.DatasetFolderPath);
            var rawFileNames = (from filePath in dataSetDirFiles
                            let ext = Path.GetExtension(filePath)
                            where !string.IsNullOrEmpty(ext)
                            let extL = ext.ToLower()
                            where (extL == ".raw" || extL == ".mzml" || extL == ".gz")
                            select filePath).ToList();
            for (int i = 0; i < rawFileNames.Count; i++)
            {
                var pbfFile = this.GetPbfFileName(rawFileNames[i]);
                if (!string.IsNullOrEmpty(pbfFile))
                {
                    rawFileNames[i] = pbfFile;
                }
            }

            return rawFileNames;
        }

        /// <summary>
        /// Copy files from DMS dataset folder to output directory.
        /// </summary>
        /// <returns>Task for awaiting file copy.</returns>
        public async Task CopyFilesAsync()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(
                () =>
                {
                    ThreadSafeDispatcher.Invoke(() => this.IsCopying = true);
                    this.Progress = 0;
                    var files = this.SelectedDataset.GetAvailableFiles();
                    if (Directory.Exists(this.OutputDirectory))
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            if (this.cancellationTokenSource.IsCancellationRequested)
                            {
                                break;
                            }

                            var fileName = Path.GetFileName(files[i]);
                            this.CopyStatusText = string.Format("Copying {0}", fileName);
                            File.Copy(files[i], string.Format("{0}\\{1}", this.OutputDirectory, fileName), true);

                            this.Progress = ((i + 1) / (double)files.Count) * 100;
                        }
                    }

                    this.CopyStatusText = string.Empty;
                    ThreadSafeDispatcher.Invoke(() => this.IsCopying = false);
                    this.Progress = 100;
                },
                this.cancellationTokenSource.Token);
        }

        /// <summary>
        /// Get the PBF file (if it exists) for a certain raw file associated with this data set.
        /// </summary>
        /// <param name="rawFilePath">The path of the raw file to find associated PBF files.</param>
        /// <returns>The full path to the PBF file.</returns>
        private string GetPbfFileName(string rawFilePath)
        {
            string pbfFilePath = null;
            if (!this.ValidateDataSet())
            {
                return null;
            }

            var dataSetDirDirectories = Directory.GetDirectories(this.SelectedDataset.DatasetFolderPath);
            var pbfFolderPath = (from folderPath in dataSetDirDirectories
                                 let folderName = Path.GetFileNameWithoutExtension(folderPath)
                                 where folderName.StartsWith("PBF_Gen")
                                 select folderPath).FirstOrDefault();
            if (!string.IsNullOrEmpty(pbfFolderPath))
            {
                var pbfIndirectionPath = string.Format(@"{0}\{1}.pbf_CacheInfo.txt", pbfFolderPath, Path.GetFileNameWithoutExtension(rawFilePath));
                if (!string.IsNullOrEmpty(pbfIndirectionPath) && File.Exists(pbfIndirectionPath))
                {
                    var lines = File.ReadAllLines(pbfIndirectionPath);
                    if (lines.Length > 0)
                    {
                        pbfFilePath = lines[0];
                    }
                }
            }

            return pbfFilePath;
        }

        /// <summary>
        /// Lookup datasets and jobs for the past NumberOfWeeks with a filter given by DatasetFilter
        /// </summary>
        private void SearchImpl()
        {
            this.Datasets.Clear();

            // Search DMS
            var dataSets = this.dmsLookupUtility.GetDatasets(this.NumberOfWeeks, this.DatasetFilter.Trim());
            foreach (var dataset in dataSets)
            {
                this.Datasets.Add(new DmsDatasetViewModel(dataset.Value));
            }

            // Select first dataset in search results, if available
            if (this.Datasets.Count == 0)
            {   // Show no results message if nothing was found.
                this.IsNoResultsShown = true;
            }
            else
            {
                this.IsNoResultsShown = false;
                this.SelectedDataset = this.Datasets[0];
            }
        }

        /// <summary>
        /// Browse output directories.
        /// </summary>
        private void BrowseOutputDirectoriesImpl()
        {
            var folderBrowser = new VistaFolderBrowserDialog();
            var result = folderBrowser.ShowDialog();
            if (result != null && result.Value)
            {
                this.OutputDirectory = folderBrowser.SelectedPath;
            }
        }

        private async Task OpenImpl()
        {
            if (this.ShouldCopyFiles)
            {
                await this.CopyFilesAsync();
            }

            if (this.DatasetSelected != null)
            {
                this.Status = true;
                this.DatasetSelected(this, EventArgs.Empty);
            }
        }

        private void CancelImpl()
        {
            if (this.IsCopying)
            {
                var result = System.Windows.MessageBox.Show(
                                    @"Are you sure that you would like to cancel the file copy?",
                                    @"Cancel?",
                                    MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    this.cancellationTokenSource.Cancel();
                }
            }
            else
            {
                if (this.DatasetSelected != null)
                {
                    this.Status = false;
                    this.DatasetSelected(this, EventArgs.Empty);
                }
            }

        }
    }
}
