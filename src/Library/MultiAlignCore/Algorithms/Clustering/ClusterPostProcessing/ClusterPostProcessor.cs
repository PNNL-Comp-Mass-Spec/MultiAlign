﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using InformedProteomics.Backend.Utils;
    using MultiAlignCore.Algorithms.SpectralProcessing;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.RawData;

    public class ClusterPostProcessor<T, U> : IClusterer<T, U>
            where T : FeatureLight, IFeatureCluster<T>, new()
            where U : FeatureLight, IFeatureCluster<T>, new()
    {
        private readonly IFeatureComparisonScorer clusterScorer;

        public event EventHandler<ProgressNotifierArgs> Progress;

        public ClusterPostProcessor(IFeatureComparisonScorer clusterScorer)
        {
            this.clusterScorer = clusterScorer;
        }

        /// <summary>
        /// Breaks up clusters into smaller separate clusters that do no match well in MS/MS.
        /// </summary>
        /// <param name="data">The clusters to break up.</param>
        /// <param name="progress">The progress reporter.</param>
        /// <returns>The resulting clusters.</returns>
        public List<U> Cluster(List<T> data, IProgress<ProgressData> progress = null)
        {
            progress = progress ?? new Progress<ProgressData>();
            var progressData = new ProgressData();
            var processedClusters = new List<U>();
            for (int i = 0; i < data.Count; i++)
            {
                processedClusters.AddRange(this.ProcessCluster(data[i]));
                progress.Report(progressData.UpdatePercent((100.0 * i++) / data.Count));
            }
            return processedClusters;
        }

        /// <summary>
        /// Breaks up clusters into smaller separate clusters that do no match well in MS/MS.
        /// </summary>
        /// <param name="cluster">The cluster to break up.</param>
        /// <returns>The resulting clusters.</returns>
        private List<U> ProcessCluster(T cluster)
        {
            var features = cluster.Features;
            var umcToClusterHash = new Dictionary<FeatureLight, U> { { features[0], new U() } };
            for (int leftFeatureIndex = 1; leftFeatureIndex < features.Count; leftFeatureIndex++)
            {
                var leftFeature = features[leftFeatureIndex];
                for (int rightFeatureIndex = 0; rightFeatureIndex < leftFeatureIndex; rightFeatureIndex++)
                {
                    var rightFeature = features[rightFeatureIndex];
                    U clusterMatch;

                    // Score features against each other.
                    var score = this.clusterScorer.ScoreComparison(leftFeature, rightFeature);
                    if (score > 0)
                    {   // The features are a good match.
                        // Add the left feature to the right feature's cluster.
                        clusterMatch = umcToClusterHash[rightFeature];
                        clusterMatch.AddChildFeature(leftFeature);
                        umcToClusterHash.Add(rightFeature, clusterMatch);
                    }
                    else
                    {   // Not a match cluster. Cluster the left feature on its own.
                        clusterMatch = new U();
                        clusterMatch.AddChildFeature(leftFeature);
                        umcToClusterHash.Add(leftFeature, clusterMatch);
                    }

                    clusterMatch.CalculateStatistics(ClusterCentroidRepresentation.Median);
                }
            }

            return umcToClusterHash.Values.Distinct().ToList();
        }

        // Not used
        public FeatureClusterParameters<T> Parameters { get; set; }

        #region WILL NOT IMPLEMENT
        public void ClusterAndProcess(List<T> data, IClusterWriter<U> writer, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public List<U> Cluster(List<T> data, List<U> clusters, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}