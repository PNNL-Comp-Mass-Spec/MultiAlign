using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.Algorithms.MSLinker;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.IO.Parameters;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Holds all options for an analysis.
    /// </summary>
    public class AnalysisOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AnalysisOptions()
        {
			AlignmentOptions                = new AlignmentOptions() ; 
			DriftTimeAlignmentOptions       = new DriftTimeAlignmentOptions();            
            //FeatureFindingOptions           = new LCMSFeatureFindingOptions();
			ClusterOptions                  = new LCMSFeatureClusteringOptions() ;
            MSLinkerOptions                 = new MSLinkerOptions();
            //FeatureFilterOptions            = new FeatureFilterOptions();
            STACOptions                     = new StacOptions();            
            MassTagDatabaseOptions          = new MultiAlignCore.IO.MTDB.MassTagDatabaseOptions();
            ConsolidationOptions            = new FeatureConsolidatorOptions();
        }

        /// <summary>
        /// Gets or sets the options for linking MS features to MSMS Spectra.
        /// </summary>
        //[ParameterFileGroupAttribute("MS-MSn Feature Linking","Options when linking MS/MS spectra to MS Features")]
        public MSLinkerOptions MSLinkerOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the UMC Finding Options.
        /// </summary>
        //[ParameterFileGroup("LC-MS Feature Finding", "Options for the LC-MS Feature Finding when loading Decon2ls output.")]
        //public LCMSFeatureFindingOptions FeatureFindingOptions
        //{
        //    get;
        //    set;
        //}
        /// <summary>
        /// Gets or sets the filter criteria for loading features.
        /// </summary>
        //[ParameterFileGroup("LC-MS Filtering", "Options for filtering LC-MS features after feature loading or finding.")]
        //public FeatureFilterOptions FeatureFilterOptions
        //{
        //    get;
        //    set;
        //}
        /// <summary>
        /// Gets or sets the mass tag database options.
        /// </summary>
        [ParameterFileGroupAttribute("Mass Tag Database", "Filtering options for loading mass tags from the database.")]
        public MultiAlignCore.IO.MTDB.MassTagDatabaseOptions MassTagDatabaseOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the options for LCMS Warp
        /// </summary>
        [DataSummaryAttribute("Alignment Options")]
        [ParameterFileGroup("LC-MS Feature Alignment", "Alignment options for LCMSWarp of LC-MS Features")]
        public AlignmentOptions AlignmentOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the alignment options when using Ion Mobility data.
        /// </summary>
        [DataSummaryAttribute("Drift Time Options")]
        [ParameterFileGroup("Drift Time Alignment - IMS", "Alignment options when data was acquired with an Ion Mobility Separation.")]
        public DriftTimeAlignmentOptions DriftTimeAlignmentOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cluster options.
        /// </summary>
        [ParameterFileGroup("LC-MS Feature Clustering", "Clustering options for LC-MS Feature Clustering across datasets.")]
        public LCMSFeatureClusteringOptions ClusterOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Stac Options
        /// </summary>
        [ParameterFileGroupAttribute("Peptide Identification - STAC", "Peak Matching options for statistical testing of AMT related peak matching.")]
        public StacOptions STACOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets how the features are to be consolidated after clustering when exporting to cross tabs.
        /// </summary>
        [ParameterFileGroup("Feature Consolidator", "Options when reporting LC-MS Feature clusters.  Determines how to consolidate features if clustering links two nearby LC-MS Features.  Used for exporting cross tabs.")]
        public FeatureConsolidatorOptions ConsolidationOptions
        {
            get;
            set;
        }


        public bool IsImsExperiment { get; set; }
        public bool HasMsMsData { get; set; }
    }
}
