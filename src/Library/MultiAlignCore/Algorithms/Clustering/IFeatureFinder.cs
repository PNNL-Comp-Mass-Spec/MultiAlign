using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Creates UMC's based on MS Features.
    /// </summary>
    public interface IFeatureFinder: IProgressNotifer
    {
        /// <summary>
        /// Finds features from the file of MS Features.
        /// </summary>
        List<UMCLight> FindFeatures(List<MSFeatureLight> features, 
                                    LcmsFeatureFindingOptions   options,                                     
                                    ISpectraProvider provider);
    }
}
