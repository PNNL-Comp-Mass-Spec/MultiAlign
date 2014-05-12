#region

using System.Collections.Generic;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignCore.Algorithms.Features
{
    /// <summary>
    ///     Consolidates an LCMS Feature List into a single list where members from one group are
    ///     condensed into a single list of features.
    /// </summary>
    public abstract class LCMSFeatureConsolidator
    {
        /// <summary>
        ///     Takes a list of UMC's (LCMS Features) and turns them into a dictionary
        ///     based on some group index.
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public abstract Dictionary<int, UMCLight> ConsolidateUMCs(List<UMCLight> features);
    }
}