using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Algorithms.SpectralProcessing;

namespace MultiAlignTestSuite.Papers.Alignment
{

    /// <summary>
    /// Options for computing spectral comparisons
    /// </summary>
    public class SpectralOptions
    {
        public double NetTolerance { get; set; }
        public double MzTolerance { get; set; }
        public double TopIonPercent { get; set; }
        public double Fdr { get; set; }
        public double SimilarityCutoff { get; set; }
        public double IdScore { get; set; }
        public SpectralComparison ComparerType { get; set; }
    }
}
