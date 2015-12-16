namespace MultiAlignRogue.Alignment
{
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;

    /// <summary>
    /// This class contains settings for aligning/warping a single separation dimension.
    /// Each separation dimension can have its own tolerances, discretization, and reference dataset.
    /// </summary>
    public class AlignmentDimensionSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlignmentDimensionSettings"/> class. 
        /// </summary>
        public AlignmentDimensionSettings()
        {
            this.SeparationType = FeatureLight.SeparationTypes.LC;
            this.Tolerance = 0.01;
            this.NumSections = 100;
            this.ContractionFactor = 3;
            this.ShouldAlignDimension = true;
        }

        /// <summary>
        /// Gets or sets the type of separation that this dimension represents.
        /// </summary>
        public FeatureLight.SeparationTypes SeparationType { get; set; }

        /// <summary>
        /// Gets or sets the tolerance used when matching features in this dimension.
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        /// Gets or sets the number of sections to discretize this dimension into.
        /// </summary>
        public int NumSections { get; set; }

        /// <summary>
        /// Get or sets a value indicating how much this dimension's sections can
        /// expand or contract.
        /// 
        /// Each section can expand to (ContractionFactor)^2 or
        /// contract to 1 / ContractionFactor.
        /// </summary>
        public int ContractionFactor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this dimension should be warped.
        /// </summary>
        public bool ShouldAlignDimension { get; set; }

        /// <summary>
        /// Gets or sets the reference to the baseline dataset to align to.
        /// </summary>
        public IDataset BaselineDataset { get; set; }

        /// <summary>
        /// Default settings for liquid-chromatography.
        /// </summary>
        public static AlignmentDimensionSettings DefaultLcSettings = new AlignmentDimensionSettings();

        /// <summary>
        /// Default settings for IMS.
        /// </summary>
        public static AlignmentDimensionSettings DefaultImsSettings = new AlignmentDimensionSettings
        {
            SeparationType = FeatureLight.SeparationTypes.DriftTime,
            ContractionFactor = 0
        };
    }
}
