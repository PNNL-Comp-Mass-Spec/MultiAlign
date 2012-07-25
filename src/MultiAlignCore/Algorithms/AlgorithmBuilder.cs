using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.FeatureMatcher;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;
using MultiAlignCore.Data;
using PNNLOmics.Algorithms.FeatureMatcher.Data;

namespace MultiAlignCore.Algorithms
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
        public void BuildPeakMatcher(AnalysisOptions options)
        {
            PeakMatchingType type = PeakMatchingType.STAC;                        

            FeatureMatcherTolerances tolerances = new FeatureMatcherTolerances();

            switch(type)
            {
                case PeakMatchingType.Traditional:

                    TraditionalPeakMatcher<UMCClusterLight> matcher     = new TraditionalPeakMatcher<UMCClusterLight>();
                    matcher.Options                                     = options.STACOptions;
                    m_provider.PeakMatcher                              = matcher;
                    break;
                
                case PeakMatchingType.STAC:
                default:
                    STACAdapter<UMCClusterLight> stanleyMatcher         = new STACAdapter<UMCClusterLight>();
                    stanleyMatcher.Options.HistogramBinWidth            = options.STACOptions.HistogramBinWidth;
                    stanleyMatcher.Options.HistogramMultiplier          = options.STACOptions.HistogramMultiplier;
                    stanleyMatcher.Options.ShiftAmount                  = options.STACOptions.ShiftAmount;
                    stanleyMatcher.Options.ShouldCalculateHistogramFDR  = options.STACOptions.ShouldCalculateHistogramFDR;
                    stanleyMatcher.Options.ShouldCalculateShiftFDR      = options.STACOptions.ShouldCalculateShiftFDR;
                    stanleyMatcher.Options.ShouldCalculateSLiC          = options.STACOptions.ShouldCalculateSLiC;
                    stanleyMatcher.Options.ShouldCalculateSTAC          = options.STACOptions.ShouldCalculateSTAC;
                    stanleyMatcher.Options.UseDriftTime                 = options.STACOptions.UseDriftTime;
                    stanleyMatcher.Options.UseEllipsoid                 = options.STACOptions.UseEllipsoid;
                    stanleyMatcher.Options.UsePriors                    = options.STACOptions.UsePriors;
                    tolerances.DriftTimeTolerance                       = System.Convert.ToSingle(options.STACOptions.DriftTimeTolerance);
                    tolerances.MassTolerancePPM                         = options.STACOptions.MassTolerancePPM;
                    tolerances.NETTolerance                             = options.STACOptions.NETTolerance;
                    tolerances.Refined                                  = options.STACOptions.Refined;
                    stanleyMatcher.Options.UserTolerances               = tolerances;                    
                    m_provider.PeakMatcher                              = stanleyMatcher;
                    break;
            }
        }

        /// <summary>
        /// Returns the list of algorithms post build.
        /// </summary>
        /// <returns></returns>
        public AlgorithmProvider GetAlgorithmProvider(AnalysisOptions options)
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
                BuildPeakMatcher(options);
            }
            return m_provider;
        }
    }
}
