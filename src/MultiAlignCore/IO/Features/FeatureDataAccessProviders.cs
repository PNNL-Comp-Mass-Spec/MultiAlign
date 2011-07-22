using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Holds all feature access providers required by a MA analysis.
    /// </summary>
    public sealed class FeatureDataAccessProviders
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="featureCache">LCMS Features</param>
        /// <param name="clusterCache">LCMS Feature clusters</param>
        /// <param name="msFeatureCache">MS Features</param>
        /// <param name="msnFeatureCache">MS/MS Features</param>
        /// <param name="msFeatureMap">MS To LCMS Feature map</param>
        /// <param name="msnFeatureMap">MS to MSn Feature map</param>
        public FeatureDataAccessProviders(  IUmcDAO         featureCache,
                                            IUmcClusterDAO  clusterCache,
                                            IMSFeatureDAO   msFeatureCache,
                                            IMSnFeatureDAO  msnFeatureCache,
                                            IGenericDAO<MSFeatureToLCMSFeatureMap> msFeatureMap,
                                            IGenericDAO<MSFeatureToMSnFeatureMap>  msnFeatureMap
                                         )
        {
            ClusterCache                = clusterCache;
            FeatureCache                = featureCache;
            MSFeatureCache              = msFeatureCache;
            MSnFeatureCache             = msnFeatureCache;
            MSFeatureToLCMSFeatureCache = msFeatureMap;
            MSFeatureToMSnFeatureCache  = msnFeatureMap;
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
        /// Gets or sets the interface to teh MS Feature to LCMS Feature map.
        /// </summary>
        public IGenericDAO<MSFeatureToMSnFeatureMap> MSFeatureToMSnFeatureCache
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
        /// <summary>
        /// Gets or sets the data acces object to MS features.
        /// </summary>
        public IMSnFeatureDAO MSnFeatureCache
        {
            get;
            set;
        }
    }
}
