namespace MultiAlignRogue.AMTMatching
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;

    using InformedProteomics.Backend.Utils;

    using MultiAlign.Data;

    using MultiAlignCore.Algorithms.FeatureMatcher;
    using MultiAlignCore.Algorithms.FeatureMatcher.Data;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MassTags;

    using MultiAlignRogue.ViewModels;

    using NHibernate.Util;

    /// <summary>
    /// View model for settings options for AMT tag matching with STAC.
    /// </summary>
    public class StacSettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// The analysis information to run matching on.
        /// </summary>
        private readonly MultiAlignAnalysis analysis;

        /// <summary>
        /// All possible datasets to run AMT tag matching on.
        /// </summary>
        private readonly ObservableCollection<DatasetInformationViewModel> datasets; 

        /// <summary>
        /// A value indicating whether the total progress bar should be displayed.
        /// </summary>
        private bool shouldShowTotalProgress;

        /// <summary>
        /// The total progress reported for AMT tag matching.
        /// </summary>
        private double totalProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="StacSettingsViewModel"/> class.
        /// </summary>
        /// <param name="analysis">The analysis information to run matching on.</param>
        /// <param name="datasets">All possible datasets to run AMT tag matching on.</param>
        public StacSettingsViewModel(MultiAlignAnalysis analysis, ObservableCollection<DatasetInformationViewModel> datasets)
        {
            this.analysis = analysis;
            this.datasets = datasets;
            this.DatabaseSelectionViewModel = DatabaseSelectionViewModel.Instance;
            this.PeakMatchingTypes = new ObservableCollection<PeakMatchingType>(
                                             Enum.GetValues(typeof(PeakMatchingType))
                                                 .Cast<PeakMatchingType>());

            // When a dataset's state changes, update the CanExecute of the PerformMatchingCommand.
            MessengerInstance.Register<PropertyChangedMessage<DatasetInformationViewModel.DatasetStates>>(
                this,
                args =>
                    {
                        if (args.Sender is DatasetInformationViewModel && args.PropertyName == "DatasetState")
                        {
                            ThreadSafeDispatcher.Invoke(this.PerformMatchingCommand.RaiseCanExecuteChanged);
                        }
                    });

            // When a dataset is selected/unselected, update the CanExecute of the PerformMatchingCommand.
            MessengerInstance.Register<PropertyChangedMessage<bool>>(
                this,
                args =>
                {
                    if (args.Sender is DatasetInformationViewModel && args.PropertyName == "IsSelected")
                    {
                        ThreadSafeDispatcher.Invoke(this.PerformMatchingCommand.RaiseCanExecuteChanged);
                    }
                });

            // Perform matching on datasets that have cluster info available, and are not
            // currently being run.
            this.PerformMatchingCommand = new RelayCommand(
                 async () => await this.PerformMatchingAsync(
                                        this.datasets.Where(ds => ds.IsClustered)
                                                     .Where(ds => !ds.DoingWork)),
                       () => this.datasets.Any(ds => ds.IsClustered && !ds.DoingWork));
        }

        /// <summary>
        /// Gets a command that performs matching operation on the selected datasets.
        /// </summary>
        public RelayCommand PerformMatchingCommand { get; private set; }

        /// <summary>
        /// Gets the singleton instance for AMT tag database selection.
        /// </summary>
        public DatabaseSelectionViewModel DatabaseSelectionViewModel { get; private set; }

        /// <summary>
        /// Gets a collection of all possible peak matching types.
        /// </summary>
        public ObservableCollection<PeakMatchingType> PeakMatchingTypes { get; private set; }

        /// <summary>
        /// Gets or sets the selected peak matching type.
        /// </summary>
        public PeakMatchingType SelectedPeakMatchingType
        {
            get { return this.analysis.Options.StacOptions.IdentificationAlgorithm; }
            set
            {
                if (this.analysis.Options.StacOptions.IdentificationAlgorithm != value)
                {
                    this.analysis.Options.StacOptions.IdentificationAlgorithm = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the matcher should calculate the SLiC score.
        /// </summary>
        public bool ShouldCalculateSlic
        {
            get { return this.analysis.Options.StacOptions.ShouldCalculateSLiC; }
            set
            {
                if (this.analysis.Options.StacOptions.ShouldCalculateSLiC != value)
                {
                    this.analysis.Options.StacOptions.ShouldCalculateSLiC = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the matcher should use the ellipsoid.
        /// </summary>
        public bool ShouldUseEllipsoid
        {
            get { return this.analysis.Options.StacOptions.UseEllipsoid; }
            set
            {
                if (this.analysis.Options.StacOptions.UseEllipsoid != value)
                {
                    this.analysis.Options.StacOptions.UseEllipsoid = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the bind width for the histograms.
        /// </summary>
        public double HistogramBinWidth
        {
            get { return this.analysis.Options.StacOptions.HistogramBinWidth; }
            set
            {
                if (this.analysis.Options.StacOptions.HistogramBinWidth != value)
                {
                    this.analysis.Options.StacOptions.HistogramBinWidth = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the multiplier for the histograms.
        /// </summary>
        public double HistogramMultiplier
        {
            get { return this.analysis.Options.StacOptions.HistogramMultiplier; }
            set
            {
                if (this.analysis.Options.StacOptions.HistogramMultiplier != value)
                {
                    this.analysis.Options.StacOptions.HistogramMultiplier = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the total progress bar should be displayed.
        /// </summary>
        public bool ShouldShowTotalProgress
        {
            get { return this.shouldShowTotalProgress; }
            set
            {
                if (this.shouldShowTotalProgress != value)
                {
                    this.shouldShowTotalProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the total progress reported for AMT tag matching.
        /// </summary>
        public double TotalProgress
        {
            get { return this.totalProgress; }
            set
            {
                if (this.totalProgress != value)
                {
                    this.totalProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Performs matching operation on the selected datasets asynchronously.
        /// </summary>
        /// <param name="datasets">The datasets to perform AMT matching on.</param>
        /// <returns>The <see cref="Task" />.</returns>
        internal async Task PerformMatchingAsync(IEnumerable<DatasetInformationViewModel> datasets)
        {
            await Task.Run(() => this.PerformMatching(datasets));
        }

        /// <summary>
        /// Performs matching operation on the selected datasets.
        /// </summary>
        /// <param name="datasets">The datasets to perform AMT matching on.</param>
        internal void PerformMatching(IEnumerable<DatasetInformationViewModel> datasets)
        {
            this.ShouldShowTotalProgress = true;
            var progressData = new ProgressData { IsPartialRange = true, MaxPercentage = 95 };
            IProgress<ProgressData> progress = new Progress<ProgressData>(pd => this.TotalProgress = progressData.UpdatePercent(pd.Percent).Percent);

            // Get all possible charge states in data.
            var chargeStates = this.analysis.DataProviders.FeatureCache.RetrieveChargeStates().ToList();

            // Initialize STAC
            var stac = new STACAdapter<UMCClusterLight>
            {
                Options = new FeatureMatcherParameters
                {
                    UseEllipsoid = this.ShouldUseEllipsoid,
                    ShouldCalculateSLiC = this.ShouldCalculateSlic,
                    HistogramBinWidth = this.HistogramBinWidth,
                    HistogramMultiplier = this.HistogramMultiplier,
                    UserTolerances = new FeatureMatcherTolerances
                    {
                        MassTolerancePPM = this.DatabaseSelectionViewModel.MassTolerance,
                        NETTolerance = this.DatabaseSelectionViewModel.NetTolerance
                    },
                    ChargeStateList = chargeStates,
                    UsePriors = false,
                    ShouldCalculateSTAC = false,
                    ShouldCalculateHistogramFDR = false,
                    ShouldCalculateShiftFDR = false,
                },
            };

            // Initialize datasets
            datasets.ForEach(ds => ds.DatasetState = DatasetInformationViewModel.DatasetStates.Matching);

            // Get clusters and database
            var clusters = this.analysis.DataProviders.ClusterCache.FindAll();
            var clusterIdMap = clusters.ToDictionary(cluster => cluster.Id);
            var database = this.analysis.MassTagDatabase;

            // Run STAC
            var matches = stac.PerformPeakMatching(clusters, database);

            datasets.ForEach(ds => ds.DatasetState = DatasetInformationViewModel.DatasetStates.PersistingMatches);

            // Persist matches
            var clusterToMassTags = matches.Select(match => new ClusterToMassTagMap(match.Observed.Id, match.Target.Id)).ToList();
            this.analysis.DataProviders.MassTagMatches.ClearAllMatches();
            this.analysis.DataProviders.MassTagMatches.AddAllStateless(clusterToMassTags, progress);

            // Write to file
            this.WriteClusterData("clustermatches.csv", matches, clusterIdMap, progress);
            this.ShouldShowTotalProgress = false;

            datasets.ForEach(ds => ds.DatasetState = DatasetInformationViewModel.DatasetStates.Matched);
        }

        /// <summary>
        /// Writes cluster data to a comma-separated values file.
        /// </summary>
        /// <param name="path">The path to the CSV file to write.</param>
        /// <param name="matches">The cluster-mass tag matches to write.</param>
        /// <param name="clusterIdMap">Dictionary mapping cluster IDs to clusters.</param>
        /// <param name="progress">Progress reporter.</param>
        private void WriteClusterData(
                                      string path,
                                      List<FeatureMatchLight<UMCClusterLight, MassTagLight>> matches,
                                      Dictionary<int, UMCClusterLight> clusterIdMap,
                                      IProgress<ProgressData> progress)
        {
            var progData = new ProgressData { ProgressObj = progress };
            int i = 1;

            using (var writer = File.CreateText(path))
            {
                writer.WriteLine(
                    "Cluster Id,Dataset Member Count,Total Member Count,Cluster Mass,Cluster NET,Abundance,Mass Tag Id,Peptide Sequence,Mass Tag Mono Mass,Mass Tag NET"); ////,STAC,STAC-UP");
                foreach (var match in matches)
                {
                    var cluster = clusterIdMap[match.Observed.Id];
                    writer.WriteLine(
                        "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                        cluster.Id,
                        cluster.DatasetMemberCount,
                        cluster.MemberCount,
                        cluster.MassMonoisotopic,
                        cluster.Net,
                        cluster.Abundance,
                        match.Target.Id,
                        match.Target.PeptideSequence,
                        match.Target.MassMonoisotopic,
                        match.Target.Net);
                        ////match.Confidence,
                        ////match.Uniqueness);

                    progData.Report(i++, matches.Count);
                }
            }
        }
    }
}
