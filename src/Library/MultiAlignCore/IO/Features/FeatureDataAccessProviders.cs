#region

using MultiAlignCore.IO.Analysis;
using MultiAlignCore.IO.Datasets;
using MultiAlignCore.IO.Hibernate;
using MultiAlignCore.IO.MassTags;
using MultiAlignCore.IO.MsMs;
using MultiAlignCore.IO.SequenceData;

#endregion

namespace MultiAlignCore.IO.Features
{
    using System;
    using System.Threading;

    using MultiAlignCore.IO.RawData;

    /// <summary>
    /// Holds all feature access providers required by a MA analysis.
    /// </summary>
    public sealed class FeatureDataAccessProviders
    {
        public FeatureDataAccessProviders()
        {
            this.DatabaseLock = new ReaderWriterLockSlim();
#pragma warning disable 618
            Synch = new object();
#pragma warning restore 618
            this.ScanSummaryProviderCache = new ScanSummaryProviderCache();
            this.IdentificationProviderCache = new IdentificationProviderCache();
        }

        public ReaderWriterLockSlim DatabaseLock { get; }

        /// <summary>
        /// Gets the object to sync on for concurrent access.
        /// </summary>
        [Obsolete("Use DatabaseLock instead.")]
        public object Synch { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="featureCache">LCMS Features</param>
        /// <param name="clusterCache">LCMS Feature clusters</param>
        /// <param name="msFeatureCache">MS Features</param>
        /// <param name="msnFeatureCache">MS/MS Features</param>
        /// <param name="msnFeatureMap">MS to MSn Feature map</param>
        /// <param name="datasetCache"></param>
        /// <param name="massTagMatches"></param>
        /// <param name="massTags"></param>
        /// <param name="factorCache"></param>
        /// <param name="factorAssignmentCache"></param>
        /// <param name="msmsClusterCache"></param>
        /// <param name="sequenceCache"></param>
        /// <param name="sequenceMapCache"></param>
        public FeatureDataAccessProviders(
            IUmcDAO featureCache,
            IUmcClusterDAO clusterCache,
            IMSFeatureDAO msFeatureCache,
            IMSnFeatureDAO msnFeatureCache,
            IMsnFeatureToMSFeatureDAO msnFeatureMap,
            IDatasetDAO datasetCache,
            IMassTagMatchDAO massTagMatches,
            IMassTagDAO massTags,
            IFactorDao factorCache,
            IDatasetToFactorMapDAO factorAssignmentCache,
            IMSMSClusterMapDAO msmsClusterCache,
            IDatabaseSearchSequenceDAO sequenceCache,
            ISequenceToMsnFeatureDAO sequenceMapCache) :
                this()
        {
            ClusterCache = clusterCache;
            FeatureCache = featureCache;
            MSFeatureCache = msFeatureCache;
            MSnFeatureCache = msnFeatureCache;
            MSFeatureToMSnFeatureCache = msnFeatureMap;
            DatasetCache = datasetCache;
            MassTagMatches = massTagMatches;
            MassTags = massTags;
            FactorAssignmentCache = factorAssignmentCache;
            FactorCache = factorCache;
            MSMSClusterCache = msmsClusterCache;
            DatabaseSequenceCache = sequenceCache;
            SequenceMsnMapCache = sequenceMapCache;
        }

        public ISequenceToMsnFeatureDAO SequenceMsnMapCache { get; set; }

        /// <summary>
        /// Gets or sets the data provider for storing MS/MS Clusters.
        /// </summary>
        public IDatabaseSearchSequenceDAO DatabaseSequenceCache { get; set; }

        /// <summary>
        /// Gets or sets the data provider for storing MS/MS Clusters.
        /// </summary>
        public IMSMSClusterMapDAO MSMSClusterCache { get; set; }

        public IFactorDao FactorCache { get; set; }
        public IDatasetToFactorMapDAO FactorAssignmentCache { get; set; }

        /// <summary>
        /// Gets or sets the mass tags loaded.
        /// </summary>
        public IMassTagDAO MassTags { get; set; }

        /// <summary>
        /// Gets or sets the cluster to mass tag matches.
        /// </summary>
        public IMassTagMatchDAO MassTagMatches { get; set; }

        /// <summary>
        /// Gets or sets the cache where the datasets information are stored.
        /// </summary>
        public IDatasetDAO DatasetCache { get; set; }

        /// <summary>
        /// Gets or sets the interface to teh MS Feature to LCMS Feature map.
        /// </summary>
        public IMsnFeatureToMSFeatureDAO MSFeatureToMSnFeatureCache { get; set; }

        /// <summary>
        /// Gets or sets the data access object to LCMS features
        /// </summary>
        public IUmcDAO FeatureCache { get; set; }

        /// <summary>
        /// Gets or sets the data access object to clusters.
        /// </summary>
        public IUmcClusterDAO ClusterCache { get; set; }

        /// <summary>
        /// Gets or sets the data access object to MS features.
        /// </summary>
        public IMSFeatureDAO MSFeatureCache { get; set; }

        /// <summary>
        /// Gets or sets the data access object to MS features.
        /// </summary>
        public IMSnFeatureDAO MSnFeatureCache { get; set; }

        public ScanSummaryDAOHibernate ScanSummaryDao { get; set; }

        public ScanSummaryProviderCache ScanSummaryProviderCache { get; set; }

        public OptionsDAOHibernate OptionsDao { get; set; }

        public IdentificationProviderCache IdentificationProviderCache { get; set; }
    }
}