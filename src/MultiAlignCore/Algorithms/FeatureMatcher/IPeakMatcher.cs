using System;
using System.Collections.Generic;

using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;

using PNNLProteomics.SMART;
using MultiAlignCore.Data.MassTags;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    /// <summary>
    /// Interface for peak matching features and databases.
    /// </summary>
    public interface IPeakMatcher<T> 
        : IStatusProvider 
        where T: FeatureLight                 
    {
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        List<FeatureMatchLight<T, MassTagLight>> PerformPeakMatching(List<T>            clusters,
                                                                    MassTagDatabase     massTagDatabase);        
    }
}
