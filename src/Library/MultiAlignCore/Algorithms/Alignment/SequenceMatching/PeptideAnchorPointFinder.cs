using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Algorithms.Alignment.SpectralMatching;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using PNNLOmics.Utilities;
using PeptideUtility = MultiAlignCore.Utilities.PeptideUtility;

namespace MultiAlignCore.Algorithms.Alignment.SequenceMatching
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
            var enumerable = peptides as Peptide[] ?? peptides.ToArray();
            var max = enumerable.Max(x => x.Scan);
            var min = enumerable.Min(x => x.Scan);

            var newPeptides = enumerable.ToList();
            newPeptides.ForEach(x => x.NET = Convert.ToDouble(x.Scan - min) / Convert.ToDouble(max - min));
            return newPeptides;
        }

        /// <summary>
        /// Links anchor points use the raw spectra provided.
        /// </summary>
        public IEnumerable<SpectralAnchorPointMatch> FindAnchorPoints(  IEnumerable<Peptide> peptidesA,
                                                                        IEnumerable<Peptide> peptidesB,
                                                                        SpectralOptions      options)
        {
            var matches = new List<SpectralAnchorPointMatch>();
            peptidesA = AssignNET(peptidesA);
            peptidesB = AssignNET(peptidesB);

            // Map sequences
            var mapA = PeptideUtility.MapWithBestSequence(peptidesA);
            var mapB = PeptideUtility.MapWithBestSequence(peptidesB);

            foreach(var sequence in mapB.Keys)
            {
                if (mapA.ContainsKey(sequence))
                {
                    var point = new SpectralAnchorPointMatch
                    {
                        AnchorPointX = {Peptide = mapA[sequence]},
                        AnchorPointY = {Peptide = mapB[sequence]}
                    };

                    var net                  = point.AnchorPointX.Net - point.AnchorPointY.Net;
                    var mz                   = point.AnchorPointX.Mz  - point.AnchorPointY.Mz;

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
