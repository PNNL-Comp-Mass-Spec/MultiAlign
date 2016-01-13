using System.Windows;
using MultiAlignCore.IO;

namespace MultiAlignRogue.AMTMatching
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Media.Animation;

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
    using MultiAlignCore.Extensions;

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
        /// Gets or sets a value indicating whether the matcher should calculate the STAC score.
        /// </summary>
        public bool ShouldCalculateStac
        {
            get { return this.analysis.Options.StacOptions.ShouldCalculateSTAC; }
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
            get { return this.analysis.Options.StacOptions.UsePriors; }
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
        /// Gets or sets the bin width for the histograms.
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
        /// Gets or sets a value indicating whether the histogram FDR should be calculated.
        /// </summary>
        public bool CalculateHistogramFdr
        {
            get { return this.analysis.Options.StacOptions.ShouldCalculateHistogramFDR; }
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
                    UsePriors = this.UsePriors,
                    ShouldCalculateSTAC = this.ShouldCalculateStac,
                    ShouldCalculateHistogramFDR = this.CalculateHistogramFdr,
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

                    progData.Report(i++, matches.Count);
                }

                // Write dataset headers
                foreach (var dataset in datasetHash)
                {
                    writer.Write("Dataset {0} Abundance\t", dataset);
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
