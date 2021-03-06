﻿using System.Collections.ObjectModel;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using GalaSoft.MvvmLight.Messaging;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Distance;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Hibernate;
using MultiAlignRogue.Utils;
using MultiAlignRogue.ViewModels;

namespace MultiAlignRogue.Clustering
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing;
    using MultiAlignCore.IO.Features;

    public class ClusterSettingsViewModel : ViewModelBase
    {
        private readonly MultiAlignAnalysis analysis;

        private readonly MultiAlignAnalysisOptions options;

        private IUmcDAO featureCache;

        private readonly IClusterViewFactory clusterViewFactory;

        private readonly IProgress<int> progress;

        private readonly AlgorithmBuilder builder;
        private AlgorithmProvider algorithms;

        public RelayCommand ClusterFeaturesCommand { get; }
        public RelayCommand DisplayClustersCommand { get; }

        public ObservableCollection<DistanceMetric> DistanceMetrics { get; }
        public ObservableCollection<ClusterCentroidRepresentation> CentroidRepresentations { get; }
        public ObservableCollection<LcmsFeatureClusteringAlgorithmType> ClusteringMethods { get; }

        public ClusterSettingsViewModel(
                                        MultiAlignAnalysis analysis,
                                        ObservableCollection<DatasetInformationViewModel> datasets,
                                        IClusterViewFactory clusterViewFactory = null,
                                        IProgress<int> progressReporter = null)
        {
            this.progress = progressReporter ?? new Progress<int>();
            this.analysis = analysis;
            this.Datasets = datasets;
            this.options = analysis.Options;
            this.builder = new AlgorithmBuilder();
            this.clusterViewFactory = clusterViewFactory ?? new ClusterViewFactory(analysis.DataProviders);

            // When dataset state changes, update can executes.
            this.MessengerInstance.Register<PropertyChangedMessage<DatasetInformationViewModel.DatasetStates>>(this, args =>
            {
                if (args.Sender is DatasetInformationViewModel && args.PropertyName == "DatasetState")
                {
                    ThreadSafeDispatcher.Invoke(() => this.ClusterFeaturesCommand.RaiseCanExecuteChanged());
                    ThreadSafeDispatcher.Invoke(() => this.DisplayClustersCommand.RaiseCanExecuteChanged());
                }
            });

            this.ClusterFeaturesCommand = new RelayCommand(this.AsyncClusterFeatures, () => this.Datasets.Any(ds => ds.FeaturesFound));
            this.DisplayClustersCommand = new RelayCommand(
                                                           async () => await this.DisplayFeatures(),
                                                           () => this.Datasets.Any(ds => ds.IsClustered));

            this.DistanceMetrics = new ObservableCollection<DistanceMetric>();
            Enum.GetValues(typeof(DistanceMetric)).Cast<DistanceMetric>().ToList().ForEach(x => this.DistanceMetrics.Add(x));
            this.CentroidRepresentations = new ObservableCollection<ClusterCentroidRepresentation>
            {
                ClusterCentroidRepresentation.Mean,
                ClusterCentroidRepresentation.Median
            };

            this.ClusteringMethods = new ObservableCollection<LcmsFeatureClusteringAlgorithmType>();
            Enum.GetValues(typeof(LcmsFeatureClusteringAlgorithmType)).Cast<LcmsFeatureClusteringAlgorithmType>().ToList().ForEach(x => this.ClusteringMethods.Add(x));

            this.PostProcessingComparisonType = new ObservableCollection<ClusterPostProcessingOptions.ClusterComparisonType>(
                Enum.GetValues(typeof(ClusterPostProcessingOptions.ClusterComparisonType)).Cast<ClusterPostProcessingOptions.ClusterComparisonType>());
        }

        public bool ShouldSeparateByCharge
        {
            get => this.options.LcmsClusteringOptions.ShouldSeparateCharge;
            set
            {
                this.options.LcmsClusteringOptions.ShouldSeparateCharge = value;
                this.RaisePropertyChanged(nameof(ShouldSeparateByCharge));
            }
        }

        public LcmsFeatureClusteringAlgorithmType SelectedLcmsFeatureClusteringAlgorithm
        {
            get => this.options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm;
            set
            {
                if (this.options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm != value)
                {
                    this.options.LcmsClusteringOptions.LcmsFeatureClusteringAlgorithm = value;
                    this.RaisePropertyChanged(nameof(SelectedLcmsFeatureClusteringAlgorithm));
                    this.RaisePropertyChanged(nameof(ShouldEnableClustererSettings));
                }
            }
        }

        public ClusterCentroidRepresentation SelectedCentroidMethod
        {
            get => this.options.LcmsClusteringOptions.ClusterCentroidRepresentation;
            set
            {
                if (this.options.LcmsClusteringOptions.ClusterCentroidRepresentation != value)
                {
                    this.options.LcmsClusteringOptions.ClusterCentroidRepresentation = value;
                    this.RaisePropertyChanged(nameof(SelectedCentroidMethod));
                }
            }
        }

        public DistanceMetric SelectedDistanceFunction
        {
            get => this.options.LcmsClusteringOptions.DistanceFunction;
            set
            {
                if (this.options.LcmsClusteringOptions.DistanceFunction != value)
                {
                    this.options.LcmsClusteringOptions.DistanceFunction = value;
                    this.RaisePropertyChanged(nameof(SelectedDistanceFunction));
                }
            }
        }

        public bool ShouldRefineWithMsMs
        {
            get => this.options.ClusterPostProcessingoptions.ShouldPerformClusterRefinement;
            set
            {
                if (this.options.ClusterPostProcessingoptions.ShouldPerformClusterRefinement != value)
                {
                    this.options.ClusterPostProcessingoptions.ShouldPerformClusterRefinement = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<ClusterPostProcessingOptions.ClusterComparisonType> PostProcessingComparisonType { get; }

        public ClusterPostProcessingOptions.ClusterComparisonType ClusterComparisonType
        {
            get => this.options.ClusterPostProcessingoptions.ComparisonType;
            set
            {
                if (this.options.ClusterPostProcessingoptions.ComparisonType != value)
                {
                    this.options.ClusterPostProcessingoptions.ComparisonType = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double ClusterPostProcessingTolerance
        {
            get => this.options.ClusterPostProcessingoptions.MsMsComparisonTolerance;
            set
            {
                if (Math.Abs(this.options.ClusterPostProcessingoptions.MsMsComparisonTolerance - value) > float.Epsilon)
                {
                    this.options.ClusterPostProcessingoptions.MsMsComparisonTolerance = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool shouldShowProgress;

        public bool ShouldShowProgress
        {
            get => this.shouldShowProgress;
            set
            {
                if (this.shouldShowProgress != value)
                {
                    this.shouldShowProgress = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private double progressPercent;

        public double ProgressPercent
        {
            get => this.progressPercent;
            set
            {
                if (Math.Abs(this.progressPercent - value) > float.Epsilon)
                {
                    this.progressPercent = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool ShouldEnableClustererSettings => this.SelectedLcmsFeatureClusteringAlgorithm != LcmsFeatureClusteringAlgorithmType.Promex;

        public async void AsyncClusterFeatures()
        {
            await Task.Run(() => this.ClusterFeatures());
        }

        public ObservableCollection<DatasetInformationViewModel> Datasets { get; }

        internal void ClusterFeatures(IProgress<PRISM.ProgressData> workflowProgress = null)
        {
            var taskBarProgress = TaskBarProgress.GetInstance();
            taskBarProgress.ShowProgress(this, true);
            workflowProgress = workflowProgress ?? new Progress<PRISM.ProgressData>();
            IProgress<PRISM.ProgressData> internalProgress = new Progress<PRISM.ProgressData>(pd =>
            {
                this.progress.Report((int)pd.Percent);
                this.ProgressPercent = pd.Percent;
                taskBarProgress.SetProgress(this, pd.Percent);
                workflowProgress.Report(pd);
            });

            this.algorithms = this.builder.GetAlgorithmProvider(this.options);
            var clusterer = this.algorithms.Clusterer;
            clusterer.Parameters = LcmsClusteringOptions.ConvertToOmics(this.options.LcmsClusteringOptions);
            this.featureCache = this.analysis.DataProviders.FeatureCache;
            if (clusterer is PromexClusterer promexClusterer)
            {
                promexClusterer.Readers = this.analysis.DataProviders.ScanSummaryProviderCache;
            }

            foreach (var dataset in this.Datasets)
            {
                if (dataset.FeaturesFound)
                {
                    dataset.DatasetState = DatasetInformationViewModel.DatasetStates.Clustering;
                }
            }

            ThreadSafeDispatcher.Invoke(this.ClusterFeaturesCommand.RaiseCanExecuteChanged);
            ThreadSafeDispatcher.Invoke(this.DisplayClustersCommand.RaiseCanExecuteChanged);

            this.ShouldShowProgress = true;
            var progressData = new PRISM.ProgressData(internalProgress);
            var clusterProgress = new Progress<PRISM.ProgressData>(pd => progressData.Report(pd.Percent));

            this.analysis.DataProviders.DatabaseLock.EnterWriteLock();
            DatabaseIndexer.IndexClustersDrop(NHibernateUtil.Path);
            this.analysis.DataProviders.ClusterCache.ClearAllClusters();
            this.analysis.DataProviders.DatabaseLock.ExitWriteLock();

            // The id for a cluster - keep track here to avoid duplicates when separating by charge.
            var clusterCount = 0;

            // Here we see if we need to separate the charge...
            // IMS is said to require charge separation
            if (!this.analysis.Options.LcmsClusteringOptions.ShouldSeparateCharge)
            {
                progressData.StepRange(45);
                var features = new List<UMCLight>();
                var i = 0;
                var datasets = this.Datasets.Where(ds => ds.FeaturesFound).ToList();
                foreach (var dataset in datasets)
                {
                    this.analysis.DataProviders.DatabaseLock.EnterReadLock();
                    features.AddRange(this.featureCache.FindByDatasetId(dataset.DatasetId));
                    this.analysis.DataProviders.DatabaseLock.ExitReadLock();
                    progressData.Report(++i, datasets.Count);
                }

                progressData.StepRange(100);
                ClusterGroupOfFeatures(clusterer, features, ref clusterCount, clusterProgress);
            }
            else
            {
                var maxChargeState = this.featureCache.FindMaxCharge();

                // Here we cluster all charge states separately.  Probably IMS Data.
                for (var chargeState = 1; chargeState <= maxChargeState; chargeState++)
                {
                    var maxPercent = ((100.0 * chargeState) / maxChargeState);
                    // TODO: Add restriction by selected dataset ids?
                    var features = this.featureCache.FindByCharge(chargeState);
                    if (features.Count < 1)
                    {
                        continue;
                    }

                    progressData.StepRange(maxPercent);
                    ClusterGroupOfFeatures(clusterer, features, ref clusterCount, clusterProgress);
                }

                this.analysis.Clusters = this.analysis.DataProviders.ClusterCache.FindAll();
            }

            this.analysis.DataProviders.DatabaseLock.EnterWriteLock();
            DatabaseIndexer.IndexClusters(NHibernateUtil.Path);
            this.analysis.DataProviders.DatabaseLock.ExitWriteLock();

            foreach (var dataset in this.Datasets)
            {
                if (dataset.DatasetState == DatasetInformationViewModel.DatasetStates.PersistingClusters)
                {
                    dataset.DatasetState = DatasetInformationViewModel.DatasetStates.Clustered;
                }
            }

            try
            {
                // Write to file
                this.WriteClusterData(string.Format("{0}_crosstab.tsv", this.analysis.AnalysisName), this.analysis.Clusters);
            }
            catch (Exception ex)
            {
                var errMsg = "Error writing results to text file: " + ex.Message;
                Logger.PrintMessage(errMsg);

                // Todo: Add this: if (!GlobalSettings.AutomatedAnalysisMode)
                MessageBox.Show(errMsg);
            }

            ThreadSafeDispatcher.Invoke(this.ClusterFeaturesCommand.RaiseCanExecuteChanged);
            ThreadSafeDispatcher.Invoke(this.DisplayClustersCommand.RaiseCanExecuteChanged);

            taskBarProgress.ShowProgress(this, false);
            this.ShouldShowProgress = false;
        }

        internal void ClusterGroupOfFeatures(IClusterer<UMCLight, UMCClusterLight> clusterer, List<UMCLight> features, ref int clusterCount, IProgress<PRISM.ProgressData> internalProgress = null)
        {
            var progressData = new PRISM.ProgressData(internalProgress);
            var clusterProgress = new Progress<PRISM.ProgressData>(pd => progressData.Report(pd.Percent));

            if (this.ShouldRefineWithMsMs)
            {
                progressData.StepRange(35);
            }
            else
            {
                progressData.StepRange(70);
            }

            var clusters = clusterer.Cluster(features, clusterProgress);

            foreach (var cluster in clusters)
            {
                cluster.Id = clusterCount++;
                cluster.UmcList.ForEach(x => x.ClusterId = cluster.Id);

                // Updates the cluster with statistics
                foreach (var feature in cluster.UmcList)
                {
                    cluster.MsMsCount += feature.MsMsCount;
                    cluster.IdentifiedSpectraCount += feature.IdentifiedSpectraCount;
                }
            }

            if (this.ShouldRefineWithMsMs)
            {
                try
                {
                    progressData.StepRange(70);
                    var clusterRefiner =
                        ClusterPostProcessorBuilder.GetClusterPostProcessor<UMCClusterLight, UMCLight>(
                            this.analysis.Options.ClusterPostProcessingoptions,
                            this.analysis.DataProviders);
                    clusters = clusterRefiner.Cluster(clusters, clusterProgress);

                }
                catch (DatasetInformation.MissingRawDataException e)
                {
                    MessageBox.Show(string.Format("{0}\nDataset: {1}", e.Message, e.GroupId));
                }
            }

            this.analysis.Clusters = clusters;
            clusters.ForEach(c => c.Abundance = c.UmcList.Sum(umc => umc.AbundanceSum));

            foreach (var dataset in this.Datasets)
            {
                if (dataset.DatasetState == DatasetInformationViewModel.DatasetStates.Clustering)
                {
                    dataset.DatasetState = DatasetInformationViewModel.DatasetStates.PersistingClusters;
                }
            }

            ThreadSafeDispatcher.Invoke(this.ClusterFeaturesCommand.RaiseCanExecuteChanged);
            ThreadSafeDispatcher.Invoke(this.DisplayClustersCommand.RaiseCanExecuteChanged);

            this.analysis.DataProviders.DatabaseLock.EnterWriteLock();
            progressData.StepRange(85);
            this.analysis.DataProviders.ClusterCache.AddAllStateless(clusters, clusterProgress);

            progressData.StepRange(100);
            this.analysis.DataProviders.FeatureCache.UpdateAll(features, clusterProgress);
            this.analysis.DataProviders.DatabaseLock.ExitWriteLock();
        }

        public async Task DisplayFeatures()
        {
            // get clusters
            var clusters = this.analysis.Clusters ??
                           await Task.Run(() => this.analysis.DataProviders.ClusterCache.FindAll());

            // reconstruct matches
            var matches = await Task.Run(() => this.ReconstructClusterMatches(clusters));

            this.clusterViewFactory.CreateNewWindow(matches, this.analysis.DataProviders.ScanSummaryProviderCache);
        }

        private List<ClusterMatch> ReconstructClusterMatches(IReadOnlyCollection<UMCClusterLight> clusters)
        {
            var matches = new List<ClusterMatch>();
            var clusterMap = clusters.ToDictionary(cluster => cluster.Id);
            var massTagMap = this.analysis.DataProviders.MassTags.FindAll().ToDictionary(massTag => massTag.Id);
            var massTagMatches = this.analysis.DataProviders.MassTagMatches.FindAll();

            // Add cluster-mass tag matches
            foreach (var match in massTagMatches)
            {
                var massTag = massTagMap[match.MassTagId];
                var cluster = clusterMap[match.ClusterId];
                matches.Add(new ClusterMatch
                {
                    Cluster = cluster,
                    MassTag = massTag,
                });
            }

            // Add clusters without mass tag matches
            var usedClusters = new HashSet<int>();
            usedClusters.UnionWith(matches.Select(match => match.Cluster.Id));
            foreach (var cluster in clusters)
            {
                if (!usedClusters.Contains(cluster.Id))
                {
                    usedClusters.Add(cluster.Id);
                    matches.Add(new ClusterMatch
                    {
                        Cluster = cluster,
                        MassTag = new MassTagLight()
                    });
                }
            }

            return matches;
        }

        /// <summary>
        /// Writes cluster data to a comma-separated values file.
        /// </summary>
        /// <param name="path">The path to the CSV file to write.</param>
        /// <param name="clusters">The cluster-mass tag matches to write.</param>
        private void WriteClusterData(string path, IReadOnlyCollection<UMCClusterLight> clusters)
        {
            //var progressData = new PRISM.ProgressData { ProgressObj = progress };

            using (var writer = File.CreateText(path))
            {
                // Headers
                writer.Write("Cluster Id\tMass\tNET\t");

                // Find unique datasets.
                var datasetHash = new HashSet<int>();
                foreach (var cluster in clusters)
                {
                    foreach (var feature in cluster.Features)
                    {
                        if (!datasetHash.Contains(feature.GroupId))
                        {
                            datasetHash.Add(feature.GroupId);
                        }
                    }
                }

                var datasets = datasetHash.ToList();
                datasets.Sort();

                // Write dataset headers
                foreach (var dataset in datasets)
                {
                    var name = this.analysis.MetaData.Datasets.Single(x => x.DatasetId == dataset).Name;
                    writer.Write("{0}_Abundance\t", name);
                }

                writer.WriteLine();

                // Write dataset level abundances.
                foreach (var cluster in clusters)
                {
                    // Write cluster information
                    writer.Write(
                        "{0}\t{1}\t{2}\t",
                        cluster.Id,
                        cluster.MassMonoisotopic,
                        cluster.Net);

                    foreach (var dataset in datasets)
                    {
                        var datasetAbundance = cluster.UmcList.Where(umc => umc.GroupId == dataset).Sum(umc => umc.AbundanceSum);
                        writer.Write("{0}\t", datasetAbundance);
                    }

                    writer.WriteLine();
                }
            }
        }
    }
}
