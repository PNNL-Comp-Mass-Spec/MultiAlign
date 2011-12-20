using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureMatcher;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Algorithms
{

    /// <summary>
    /// Class that holds the algorithms to use.
    /// </summary>
    public class AlgorithmProvider: IProgressNotifer
    {
        /// <summary>
        /// Fired when a status message needs to be logged.
        /// </summary> 
        public event System.EventHandler<ProgressNotifierArgs> Progress;


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
        private void RegisterEvents(params IProgressNotifer [] providers)
        {
            foreach (IProgressNotifer provider in providers)
            {
                provider.Progress += new System.EventHandler<ProgressNotifierArgs>(provider_Progress); 
            }
        }

        void provider_Progress(object sender, ProgressNotifierArgs e)
        {
            if (Progress != null)
            {
                Progress(sender, e);
            }
        }                
    }
}
