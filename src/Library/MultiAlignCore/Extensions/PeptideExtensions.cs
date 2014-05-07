using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Extensions
{
    public static class PeptideExtensions
    {
        public static Dictionary<int, List<Peptide>> CreateScanMaps(this List<Peptide> peptides)
        {
            Dictionary<int, List<Peptide>> peptideMap = new Dictionary<int, List<Peptide>>();
            foreach (Peptide p in peptides)
            {
                if (!peptideMap.ContainsKey(p.Scan))
                {
                    peptideMap.Add(p.Scan, new List<Peptide>());
                }
                peptideMap[p.Scan].Add(p);
            }

            return peptideMap;
        }

        public static UMCLight GetParentUmc(this Peptide peptide)
        {
            if (peptide == null) return null;

            if (peptide.Spectrum == null) return null;

            return peptide.Spectrum.GetParentUmc();
        }
    }

    public static class MsnExtensions
    {        
        public static UMCLight GetParentUmc(this MSSpectra spectrum)
        {
            if (spectrum == null) return null;

            if (spectrum.ParentFeature != null)
            {
                return spectrum.ParentFeature.GetParentUmc();
            }
            return null;
        }
    }
}
