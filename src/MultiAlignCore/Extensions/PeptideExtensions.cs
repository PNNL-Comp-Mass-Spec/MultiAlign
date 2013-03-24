using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

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
    }
}
