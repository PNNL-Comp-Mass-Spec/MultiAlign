using MultiAlignCore.Algorithms.FeatureMatcher;
using PNNLOmics.Algorithms;

namespace MultiAlignCore.Algorithms.Options
{
    /// <summary>
    /// Analysis Options for MultiAlign 
    /// </summary>
    public class MultiAlignAnalysisOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MultiAlignAnalysisOptions()
        {
            InstrumentTolerances    = new FeatureTolerances();                        
            MassTagDatabaseOptions  = new MassTagDatabaseOptions();
            MsFilteringOptions      = new MsFeatureFilteringOptions();
            LcmsFilteringOptions    = new LcmsFeatureFilteringOptions();            
            AlignmentOptions        = new AlignmentOptions();
            LcmsClusteringOptions   = new LcmsClusteringOptions(InstrumentTolerances);
            StacOptions             = new STACOptions();
            HasMsMs                 = false;
            UsedIonMobility         = false;
        }
        /// <summary>
        /// Gets or sets instrument tolerances
        /// </summary>
        public FeatureTolerances InstrumentTolerances { get; set; }

        /// <summary>
        /// Gets or sets the options for loading data from a mass tag database.
        /// </summary>
        public MassTagDatabaseOptions MassTagDatabaseOptions { get; set; }
        /// <summary>
        /// Gets or sets the options for MS filtering.
        /// </summary>
        public MsFeatureFilteringOptions MsFilteringOptions { get; set; }        
        /// <summary>
        /// Gets or sets the options for LC-MS filtering.
        /// </summary>
        public LcmsFeatureFilteringOptions LcmsFilteringOptions { get; set; }
        /// <summary>
        /// Gets or sets the options for LC-MS alignment
        /// </summary>
        public AlignmentOptions AlignmentOptions { get; set; }
        /// <summary>
        /// Gets or sets the options for clustering LC-MS data
        /// </summary>
        public LcmsClusteringOptions LcmsClusteringOptions { get; set; }
        /// <summary>
        /// Gets or sets the options for STAC identification
        /// </summary>
        public STACOptions StacOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether there was Fragmentation
        /// </summary>
        public bool HasMsMs { get; set; }
        /// <summary>
        /// Gets or sets whether the analysis used Ion Mobility
        /// </summary>
        public bool UsedIonMobility { get; set; }
    }
}
