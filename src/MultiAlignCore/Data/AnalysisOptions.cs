using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.MTDB;
using MultiAlignCore.Algorithms.MSLinker;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Holds all options for an analysis.
    /// </summary>
    public class AnalysisOptions
    {
        public AnalysisOptions()
        {
			AlignmentOptions                = new AlignmentOptions() ; 
			DriftTimeAlignmentOptions       = new DriftTimeAlignmentOptions();
            UseMassTagDBAsBaseline          = false;
            FeatureFindingOptions           = new LCMSFeatureFindingOptions();
			ClusterOptions                  = new LCMSFeatureClusteringOptions() ;
            MSLinkerOptions                 = new MSLinkerOptions();
            FeatureFilterOptions            = new FeatureFilterOptions();
            STACOptions                     = new STACOptions();            
            MassTagDatabaseOptions          = new MassTagDatabaseOptions();
        }
        /// <summary>
        /// Stac Options
        /// </summary>
        public STACOptions STACOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the options for linking MS features to MSMS Spectra.
        /// </summary>
        public MSLinkerOptions MSLinkerOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the mass tag database options.
        /// </summary>
        public MassTagDatabaseOptions MassTagDatabaseOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the UMC Finding Options.
        /// </summary>
        public LCMSFeatureFindingOptions FeatureFindingOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cluster options.
        /// </summary>
        public LCMSFeatureClusteringOptions ClusterOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to use the mass tag database as the baseline dataset.
        /// </summary>
        [DataSummaryAttribute("Use MTDB As Baseline")]
        public bool UseMassTagDBAsBaseline
        {
            get;
            set;
        }
        [DataSummaryAttribute("Alignment Options")]
        public AlignmentOptions AlignmentOptions
        {
            get;
            set;
        }
        [DataSummaryAttribute("Drift Time Options")]
        public DriftTimeAlignmentOptions DriftTimeAlignmentOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the filter criteria for loading features.
        /// </summary>
        public FeatureFilterOptions FeatureFilterOptions
        {
            get;
            set;
        }
    }
}
