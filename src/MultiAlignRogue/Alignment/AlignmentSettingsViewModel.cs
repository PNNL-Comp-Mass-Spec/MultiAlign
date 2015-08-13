using MultiAlign.ViewModels.Databases;
using MultiAlign.Windows.Viewers.Databases;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.MTDB;
using PNNLOmics.Data.Features;

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

    using MultiAlign.Data;
    using MultiAlign.ViewModels.Datasets;

    using MultiAlignCore.Algorithms;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.IO;
    using MultiAlignCore.IO.Hibernate;

    using PNNLOmics.Algorithms.Alignment.LcmsWarp;

    public class AlignmentSettingsViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysis analysis;

        private readonly FeatureLoader featureCache;

        private readonly IAlignmentWindowFactory alignmentWindowFactory;

        private readonly IProgress<int> progress;

        private readonly AlgorithmBuilder builder;

        private readonly LCMSFeatureAligner aligner;

        private AlgorithmProvider algorithms;

        private IReadOnlyCollection<DatasetInformationViewModel> selectedDatasets;

        private DatasetInformationViewModel selectedBaseline;

        private List<classAlignmentData> alignmentInformation;

        public AlignmentSettingsViewModel(MultiAlignAnalysis analysis,
                                          FeatureLoader featureCache,
                                          IAlignmentWindowFactory alignmentWindowFactory = null,
                                          IProgress<int> progressReporter = null)
        {
            this.analysis = analysis;
            this.featureCache = featureCache;
            this.alignmentWindowFactory = alignmentWindowFactory ?? new AlignmentViewFactory();
            this.progress = progressReporter ?? new Progress<int>();
            this.aligner = new LCMSFeatureAligner();
            this.builder = new AlgorithmBuilder();
            this.CalibrationOptions = new ObservableCollection<AlignmentType>(Enum.GetValues(typeof(AlignmentType)).Cast<AlignmentType>());
            this.AlignmentAlgorithms = new ObservableCollection<FeatureAlignmentType>(
                                           Enum.GetValues(typeof(FeatureAlignmentType)).Cast<FeatureAlignmentType>());
            this.ShouldAlignToBaseline = true;
            this.ShouldAlignToAMT = false;
            this.selectedDatasets = new ReadOnlyCollection<DatasetInformationViewModel>(new List<DatasetInformationViewModel>());
            this.alignmentInformation = new List<classAlignmentData>();

            this.MessengerInstance.Register<PropertyChangedMessage<IReadOnlyCollection<DatasetInformationViewModel>>>(this, sds =>
            {
                this.selectedDatasets = sds.NewValue;
                ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
            });

            this.AlignCommand = new RelayCommand(this.AsyncAlign, () => this.selectedDatasets != null &&
                                                                                  this.selectedDatasets.Count > 0 &&
                                                                                  this.selectedDatasets.Any(file => !file.DoingWork));
            this.DisplayAlignmentCommand = new RelayCommand(this.DisplayAlignment, () => this.selectedDatasets.Any(file => file.IsAligned && !file.Dataset.IsBaseline));
        }

        public RelayCommand AlignCommand { get; private set; }

        public RelayCommand DisplayAlignmentCommand { get; private set; }

        public ObservableCollection<FeatureAlignmentType> AlignmentAlgorithms { get; private set; }

        public ObservableCollection<AlignmentType> CalibrationOptions { get; private set; }

        public bool ShouldAlignToBaseline { get; set; }

        public bool ShouldAlignToAMT { get; set; }

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
                    }
                    else
                    {
                        this.selectedBaseline = value;
                        this.analysis.MetaData.BaselineDataset = value.Dataset;
                    }

                    this.RaisePropertyChanged();
                }
            }
        }

        public double MassTolerance
        {
            get { return this.analysis.Options.LcmsClusteringOptions.InstrumentTolerances.Mass; }
            set
            {
                this.analysis.Options.LcmsClusteringOptions.InstrumentTolerances.Mass = value;
                this.RaisePropertyChanged();
            }
        }

        public double NetTolerance
        {
            get { return this.analysis.Options.LcmsClusteringOptions.InstrumentTolerances.Net; }
            set
            {
                this.analysis.Options.LcmsClusteringOptions.InstrumentTolerances.Net = value;
                this.RaisePropertyChanged();
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

        public async void AsyncAlign()
        {
            if (ShouldAlignToBaseline == true && this.SelectedBaseline != null)
            {
                await Task.Run(() => this.AlignToBaseline());
            }
            else if (ShouldAlignToAMT == true)
            {
                //await Task.Run(() => this.AlignToAMT());
                ThreadSafeDispatcher.Invoke(this.AlignToAMT);
            }
        }


        private void AlignToBaseline()
        {
            if (this.SelectedBaseline != null && this.SelectedBaseline.Dataset.FeaturesFound)
            {
                //Update algorithms and providers
                this.featureCache.Providers = this.analysis.DataProviders;
                this.algorithms = this.builder.GetAlgorithmProvider(this.analysis.Options);
                this.aligner.m_algorithms = this.algorithms;

                /*var baselineFeatures = this.featureCache.LoadDataset(this.selectedBaseline.Dataset, this.analysis.Options.MsFilteringOptions,
                    this.analysis.Options.LcmsFindingOptions, this.analysis.Options.LcmsFilteringOptions);*/
                var baselineFeatures = this.featureCache.Providers.FeatureCache.FindByDatasetId(this.selectedBaseline.DatasetId);
                var alignmentData = new AlignmentDAOHibernate();
                alignmentData.ClearAll();

                this.SelectedBaseline.DatasetState = DatasetInformation.DatasetStates.Aligning;
                var selectedFiles = this.selectedDatasets.Where(file => !file.DoingWork).ToList();
                foreach (var file in selectedFiles)
                {
                    file.DatasetState = DatasetInformation.DatasetStates.Aligning;
                }

                foreach (var file in selectedFiles)
                {
                    ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                    ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                    if (file.Dataset.IsBaseline || !file.FeaturesFound)
                    {
                        file.DatasetState = DatasetInformation.DatasetStates.Aligned;
                        continue;
                    }
                    /*var features = this.featureCache.LoadDataset(file.Dataset, this.analysis.Options.MsFilteringOptions,
                        this.analysis.Options.LcmsFindingOptions, this.analysis.Options.LcmsFilteringOptions);*/

                    IList<UMCLight> features = this.featureCache.Providers.FeatureCache.FindByDatasetId(file.DatasetId);
                    var alignment = this.aligner.AlignToDataset(ref features, baselineFeatures, file.Dataset, this.selectedBaseline.Dataset);
                    //Check if there is information from a previous alignment for this dataset. If so, replace it. If not, just add the new one.
                    var priorAlignment = from x in this.alignmentInformation where x.DatasetID == alignment.DatasetID select x;
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
                    file.DatasetState = DatasetInformation.DatasetStates.Aligned;
                    ThreadSafeDispatcher.Invoke(() => this.AlignCommand.RaiseCanExecuteChanged());
                    ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                }

                this.SelectedBaseline.DatasetState = DatasetInformation.DatasetStates.Aligned;
            }
            else
            {
                MessageBox.Show("Please select a baseline with detected features.");
            }
        }
        

        private void AlignToAMT()
        {
            var dmsWindow = new DatabaseSearchToolWindow();
            var databaseView = new DatabasesViewModel();
            dmsWindow.DataContext = databaseView;
            dmsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var loader = MassTagDatabaseLoaderFactory.Create(MtdbDatabaseServerType.Dms);
            var databases = loader.LoadDatabases();

            foreach (var database in databases)
            {
                databaseView.AddDatabase(database);
            }


        }

        private void DisplayAlignment()
        {
            foreach (var file in (this.selectedDatasets.Where(x => x.IsAligned && !x.Dataset.IsBaseline)))
            {
                var alignment = this.alignmentInformation.Find(x => x.DatasetID == file.DatasetId);
                this.alignmentWindowFactory.CreateNewWindow(alignment);
            }
        }
    }
}
