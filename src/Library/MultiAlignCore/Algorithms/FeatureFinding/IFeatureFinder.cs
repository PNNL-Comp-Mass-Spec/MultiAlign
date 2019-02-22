using System;
using System.Collections.Generic;
using InformedProteomics.Backend.Utils;
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
                                    LcmsFeatureFindingOptions options,
                                    IScanSummaryProvider provider,
                                    IProgress<PRISM.ProgressData> progress = null);
    }
}
