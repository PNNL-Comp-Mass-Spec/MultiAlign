using MultiAlignCore.Algorithms.Options;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using System.Collections.Generic;

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
        List<UMCLight> FindFeatures(List<MSFeatureLight> features, 
                                    LcmsFeatureFindingOptions   options,                                     
                                    ISpectraProvider provider);
    }
}
