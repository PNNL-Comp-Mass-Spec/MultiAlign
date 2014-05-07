using PNNLOmics.Algorithms;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    /// <summary>
    /// Interface for peak matching features and databases.
    /// </summary>
    public interface IPeakMatcher<T> 
        : IProgressNotifer 
        where T: FeatureLight                 
    {
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        List<FeatureMatchLight<T, MassTagLight>> PerformPeakMatching(List<T>            clusters,
                                                                    MassTagDatabase     massTagDatabase);        
    }
}
