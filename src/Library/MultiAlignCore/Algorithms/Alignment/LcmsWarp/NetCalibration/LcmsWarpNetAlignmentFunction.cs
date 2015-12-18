namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp.NetCalibration
{
    using System.Collections.Generic;
    using System.Linq;

    using MultiAlignCore.Data.Features;

    /// <summary>
    /// This class represents the alignment function that is calculated by the LCMSWarp algorithm.
    /// This class stores the mapping between alignee sections and baseline sections as a
    /// list of <see cref="LcmsWarpAlignmentMatch" />, and it uses this list and the alignee section
    /// info to warp the NETs of features for this alignment function.
    /// </summary>
    public class LcmsWarpNetAlignmentFunction : IAlignmentFunction
    {
        /// <summary>
        /// Gets or sets the alignee section info for warping separation values.
        /// </summary>
        public LcmsWarpSectionInfo AligneeSections { get; set; }

        /// <summary>
        /// Gets or sets a list of matches mapping alignee sections to baseline sections.
        /// </summary>
        public List<LcmsWarpAlignmentMatch> Matches { get; set; }

        /// <summary>
        /// Gets or sets the type of separation that this alignment function maps.
        /// </summary>
        public FeatureLight.SeparationTypes SeparationType { get; set; }

        /// <summary>
        /// Warp a separation value by the alignment function.
        /// </summary>
        /// <param name="value">The separation value to warp.</param>
        /// <returns>The warped separation value.</returns>
        public double WarpValue(double value)
        {
            var section = this.AligneeSections.GetSectionNumber(value);

            var netStart = this.Matches[section].AligneeNetStart;
            var netEnd = this.Matches[section].AligneeNetEnd;
            var netStartBaseline = this.Matches[section].BaselineNetStart;
            var netEndBaseline = this.Matches[section].BaselineNetEnd;

            return ((value - netStart) * (netEndBaseline - netStartBaseline) / (netEnd - netStart)) + netStartBaseline;
        }

        /// <summary>
        /// Warp the feature's separation value by the alignment function for the given separation value type.
        /// </summary>
        /// <param name="feature">The feature to warp.</param>
        public UMCLight GetWarpedFeature(UMCLight feature)
        {
            var warpedFeature = new UMCLight(feature);
            var separationValue = feature.GetSeparationValue(this.SeparationType);
            var warpedValue = this.WarpValue(separationValue);
            warpedFeature.SetSeparationValue(this.SeparationType, warpedValue);

            return feature;
        }

        /// <summary>
        /// Warp each feature's separation value by the alignment function for the given separation value type.
        /// </summary>
        /// <param name="features">The features to warp.</param>
        public IEnumerable<UMCLight> GetWarpedFeatures(IEnumerable<UMCLight> features)
        {
            return features.Select(this.GetWarpedFeature);
        }
    }
}
