using System.Collections.Generic;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// Holds the alignment data relevant to the LCMSWarp alignment.
    /// This includes Histograms for the Mass, Net and Drift Errors,
    /// Dataset name, Heatmap scores, the NET slope, intercept and r squared values,
    /// the mean and standard deviations for NET and Mass and the alignment function itself.
    /// </summary>
    public sealed class LcmsWarpAlignmentData
    {
        /// <summary>
        /// Property to hold the function for the alignment
        /// </summary>
        public LcmsWarpAlignmentFunction AlignmentFunction { get; set; }

        /// <summary>
        /// Property to hold the heat scores for the alignment
        /// </summary>
        public double[,] HeatScores { get; set; }

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
        public double NetRsquared { get; set; }

        /// <summary>
        /// Property to hold the slope from a linear regression of the whole alignment
        /// </summary>
        public double NetSlope { get; set; }

        /// <summary>
        /// Property to hold the intercept value from a linear regression of the whole alignment
        /// </summary>
        public double NetIntercept { get; set; }

        /// <summary>
        /// Property to hold the mean of the mass values from the alignment
        /// </summary>
        public double MassMean { get; set; }

        /// <summary>
        /// Property to hold the mean of the normalized elution time values from the alignment
        /// </summary>
        public double NetMean { get; set; }

        /// <summary>
        /// Property to hold the Standard Deviation of the mass from the alignment
        /// </summary>
        public double MassStandardDeviation { get; set; }

        /// <summary>
        /// Property to hold the Standard Deviation of the normalized elution time from the alignment
        /// </summary>
        public double NetStandardDeviation { get; set; }

        /// <summary>
        /// Gets or sets the residual alignment data.
        /// </summary>        
        public ResidualData ResidualData { get; set; }

        /// <summary>
        ///     Gets or sets the anchor poitns defined by spectral matching if that algorithm was used.
        ///     This is a stop gap until I can port this all to better objects.
        /// </summary>
        public IEnumerable<SpectralAnchorPointMatch> Matches { get; set; }

        ///// <summary>
        ///// Test to see if the alignment datasets are equal based on
        ///// the ID number of the alignment data. Returns true if
        ///// the dataset IDs are the same, false in any other case
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public override bool Equals(object obj)
        //{
        //    var factor = (LcmsWarpAlignmentData)obj;

        //    return factor != null && m_datasetId.Equals(factor.DatasetId);
        //}

        //public override int GetHashCode()
        //{
        //    int hash = 17;

        //    hash = hash * 23 + m_datasetId.GetHashCode();

        //    return hash;
        //}
    }
}