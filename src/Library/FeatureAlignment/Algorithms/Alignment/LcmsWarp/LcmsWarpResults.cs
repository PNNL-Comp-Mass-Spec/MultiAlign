﻿using System.Collections.Generic;
using FeatureAlignment.Data.Features;

namespace FeatureAlignment.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// This class represents the LCMSWarp results for a single dimension.
    /// It includes both the statistics generated by warping this
    /// dimension to a baseline dimension as well as the resulting
    /// alignment function.
    /// </summary>
    public class LcmsWarpResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LcmsWarpResults"/> class.
        /// </summary>
        public LcmsWarpResults()
        {
            this.SeparationType = SeparationTypes.LC;
        }

        /// <summary>
        /// Id value for the database, set when persisted
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the alignment function which can warp values from
        /// an unaligned dataset to an aligned one.
        /// </summary>
        public IAlignmentFunction AlignmentFunction { get; set; }

        /// <summary>
        /// Gets or sets the separation type for these results
        /// </summary>
        public SeparationTypes SeparationType { get; set; }

        /// <summary>
        /// Gets or sets the the error histogram from the alignment
        /// </summary>
        public Dictionary<double, int> ErrorHistogram { get; set; }

        /// <summary>
        /// Gets or sets the alignment score heat map.
        /// </summary>
        public double[,] AlignmentScoreHeatMap { get; set; }

        /// <summary>
        /// Gets or sets the two-dimensional statistics for this dimension + mass dimension.
        /// </summary>
        public LcmsWarpStatistics Statistics { get; set; }

        /// <summary>
        /// Gets or sets the RSquared for the line fit to this dimension.
        /// </summary>
        public double RSquared { get; set; }

        /// <summary>
        /// Gets or sets the slope for the line fit to this dimension.
        /// </summary>
        public double Slope { get; set; }
    }
}