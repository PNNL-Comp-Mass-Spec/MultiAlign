using System;
using System.Collections.Generic;


using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLProteomics.Data.Analysis;
using MultiAlignEngine.PeakMatching;
using PNNLProteomics.Data.Analysis;

namespace MultiAlignWin
{
    /// <summary>
    /// Extracts the protein and peptide maps discovered through peak matching.
    /// </summary>
    public class ProteinMapExtractor
    {
        /// <summary>
        /// Extracts proteins and their peptides from the analysis object if peak matching is performed.
        /// </summary>
        /// <param name="analysis"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> ExtractProteinMaps(clsMultiAlignAnalysis analysis)
        {           
            Dictionary<string, List<string>> proteins = new Dictionary<string,List<string>>();
           
            if (analysis.PeakMatchedToMassTagDB == false)
            {
                return proteins;
            }                      

            /// 
            /// Peak matching mapping arrays 
            /// 
            clsPeakMatchingResults.clsPeakMatchingTriplet[] arrPeakMatchingTriplets = null;
            clsProtein[] arrPeakMatchingProteins = null;
            clsMassTag[] arrPeakMatchingMassTags = null;

            if (analysis.PeakMatchingResults != null)
            {
                arrPeakMatchingTriplets = analysis.PeakMatchingResults.marrPeakMatchingTriplet;
                arrPeakMatchingProteins = analysis.PeakMatchingResults.marrProteins;
                arrPeakMatchingMassTags = analysis.PeakMatchingResults.marrMasstags;
            }

            for (int i = 0; i < arrPeakMatchingTriplets.Length; i++)
            {
                ///
                /// So this peakmatchtriplet corresponds to the current cluster.                     
                ///                     
                clsPeakMatchingResults.clsPeakMatchingTriplet triplet = arrPeakMatchingTriplets[i];
                clsMassTag massTag = arrPeakMatchingMassTags[triplet.mintMassTagIndex];
                clsProtein protein = arrPeakMatchingProteins[triplet.mintProteinIndex];
                
                if (proteins.ContainsKey(protein.mstrProteinName) == false)
                    proteins.Add(protein.mstrProteinName, new List<string>());

                proteins[protein.mstrProteinName].Add(massTag.mstrPeptide);
            }

            return proteins;     
        }
    }
}
