﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace MultiAlignTestSuite.Papers.Alignment
{
    /// <summary>
    /// Defines an anchor point match
    /// </summary>
    public class AnchorPointMatch
    {
        public AnchorPointMatch()
        {
        }


        public int Id { get; set; }

        public AnchorPoint AnchorPointX { get; set; }
        public AnchorPoint AnchorPointY { get; set; }
        public double SimilarityScore   { get; set; }

        /// <summary>
        /// Gets or sets whether this match is valid, or invalid (false positive)
        /// </summary>
        public bool IsValidMatch { get; set; }
    }
    /// <summary>
    /// Defines an anchor point
    /// </summary>
    public class AnchorPoint
    {
        public Peptide   Peptide    { get; set; }
        public MSSpectra Spectrum   { get; set; }
        public double    Net        { get; set; }
        public double    Mass       { get; set; }
        public double    Mz         { get; set; }
        public int       Scan       { get; set; }
    }
}
