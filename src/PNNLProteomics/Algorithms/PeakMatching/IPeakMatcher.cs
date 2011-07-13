using System;
using System.Collections.Generic;

using PNNLProteomics.Data;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;

using PNNLProteomics.SMART;
using PNNLProteomics.Data.MassTags;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace PNNLProteomics.Algorithms.PeakMatching
{
    /// <summary>
    /// Interface for peak matching features and databases.
    /// </summary>
    public interface IPeakMatcher<T>
        where T: FeatureLight
    {
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        List<MassTagFeatureMatch<T>> PerformPeakMatching(List<T>                clusters,
                                                        MassTagDatabase         massTagDatabase,
                                                        clsPeakMatchingOptions  options,
                                                        double                  daltonShift);
        /// <summary>
        /// Calculates the SMART Scores if matched to a AMTDB 
        /// for peptide identification.
        /// </summary>
        classSMARTResults PerformSTAC(  List<T>             clusters,
                                        MassTagDatabase     massTagDatabase,
                                        classSMARTOptions   options);        

        /// <summary>
        /// Converts the SMART Results into peak matching results.
        /// </summary>
        /// <param name="smart">Results computed using SMART.</param>
        /// <returns>Peak matching results.</returns>
        List<MassTagFeatureMatch<T>> ConvertSTACResultsToPeakResults(classSMARTResults  smart,
                                                                    MassTagDatabase     massTagDatabase,
                                                                    List<T>             clusters);             
    }
}
