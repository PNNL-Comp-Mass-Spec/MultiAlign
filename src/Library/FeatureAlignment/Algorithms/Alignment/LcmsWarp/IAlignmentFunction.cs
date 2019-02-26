using System.Collections.Generic;
using FeatureAlignment.Data.Features;

namespace FeatureAlignment.Algorithms.Alignment.LcmsWarp
{
    public interface IAlignmentFunction
    {
        /// <summary>
        /// Warp a value by the alignment function.
        /// </summary>
        /// <param name="value">The value to warp.</param>
        /// <returns>The warped value.</returns>
        double WarpValue(double value);

        /// <summary>
        /// Warp the feature's value by the alignment function for the given value type.
        /// </summary>
        /// <param name="feature">The feature to warp.</param>
        UMCLight GetWarpedFeature(UMCLight feature);

        /// <summary>
        /// Warp each feature's separation value by the alignment function for the value type.
        /// </summary>
        /// <param name="features">The features to warp.</param>
        IEnumerable<UMCLight> GetWarpedFeatures(IEnumerable<UMCLight> features);
    }
}
