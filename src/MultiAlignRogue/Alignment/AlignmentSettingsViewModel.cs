using MultiAlignCore.Algorithms.Alignment;
using NHibernate.Mapping.ByCode;
using MultiAlignCore.Algorithms.Options;

namespace MultiAlignRogue.Alignment
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;

    using InformedProteomics.Backend.Utils;

    using MultiAlign.Data;
    using MultiAlign.ViewModels.Databases;

    using MultiAlignCore.Algorithms;
    using MultiAlignCore.Algorithms.Alignment.LcmsWarp;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO;
    using MultiAlignCore.IO.Hibernate;

    using MultiAlignRogue.Utils;
    using MultiAlignRogue.ViewModels;

    public class AlignmentSettingsViewModel : ViewModelBase
    {
        private const int DEFAULT_IMAGE_DPI = 96;
        private const int DEFAULT_IMAGE_WIDTH = 1024;
        private const int DEFAULT_IMAGE_HEIGHT = 768;

        /// <summary>
        /// The analysis information to run alignment on.
        /// </summary>
        private readonly MultiAlignAnalysis analysis;

        /// <summary>
        /// The cache for loading features from the analysis database.
        /// </summary>
        private readonly FeatureLoader featureCache;

        /// <summary>
        /// Factory for creating windows related to alignment (such as alignment plot windows).
        /// </summary>
        private readonly IAlignmentWindowFactory alignmentWindowFactory;

        /// <summary>
        /// Algorithm builder for creating correct aligner based on aligner selection type.
        /// </summary>
        private readonly AlgorithmBuilder builder;

        /// <summary>
        /// The selected aligner.
        /// </summary>
        private readonly LCMSFeatureAligner aligner;

        /// <summary>
        /// Stores the selected alignment algorithm based on parameters selected here.
        /// </summary>
        private AlgorithmProvider algorithms;

        /// <summary>
        /// The selected dataset for pairwise alignments.
        /// </summary>
        private DatasetInformationViewModel selectedBaseline;

        /// <summary>
        /// The alignment data for each aligned dataset.
        /// </summary>
        private readonly List<AlignmentData> alignmentInformation;

        /// <summary>
        /// The progress percentage for the alignment process.
        /// </summary>
        private double alignmentProgress;

        /// <summary>
        /// A value indicating whether the alignment progress bar should be displayed.
        /// </summary>
        private bool showAlignmentProgress;

        /// <summary>
        /// DPI of saved images
        /// </summary>
        private double imageDPI = DEFAULT_IMAGE_DPI;

        /// <summary>
        /// Pixel width of saved images
        /// </summary>
        private double imageWidth = DEFAULT_IMAGE_WIDTH;

        /// <summary>
        /// Pixel height of saved images
        /// </summary>
        private double imageHeight = DEFAULT_IMAGE_HEIGHT;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlignmentSettingsViewModel"/> class.
        /// </summary>
        /// <param name="analysis">The analysis information to run alignment on.</param>
        /// <param name="featureCache">The cache for loading features from the analysis database.</param>
        /// <param name="datasets">List of all potential datasets to run alignment on.</param>
        /// <param name="alignmentWindowFactory">
        /// Factory for creating windows related to alignment (such as alignment plot windows).
        /// </param>
        public AlignmentSettingsViewModel(MultiAlignAnalysis analysis,
                                          FeatureLoader featureCache,
                                          ObservableCollection<DatasetInformationViewModel> datasets,
                                          IAlignmentWindowFactory alignmentWindowFactory = null)
        {
            this.analysis = analysis;
            this.featureCache = featureCache;
            this.Datasets = datasets;
            if (analysis.MetaData.BaselineDataset != null)
            {
                // Don't set baseline if there are no files to choose from yet
                this.selectedBaseline = datasets.First(x => x.Name == analysis.MetaData.BaselineDataset.DatasetName);
            }

            this.alignmentWindowFactory = alignmentWindowFactory ?? new AlignmentViewFactory();
            this.aligner = new LCMSFeatureAligner();
            this.builder = new AlgorithmBuilder();
            this.CalibrationOptions = new ObservableCollection<LcmsWarpAlignmentType>(Enum.GetValues(typeof(LcmsWarpAlignmentType)).Cast<LcmsWarpAlignmentType>());
            this.AlignmentAlgorithms = new ObservableCollection<FeatureAlignmentType>(
                                           Enum.GetValues(typeof(FeatureAlignmentType)).Cast<FeatureAlignmentType>());
            this.alignmentInformation = new List<AlignmentData>();

            this.DatabaseSelectionViewModel = DatabaseSelectionViewModel.Instance;

            // When dataset is selected/unselected, update CanExecutes for alignment and plotting commands.
            this.MessengerInstance.Register<PropertyChangedMessage<bool>>(
                this,
                args =>
                {
                    if (args.Sender is DatasetInformationViewModel && args.PropertyName == "IsSelected")
                    {
                        ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                        ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                        ThreadSafeDispatcher.Invoke(() => this.SaveAlignmentPlotsCommand.RaiseCanExecuteChanged());
                    }
                });

            // When dataset state changes, update CanExecutes for alignment and plotting commands.
            this.MessengerInstance.Register<PropertyChangedMessage<DatasetInformationViewModel.DatasetStates>>(
                this,
                args =>
                {
                    if (args.Sender is DatasetInformationViewModel && args.PropertyName == "DatasetState")
                    {
                        ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                        ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                        ThreadSafeDispatcher.Invoke(() => this.SaveAlignmentPlotsCommand.RaiseCanExecuteChanged());
                    }
                });

            // When database server is selected, update CanExecute for the alignment command.
            this.MessengerInstance.Register<PropertyChangedMessage<DmsDatabaseServerViewModel>>(
                this,
                args =>
                {
                    if (args.Sender is DatabaseSelectionViewModel && args.PropertyName == "SelectedDatabaseServer")
                    {
                        ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                    }
                });

            this.AlignCommand = new RelayCommand(this.AsyncAlign, this.CanAlign);

            // Executable if the selected files are aligned and alignment information is available.
            this.DisplayAlignmentCommand = new RelayCommand(
                                                            this.DisplayAlignment,
                                                            () => this.Datasets.Any(file => file.IsAligned && file.IsSelected &&
                                                                  this.alignmentInformation.Any(data => data.DatasetID == file.DatasetId)));

            // Executable if the selected files are aligned and alignment information is available.
            this.SaveAlignmentPlotsCommand = new RelayCommand(
                                                            this.SaveAlignmentPlots,
                                                            () => this.Datasets.Any(file => file.IsAligned && file.IsSelected &&
                                                                  this.alignmentInformation.Any(data => data.DatasetID == file.DatasetId)));

            this.RestoreDefaultsCommand = new RelayCommand(this.RestoreDefaults);
        }

        public AlignmentSettingsViewModel()
        {
            this.DimensionSettings = new ObservableCollection<AlignmentDimensionSettingsViewModel>
            {
                new AlignmentDimensionSettingsViewModel(new AlignmentDimensionSettings()),
                new AlignmentDimensionSettingsViewModel(new AlignmentDimensionSettings { SeparationType = FeatureLight.SeparationTypes.DriftTime })
            };
        }

        /// <summary>
        /// Gets the alignment settings for each separation dimension.
        /// </summary>
        public ObservableCollection<AlignmentDimensionSettingsViewModel> DimensionSettings { get; private set; }

        /// <summary>
        /// Gets a command that performs the alignment on the selected atasets.
        /// </summary>
        public RelayCommand AlignCommand { get; private set; }

        /// <summary>
        /// Gets a command that displays the alignment information for the selected datasets.
        /// </summary>
        public RelayCommand DisplayAlignmentCommand { get; private set; }

        /// <summary>
        /// Gets a command that saves the alignment plots for the selected datasets.
        /// </summary>
        public RelayCommand SaveAlignmentPlotsCommand { get; private set; }

        public RelayCommand RestoreDefaultsCommand { get; private set; }

        /// <summary>
        /// Gets the list of possible alignment algorithms.
        /// </summary>
        public ObservableCollection<FeatureAlignmentType> AlignmentAlgorithms { get; private set; }

        /// <summary>
        /// Gets the list of possible alignment types.
        /// </summary>
        public ObservableCollection<LcmsWarpAlignmentType> CalibrationOptions { get; private set; }

        /// <summary>
        /// Gets the list of datasets that alignment can be ru non.
        /// </summary>
        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; }

        /// <summary>
        /// Gets the view model for selecting an AMT database.
        /// </summary>
        public DatabaseSelectionViewModel DatabaseSelectionViewModel { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this alignment is to a baseline dataset.
        /// True when aligning to a baseline dataset; false when aligning to an AMT tag database
        /// </summary>
        public bool ShouldAlignToBaseline
        {
            get { return !this.analysis.Options.AlignmentOptions.AlignToAMT; }
            set
            {
                var alignToAmt = !value;
                if (this.analysis.Options.AlignmentOptions.AlignToAMT == alignToAmt)
                {
                    return;
                }

                this.analysis.Options.AlignmentOptions.AlignToAMT = alignToAmt;
                this.AlignCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("ShouldAlignToAMT");
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this alignment is to an AMT database.
        /// </summary>
        public bool ShouldAlignToAMT
        {
            get { return this.analysis.Options.AlignmentOptions.AlignToAMT; }
            set
            {
                if (this.analysis.Options.AlignmentOptions.AlignToAMT == value)
                {
                    return;
                }

                this.analysis.Options.AlignmentOptions.AlignToAMT = value;
                this.AlignCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("ShouldAlignToBaseline");
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected baseline dataset.
        /// </summary>
        public DatasetInformationViewModel SelectedBaseline
        {
            get { return this.selectedBaseline; }
            set
            {
                if (this.selectedBaseline != value)
                {
                    if (value == null)
                    {
                        this.analysis.MetaData.BaselineDataset = null;
                        this.analysis.Options.AlignmentOptions.UsePromiscuousPoints = true;
                    }
                    else
                    {
                        this.selectedBaseline = value;
                        this.analysis.MetaData.BaselineDataset = value.Dataset;
                        this.analysis.Options.AlignmentOptions.UsePromiscuousPoints = true;
                    }

                    this.AlignCommand.RaiseCanExecuteChanged();
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected alignment algorithm.
        /// </summary>
        public FeatureAlignmentType SelectedAlignmentAlgorithm
        {
            get { return this.analysis.Options.AlignmentOptions.AlignmentAlgorithm; }
            set
            {
                if (this.analysis.Options.AlignmentOptions.AlignmentAlgorithm != value)
                {
                    this.analysis.Options.AlignmentOptions.AlignmentAlgorithm = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the possible alignment types.
        /// </summary>
        public LcmsWarpAlignmentType SelectedCalibrationType
        {
            get { return this.analysis.Options.AlignmentOptions.AlignmentType; }
            set
            {
                if (this.analysis.Options.AlignmentOptions.AlignmentType != value)
                {
                    this.analysis.Options.AlignmentOptions.AlignmentType = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment contraction/expansion factor.
        /// </summary>
        public int AlignmentContractionFactor
        {
            get { return this.analysis.Options.AlignmentOptions.ContractionFactor; }
            set
            {
                if (this.analysis.Options.AlignmentOptions.ContractionFactor != value)
                {
                    this.analysis.Options.AlignmentOptions.ContractionFactor = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of sections to break the NETs into.
        /// </summary>
        public int AlignmentNumTimeSections
        {
            get { return this.analysis.Options.AlignmentOptions.NumTimeSections; }
            set
            {
                if (this.analysis.Options.AlignmentOptions.NumTimeSections != value)
                {
                    this.analysis.Options.AlignmentOptions.NumTimeSections = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the progress percentage for the alignment process.
        /// </summary>
        public double AlignmentProgress
        {
            get { return this.alignmentProgress; }
            private set
            {
                if (Math.Abs(this.alignmentProgress - value) > float.Epsilon)
                {
                    this.alignmentProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the alignment progress bar should be displayed.
        /// </summary>
        public bool ShowAlignmentProgress
        {
            get { return this.showAlignmentProgress; }
            private set
            {
                if (this.showAlignmentProgress != value)
                {
                    this.showAlignmentProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum number times a mass tag should be observed to be elgible for being
        /// a baseline feature.
        /// </summary>
        public int MinObservationCount
        {
            get { return this.analysis.Options.MassTagDatabaseOptions.MinimumObservationCountFilter; }
            set
            {
                if (this.analysis.Options.MassTagDatabaseOptions.MinimumObservationCountFilter != value)
                {
                    this.analysis.Options.MassTagDatabaseOptions.MinimumObservationCountFilter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// DPI of saved images
        /// </summary>
        public double ImageDPI
        {
            get { return this.imageDPI; }
            set
            {
                if (!this.imageDPI.Equals(value))
                {
                    this.imageDPI = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Pixel Width of saved images
        /// </summary>
        public double ImageWidth
        {
            get { return this.imageWidth; }
            set
            {
                if (!this.imageWidth.Equals(value))
                {
                    this.imageWidth = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Pixel Height of saved images
        /// </summary>
        public double ImageHeight
        {
            get { return this.imageHeight; }
            set
            {
                if (!this.imageHeight.Equals(value))
                {
                    this.imageHeight = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Performs alignment asynchronously.
        /// </summary>
        public async void AsyncAlign()
        {
            if (this.ShouldAlignToBaseline && this.SelectedBaseline != null)
            {
                await Task.Run(() => this.AlignToBaseline());
            }
            else if (this.ShouldAlignToAMT)
            {
                await Task.Run(() => this.AlignToBaseline());
            }
        }

        /// <summary>
        /// Reset the settings the default values
        /// </summary>
        public void RestoreDefaults()
        {
            var defaultOptions = new MultiAlignAnalysisOptions();

            ShouldAlignToAMT = defaultOptions.AlignmentOptions.AlignToAMT;

            SelectedAlignmentAlgorithm = defaultOptions.AlignmentOptions.AlignmentAlgorithm;
            SelectedCalibrationType = defaultOptions.AlignmentOptions.AlignmentType;

            AlignmentContractionFactor = defaultOptions.AlignmentOptions.ContractionFactor;
            AlignmentNumTimeSections = defaultOptions.AlignmentOptions.NumTimeSections;
            MinObservationCount = defaultOptions.MassTagDatabaseOptions.MinimumObservationCountFilter;

            DatabaseSelectionViewModel.MassTolerance = defaultOptions.LcmsClusteringOptions.InstrumentTolerances.Mass;
            DatabaseSelectionViewModel.NetTolerance = defaultOptions.LcmsClusteringOptions.InstrumentTolerances.Net;

            imageDPI = DEFAULT_IMAGE_DPI;
            imageWidth = DEFAULT_IMAGE_WIDTH;
            imageHeight = DEFAULT_IMAGE_HEIGHT;
        }

        /// <summary>
        /// Perform alignment.
        /// </summary>
        /// <param name="workFlowDatasets">Datasets to run on when being called externally form this view model.</param>
        /// <param name="workflowProgress">The progress reporter for when this method is called externally from this view model.</param>
        internal void AlignToBaseline(List<DatasetInformationViewModel> workFlowDatasets = null, IProgress<ProgressData> workflowProgress = null)
        {

            // Use Promiscuous points when aligning to an AMT tag database
            // Do not use Promiscuous points when aligning to a baseline dataset
            this.analysis.Options.AlignmentOptions.UsePromiscuousPoints = !this.ShouldAlignToBaseline;

            // Flag whether we are aligning to an AMT tag database
            this.analysis.Options.AlignmentOptions.LCMSWarpOptions.AlignToMassTagDatabase = !this.ShouldAlignToBaseline;

            // Show the progress bar
            this.ShowAlignmentProgress = true;
            var taskBarProgress = TaskBarProgress.GetInstance();
            taskBarProgress.ShowProgress(this, true);

            //Update algorithms and providers
            this.featureCache.Providers = this.analysis.DataProviders;
            this.algorithms = this.builder.GetAlgorithmProvider(this.analysis.Options);

            ////this.algorithms.DatabaseAligner.Progress += aligner_Progress;
            ////this.algorithms.DatasetAligner.Progress += aligner_Progress;

            this.aligner.m_algorithms = this.algorithms;
            var baselineFeatures = new List<UMCLight>();

            if (this.ShouldAlignToBaseline)
            {
                baselineFeatures = this.featureCache.Providers.FeatureCache.FindByDatasetId(this.selectedBaseline.DatasetId);
                this.SelectedBaseline.DatasetState = DatasetInformationViewModel.DatasetStates.Baseline;
                var priorAlignment = (from x in this.alignmentInformation where x.DatasetID == this.selectedBaseline.DatasetId select x).ToList();
                if (priorAlignment.Any())
                {
                    this.alignmentInformation.Remove(priorAlignment.Single());
                }
            }
            var alignmentData = new AlignmentDAOHibernate();
            alignmentData.ClearAll();

            var selectedFiles = workFlowDatasets ??
                                this.Datasets.Where(file => file.IsSelected && !file.DoingWork &&
                                                            (this.ShouldAlignToAMT || !file.IsBaseline)).ToList();
            foreach (var file in selectedFiles)
            {
                file.DatasetState = DatasetInformationViewModel.DatasetStates.Aligning;
            }

            workflowProgress = workflowProgress ?? new Progress<ProgressData>();
            IProgress<ProgressData> totalProgress = new Progress<ProgressData>(pd =>
            {
                this.AlignmentProgress = pd.Percent;
                workflowProgress.Report(pd);
                taskBarProgress.SetProgress(this, pd.Percent);
            });
            var totalProgressData = new ProgressData(totalProgress);

            DatabaseIndexer.IndexClustersDrop(NHibernateUtil.Path);
            DatabaseIndexer.IndexFeaturesDrop(NHibernateUtil.Path);

            var i = 1;
            foreach (var file in selectedFiles)
            {
                ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.SaveAlignmentPlotsCommand.RaiseCanExecuteChanged());
                if ((file.Dataset.IsBaseline || !file.FeaturesFound) && this.ShouldAlignToBaseline)
                {
                    file.DatasetState = DatasetInformationViewModel.DatasetStates.Aligned;
                    continue;
                }

                this.analysis.DataProviders.DatabaseLock.EnterReadLock();
                IList<UMCLight> features = this.featureCache.Providers.FeatureCache.FindByDatasetId(file.DatasetId);
                this.analysis.DataProviders.DatabaseLock.ExitReadLock();
                AlignmentData alignment;

                totalProgressData.StepRange((100.0 * i++) / selectedFiles.Count);

                var fileInstance = file;
                var datasetProgress =
                    new Progress<ProgressData>(
                        pd =>
                        {
                            fileInstance.Progress = pd.Percent;
                            totalProgressData.Report(pd.Percent);
                        });

                if (this.ShouldAlignToBaseline)
                {
                    // Aligning to a baseline dataset
                    alignment = this.aligner.AlignToDataset(ref features, file.Dataset, baselineFeatures, datasetProgress);
                    alignment.BaselineIsAmtDB = false;
                }
                else
                {
                    // Aligning to a database
                    alignment = this.aligner.AlignToDatabase(ref features, file.Dataset, this.analysis.MassTagDatabase, datasetProgress);
                    alignment.BaselineIsAmtDB = true;
                }

                // Check if there is information from a previous alignment for this dataset. If so, replace it. If not, just add the new one.
                var priorAlignment = this.alignmentInformation.Where(x => x.DatasetID == alignment.DatasetID).ToList();
                if (priorAlignment.Any())
                {
                    this.alignmentInformation.Remove(priorAlignment.Single());
                    this.alignmentInformation.Add(alignment);
                }
                else
                {
                    this.alignmentInformation.Add(alignment);
                }
                file.Dataset.AlignmentData = alignment;
                
                this.analysis.DataProviders.DatabaseLock.EnterWriteLock();
                this.featureCache.CacheFeatures(features);
                this.analysis.DataProviders.DatabaseLock.ExitWriteLock();
                file.DatasetState = DatasetInformationViewModel.DatasetStates.Aligned;
                ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.SaveAlignmentPlotsCommand.RaiseCanExecuteChanged());
                file.Progress = 0;
            }

            if (this.ShouldAlignToBaseline)
            {
                this.SelectedBaseline.DatasetState = DatasetInformationViewModel.DatasetStates.Aligned;
            }

            DatabaseIndexer.IndexFeatures(NHibernateUtil.Path);

            taskBarProgress.ShowProgress(this, false);
            this.ShowAlignmentProgress = false;
            this.AlignmentProgress = 0;
        }

        /// <summary>
        /// Display alignment information for the selected datasets.
        /// </summary>
        private void DisplayAlignment()
        {
            var files = this.Datasets.Where(x => x.IsSelected && x.IsAligned && !x.IsBaseline);
            var alignments = files.SelectMany(file => this.alignmentInformation.Where(x => x.DatasetID == file.DatasetId))
                                  .Where(alignment => alignment != null)
                                  .Select(alignment => new AlignmentViewModel(alignment));

            this.alignmentWindowFactory.CreateNewWindow(alignments);
        }

        /// <summary>
        /// Save alignment plots for the selected datasets.
        /// </summary>
        private void SaveAlignmentPlots()
        {
            foreach (var file in (this.Datasets.Where(x => x.IsSelected && x.IsAligned))) //x => x.IsAligned && !x.Dataset.IsBaseline
            {
                var alignment = this.alignmentInformation.Find(x => x.DatasetID == file.DatasetId);
                if (alignment != null)
                {
                    var plots = new AlignmentPlotCreator(alignment);

                    var dirPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), file.Name);
                    if (!System.IO.Directory.Exists(dirPath))
                    {
                        System.IO.Directory.CreateDirectory(dirPath);
                    }
                    var path = System.IO.Path.Combine(dirPath, "Alignment_");

                    plots.SavePlots(path, this.ImageWidth, this.ImageHeight, this.ImageDPI);
                }
                else
                {
                    MessageBox.Show(String.Format("No alignment plots to save for {0}", file.Dataset.DatasetName));
                }
            }
        }

        /// <summary>
        /// Determines whether a valid permutation of configuration settings have been selected.
        /// </summary>
        /// <returns>A value indicating whether alignment can be performed.</returns>
        private bool CanAlign()
        {
            var selectedDatasets = this.Datasets.Where(ds => ds.IsSelected).ToList();
            var validDatasetsSelected = selectedDatasets.Any(ds => !ds.DoingWork && ds.FeaturesFound);
            var validBaselineSelected = this.ShouldAlignToBaseline && this.SelectedBaseline != null &&
                // something other than baseline should be selected
                                        selectedDatasets.Any(ds => ds != this.SelectedBaseline);
            var validAmtSelected = this.ShouldAlignToAMT && this.analysis.MassTagDatabase != null;
            return validDatasetsSelected && (validBaselineSelected || validAmtSelected);
        }
    }
}
