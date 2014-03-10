using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.IO.Parameters;
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
            MassTagDatabaseOptions  = new IO.MTDB.MassTagDatabaseOptions();
            MsFilteringOptions      = new MsFeatureFilteringOptions();
            LcmsFindingOptions      = new LcmsFeatureFindingOptions(InstrumentTolerances);
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
        [ParameterFileGroup("Mass Tag Database", "Filtering options for loading mass tags from the database.")]
        public IO.MTDB.MassTagDatabaseOptions MassTagDatabaseOptions { get; set; }
        /// <summary>
        /// Gets or sets the options for MS filtering.
        /// </summary>
        public MsFeatureFilteringOptions MsFilteringOptions { get; set; }
        /// <summary>
        /// Gets or sets the feature finding options.
        /// </summary>
        public LcmsFeatureFindingOptions LcmsFindingOptions { get; set; }
        /// <summary>
        /// Gets or sets the options for LC-MS filtering.
        /// </summary>
        public LcmsFeatureFilteringOptions LcmsFilteringOptions { get; set; }
        /// <summary>
        /// Gets or sets the options for LC-MS alignment
        /// </summary>
        [ParameterFileGroup("LC-MS Feature Alignment", "Alignment options for LCMSWarp of LC-MS Features")]
        public AlignmentOptions AlignmentOptions { get; set; }
        /// <summary>
        /// Gets or sets the options for clustering LC-MS data
        /// </summary>
        public LcmsClusteringOptions LcmsClusteringOptions { get; set; }
        /// <summary>
        /// Gets or sets the options for STAC identification
        /// </summary>
        [ParameterFileGroupAttribute("Peptide Identification - STAC", "Peak Matching options for statistical testing of AMT related peak matching.")]        
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
