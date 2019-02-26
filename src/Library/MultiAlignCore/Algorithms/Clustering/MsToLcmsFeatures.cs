using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Algorithms.Chromatograms;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.RawData;
using PNNLOmics.Data.Constants.Libraries;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Class with methods to convert a set of MS features to LCMS features.
    /// </summary>
    public class MsToLcmsFeatures
    {
        /// <summary>
        /// Spectra provider for extracting XICs and MS/MS spectra data.
        /// </summary>
        private readonly IScanSummaryProvider provider;

        /// <summary>
        /// Clusterer for first pass clustering.
        /// </summary>
        private readonly IClusterer<MSFeatureLight, UMCLight> firstPassClusterer;

        /// <summary>
        /// Clusterer for second pass clustering.
        /// </summary>
        private readonly IClusterer<UMCLight, UMCLight> secondPassClusterer;

        /// <summary>
        /// Options for clustering features (NET tolerance, Mass tolerance, M/Z tolerance).
        /// </summary>
        private readonly LcmsFeatureFindingOptions options;

        public MsToLcmsFeatures(IScanSummaryProvider provider, LcmsFeatureFindingOptions options = null)
        {
            if (provider == null)
            {
                throw new ArgumentNullException();
            }

            Comparison<MSFeatureLight> mzSort = (x, y) => x.Mz.CompareTo(y.Mz);
            Comparison<UMCLight> monoSort = (x, y) => x.MassMonoisotopic.CompareTo(y.MassMonoisotopic);
            Func<MSFeatureLight, MSFeatureLight, double> mzDiff = (x, y) => FeatureLight.ComputeMassPPMDifference(x.Mz, y.Mz);
            Func<UMCLight, UMCLight, double> monoDiff = (x, y) => FeatureLight.ComputeMassPPMDifference(x.MassMonoisotopic, y.MassMonoisotopic);

            this.provider = provider;
            this.options = options ?? new LcmsFeatureFindingOptions();

            // Set clusterers
            if (this.options.FirstPassClusterer == MsFeatureClusteringAlgorithmType.BinarySearchTree)
            {
                this.firstPassClusterer = new MsFeatureTreeClusterer<MSFeatureLight, UMCLight>(
                    mzSort,
                    mzDiff,
                    MassComparison.Mz,
                    this.options.InstrumentTolerances.Mass);
            }
            else
            {
                this.firstPassClusterer = ClusterFactory.Create(this.options.FirstPassClusterer);
            }

            if (this.options.SecondPassClusterer == GenericClusteringAlgorithmType.BinarySearchTree)
            {
                this.secondPassClusterer = new MsFeatureTreeClusterer<UMCLight, UMCLight>(
                                                            monoSort,
                                                            monoDiff,
                                                            MassComparison.Monoisotopic,
                                                            this.options.InstrumentTolerances.Mass);
            }
            else
            {
                var clusterFactory = new GenericClusterFactory<UMCLight, UMCLight>();
                this.secondPassClusterer = clusterFactory.Create(this.options.SecondPassClusterer);
            }
        }

        /// <summary>
        /// This generates LCMS-Features from a list of seed MSFeatures (typically from _isos file).
        /// </summary>
        /// <param name="msFeatures">The seed <see cref="MSFeatureLight" />.</param>
        /// <param name="progress"></param>
        /// <returns>LCMS-Features as a list of <see cref="UMCLight" />.</returns>
        public List<UMCLight> Convert(List<MSFeatureLight> msFeatures, IProgress<PRISM.ProgressData> progress = null)
        {
            // This method converts MS-Features (MSFeatureLight) into LCMS-Features (UMCLight) in 3 steps:
            //      1. First pass clustering: Cluster MSFeatureLights (typically corresponding to
            //         lines in an _isos file) features by M/Z into UMCLights.
            //      2. Create XICs for clusters. (Each XIC point becomes a new MSFeature,
            //         replacing the previous MSFeatures).
            //      3. Second pass clustering: Cluster the UMCLights with XICs into UMCLights.
            // The only mandatory step is the first one.

            this.SetNets(msFeatures);

            // Set up progress reporter
            var progressData = new PRISM.ProgressData(progress);
            var internalProgress = new Progress<PRISM.ProgressData>(pd =>  progressData.Report(pd.Percent));

            var features = new List<UMCLight>();
            using (var logger = new StreamWriter("msfeatureClusteringStats.txt", true))
            {
                logger.WriteLine();
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                // Step 1
                progressData.StepRange(5);
                progressData.Status = "First Pass Clustering";
                features = this.FirstPassClustering(msFeatures, internalProgress);
                stopWatch.Stop();
                logger.WriteLine("{0}: {1}s", progressData.Status, stopWatch.Elapsed.TotalSeconds);

                // Step 2
                if (this.options.FindXics && this.provider is InformedProteomicsReader)
                {
                    stopWatch.Restart();
                    progressData.Status = "Creating Xics";
                    progressData.StepRange(90);
                    features = this.CreateXics(features, internalProgress);
                    features.ForEach(feature => feature.CalculateStatistics());
                    stopWatch.Stop();
                    logger.WriteLine("{0}: {1}s", progressData.Status, stopWatch.Elapsed.TotalSeconds);
                }

                // Step 3
                if (this.options.SecondPassClustering)
                {
                    stopWatch.Restart();
                    progressData.Status = "Second Pass Clustering";
                    progressData.StepRange(100);
                    features = this.SecondPassClustering(features, internalProgress);
                    stopWatch.Stop();
                    logger.WriteLine("{0}: {1}s", progressData.Status, stopWatch.Elapsed.TotalSeconds);
                }
            }

            return features;
        }

        /// <summary>
        /// Assigns the NET for the raw MS features.
        /// </summary>
        /// <param name="rawFeatures">The raw features to set nets for.</param>
        private void SetNets(List<MSFeatureLight> rawFeatures)
        {
            var max = this.provider.GetScanSummary(this.provider.MaxScan).Time;
            var min = this.provider.GetScanSummary(this.provider.MinScan).Time;
            var diff = max - min;
            rawFeatures.ForEach(feat => feat.Net = (this.provider.GetScanSummary(feat.Scan).Time - min) / diff);
        }

        /// <summary>
        /// First pass clustering: Cluster MSFeatureLight (typically corresponding to
        ///  lines in an _isos file) features by M/Z into UMCLights.
        /// </summary>
        /// <param name="msFeatures"></param>
        /// <param name="progress">The progress reporter.</param>
        /// <returns>Clustered MSFeatures as <see cref="UMCLight" />.</returns>
        private List<UMCLight> FirstPassClustering(List<MSFeatureLight> msFeatures, IProgress<PRISM.ProgressData> progress)
        {
            var featureList = new List<UMCLight>();
            var features =  firstPassClusterer.Cluster(msFeatures, progress);
            foreach (var feature in features)
            {
                if (feature.MsFeatures.Count == 0)
                {
                    continue;
                }

                ////feature.CalculateStatistics(ClusterCentroidRepresentation.Median);
                feature.MassMonoisotopic = (feature.Mz * feature.ChargeState) - (SubAtomicParticleLibrary.MASS_PROTON * feature.ChargeState);
                featureList.Add(feature);
            }

            return featureList;
        }

        /// <summary>
        /// Create XICs for clusters. (Each XIC point becomes a new MSFeature,
        //  replacing the previous MSFeatures).
        /// </summary>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="umcLights"></param>
        /// <returns></returns>
        private List<UMCLight> CreateXics(List<UMCLight> umcLights, IProgress<PRISM.ProgressData> progress)
        {
            var xicCreator = new XicCreator { XicRefiner = new XicRefiner(this.options.XicRelativeIntensityThreshold) };
            return xicCreator.CreateXicNew(
                            umcLights,
                            this.options.InstrumentTolerances.Mass,
                            provider as InformedProteomicsReader,
                            this.options.RefineXics,
                            progress).ToList();
        }

        /// <summary>
        /// Second pass clustering: Cluster the UMCLights with XICs.
        /// </summary>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="umcLights"></param>
        /// <returns></returns>
        private List<UMCLight> SecondPassClustering(List<UMCLight> umcLights, IProgress<PRISM.ProgressData> progress)
        {
            return secondPassClusterer.Cluster(umcLights, progress).ToList();
        }
    }
}
