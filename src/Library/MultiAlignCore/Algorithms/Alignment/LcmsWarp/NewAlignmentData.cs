namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using System.Collections.Generic;
    using MultiAlignCore.Data.Features;

    /// <summary>
    /// This class stores the results of running LCMSWarp on a set of features.
    /// It stores statistics and the alignment function for all aligned dimensions,
    /// as well as the resulting warped features.
    /// </summary>
    public class NewAlignmentData
    {
        /// <summary>
        /// Gets or sets the alignment data for all warped separation dimensions.
        /// These include both the alignment function and the alignment statistics.
        /// </summary>
        public Dictionary<FeatureLight.SeparationTypes, LcmsWarpResults> SeparationAlignments { get; set; }
        
        /// <summary>
        /// Gets or sets the alignment data for the mass.
        /// This includes both the mass alignment function and the mass alignment statistics.
        /// </summary>
        public LcmsWarpResults MassAlignment { get; set; }

        /// <summary>
        /// Gets or sets the list of fully warped features.
        /// </summary>
        public List<UMCLight> AlignedFeatures { get; set; } 
    }
}
