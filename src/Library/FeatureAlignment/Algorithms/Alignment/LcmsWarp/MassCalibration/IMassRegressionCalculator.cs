using System.Collections.Generic;

namespace FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    public interface IMassRegressionCalculator
    {
        /// <summary>
        /// Performs Mass calibration, depending on calibration type, utilizing MZ
        /// regression, scan regression, or both (with the MZ regression preceeding
        /// the scan regression)
        /// </summary>
        LcmsWarpMassAlignmentFunction CalculateCalibration(List<LcmsWarpFeatureMatch> matches);
    }
}
