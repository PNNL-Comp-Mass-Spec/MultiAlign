using System.Collections.Generic;

namespace FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    public class LcmsWarpNetMzCalibrator : IMassRegressionCalculator
    {
        /// <summary>
        /// The mass calibration options.
        /// </summary>
        private readonly LcmsWarpAlignmentOptions options;

        public LcmsWarpNetMzCalibrator(LcmsWarpAlignmentOptions options)
        {
            this.options = options;
        }

        public LcmsWarpMassAlignmentFunction CalculateCalibration(List<LcmsWarpFeatureMatch> matches)
        {
            var massCalibrator = new LcmsWarpMzCalibrator(this.options);
            var massCalibration = massCalibrator.CalculateCalibration(matches);

            var netMassCalibrator = new LcmsWarpNetMassCalibrator(this.options);
            var netMassCalibration = netMassCalibrator.CalculateCalibration(matches);

            return new LcmsWarpMassAlignmentFunction
            {
                Calibrations = new List<LcmsWarpCombinedRegression>
                {
                    massCalibration.Calibrations[0],
                    netMassCalibration.Calibrations[0]
                }
            };
        }
    }
}
