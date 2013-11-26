using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Algorithms.SpectralProcessing;

namespace MultiAlignTestSuite.Papers.Alignment.SSM
{

    /// <summary>
    /// Finds Anchor Points using Spectral Comparisons
    /// </summary>
    public class PeptideAnchorPointFinder
    {

        /// <summary>
        /// Filters the peptide list.
        /// </summary>
        /// <param name="peptides"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public IEnumerable<Peptide> Filter(IEnumerable<Peptide> peptides, double score)
        {
            return peptides.Where(x => x.Score < score);
        }

        /// <summary>
        /// Filters the peptide list.
        /// </summary>
        /// <param name="peptides"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public IEnumerable<Peptide> AssignNET(IEnumerable<Peptide> peptides)
        {
            int max = peptides.Max(x => x.Scan);
            int min = peptides.Min(x => x.Scan);

            List<Peptide> newPeptides = peptides.ToList();
            newPeptides.ForEach(x => x.NET = Convert.ToDouble(x.Scan - min) / Convert.ToDouble(max - min));
            return newPeptides;
        }

        /// <summary>
        /// Links anchor points use the raw spectra provided.
        /// </summary>
        /// <param name="readerX"></param>
        /// <param name="readerY"></param>
        /// <param name="comparer"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<AnchorPointMatch> FindAnchorPoints(  IEnumerable<Peptide> peptidesA,
                                                                IEnumerable<Peptide> peptidesB,
                                                                SpectralOptions      options)
        {
            List<AnchorPointMatch> matches          = new List<AnchorPointMatch>();                                            
            peptidesA = AssignNET(peptidesA);
            peptidesB = AssignNET(peptidesB);

            // Map sequences
            Dictionary<string, Peptide> mapA = PeptideUtility.MapWithBestSequence(peptidesA);
            Dictionary<string, Peptide> mapB = PeptideUtility.MapWithBestSequence(peptidesB);
                                
            foreach(string sequence in mapB.Keys)
            {
                if (mapA.ContainsKey(sequence))
                {
                    AnchorPointMatch point      = new AnchorPointMatch();
                    point.AnchorPointX.Peptide  = mapA[sequence];
                    point.AnchorPointY.Peptide  = mapB[sequence];
                    
                    double net                  = point.AnchorPointX.Net - point.AnchorPointY.Net;
                    double mz                   = point.AnchorPointX.Mz  - point.AnchorPointY.Mz;

                    if (Math.Abs(net) < options.NetTolerance && Math.Abs(mz) < options.MzTolerance)
                    {
                        matches.Add(point);                    
                    }                                                                
                }
            }

            return matches;
        }
    }
}
