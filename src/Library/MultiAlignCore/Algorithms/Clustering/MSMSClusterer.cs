using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Algorithms;
using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Algorithms.SpectralProcessing;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using PNNLOmics.Data.Constants.Libraries;

namespace MultiAlignCore.Algorithms.Clustering
{
    using MultiAlignCore.IO.RawData;

    /// <summary>
    /// Aligns multiple datasets based on MS/MS clustering methods.
    /// </summary>
    public class MSMSClusterer : IProgressNotifer
    {
        /// <summary>
        /// Fired when progress is made.
        /// </summary>
        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MSMSClusterer()
        {
            AdductMass = SubAtomicParticleLibrary.MASS_PROTON;
            ScanRange = 800;
            MinimumClusterSize = 2;
            MzTolerance = .5;
            MassTolerance = 6;

        }

        #region Properties
        /// <summary>
        /// Gets or sets the scan range.
        /// </summary>
        public int ScanRange
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the similarity tolerance to use.
        /// </summary>
        public double SimilarityTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the object used to compare two spectra for alignment matching.
        /// </summary>
        public ISpectralComparer SpectralComparer
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the minimum cluster size.
        /// </summary>
        public int MinimumClusterSize
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the m/z tolerance for precursor matches.
        /// </summary>
        public double MzTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the mass tolerance.
        /// </summary>
        public double MassTolerance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the adduct mass e.g. Proton H+
        /// </summary>
        public double AdductMass
        {
            get;
            set;
        }
        #endregion


        private void UpdateStatus(string message)
        {
            Progress?.Invoke(this, new ProgressNotifierArgs(message));
        }
        /// <summary>
        /// Clusters spectra together based on similarity.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="features"></param>
        private List<MsmsCluster> Cluster(int start,
                                         int stop,
                                         List<MSFeatureLight> features,
                                         SpectraProviderCache provider,
                                         double similarityTolerance)
        {
            var massTolerance = MassTolerance;

            // Maps the feature to a cluster ID.
            var featureMap = new Dictionary<MSFeatureLight, int>();

            // Maps the cluster ID to a cluster.
            var clusterMap = new Dictionary<int, MsmsCluster>();
            var clusters = new List<MsmsCluster>();

            // Create singleton clusters.
            var id = 0;
            for (var i = start; i < stop; i++)
            {
                var feature = features[i];
                var cluster = new MsmsCluster();
                cluster.Id = id++;
                cluster.MeanScore = 0;
                cluster.Features.Add(feature);

                featureMap.Add(feature, cluster.Id);
                clusterMap.Add(cluster.Id, cluster);
            }
            var protonMass = AdductMass;

            // Then iterate and cluster.
            for (var i = start; i < stop; i++)
            {
                var featureI = features[i];
                var clusterI = clusterMap[featureMap[featureI]];

                for (var j = i + 1; j < stop; j++)
                {

                    var featureJ = features[j];
                    var clusterJ = clusterMap[featureMap[featureJ]];

                    // Don't cluster the same thing
                    if (clusterI.Id == clusterJ.Id)
                        continue;

                    // Don't cluster from the same dataset.  Let the linkage algorithm decide if they
                    // belong in the same cluster, and later, go back and determine if the cluster is valid or not.
                    if (featureI.GroupId == featureJ.GroupId)
                        continue;

                    // Check the scan difference.  If it fits then we are within range.
                    var scanDiff = Math.Abs(featureI.Scan - featureJ.Scan);
                    if (scanDiff <= ScanRange)
                    {

                        // Use the most abundant mass because it had a higher chance of being fragmented.
                        var mzI = (featureI.MassMonoisotopicMostAbundant / featureI.ChargeState) + protonMass;
                        var mzJ = (featureJ.MassMonoisotopicMostAbundant / featureJ.ChargeState) + protonMass;

                        var mzDiff = Math.Abs(mzI - mzJ);
                        if (mzDiff <= MzTolerance)
                        {
                            var scanSummary = new ScanSummary();
                            if (featureI.MSnSpectra[0].Peaks.Count <= 0)
                            {
                                var spectraProvider = provider.GetSpectraProvider(featureI.GroupId);
                                featureI.MSnSpectra[0].Peaks = spectraProvider.GetRawSpectra(featureI.MSnSpectra[0].Scan, featureI.GroupId, out scanSummary);
                                featureI.MSnSpectra[0].Peaks = XYData.Bin(featureI.MSnSpectra[0].Peaks,
                                                                            0,
                                                                            2000,
                                                                            MzTolerance);
                            }
                            if (featureJ.MSnSpectra[0].Peaks.Count <= 0)
                            {
                                var spectraProvider = provider.GetSpectraProvider(featureJ.GroupId);
                                featureJ.MSnSpectra[0].Peaks = spectraProvider.GetRawSpectra(featureJ.MSnSpectra[0].Scan, out scanSummary);
                                featureJ.MSnSpectra[0].Peaks = XYData.Bin(featureJ.MSnSpectra[0].Peaks,
                                                                            0,
                                                                            2000,
                                                                            MzTolerance);
                            }


                            // Compute similarity
                            var score = SpectralComparer.CompareSpectra(featureI.MSnSpectra[0], featureJ.MSnSpectra[0]);

                            if (score >= similarityTolerance)
                            {
                                clusterJ.MeanScore += score;
                                foreach (var xFeature in clusterI.Features)
                                {
                                    clusterJ.Features.Add(xFeature);
                                    featureMap[xFeature] = clusterJ.Id;
                                    clusterMap.Remove(clusterI.Id);
                                }
                            }
                        }
                    }
                }
            }

            clusters.AddRange(clusterMap.Values);

            for (var i = start; i < stop; i++)
            {
                features[i].MSnSpectra[0].Peaks.Clear();
            }
            foreach (var cluster in clusters)
            {
                cluster.MeanScore /= (cluster.Features.Count - 1);
            }
            return clusters;
        }
        /// <summary>
        /// Aligns features based on MSMS spectral similarity.
        /// </summary>
        /// <param name="featureMap"></param>
        /// <param name="msms"></param>
        public List<MsmsCluster> Cluster(List<UMCLight> features, SpectraProviderCache provider)
        {

            UpdateStatus("Mapping UMC's to MS/MS spectra using intensity profile.");
            // Step 1: Cluster the spectra
            // Create the collection of samples.
            var msFeatures = new List<MSFeatureLight>();

            // Sort through the features
            foreach (var feature in features)
            {
                // Sort out charge states...?
                var chargeMap = new Dictionary<int, MSFeatureLight>();

                double abundance = int.MinValue;
                MSFeatureLight maxFeature = null;

                // Find the max abundance spectrum.  This the number of features we have to search.
                foreach (var msFeature in feature.MsFeatures)
                {
                    if (msFeature.Abundance > abundance && msFeature.MSnSpectra.Count > 0)
                    {
                        abundance = msFeature.Abundance;
                        maxFeature = msFeature;
                    }
                }

                if (maxFeature != null)
                {
                    msFeatures.Add(maxFeature);
                }
            }

            UpdateStatus(string.Format("Found {0} total spectra for clustering.", msFeatures.Count));

            UpdateStatus("Sorting spectra.");
            // Sort based on mass using the max abundance of the feature.
            msFeatures.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                { return x.MassMonoisotopicMostAbundant.CompareTo(y.MassMonoisotopicMostAbundant); });

            // Then cluster the spectra.
            var j = 1;
            var h = 0;
            var N = msFeatures.Count;

            var clusters = new List<MsmsCluster>();
            var tol = MassTolerance;
            var lastTotal = 0;
            UpdateStatus("Clustering spectra.");
            while (j < N)
            {
                var i = j - 1;
                var featureJ = msFeatures[j];
                var featureI = msFeatures[i];
                var diff = FeatureLight.ComputeMassPPMDifference(featureJ.MassMonoisotopicMostAbundant, featureI.MassMonoisotopicMostAbundant);

                if (Math.Abs(diff) > tol)
                {
                    // We only care to create clusters of size greater than one.
                    if ((j - h) > 1)
                    {
                        var data = Cluster(h,
                                                            j,
                                                            msFeatures,
                                                            provider,
                                                            SimilarityTolerance);
                        clusters.AddRange(data);
                    }

                    // Reset the count, we're done looking at those clusters.
                    h = j;
                }
                if (j - lastTotal > 500)
                {
                    lastTotal = j;
                    UpdateStatus(string.Format("Processed {0} / {1} total spectra.", lastTotal, N));
                }
                j++;
            }
            UpdateStatus("Finishing last cluster data.");

            // Cluster the rest
            if ((j - h) > 1)
            {
                var data = Cluster(h,
                                                    j,
                                                    msFeatures,
                                                    provider,
                                                    SimilarityTolerance);
                clusters.AddRange(data);
            }
            UpdateStatus("Finished clustering.");
            var passingClusters = clusters.Where(cluster => cluster.Features.Count >= MinimumClusterSize);
            return passingClusters.ToList();
        }
    }
}
