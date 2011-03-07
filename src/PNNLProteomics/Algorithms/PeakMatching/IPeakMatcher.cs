using System;
using System.Collections.Generic;

using PNNLProteomics.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;
using PNNLProteomics.SMART;

namespace PNNLProteomics.Algorithms.PeakMatching
{
    /// <summary>
    /// Interface for peak matching features and databases.
    /// </summary>
    public interface IPeakMatcher
    {
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        clsPeakMatchingResults PerformPeakMatching(List<clsCluster> clusters,
                                                          clsMassTagDB massTagDatabase,
                                                          clsPeakMatchingOptions options,
                                                          double daltonShift);
        /// <summary>
        /// Calculates the SMART Scores if matched to a AMTDB 
        /// for peptide identification.
        /// </summary>
        classSMARTResults PerformSTAC(List<clsCluster> clusters,
                                                clsMassTagDB massTagDatabase,
                                                classSMARTOptions options);        
        /// <summary>
        /// Converts the SMART Results into peak matching results.
        /// </summary>
        /// <param name="smart">Results computed using SMART.</param>
        /// <returns>Peak matching results.</returns>
        clsPeakMatchingResults ConvertSTACResultsToPeakResults(classSMARTResults smart,
                                                                       clsMassTagDB massTagDatabase,
                                                                       List<clsCluster> clusters);             
    }
}
