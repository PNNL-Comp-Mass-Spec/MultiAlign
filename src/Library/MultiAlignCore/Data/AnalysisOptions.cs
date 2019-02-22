#region

using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.IO.Parameters;

#endregion

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
            AlignmentOptions = new AlignmentOptions();
            DriftTimeAlignmentOptions = new DriftTimeAlignmentOptions();
            ClusterOptions = new LCMSFeatureClusteringOptions();
            STACOptions = new StacOptions();
            ConsolidationOptions = new FeatureConsolidatorOptions();
        }

        /// <summary>
        /// Gets or sets the options for LCMS Warp
        /// </summary>
        [DataSummary("Alignment Options")]
        [ParameterFileGroup("LC-MS Feature Alignment", "Alignment options for LCMSWarp of LC-MS Features")]
        public AlignmentOptions AlignmentOptions { get; set; }

        /// <summary>
        /// Gets or sets the alignment options when using Ion Mobility data.
        /// </summary>
        [DataSummary("Drift Time Options")]
        [ParameterFileGroup("Drift Time Alignment - IMS",
            "Alignment options when data was acquired with an Ion Mobility Separation.")]
        public DriftTimeAlignmentOptions DriftTimeAlignmentOptions { get; set; }

        /// <summary>
        /// Gets or sets the cluster options.
        /// </summary>
        [ParameterFileGroup("LC-MS Feature Clustering",
            "Clustering options for LC-MS Feature Clustering across datasets.")]
        public LCMSFeatureClusteringOptions ClusterOptions { get; set; }

        /// <summary>
        /// Stac Options
        /// </summary>
        [ParameterFileGroup("Peptide Identification - STAC",
            "Peak Matching options for statistical testing of AMT related peak matching.")]
        public StacOptions STACOptions { get; set; }

        /// <summary>
        /// Gets or sets how the features are to be consolidated after clustering when exporting to cross tabs.
        /// </summary>
        [ParameterFileGroup("Feature Consolidator",
            "Options when reporting LC-MS Feature clusters.  Determines how to consolidate features if clustering links two nearby LC-MS Features.  Used for exporting cross tabs."
            )]
        public FeatureConsolidatorOptions ConsolidationOptions { get; set; }


        public bool IsImsExperiment { get; set; }
        public bool HasMsMsData { get; set; }
    }
}