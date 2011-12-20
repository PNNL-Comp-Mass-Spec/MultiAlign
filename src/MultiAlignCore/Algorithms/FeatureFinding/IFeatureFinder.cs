using System;
using System.Collections.Generic;
using System.Linq;
using PNNLOmics.Data.Features;
using System.Text;
using MultiAlignEngine.Features;

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    /// Creates UMC's based on MS Features.
    /// </summary>
    public interface IFeatureFinder
    {
        /// <summary>
        /// Finds features from the file of MS Features.
        /// </summary>
        /// <param name="path">Path containing MS Features.</param>
        /// <returns>List of UMC's.</returns>
        List<UMCLight> FindFeatures(List<MSFeatureLight> features, LCMSFeatureFindingOptions options);
    }
}
