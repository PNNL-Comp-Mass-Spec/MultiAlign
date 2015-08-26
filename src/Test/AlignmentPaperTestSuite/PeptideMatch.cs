using System;
using MultiAlignCore.Data;

namespace AlignmentPaperTestSuite
{
    public class PeptideMatch
    {
        public bool IsMatch { get; set; }
        public double Similarity { get; set; }
        public string Peptide { get; set; }
        public int ScanX { get; set; }
        public int ScanY { get; set; }
        public double MzX { get; set; }
        public double NetX { get; set; }
        public double NetAlignedX { get; set; }
        public double MzY { get; set; }
        public double NetY { get; set; }
        public double NetAlignedY { get; set; }
        public MSSpectra SpectrumX { get; set; }
        public MSSpectra SpectrumY { get; set; }

        /// <summary>
        ///     Gets the NET difference between two features.
        /// </summary>
        public double NetDifference
        {
            get { return Math.Abs(NetX - NetY); }
        }

        public int Index { get; set; }
    }
}