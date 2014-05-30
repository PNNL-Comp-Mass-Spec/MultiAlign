#region

using MultiAlignCore.IO.Analysis;
using MultiAlignCore.IO.SequenceData;

#endregion

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    ///     Holds all feature access providers required by a MA analysis.
    /// </summary>
    public sealed class FeatureDataAccessProviders
    {
        public FeatureDataAccessProviders()
        {
            Synch = new object();
        }

        /// <summary>
        ///     Gets the object to synch on for concurrent access.
        /// </summary>
        public object Synch { get; private set; }

        /// <summary>
        ///     Constructor.
        /// </summary>
        public FeatureDataAccessProviders(IUmcDAO featureCache,
            IUmcClusterDAO clusterCache,
            IMSFeatureDAO msFeatureCache,
            IMSnFeatureDAO msnFeatureCache,
            IMsnFeatureToMSFeatureDAO msnFeatureMap,
            IDatasetDAO datasetCache,
            IMassTagMatchDAO massTagMatches,
            IMassTagDAO massTags,
            IDatabaseSearchSequenceDAO sequenceCache,
            ISequenceToMsnFeatureDao sequenceMapCache) :
                this()
        {
            ClusterCache = clusterCache;
            FeatureCache = featureCache;
            MsFeatureCache = msFeatureCache;
            MSnFeatureCache = msnFeatureCache;
            MsFeatureToMSnFeatureCache = msnFeatureMap;
            DatasetCache = datasetCache;
            MassTagMatches = massTagMatches;
            MassTags = massTags;
            DatabaseSequenceCache = sequenceCache;
            SequenceMsnMapCache = sequenceMapCache;
        }

        public ISequenceToMsnFeatureDao SequenceMsnMapCache { get; set; }

        /// <summary>
        ///     Gets or sets the data provider for storing MS/MS Clusters.
        /// </summary>
        public IDatabaseSearchSequenceDAO DatabaseSequenceCache { get; set; }


        /// <summary>
        ///     Gets or sets the mass tags loaded.
        /// </summary>
        public IMassTagDAO MassTags { get; set; }

        /// <summary>
        ///     Gets or sets the cluster to mass tag matches.
        /// </summary>
        public IMassTagMatchDAO MassTagMatches { get; set; }

        /// <summary>
        ///     Gets or sets the cache where the datasets information are stored.
        /// </summary>
        public IDatasetDAO DatasetCache { get; set; }

        /// <summary>
        ///     Gets or sets the interface to teh MS Feature to LCMS Feature map.
        /// </summary>
        public IMsnFeatureToMSFeatureDAO MsFeatureToMSnFeatureCache { get; set; }

        /// <summary>
        ///     Gets or sets the data acces object to LCMS features
        /// </summary>
        public IUmcDAO FeatureCache { get; set; }

        /// <summary>
        ///     Gets or sets the data access object to clusters.
        /// </summary>
        public IUmcClusterDAO ClusterCache { get; set; }

        /// <summary>
        ///     Gets or sets the data acces object to MS features.
        /// </summary>
        public IMSFeatureDAO MsFeatureCache { get; set; }

        /// <summary>
        ///     Gets or sets the data acces object to MS features.
        /// </summary>
        public IMSnFeatureDAO MSnFeatureCache { get; set; }
    }
}