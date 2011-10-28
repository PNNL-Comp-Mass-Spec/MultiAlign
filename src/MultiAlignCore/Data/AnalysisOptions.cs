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

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Holds all options for an analysis.
    /// </summary>
    public class AnalysisOptions
    {
        public AnalysisOptions()
        {
			DefaultAlignmentOptions         = new MultiAlignEngine.Alignment.clsAlignmentOptions() ; 
			DriftTimeAlignmentOptions       = new DriftTimeAlignmentOptions();
            UseMassTagDBAsBaseline          = false;
            UMCFindingOptions               = new UMCFeatureFinderOptions();
            SMARTOptions                    = new Algorithms.PeakMatching.SMARTOptions();
			ClusterOptions                  = new MultiAlignEngine.Clustering.clsClusterOptions() ;
            MSLinkerOptions                 = new Algorithms.MSLinker.MSLinkerOptions();
            FeatureFilterOptions            = new FeatureFilterOptions();            
            PeakMatchingOptions             = new clsPeakMatchingOptions();
        }
        /// <summary>
        /// Stac Options
        /// </summary>
        public FeatureMatcherParameters STACOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cluster options.
        /// </summary>
        public clsPeakMatchingOptions PeakMatchingOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the SMART Options to use.
        /// </summary>
        public SMARTOptions SMARTOptions
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
        public MultiAlignEngine.MassTags.clsMassTagDatabaseOptions MassTagDBOptions
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
        [clsDataSummaryAttribute("Use MTDB As Baseline")]
        public bool UseMassTagDBAsBaseline
        {
            get;
            set;
        }
        [clsDataSummaryAttribute("Default Alignment Options")]
        public clsAlignmentOptions DefaultAlignmentOptions
        {
            get;
            set;
        }
        [clsDataSummaryAttribute("Drift Time Options")]
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
