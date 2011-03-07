using System;
using Mammoth.Data;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

using PNNLProteomics.Algorithms.Alignment;
using PNNLProteomics.Algorithms.Clustering;
using PNNLProteomics.Algorithms.PeakMatching;

namespace PNNLProteomics.Algorithms
{

    /// <summary>
    /// Class that holds the algorithms to use.
    /// </summary>
    public class AlgorithmProvider
    {
        /// <summary>
        /// Clusters features into feature clusters.
        /// </summary>
        private IClusterer<UMCLight, MammothCluster> m_clusterer;
        /// <summary>
        /// Aligns features to features or MTDB's
        /// </summary>
        private IFeatureAligner m_aligner;
        /// <summary>
        /// Peak matches features to a database.
        /// </summary>
        private IPeakMatcher m_peakMatcher;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AlgorithmProvider()
        {
            m_clusterer     = null;
            m_aligner       = null;
            m_peakMatcher   = null;
        }

        /// <summary>
        /// Gets or sets the clustering algorithm used.
        /// </summary>
        public IClusterer<UMCLight, MammothCluster> Clusterer
        {
            get
            {
                return m_clusterer;
            }
            set
            {
                m_clusterer = value;
            }
        }
        /// <summary>
        /// Gets or sets the feature/database aligner.
        /// </summary>
        public IFeatureAligner Aligner
        {
            get
            {
                return m_aligner;
            }
            set
            {
                m_aligner = value;
            }
        }
        /// <summary>
        /// Gets or sets the peak matcher object.
        /// </summary>
        public IPeakMatcher PeakMatcher
        {
            get
            {
                return m_peakMatcher;
            }
            set
            {
                m_peakMatcher = value;
            }
        }
    }
}
