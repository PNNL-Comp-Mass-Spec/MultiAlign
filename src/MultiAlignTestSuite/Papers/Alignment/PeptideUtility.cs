using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace MultiAlignTestSuite.Papers.Alignment
{
    public static class PeptideUtility
    {
        /// <summary>
        /// Determines if a peptide passes the cutoff
        /// </summary>
        /// <param name="peptidex"></param>
        /// <param name="peptidey"></param>
        /// <param name="score"></param>
        /// <param name="fdr"></param>
        /// <returns></returns>
        public static bool PassesCutoff(Peptide peptide, double score, double fdr)
        {
            bool passes = true;

            if (peptide == null)
                return false;

            if (peptide.Fdr   > fdr)    return false;            
            if (peptide.Score > score)  return false;            
            return passes;
        }

        /// <summary>
        /// Cleans the peptide string
        /// </summary>
        /// <param name="peptide"></param>
        /// <returns></returns>
        public static string CleanString(string peptide)
        {
            string[] peptides = peptide.Split('.');

            if (peptides.Length > 2)
            {
                return peptides[1];
            }
            return peptides[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peptides"></param>
        /// <returns></returns>
        public static Dictionary<string, Peptide> MapWithBestSequence(IEnumerable<Peptide> peptides)
        {
            Dictionary<string, List<Peptide>> map = MapSequence(peptides);
            return SortBestSequence(map);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="peptides"></param>
        /// <returns></returns>
        public static Dictionary<int, Peptide> MapWithBestScan(IEnumerable<Peptide> peptides)
        {
            Dictionary<int, List<Peptide>> map = MapScan(peptides);
            return SortBestScan(map);
        }

        /// <summary>
        /// Maps peptide sequences to a dictionary based on sequence
        /// </summary>
        /// <param name="peptides"></param>
        /// <returns></returns>
        public static Dictionary<string, List<Peptide>> MapSequence(IEnumerable<Peptide> peptides)
        {
            Dictionary<string, List<Peptide>> map = new Dictionary<string, List<Peptide>>();

            foreach (Peptide p in peptides)
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
        /// <param name="peptides"></param>
        /// <returns></returns>
        public static Dictionary<int, List<Peptide>> MapScan(IEnumerable<Peptide> peptides)
        {

            Dictionary<int, List<Peptide>> map = new Dictionary<int, List<Peptide>>();

            foreach (Peptide p in peptides)
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

            Dictionary<int, Peptide> map = new Dictionary<int, Peptide>();

            foreach (int scan in peptides.Keys)
            {
                List<Peptide> items = peptides[scan];
                Peptide p           = items.OrderBy(x => x.Score).FirstOrDefault();
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

            Dictionary<string, Peptide> map = new Dictionary<string, Peptide>();

            foreach (string sequence in peptides.Keys)
            {
                List<Peptide> items = peptides[sequence];
                Peptide p           = items.OrderBy(x => x.Score).FirstOrDefault();
                map.Add(sequence, p);
            }

            return map;
        }
    }
}
