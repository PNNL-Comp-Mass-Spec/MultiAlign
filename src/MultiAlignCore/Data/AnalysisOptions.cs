using System.Collections.Generic;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.PeakMatching;
using MultiAlignCore.Data.Features;
using MultiAlignEngine;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.PeakMatching;
using PNNLOmics.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.IO.MTDB;

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
            UMCFindingOptions               = new UMCFeatureFinderOptions();
			ClusterOptions                  = new MultiAlignEngine.Clustering.clsClusterOptions() ;
            MSLinkerOptions                 = new Algorithms.MSLinker.MSLinkerOptions();
            FeatureFilterOptions            = new FeatureFilterOptions();
            STACAdapterOptions              = new STACOptions();            
            MassTagDatabaseOptions          = new MassTagDatabaseOptions();
        }
        /// <summary>
        /// Stac Options
        /// </summary>
        public STACOptions STACAdapterOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the options for linking MS features to MSMS Spectra.
        /// </summary>
        public MultiAlignCore.Algorithms.MSLinker.MSLinkerOptions MSLinkerOptions
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
        public UMCFeatureFinderOptions UMCFindingOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cluster options.
        /// </summary>
        public clsClusterOptions ClusterOptions
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
