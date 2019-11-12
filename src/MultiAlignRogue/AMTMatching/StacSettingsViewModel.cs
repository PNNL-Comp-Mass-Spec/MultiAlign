using System.Windows;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Data;
using MultiAlignCore.IO;
using MultiAlignRogue.Utils;

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

    using MultiAlignCore.Algorithms.FeatureMatcher;
    using MultiAlignCore.Algorithms.FeatureMatcher.Data;
    using MultiAlignCore.Extensions;

    using MultiAlignRogue.ViewModels;

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
        /// A value indicating whether the total progress bar should be displayed.
        /// </summary>
        private bool shouldShowTotalProgress;

        /// <summary>
        /// The total progress reported for AMT tag matching.
        /// </summary>
        private double totalProgress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="analysis">The analysis information to run matching on.</param>
        /// <param name="datasets">All possible datasets to run AMT tag matching on.</param>
        public StacSettingsViewModel(MultiAlignAnalysis analysis, IReadOnlyCollection<DatasetInformationViewModel> datasets)
        {
            this.analysis = analysis;
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
                                        datasets.Where(ds => ds.IsClustered)
                                                     .Where(ds => !ds.DoingWork)),
                       () => datasets.Any(ds => ds.IsClustered && !ds.DoingWork));
        }

        /// <summary>
        /// Gets a command that performs matching operation on the selected datasets.
        /// </summary>
        public RelayCommand PerformMatchingCommand { get; }

        /// <summary>
        /// Gets the singleton instance for AMT tag database selection.
        /// </summary>
        public DatabaseSelectionViewModel DatabaseSelectionViewModel { get; }

        /// <summary>
        /// Gets a collection of all possible peak matching types.
        /// </summary>
        public ObservableCollection<PeakMatchingType> PeakMatchingTypes { get; }

        /// <summary>
        /// Gets or sets the selected peak matching type.
        /// </summary>
        public PeakMatchingType SelectedPeakMatchingType
        {
            get => this.analysis.Options.StacOptions.IdentificationAlgorithm;
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
        /// Gets or sets a value indicating whether the matcher should calculate the STAC score.
        /// </summary>
        public bool ShouldCalculateStac
        {
            get => this.analysis.Options.StacOptions.ShouldCalculateSTAC;
            set
            {
                if (this.analysis.Options.StacOptions.ShouldCalculateSTAC != value)
                {
                    this.analysis.Options.StacOptions.ShouldCalculateSTAC = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether prior probabilities for mass tags should be used.
        /// </summary>
        public bool UsePriors
        {
            get => this.analysis.Options.StacOptions.UsePriors;
            set
            {
                if (this.analysis.Options.StacOptions.UsePriors)
                {
                    this.analysis.Options.StacOptions.UsePriors = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the matcher should calculate the SLiC score.
        /// </summary>
        // ReSharper disable once IdentifierTypo
        public bool ShouldCalculateSlic
        {
            get => this.analysis.Options.StacOptions.ShouldCalculateSLiC;
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
            get => this.analysis.Options.StacOptions.UseEllipsoid;
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
        /// Gets or sets the bin width for the histograms.
        /// </summary>
        public double HistogramBinWidth
        {
            get => this.analysis.Options.StacOptions.HistogramBinWidth;
            set
            {
                if (Math.Abs(this.analysis.Options.StacOptions.HistogramBinWidth - value) > float.Epsilon)
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
            get => this.analysis.Options.StacOptions.HistogramMultiplier;
            set
            {
                if (Math.Abs(this.analysis.Options.StacOptions.HistogramMultiplier - value) > float.Epsilon)
                {
                    this.analysis.Options.StacOptions.HistogramMultiplier = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the histogram FDR should be calculated.
        /// </summary>
        public bool CalculateHistogramFdr
        {
            get => this.analysis.Options.StacOptions.ShouldCalculateHistogramFDR;
            set
            {
                if (this.analysis.Options.StacOptions.ShouldCalculateHistogramFDR != value)
                {
                    this.analysis.Options.StacOptions.ShouldCalculateHistogramFDR = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the total progress bar should be displayed.
        /// </summary>
        public bool ShouldShowTotalProgress
        {
            get => this.shouldShowTotalProgress;
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
            get => this.totalProgress;
            set
            {
                if (Math.Abs(this.totalProgress - value) > float.Epsilon)
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
            var datasetsToUse = datasets.ToList();

            this.ShouldShowTotalProgress = true;
            var progressData = new PRISM.ProgressData(new Progress<PRISM.ProgressData>(pd =>
            {
                // This is the progress percent after stepping by progressData
                this.TotalProgress = pd.Percent;
            })) {IsPartialRange = true, MaxPercentage = 95};
            var progress = new Progress<PRISM.ProgressData>(pd => progressData.Report(pd.Percent));

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
                    UsePriors = this.UsePriors,
                    ShouldCalculateSTAC = this.ShouldCalculateStac,
                    ShouldCalculateHistogramFDR = this.CalculateHistogramFdr,
                    ShouldCalculateShiftFDR = false,
                },
            };

            // Initialize the datasets
            foreach (var dataset in datasetsToUse)
            {
                dataset.DatasetState = DatasetInformationViewModel.DatasetStates.Matching;
            }

            // Get clusters and database
            var clusters = this.analysis.DataProviders.ClusterCache.FindAll();
            var clusterIdMap = clusters.ToDictionary(cluster => cluster.Id);
            var database = this.analysis.MassTagDatabase;

            // Run STAC
            var matches = stac.PerformPeakMatching(clusters, database);

            foreach (var dataset in datasetsToUse)
            {
                dataset.DatasetState = DatasetInformationViewModel.DatasetStates.PersistingMatches;
            }

            // Persist matches
            var clusterToMassTags = matches.Select(match => new ClusterToMassTagMap(match.Observed.Id, match.Target.Id)).ToList();
            this.analysis.DataProviders.MassTagMatches.ClearAllMatches();
            this.analysis.DataProviders.MassTagMatches.AddAllStateless(clusterToMassTags, progress);

            try
            {
                // Write to file
                this.WriteClusterData("crosstab.tsv", matches, clusterIdMap, progress);
            }
            catch (Exception ex)
            {
                var errMsg = "Error writing results to text file: " + ex.Message;
                Logger.PrintMessage(errMsg);

                // Todo: Add this: if (!GlobalSettings.AutomatedAnalysisMode)
                    MessageBox.Show(errMsg);
            }

            this.ShouldShowTotalProgress = false;

            foreach (var dataset in datasetsToUse)
            {
                dataset.DatasetState = DatasetInformationViewModel.DatasetStates.Matched;
            }
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
            IReadOnlyCollection<FeatureMatchLight<UMCClusterLight, MassTagLight>> matches,
            IReadOnlyDictionary<int, UMCClusterLight> clusterIdMap,
            IProgress<PRISM.ProgressData> progress)
        {
            var progressData = new PRISM.ProgressData(progress);
            var i = 1;

            using (var writer = File.CreateText(path))
            {
                writer.Write(
                    "Cluster Id\tDataset Member Count\tTotal Member Count\tCluster Mass\tCluster NET\tAbundance\tMass Tag Id\tProtein\tPeptide Sequence\tMod count\tMod description\tMass Tag Mono Mass\tMass Tag NET\tPMT Quality Score\tMS-GF+ Spec Prob\tSTAC\t"); ////STAC-UP\t");

                var datasetHash = new HashSet<int>();

                // reconstruct clusters
                foreach (var match in matches)
                {
                    var cluster = match.Observed;
                    cluster.ReconstructUMCCluster(this.analysis.DataProviders, true, false, false, false);
                    foreach (var umc in cluster.UmcList)
                    {
                        if (!datasetHash.Contains(umc.GroupId))
                        {
                            datasetHash.Add(umc.GroupId);
                        }
                    }

                    progressData.Report(i++, matches.Count);
                }

                // Write dataset headers
                foreach (var dataset in datasetHash)
                {
                    var name = this.analysis.MetaData.Datasets.Single(x => x.DatasetId == dataset).Name;
                    writer.Write("{0} Abundance\t", name);
                }

                writer.WriteLine();

                foreach (var match in matches)
                {
                    var proteins = this.analysis.MassTagDatabase.Proteins[match.Target.Id];
                    var proteinStr = string.Empty;
                    foreach (var protein in proteins)
                    {
                        proteinStr += protein.Name;
                        if (protein != proteins.Last())
                        {
                            proteinStr += ",";
                        }
                    }

                    // Write cluster information
                    var cluster = clusterIdMap[match.Observed.Id];
                    writer.Write(
                        "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t",
                        cluster.Id,
                        cluster.DatasetMemberCount,
                        cluster.MemberCount,
                        cluster.MassMonoisotopic,
                        cluster.Net,
                        cluster.Abundance,
                        match.Target.Id,
                        proteinStr,
                        match.Target.PeptideSequence,
                        match.Target.ModificationCount,
                        match.Target.Modifications,
                        match.Target.MassMonoisotopic,
                        match.Target.Net,
                        match.Target.QualityScore,
                        match.Target.MsgfSpecProbMax,
                        match.Confidence);
                    ////match.Uniqueness);

                    foreach (var dataset in datasetHash)
                    {
                        var datasetAbundance =
                            match.Observed.UmcList.Where(umc => umc.GroupId == dataset).Sum(umc => umc.AbundanceSum);
                        writer.Write("{0}\t", datasetAbundance);
                    }

                    writer.WriteLine();
                }
            }
        }
    }
}
