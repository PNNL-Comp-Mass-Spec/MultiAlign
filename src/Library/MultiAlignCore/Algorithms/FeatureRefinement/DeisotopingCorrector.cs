namespace MultiAlignCore.Algorithms.FeatureRefinement
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Data.Biology;
    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Algorithms.Distance;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;

    /// <summary>
    /// This class runs clustering that attempts to cluster together features
    /// that are isotopes of each other.
    /// </summary>
    /// <remarks>
    /// Clusters two features together if they are within a small tolerance
    /// of +/- N isotopes of each other.
    /// </remarks>
    public class DeisotopingCorrector : ISettingsContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeisotopingCorrector" /> class. 
        /// </summary>
        public DeisotopingCorrector()
        {
            this.RestoreDefaults();
        }

        /// <summary>
        /// Gets or sets the type of clustering algorithm to use.
        /// </summary>
        public ClusteringAlgorithmTypes ClusteringAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the mass, NET, and drift time tolerances.
        /// </summary>
        public FeatureTolerances Tolerances { get; set; }

        /// <summary>
        /// Gets or sets the number of isotopes to compare features by.
        /// </summary>
        /// <remarks>The higher this number, the slower it is.</remarks>
        public int NumberOfIsotopes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether potential isotope peaks
        /// should be combined even though they are different charge states.
        /// </summary>
        public bool ShouldSeparateChargeStates { get; set; }

        /// <summary>
        /// Run Deisotoping clustering on the features.
        /// </summary>
        /// <param name="features">The features to deisotope.</param>
        /// <param name="progress">Progress reporter for desitoping process.</param>
        /// <returns>The deisotoped features.</returns>
        public List<UMCLight> Run(List<UMCLight> features, IProgress<ProgressData> progress = null)
        {
            var clusterer = ClusterFactory.CreateSecondPassFeatureClusterer(this.ClusteringAlgorithm);
            clusterer.Parameters = new FeatureClusterParameters<UMCLight>
            {
                CentroidRepresentation = ClusterCentroidRepresentation.Apex,
                OnlyClusterSameChargeStates = !this.ShouldSeparateChargeStates,
                Tolerances = this.Tolerances,
                RangeFunction = this.IsotopesWithinRange,
                DistanceFunction = DistanceFactory<UMCLight>.CreateDistanceFunction(DistanceMetric.Euclidean)
            };

            progress = progress ?? new Progress<ProgressData>();

            return clusterer.Cluster(features, progress);
        }

        /// <summary>
        /// Restore defaults back to their original settings.
        /// </summary>
        public void RestoreDefaults()
        {
            this.ClusteringAlgorithm = ClusteringAlgorithmTypes.SingleLinkage;

            // Use very narrow tolerance by default
            this.Tolerances = new FeatureTolerances
            {
                Mass = 2,
                Net = 0.01,
                DriftTime = 3
            };

            this.NumberOfIsotopes = 2;
            this.ShouldSeparateChargeStates = true;
        }

        /// <summary>
        /// Checks to see if feature x is within range of being an isotope of feature y.
        /// </summary>
        /// <param name="x">The first feature.</param>
        /// <param name="y">The second feature.</param>
        /// <returns>A value indicating whether feature x is within an isotope distance of feature y.</returns>
        private bool IsotopesWithinRange(UMCLight x, UMCLight y)
        {
            // later is more related to determining a scalar value instead.
            var netDiff = Math.Abs(x.Net - y.Net);
            var driftDiff = Math.Abs(x.DriftTime - y.DriftTime);

            for (int i = -1*this.NumberOfIsotopes; i <= this.NumberOfIsotopes; i++)
            {
                if (i == 0)
                {   // Skipping offset 0 because that should be taken care of  by normal clustering
                    continue;
                }

                var mass = x.MassMonoisotopicAligned + i * Constants.C13MinusC12;
                var massDiff = Math.Abs(FeatureLight.ComputeMassPPMDifference(mass, y.MassMonoisotopicAligned));
                if (massDiff <= this.Tolerances.Mass && netDiff <= this.Tolerances.Net && driftDiff <= this.Tolerances.DriftTime)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
