using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Holds all feature access providers required by a MA analysis.
    /// </summary>
    public sealed class FeatureDataAccessProviders
    {

        public FeatureDataAccessProviders(  IUmcDAO         featureCache,
                                            IUmcClusterDAO  clusterCache,
                                            IMSFeatureDAO   msFeatureCache,
                                            IGenericDAO<MSFeatureToLCMSFeatureMap> msFeatureMap
                                         )
        {
            ClusterCache                = clusterCache;
            FeatureCache                = featureCache;
            MSFeatureCache              = msFeatureCache;
            MSFeatureToLCMSFeatureCache = msFeatureMap;
        }
        /// <summary>
        /// Gets or sets the interface to teh MS Feature to LCMS Feature map.
        /// </summary>
        public IGenericDAO<MSFeatureToLCMSFeatureMap> MSFeatureToLCMSFeatureCache
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the data acces object to LCMS features
        /// </summary>
        public IUmcDAO FeatureCache
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the data access object to clusters.
        /// </summary>
        public IUmcClusterDAO ClusterCache
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the data acces object to MS features.
        /// </summary>
        public IMSFeatureDAO MSFeatureCache
        {
            get;
            set;
        }

    }
}
