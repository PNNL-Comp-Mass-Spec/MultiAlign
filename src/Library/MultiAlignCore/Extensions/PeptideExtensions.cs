#region

using System.Collections.Generic;
using PNNLOmics.Data;

#endregion

namespace MultiAlignCore.Extensions
{
    public static class PeptideExtensions
    {
        public static Dictionary<int, List<Peptide>> CreateScanMaps<T>(this List<T> peptides) where T : Peptide
        {
            var map = new Dictionary<int, List<Peptide>>();
            foreach (var peptide in peptides)
            {
                if (!map.ContainsKey(peptide.Scan))
                {
                    map.Add(peptide.Scan, new List<Peptide>());
                }
                map[peptide.Scan].Add(peptide);
            }
            return map;
        }
    }
}
