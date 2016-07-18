#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Alignment.LcmsWarp;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;
using MultiAlignCore.Data.Features;

#endregion

namespace MultiAlignCore.Data.Alignment
{
    /// <summary>
    /// Holds alignment data
    /// This includes Histograms for the Mass, Net and Drift Errors,
    /// Dataset name, Heatmap scores, the NET slope, intercept and r squared values,
    /// the mean and standard deviations for NET and Mass and the alignment function itself.
    /// </summary>
    [Serializable]
    public class AlignmentData
    {
        public int DatasetID { get; set; }

        /// <summary>
        /// Property to hold the function for the alignment
        /// </summary>
        public LcmsWarpAlignmentFunction AlignmentFunction { get; set; }

        /// <summary>
        /// Alignment functions for the dataset, separated by separation type
        /// </summary>
        public IDictionary<FeatureLight.SeparationTypes, LcmsWarpResults> AlignmentFunctions { get; set; }

        /// <summary>
        /// The Alignment function used for mass warping
        /// </summary>
        public LcmsWarpResults MassAlignmentFunction { get; set; }

        public string AligneeDataset { get; set; }
        public bool BaselineIsAmtDB { get; set; }

        /// <summary>
        /// Property to hold the heat scores for the alignment
        /// </summary>
        public double[,] HeatScores { get; set; }
        public int MinScanBaseline { get; set; }
        public int MaxScanBaseline { get; set; }
        public float MinMTDBNET { get; set; }
        public float MaxMTDBNET { get; set; }

        /// <summary>
        /// Property to hold the mass error histogram from the alignment
        /// </summary>
        public Dictionary<double, int> MassErrorHistogram { get; set; }

        /// <summary>
        /// Property to hold the net error histogram from the alignment
        /// </summary>
        public Dictionary<double, int> NetErrorHistogram { get; set; }

        /// <summary>
        /// Property to hold the drift error histogram from the alignment
        /// </summary>
        public Dictionary<double, int> DriftErrorHistogram { get; set; }

        /// <summary>
        /// Property to hold the R squared value from a linear regression of the whole alignment
        /// </summary>
        public double NETRsquared { get; set; }

        /// <summary>
        /// Property to hold the slope from a linear regression of the whole alignment
        /// </summary>
        public double NETSlope { get; set; }

        /// <summary>
        /// Property to hold the intercept value from a linear regression of the whole alignment
        /// </summary>
        public double NETIntercept { get; set; }

        /// <summary>
        /// Property to hold the mean of the mass values from the alignment
        /// </summary>
        public double MassMean { get; set; }

        /// <summary>
        /// Property to hold the mean of the normalized elution time values from the alignment
        /// </summary>
        public double NETMean { get; set; }

        /// <summary>
        /// Property to hold the Standard Deviation of the mass from the alignment
        /// </summary>
        public double MassStandardDeviation { get; set; }

        /// <summary>
        /// Property to hold the Standard Deviation of the normalized elution time from the alignment
        /// </summary>
        public double NETStandardDeviation { get; set; }

        /// <summary>
        ///     Gets or sets the anchor points defined by spectral matching if that algorithm was used.
        ///     This is a stop gap until I can port this all to better objects.
        /// </summary>
        public IEnumerable<SpectralAnchorPointMatch> Matches { get; set; }

        /// <summary>
        ///     Gets or sets the residual alignment data.
        /// </summary>
        public ResidualData ResidualData { get; set; }

        public double MassKurtosis
        {
            get { return Math.Pow(MassMean, 4) / Math.Pow(MassStandardDeviation, 4); }
        }

        public double NETKurtosis
        {
            get { return Math.Pow(NETMean, 4) / Math.Pow(NETStandardDeviation, 4); }
        }

        public override bool Equals(object obj)
        {
            var factor = (AlignmentData) obj;

            if (factor == null)
            {
                return false;
            }
            if (!DatasetID.Equals(factor.DatasetID))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + DatasetID.GetHashCode();
            return hash;
        }
    }
}