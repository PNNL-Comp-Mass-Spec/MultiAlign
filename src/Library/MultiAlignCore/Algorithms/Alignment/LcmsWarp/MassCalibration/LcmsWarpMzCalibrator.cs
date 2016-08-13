namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    using System.Collections.Generic;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;

    public class LcmsWarpMzCalibrator : IMassRegressionCalculator
    {
        /// <summary>
        /// The mass calibration options.
        /// </summary>
        private readonly LcmsWarpAlignmentOptions options;

        public LcmsWarpMzCalibrator(LcmsWarpAlignmentOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Performs Mass error regression based on MZ of the match.
        /// </summary>
        /// <param name="matches">The matches to calibrate.</param>
        public LcmsWarpMassAlignmentFunction CalculateCalibration(List<LcmsWarpFeatureMatch> matches)
        {
            var mzRecalibration = new LcmsWarpCombinedRegression();
            mzRecalibration.SetCentralRegressionOptions(
                                                        this.options.MassCalibNumXSlices,
                                                        this.options.MassCalibNumYSlices,
                                                        this.options.MassCalibMaxJump,
                                                        this.options.MassCalibMaxZScore,
                                                        this.options.RegressionType);

            // Copy all MZs and mass errors into a list of regression points
            var calibrations = new List<RegressionPoint>();

            foreach (var match in matches)
            {
                var feature = match.AligneeFeature;
                var baselineFeature = match.BaselineFeature;
                var ppm = FeatureLight.ComputeMassPPMDifference(feature.MassMonoisotopic, baselineFeature.MassMonoisotopic);
                var netDiff = baselineFeature.Net - feature.NetAligned;

                calibrations.Add(new RegressionPoint(feature.Mz, 0, netDiff, ppm));
            }

            mzRecalibration.CalculateRegressionFunction(calibrations, "MzMassError");

            return new LcmsWarpMassAlignmentFunction
            {
                Calibrations = new List<LcmsWarpCombinedRegression> { mzRecalibration }
            };
        }
    }
}
