#region

using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Extensions;

#endregion

namespace MultiAlignCore.Algorithms
{
    public class PeptideMsMsLinker
    {
        public void LinkPeptidesToSpectra(List<MSSpectra> msnSpectra, List<Peptide> peptides)
        {
            Dictionary<int, List<Peptide>> peptideMap = peptides.CreateScanMaps();
            Dictionary<int, List<MSSpectra>> msFeatureMap = msnSpectra.CreateScanMapsForMsMs();

            // Number of spectra that are identified.

            foreach (var scan in peptideMap.Keys)
            {
                // Here we don't allow any scans without MS/MS to make it through.                
                // We only add one, because per MS/MS scan there should be only one MSSpectra. 
                if (!msFeatureMap.ContainsKey(scan))
                    continue;

                // Then we want to match the list of all possible peptides for a given MS/MS to
                // that MS/MS
                var potentialPeptides = peptideMap[scan];
                var spectra = msFeatureMap[scan];

                // This is probably a N to 1 thing (N = number of peptide id's)                
                foreach (var peptide in potentialPeptides)
                {
                    foreach (var spectrum in spectra)
                    {
                        spectrum.Peptides.Add(peptide);
                        peptide.Spectrum = spectrum;
                    }
                }
            }
        }
    }
}