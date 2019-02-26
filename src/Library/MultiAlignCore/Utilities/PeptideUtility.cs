using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data.MassTags;

namespace MultiAlignCore.Utilities
{
    public static class PeptideUtility
    {
        /// <summary>
        /// Determines if a peptide passes the cutoff
        /// </summary>
        public static bool PassesCutoff(Peptide peptide, double score, double fdr)
        {
            if (peptide == null)
                return false;

            if (peptide.Fdr   > fdr)    return false;
            return !(peptide.Score > score);
        }

        /// <summary>
        /// Cleans the peptide string
        /// </summary>
        public static string CleanString(string peptide)
        {
            var peptides = peptide.Split('.');
            return peptides.Length > 2 ? peptides[1] : peptides[0];
        }

        public static Dictionary<string, Peptide> MapWithBestSequence(IEnumerable<Peptide> peptides)
        {
            var map = MapSequence(peptides);
            return SortBestSequence(map);
        }
        public static Dictionary<int, Peptide> MapWithBestScan(IEnumerable<Peptide> peptides)
        {
            var map = MapScan(peptides);
            return SortBestScan(map);
        }

        /// <summary>
        /// Maps peptide sequences to a dictionary based on sequence
        /// </summary>
        public static Dictionary<string, List<Peptide>> MapSequence(IEnumerable<Peptide> peptides)
        {
            var map = new Dictionary<string, List<Peptide>>();

            foreach (var p in peptides)
            {

                if (!map.ContainsKey(p.Sequence))
                {
                    map.Add(p.Sequence, new List<Peptide>());
                }
                map[p.Sequence].Add(p);
            }

            return map;
        }
        /// <summary>
        /// Maps peptide sequences to a dictionary based on scan
        /// </summary>
        public static Dictionary<int, List<Peptide>> MapScan(IEnumerable<Peptide> peptides)
        {

            var map = new Dictionary<int, List<Peptide>>();

            foreach (var p in peptides)
            {
                if (!map.ContainsKey(p.Scan))
                    map.Add(p.Scan, new List<Peptide>());
                map[p.Scan].Add(p);
            }

            return map;
        }
        /// <summary>
        /// Maps peptide sequences to a dictionary based on scan
        /// </summary>
        /// <param name="peptides"></param>
        /// <returns></returns>
        public static Dictionary<int, Peptide> SortBestScan(Dictionary<int, List<Peptide>> peptides)
        {

            var map = new Dictionary<int, Peptide>();

            foreach (var scan in peptides.Keys)
            {
                var items = peptides[scan];
                var p           = items.OrderBy(x => x.Score).FirstOrDefault();
                map.Add(scan, p);
            }

            return map;
        }

        /// <summary>
        /// Maps peptide sequences to a dictionary based on scan
        /// </summary>
        /// <param name="peptides"></param>
        /// <returns></returns>
        public static Dictionary<string, Peptide> SortBestSequence(Dictionary<string, List<Peptide>> peptides)
        {

            var map = new Dictionary<string, Peptide>();

            foreach (var sequence in peptides.Keys)
            {
                var items = peptides[sequence];
                var p           = items.OrderBy(x => x.Score).FirstOrDefault();
                map.Add(sequence, p);
            }

            return map;
        }
    }
}
