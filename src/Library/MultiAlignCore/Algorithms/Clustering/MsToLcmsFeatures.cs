using System;
using System.Collections.Generic;
using System.Linq;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Algorithms.Chromatograms;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.RawData;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Class with methods to convert a set of MS features to LCMS features.
    /// </summary>
    public class MsToLcmsFeatures
    {
        /// <summary>
        /// Clusterer for first pass clustering.
        /// </summary>
        private readonly IClusterer<MSFeatureLight, UMCLight> firstPassClusterer;

        /// <summary>
        /// Clusterer for second pass clustering.
        /// </summary>
        private readonly IClusterer<UMCLight, UMCLight> secondPassClusterer; 

        /// <summary>
        /// Options for filtering UMCLights.
        /// </summary>
        private LcmsFeatureFilteringOptions options;

        /// <summary>
        /// Options for clustering features (NET tolerance, Mass tolerance, M/Z tolerance).
        /// </summary>
        private FeatureTolerances tolerances;

        /// <summary>
        /// Spectra provider for extracting XICs and MS/MS spectra data.
        /// </summary>
        private InformedProteomicsReader provider;
        
        public MsToLcmsFeatures(InformedProteomicsReader provider, LcmsFeatureFilteringOptions options = null, FeatureTolerances tolerances = null)
        {
            Comparison<MSFeatureLight> mzSort = (x, y) => x.Mz.CompareTo(y.Mz);
            Comparison<UMCLight> monoSort = (x, y) => x.MassMonoisotopic.CompareTo(y.MassMonoisotopic);
            Func<MSFeatureLight, MSFeatureLight, double> mzDiff = (x, y) => FeatureLight.ComputeMassPPMDifference(x.Mz, y.Mz);
            Func<UMCLight, UMCLight, double> monoDiff = (x, y) => FeatureLight.ComputeMassPPMDifference(x.MassMonoisotopic, y.MassMonoisotopic);

            this.provider = provider;
            this.options = options ?? new LcmsFeatureFilteringOptions();
            this.tolerances = tolerances ?? new FeatureTolerances();
            this.firstPassClusterer = new MsFeatureTreeClusterer<MSFeatureLight, UMCLight>(mzSort, mzDiff, MassComparison.Mz, tolerances.Mass);
            this.secondPassClusterer = new MsFeatureTreeClusterer<UMCLight, UMCLight>(monoSort, monoDiff, MassComparison.Monoisotopic, tolerances.Mass);
        }

        /// <summary>
        /// This generates LCMS-Features from a list of seed MSFeatures (typically from _isos file).
        /// </summary>
        /// <param name="msFeatures">The seed <see cref="MSFeatureLight" />.</param>
        /// <returns>LCMS-Features as a list of <see cref="UMCLight" />.</returns>
        public List<UMCLight> Convert(List<MSFeatureLight> msFeatures, IProgress<ProgressData> progress = null)
        {
            // This method converts MS-Features (MSFeatureLight) into LCMS-Features (UMCLight) in 3 steps:
            //      1. First pass clustering: Cluster MSFeatureLights (typically corresponding to
            //         lines in an _isos file) features by M/Z into UMCLights.
            //      2. Create XICs for clusters. (Each XIC point becomes a new MSFeature,
            //         replacing the previous MSFeatures).
            //      3. Second pass clustering: Cluster the UMCLights with XICs into UMCLights.
            // The only mandatory step is the first one.

            // Set up progress reporter
            progress = progress ?? new Progress<ProgressData>();
            var progressData = new ProgressData { IsPartialRange = true };
            var internalProgress = new Progress<ProgressData>(pd => progress.Report(progressData.UpdatePercent(pd.Percent)));

            // Step 1
            progressData.MaxPercentage = 5;
            progressData.Status = "First Pass Clustering";
            var features = this.FirstPassClustering(msFeatures, internalProgress);

            // Step 2
            progressData.Status = "Creating Xics";
            progressData.StepRange(90);
            features = this.CreateXics(features, internalProgress);
            features.ForEach(feature => feature.CalculateStatistics(ClusterCentroidRepresentation.Median));

            // Step 3
            progressData.Status = "Second Pass Clustering";
            progressData.StepRange(100);
            features = this.SecondPassClustering(features, internalProgress);

            return features;
        }

        /// <summary>
        /// First pass clustering: Cluster MSFeatureLight (typically corresponding to
        //  lines in an _isos file) features by M/Z into UMCLights.
        /// </summary>
        /// <param name="msFeatures"></param>
        /// <param name="progress">The progress reporter.</param>
        /// <returns>Clustered MSFeatures as <see cref="UMCLight" />.</returns>
        private List<UMCLight> FirstPassClustering(List<MSFeatureLight> msFeatures, IProgress<ProgressData> progress)
        {
            return firstPassClusterer.Cluster(msFeatures).ToList();
        }

        /// <summary>
        /// Create XICs for clusters. (Each XIC point becomes a new MSFeature,
        //  replacing the previous MSFeatures).
        /// </summary>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="umcLights"></param>
        /// <returns></returns>
        private List<UMCLight> CreateXics(List<UMCLight> umcLights, IProgress<ProgressData> progress)
        {
            var xicCreator = new XicCreator();
            return xicCreator.CreateXicNew(umcLights, tolerances.Mass, provider).ToList();
        }

        /// <summary>
        /// Second pass clustering: Cluster the UMCLights with XICs.
        /// </summary>
        /// <param name="progress">The progress reporter.</param>
        /// <param name="umcLights"></param>
        /// <returns></returns>
        private List<UMCLight> SecondPassClustering(List<UMCLight> umcLights, IProgress<ProgressData> progress)
        {
            return secondPassClusterer.Cluster(umcLights).ToList();
        }
    }
}
