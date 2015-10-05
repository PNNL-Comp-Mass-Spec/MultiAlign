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
            this.CalibrationOptions = new ObservableCollection<AlignmentType>(Enum.GetValues(typeof(AlignmentType)).Cast<AlignmentType>());
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
        }

        public RelayCommand AlignCommand { get; private set; }

        public RelayCommand DisplayAlignmentCommand { get; private set; }

        public ObservableCollection<FeatureAlignmentType> AlignmentAlgorithms { get; private set; }

        public ObservableCollection<AlignmentType> CalibrationOptions { get; private set; }

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; private set; }

        public DatabaseSelectionViewModel DatabaseSelectionViewModel { get; private set; }

        /// <summary>
        /// True when aligning to a baseline dataset; false when aligning to an AMT tag database
        /// </summary>
        public bool ShouldAlignToBaseline
        {
            get { return !this.analysis.Options.AlignmentOptions.AlignToAMT; }
            set
            {
                if (this.analysis.Options.AlignmentOptions.AlignToAMT == value)
                {
                    this.analysis.Options.AlignmentOptions.AlignToAMT = !value;
                    this.AlignCommand.RaiseCanExecuteChanged();
                    this.RaisePropertyChanged("ShouldAlignToAMT");
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool ShouldAlignToAMT
        {
            get { return this.analysis.Options.AlignmentOptions.AlignToAMT; }
            set
            {
                if (this.analysis.Options.AlignmentOptions.AlignToAMT != value)
                {
                    this.analysis.Options.AlignmentOptions.AlignToAMT = value;
                    this.AlignCommand.RaiseCanExecuteChanged();
                    this.RaisePropertyChanged("ShouldAlignToBaseline");
                    this.RaisePropertyChanged();
                }
            }
        }

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

        public AlignmentType SelectedCalibrationType
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
        

        private double alignmentProgress;
        public double AlignmentProgress
        {
            get { return this.alignmentProgress; }
            set
            {
                if (Math.Abs(this.alignmentProgress - value) > float.Epsilon)
                {
                    this.alignmentProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool showAlignmentProgress;
        public bool ShowAlignmentProgress
        {
            get { return this.showAlignmentProgress; }
            set
            {
                if (this.showAlignmentProgress != value)
                {
                    this.showAlignmentProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

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
        
        public async void AsyncAlign()
        {
            if (ShouldAlignToBaseline && this.SelectedBaseline != null)
            {
                await Task.Run(() => this.AlignToBaseline());
            }
            else if (ShouldAlignToAMT)
            {
                await Task.Run(() => this.AlignToBaseline());
            }
        }

        internal void AlignToBaseline(List<DatasetInformationViewModel> workFlowDatasets = null, IProgress<ProgressData> workflowProgress = null)
        {

            // Use Promiscuous points when aligning to an AMT tag database
            // Do not use Promiscuous points when aligning to a baseline dataset
            this.analysis.Options.AlignmentOptions.UsePromiscuousPoints = !ShouldAlignToBaseline;

            // Flag whether we are aligning to an AMT tag database
            this.analysis.Options.AlignmentOptions.LCMSWarpOptions.AlignToMassTagDatabase = !ShouldAlignToBaseline;
            
            // Show the progress bar
            ShowAlignmentProgress = true;
            TaskBarProgressSingleton.ShowTaskBarProgress(this, true);

            //Update algorithms and providers
            this.featureCache.Providers = this.analysis.DataProviders;
            this.algorithms = this.builder.GetAlgorithmProvider(this.analysis.Options);

            ////this.algorithms.DatabaseAligner.Progress += aligner_Progress;
            ////this.algorithms.DatasetAligner.Progress += aligner_Progress;

            this.aligner.m_algorithms = this.algorithms;
            var baselineFeatures = new List<UMCLight>();

            if (ShouldAlignToBaseline)
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
                TaskBarProgressSingleton.SetTaskBarProgress(this, pd.Percent);
            });
            var totalProgressData = new ProgressData(totalProgress);

            DatabaseIndexer.IndexClustersDrop(NHibernateUtil.Path);
            DatabaseIndexer.IndexFeaturesDrop(NHibernateUtil.Path);

            int i = 1;
            foreach (var file in selectedFiles)
            {
                ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                if ((file.Dataset.IsBaseline || !file.FeaturesFound) && ShouldAlignToBaseline)
                {
                    file.DatasetState = DatasetInformationViewModel.DatasetStates.Aligned;
                    continue;
                }

                IList<UMCLight> features = this.featureCache.Providers.FeatureCache.FindByDatasetId(file.DatasetId);
                AlignmentData alignment;

                totalProgressData.StepRange((100.0 * i++) / selectedFiles.Count);

                var datasetProgress =
                    new Progress<ProgressData>(
                        pd =>
                        {
                            file.Progress = pd.Percent;
                            totalProgressData.Report(pd.Percent);
                        });

                if (this.ShouldAlignToBaseline)
                {                 
                    // Aligning to a baseline dataset
                    alignment = this.aligner.AlignToDataset(ref features, file.Dataset, baselineFeatures, datasetProgress);
                    alignment.baselineIsAmtDB = false;
                }
                else
                {
                    // Aligning to a database
                    alignment = this.aligner.AlignToDatabase(ref features, file.Dataset, this.analysis.MassTagDatabase, datasetProgress);
                    alignment.baselineIsAmtDB = true;
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
             
                this.featureCache.CacheFeatures(features);
                file.DatasetState = DatasetInformationViewModel.DatasetStates.Aligned;
                ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                file.Progress = 0;
            }

            if (ShouldAlignToBaseline)
            {
                this.SelectedBaseline.DatasetState = DatasetInformationViewModel.DatasetStates.Aligned;
            }

            DatabaseIndexer.IndexFeatures(NHibernateUtil.Path);

            TaskBarProgressSingleton.ShowTaskBarProgress(this, false);
            ShowAlignmentProgress = false;
            this.AlignmentProgress = 0;
        }

        private void DisplayAlignment()
        {
            foreach (var file in (this.Datasets.Where(x => x.IsSelected && x.IsAligned))) //x => x.IsAligned && !x.Dataset.IsBaseline
            {
                var alignment = this.alignmentInformation.Find(x => x.DatasetID == file.DatasetId);
                if (alignment != null)
                {
                    this.alignmentWindowFactory.CreateNewWindow(alignment);
                }
                else
                {
                    MessageBox.Show(String.Format("No alignment to show for {0}", file.Dataset.DatasetName));
                }
            }
        }

        private bool CanAlign()
        {
            var selectedDatasets = this.Datasets.Where(ds => ds.IsSelected).ToList();
            var validDatasetsSelected = selectedDatasets.Any(ds => !ds.DoingWork && ds.FeaturesFound);
            var validBaselineSelected = this.ShouldAlignToBaseline && this.SelectedBaseline != null &&
                                        // something other than baseline should be selected
                                        selectedDatasets.Any(ds => ds != this.SelectedBaseline);
            var validAmtSelected = this.ShouldAlignToAMT && analysis.MassTagDatabase != null;
            return validDatasetsSelected && (validBaselineSelected || validAmtSelected);
        }
    }
}
