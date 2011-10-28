using MultiAlignCore.Algorithms.PeakMatching;
using PNNLOmics.Algorithms.FeatureClustering;
using MultiAlignCore.Algorithms.Alignment;
using PNNLOmics.Data.Features;
using PNNLOmics.Utilities;
using MultiAlignCore.Algorithms.FeatureMatcher;

namespace MultiAlignCore.Algorithms
{

    /// <summary>
    /// Class that holds the algorithms to use.
    /// </summary>
    public class AlgorithmProvider
    {
        /// <summary>
        /// Fired when a status message needs to be logged.
        /// </summary>
        public event MessageEventHandler Status;

        /// <summary>
        /// Clusters features into feature clusters.
        /// </summary>
        private IClusterer<UMCLight, UMCClusterLight> m_clusterer;
        /// <summary>
        /// Aligns features to features or MTDB's
        /// </summary>
        private IFeatureAligner m_aligner;
        /// <summary>
        /// Peak matches features to a database.
        /// </summary>
        private IPeakMatcher<UMCClusterLight> m_peakMatcher;

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
        public IClusterer<UMCLight, UMCClusterLight> Clusterer
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
        public IPeakMatcher<UMCClusterLight> PeakMatcher
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
        
        /// <summary>
        /// Registers events for algorithms.
        /// </summary>
        public void RegisterEvents()
        {
            RegisterEvents(PeakMatcher, Aligner);
        }
        /// <summary>
        /// Registers status event handlers for each algorithm type.
        /// </summary>
        /// <param name="providers"></param>
        private void RegisterEvents(params IStatusProvider [] providers)
        {
            foreach (IStatusProvider provider in providers)
            {
                provider.Status += new MessageEventHandler(StatusHandler);
            }
        }
        /// <summary>
        /// Propogates the messages to the main listener/observer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StatusHandler(object sender, MessageEventArgs e)
        {
            if (Status != null)
            {
                Status(sender, e);
            }
        }        
    }
}
