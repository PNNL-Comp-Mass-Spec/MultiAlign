using System;
using System.Collections.Generic;

using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

using PNNLProteomics.Algorithms.Alignment;
using PNNLProteomics.Algorithms.Clustering;
using PNNLProteomics.Algorithms.PeakMatching;

namespace PNNLProteomics.Algorithms
{
    /// <summary>
    /// Builds the set of algorithms using the builder design pattern.
    /// </summary>
    public class AlgorithmBuilder
    {
        /// <summary>
        /// Final provider.
        /// </summary>
        private AlgorithmProvider m_provider;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AlgorithmBuilder()
        {
            m_provider = new AlgorithmProvider();
        }

        /// <summary>
        /// Builds the algorithm types.
        /// </summary>
        /// <param name="clusterType"></param>
        /// <returns></returns>
        public void BuildClusterer(ClusteringAlgorithmType clusterType)
        {
            IClusterer<UMCLight, UMCClusterLight> clusterer = null;
            switch (clusterType)
            {
                    
                case ClusteringAlgorithmType.AverageLinkage:
                    clusterer = new UMCAverageLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmType.Centroid:
                    clusterer = new UMCCentroidClusterer<UMCLight, UMCClusterLight>();
                    break;
                case ClusteringAlgorithmType.SingleLinkage:
                    clusterer = new UMCSingleLinkageClusterer<UMCLight, UMCClusterLight>();
                    break;
            }

            m_provider.Clusterer = clusterer;               
        }
        /// <summary>
        /// Builds the feature aligner.
        /// </summary>
        public void BuildAligner()
        {
            m_provider.Aligner = new LCMSWarpFeatureAligner();
        }
        /// <summary>
        /// Builds a peak matcher object.
        /// </summary>
        public void BuildPeakMatcher()
        {
            m_provider.PeakMatcher = new PeakMatcher();
        }

        /// <summary>
        /// Returns the list of algorithms post build.
        /// </summary>
        /// <returns></returns>
        public AlgorithmProvider GetAlgorithmProvider()
        {
            if (m_provider.Clusterer == null)
            {
                BuildClusterer(ClusteringAlgorithmType.AverageLinkage);
            }
            if (m_provider.Aligner == null)
            {
                BuildAligner();
            }
            if (m_provider.PeakMatcher == null)
            {
                BuildPeakMatcher();
            }
            return m_provider;
        }
    }
}
