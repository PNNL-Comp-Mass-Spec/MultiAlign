﻿#region

using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Data.Features;

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


        public static Dictionary<int, List<Peptide>> CreateScanMaps(this List<Peptide> peptides)
        {
            var peptideMap = new Dictionary<int, List<Peptide>>();
            foreach (var p in peptides)
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
            return peptide?.Spectrum?.GetParentUmc();
        }

        public static Dictionary<int, List<Peptide>> CreateScanMaps(this IEnumerable<Peptide> peptides)
        {
            var peptideMap = new Dictionary<int, List<Peptide>>();
            foreach (var p in peptides)
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
