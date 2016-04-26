namespace MultiAlignRogue.FeatureRefinement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using System.Windows.Input;

    using GalaSoft.MvvmLight.CommandWpf;
    using GalaSoft.MvvmLight.Messaging;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Algorithms.FeatureRefinement;
    using MultiAlignCore.IO.Features;
    using MultiAlignCore.IO.InputFiles;

    using MultiAlignRogue.Clustering;
    using MultiAlignRogue.Utils;
    using MultiAlignRogue.ViewModels;

    /// <summary>
    /// This class is a view model for editing the settings for running the feature refinement step of MultiAlign.
    /// </summary>
    public sealed class FeatureRefinementViewModel : SettingsEditorViewModelBase
    {
        /// <summary>
        /// The <see cref="FeatureRefiner" /> model that this class edits.
        /// </summary>
        private readonly FeatureRefiner featureRefiner;

        /// <summary>
        /// The total progress (as a percent) of the feature refinement process.
        /// </summary>
        private double progress;

        /// <summary>
        /// All datasets for the current project.
        /// </summary>
        private readonly IEnumerable<DatasetInformationViewModel> datasets;

        /// <summary>
        /// The data access providers for pulling/persisting features from the database.
        /// </summary>
        private readonly FeatureDataAccessProviders dataAccessProviders;

        /// <summary>
        /// A value indicating whether it is possible to create XICs for the selected dataset.
        /// </summary>
        private bool enableXicCreation;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureRefinementViewModel" /> class. 
        /// </summary>
        /// <param name="featureRefiner">The <see cref="FeatureRefiner" /> model that this class edits.</param>
        /// <param name="datasets">All datasets for the current project.</param>
        /// <param name="dataAccessProviders">The data access providers for pulling/persisting features from the database.</param>
        public FeatureRefinementViewModel(
                                          FeatureRefiner featureRefiner,
                                          IEnumerable<DatasetInformationViewModel> datasets,
                                          FeatureDataAccessProviders dataAccessProviders) 
            : base(featureRefiner)
        {
            this.featureRefiner = featureRefiner;
            this.datasets = datasets;
            this.dataAccessProviders = dataAccessProviders;
            this.DeisotopingCorrectorViewModel = new DeisotopingCorrectorViewModel(featureRefiner.DeiosotopingCorrector);
            this.ClusteringSettingsViewModel = new ClusterAlgorithmSettingsViewModel(featureRefiner.ClusteringOptions);
            this.XicCreatorViewModel = new XicCreatorViewModel(featureRefiner.XicCreator);
            this.RunCommand = new RelayCommand(() => this.RunFeatureRefinement());

            // When dataset is selected, determine if raw data is available to toggle XIC creation
            this.MessengerInstance.Register<PropertyChangedMessage<bool>>(this, this.UpdateDatasetSelection);
        }

        /// <summary>
        /// Gets or sets a value indicating whether deisotoping correction clustering should be
        /// performed on the LCMS features.
        /// </summary>
        public bool ShouldRunDeisotopingCorrection
        {
            get { return this.featureRefiner.ShouldRunDeisotopingCorrection; }
            set
            {
                if (this.featureRefiner.ShouldRunDeisotopingCorrection != value)
                {
                    this.featureRefiner.ShouldRunDeisotopingCorrection = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model for editing the deisotoping settings.
        /// </summary>
        public DeisotopingCorrectorViewModel DeisotopingCorrectorViewModel { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether clustering should be performed on the LCMS features.
        /// </summary>
        public bool ShouldCluster
        {
            get { return this.featureRefiner.ShouldCluster; }
            set
            {
                if (this.featureRefiner.ShouldCluster != value)
                {
                    this.featureRefiner.ShouldCluster = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the view model for editing the clustering settings.
        /// </summary>
        public ClusterAlgorithmSettingsViewModel ClusteringSettingsViewModel { get; private set; }

        /// <summary>
        /// Gets a value indicating whether it is possible to create XICs for the selected dataset.
        /// </summary>
        public bool EnableXicCreation
        {
            get { return this.enableXicCreation; }
            private set
            {
                if (this.enableXicCreation != value)
                {
                    this.enableXicCreation = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether extracted ion chromatograms should
        /// be extracted from the raw data for each feature.
        /// </summary>
        public bool ShouldCreateXics
        {
            get { return this.featureRefiner.ShouldCreateXics; }
            set
            {
                if (this.featureRefiner.ShouldCreateXics != value)
                {
                    this.featureRefiner.ShouldCreateXics = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gest the view model for editing the XIC creation settings.
        /// </summary>
        public XicCreatorViewModel XicCreatorViewModel { get; private set; }

        /// <summary>
        /// Gets or sets the total progress (as a percent) of the feature refinement process.
        /// </summary>
        public double Progress
        {
            get { return this.progress; }
            private set
            {
                if (!this.progress.Equals(value))
                {
                    this.progress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a command that runs the feature refinement process.
        /// </summary>
        public ICommand RunCommand { get; private set; }

        /// <summary>
        /// Override <see cref="SettingsEditorViewModelBase.UpdateAll" /> to update child view models.
        /// </summary>
        protected override void UpdateAll()
        {
            this.DeisotopingCorrectorViewModel.RestoreDefaultsCommand.Execute(null);
            this.XicCreatorViewModel.RestoreDefaultsCommand.Execute(null);
            this.ClusteringSettingsViewModel.RestoreDefaultsCommand.Execute(null);
            base.UpdateAll();
        }

        /// <summary>
        /// Run the feature refinement process.
        /// </summary>
        /// <param name="progressReporter">The progress reporter.</param>
        public void RunFeatureRefinement(IProgress<ProgressData> progressReporter = null)
        {
            progressReporter = progressReporter ?? new Progress<ProgressData>(pd => this.Progress = pd.Percent);

            foreach (var dataset in this.datasets.Where(ds => ds.IsSelected))
            {
                var features = this.dataAccessProviders.FeatureCache.FindByDatasetId(dataset.DatasetId);
                var scanSummaryProvider = this.dataAccessProviders
                                              .ScanSummaryProviderCache
                                              .GetScanSummaryProvider(dataset.DatasetId);
                var refinedFeatures = this.featureRefiner.Run(features, scanSummaryProvider, progressReporter);
                this.dataAccessProviders.FeatureCache.DeleteByDataset(dataset.DatasetId);
                this.dataAccessProviders.FeatureCache.AddAllStateless(refinedFeatures);
            }
        }

        /// <summary>
        /// Message handler for when a dataset is selected/unselected.
        /// Update information in this dataset based on the selected dataset.
        /// </summary>
        /// <param name="message">The message containing information about the dataset selected.</param>
        public void UpdateDatasetSelection(PropertyChangedMessage<bool> message)
        {
            var ds = message.Sender as DatasetInformationViewModel;
            if (ds != null && message.PropertyName == "IsSelected" && message.NewValue)
            {
                this.EnableXicCreation = ds.Dataset.RawFile.FileType == InputFileType.Raw;
                this.ShouldCreateXics &= this.EnableXicCreation;
            }
        }
    }
}
