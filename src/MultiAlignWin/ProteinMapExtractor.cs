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
    /// Class that holds a list of peptides and the number of times they were matched
    /// </summary>
    public class PeptideMatchExtender
    {
        /// <summary>
        /// Dictionary holding peptide string and the matching counts
        /// </summary>
        private Dictionary<string, int> m_counts;

        public PeptideMatchExtender()
        {
            m_counts = new Dictionary<string, int>();
        }
        /// <summary>
        /// Gets or sets the peptide count dictionary.
        /// </summary>
        public Dictionary<string, int> PeptideMap
        {
            get
            {
                return m_counts;
            }
            set
            {
                m_counts = value;
            }
        }

        public void AddPeptide(string peptide)
        {
            if (m_counts.ContainsKey(peptide) == false)
                m_counts.Add(peptide, 1);
            else
                m_counts[peptide]++;
        }
    }
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
        public Dictionary<string, List<string>> ExtractProteinMaps(MultiAlignAnalysis analysis)
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
