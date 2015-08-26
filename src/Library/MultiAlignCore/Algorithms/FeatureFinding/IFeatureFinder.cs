using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignCore.Algorithms.FeatureFinding
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
                                    ISpectraProvider provider, DatasetInformation information);

        /// <summary>
        /// Finds features from the file of MS Features.
        /// </summary>
        [Obsolete("Does not provide means to properly determine elution time")]
        List<UMCLight> FindFeatures(List<MSFeatureLight> features,
                                    LcmsFeatureFindingOptions options,
                                    ISpectraProvider provider);
    }
}
