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
            this.selectedDatasets = new ReadOnlyCollection<DatasetInformationViewModel>(new List<DatasetInformationViewModel>());
            this.alignmentInformation = new List<classAlignmentData>();

            this.MessengerInstance.Register<PropertyChangedMessage<IReadOnlyCollection<DatasetInformationViewModel>>>(this, sds =>
            {
                this.selectedDatasets = sds.NewValue;
                ThreadSafeDispatcher.Invoke(() => this.AlignToBaselineCommand.RaiseCanExecuteChanged());
                ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
            });

            this.AlignToBaselineCommand = new RelayCommand(this.AsyncAlignToBaseline, () => this.SelectedBaseline != null && 
                                                                                  this.selectedDatasets != null &&
                                                                                  this.selectedDatasets.Count > 0 &&
                                                                                  this.selectedDatasets.Any(file => !file.DoingWork));
            this.DisplayAlignmentCommand = new RelayCommand(this.DisplayAlignment, () => this.selectedDatasets.Any(file => file.IsAligned));
        }

        public RelayCommand AlignToBaselineCommand { get; private set; }

        public RelayCommand DisplayAlignmentCommand { get; private set; }

        public ObservableCollection<FeatureAlignmentType> AlignmentAlgorithms { get; private set; }

        public ObservableCollection<AlignmentType> CalibrationOptions { get; private set; }

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

        public async void AsyncAlignToBaseline()
        {
            await Task.Run(() => this.AlignToBaseline());
        }

        private void AlignToBaseline()
        {
            if (this.SelectedBaseline != null && this.SelectedBaseline.Dataset.FeaturesFound)
            {
                //Update algorithms and providers
                this.featureCache.Providers = this.analysis.DataProviders;
                this.algorithms = this.builder.GetAlgorithmProvider(this.analysis.Options);
                this.aligner.m_algorithms = this.algorithms;

                var baselineFeatures = this.featureCache.LoadDataset(this.selectedBaseline.Dataset, this.analysis.Options.MsFilteringOptions,
                    this.analysis.Options.LcmsFindingOptions, this.analysis.Options.LcmsFilteringOptions);
                var alignmentData = new AlignmentDAOHibernate();
                alignmentData.ClearAll();

                foreach (var file in this.selectedDatasets.Where(file => !file.DoingWork))
                {
                    file.IsAligning = true;
                    ThreadSafeDispatcher.Invoke(() => this.AlignToBaselineCommand.RaiseCanExecuteChanged());
                    ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                    if (file.Dataset.IsBaseline || !file.FeaturesFound) continue;
                    var features = this.featureCache.LoadDataset(file.Dataset, this.analysis.Options.MsFilteringOptions,
                        this.analysis.Options.LcmsFindingOptions, this.analysis.Options.LcmsFilteringOptions);
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
                    file.IsAligned = true;
                    file.IsAligning = false;
                    ThreadSafeDispatcher.Invoke(() => this.AlignToBaselineCommand.RaiseCanExecuteChanged());
                    ThreadSafeDispatcher.Invoke(() => this.DisplayAlignmentCommand.RaiseCanExecuteChanged());
                }
            }
            else
            {
                MessageBox.Show("Please select a baseline with detected features.");
            }
        }

        private void DisplayAlignment()
        {
            foreach (var file in (this.selectedDatasets.Where(x => x.IsAligned)))
            {
                var alignment = this.alignmentInformation.Find(x => x.DatasetID == file.DatasetId);
                this.alignmentWindowFactory.CreateNewWindow(alignment);
            }
        }
    }
}
