namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    using System.Collections.Generic;

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
