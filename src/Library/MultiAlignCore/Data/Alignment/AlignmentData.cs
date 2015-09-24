#region

using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Alignment.LcmsWarp;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;

#endregion

namespace MultiAlignCore.Data.Alignment
{
    [Serializable]
    public class AlignmentData
    {
        public int DatasetID { get; set; }
        public LcmsWarpAlignmentFunction AlignmentFunction { get; set; }

        public string AligneeDataset { get; set; }
        public bool BaselineIsAmtDB { get; set; }

        public double[,] HeatScores { get; set; }
        public int MinScanBaseline { get; set; }
        public int MaxScanBaseline { get; set; }
        public float MinMTDBNET { get; set; }
        public float MaxMTDBNET { get; set; }
        public Dictionary<double, int> MassErrorHistogram { get; set; }
        public Dictionary<double, int> NetErrorHistogram { get; set; }
        public Dictionary<double, int> DriftErrorHistogram { get; set; }
        public double NETRsquared { get; set; }
        public double NETSlope { get; set; }
        public double NETIntercept { get; set; }
        public double MassMean { get; set; }
        public double NETMean { get; set; }
        public double MassStandardDeviation { get; set; }
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
            get { return Math.Pow(MassMean, 4)/Math.Pow(MassStandardDeviation, 4); }
        }

        public double NETKurtosis
        {
            get { return Math.Pow(NETMean, 4)/Math.Pow(NETStandardDeviation, 4); }
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

            hash = hash*23 + DatasetID.GetHashCode();

            return hash;
        }
    }
}