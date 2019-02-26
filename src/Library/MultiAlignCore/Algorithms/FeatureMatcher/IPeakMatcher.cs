#region

using System.Collections.Generic;
using FeatureAlignment.Algorithms;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms.FeatureMatcher
{
    /// <summary>
    /// Interface for peak matching features and databases.
    /// </summary>
    public interface IPeakMatcher<T>
        : IProgressNotifer
        where T : FeatureLight
    {
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        List<FeatureMatchLight<T, MassTagLight>> PerformPeakMatching(List<T> clusters,
            MassTagDatabase massTagDatabase);
    }
}