#region

using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.Parameters;

#endregion

namespace MultiAlignCore.Algorithms.Options
{
    using MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing;

    /// <summary>
    ///     Analysis Options for MultiAlign
    /// </summary>
    public class MultiAlignAnalysisOptions
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public MultiAlignAnalysisOptions()
        {
            DataLoadOptions = new DataLoadingOptions();
            InstrumentTolerances = new FeatureTolerances();
            MassTagDatabaseOptions = new MassTagDatabaseOptions();
            MsFilteringOptions = new MsFeatureFilteringOptions();
            LcmsFindingOptions = new LcmsFeatureFindingOptions(InstrumentTolerances);
            LcmsFilteringOptions = new LcmsFeatureFilteringOptions
            {
                FilterOnMinutes = true,
                FeatureLengthRangeMinutes = new FilterRange(0, 20),
                MinimumDataPoints = 3,
                FeatureLengthRangeScans = new FilterRange(0, 2000)
            };

            AlignmentOptions = new AlignmentOptions();
            LcmsClusteringOptions = new LcmsClusteringOptions(InstrumentTolerances);
            StacOptions = new StacOptions();
            HasMsMs = false;
            UsedIonMobility = false;
            this.ClusterPostProcessingoptions = new ClusterPostProcessingOptions();
        }

        /// <summary>
        ///     Gets or sets data loading options
        /// </summary>
        public DataLoadingOptions DataLoadOptions { get; set; }

        /// <summary>
        ///     Gets or sets instrument tolerances
        /// </summary>
        public FeatureTolerances InstrumentTolerances { get; set; }

        /// <summary>
        ///     Gets or sets the options for loading data from a mass tag database.
        /// </summary>
        public MassTagDatabaseOptions MassTagDatabaseOptions { get; set; }

        /// <summary>
        ///     Gets or sets the options for MS filtering.
        /// </summary>
        public MsFeatureFilteringOptions MsFilteringOptions { get; set; }

        /// <summary>
        ///     Gets or sets the feature finding options.
        /// </summary>
        public LcmsFeatureFindingOptions LcmsFindingOptions { get; set; }

        /// <summary>
        ///     Gets or sets the options for LC-MS filtering.
        /// </summary>
        public LcmsFeatureFilteringOptions LcmsFilteringOptions { get; set; }

        /// <summary>
        ///     Gets or sets the options for LC-MS alignment
        /// </summary>
        [ParameterFileGroup("LC-MS Feature Alignment", "Alignment options for LCMSWarp of LC-MS Features")]
        public AlignmentOptions AlignmentOptions { get; set; }

        /// <summary>
        ///     Gets or sets the options for clustering LC-MS data
        /// </summary>
        public LcmsClusteringOptions LcmsClusteringOptions { get; set; }

        public ClusterPostProcessingOptions ClusterPostProcessingoptions { get; set; }

        /// <summary>
        ///     Gets or sets the options for STAC identification
        /// </summary>
        public StacOptions StacOptions { get; set; }

        /// <summary>
        ///     Gets or sets whether there was Fragmentation
        /// </summary>
        public bool HasMsMs { get; set; }

        /// <summary>
        ///     Gets or sets whether the analysis used Ion Mobility
        /// </summary>
        public bool UsedIonMobility { get; set; }

        /// <summary>
        ///     Gets or sets the alignment spectral options
        /// </summary>
        public SpectralOptions SpectralOptions { get; set; }
    }
}