namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    using System.Collections.Generic;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;

    public class LcmsWarpNetMassCalibrator : IMassRegressionCalculator
    {
        /// <summary>
        /// The mass calibration options.
        /// </summary>
        private readonly LcmsWarpAlignmentOptions options;

        public LcmsWarpNetMassCalibrator(LcmsWarpAlignmentOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Performs Mass error regression based on NET of the match
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        public LcmsWarpMassAlignmentFunction CalculateCalibration(List<LcmsWarpFeatureMatch> matches)
        {
            var netMassRecalibration = new LcmsWarpCombinedRegression();
            netMassRecalibration.SetCentralRegressionOptions(
                                                             this.options.MassCalibNumXSlices,
                                                             this.options.MassCalibNumYSlices,
                                                             this.options.MassCalibMaxJump,
                                                             this.options.MassCalibMaxZScore,
                                                             this.options.RegressionType);
            var calibrations = new List<RegressionPoint>();

            foreach (var match in matches)
            {
                var feature = match.AligneeFeature;
                var baselineFeature = match.BaselineFeature;
                var ppm = FeatureLight.ComputeMassPPMDifference(feature.MassMonoisotopic, baselineFeature.MassMonoisotopic);
                var netDiff = baselineFeature.Net - feature.NetAligned;

                calibrations.Add(new RegressionPoint(feature.Net, 0, netDiff, ppm));
            }

            netMassRecalibration.CalculateRegressionFunction(calibrations);

            return new LcmsWarpMassAlignmentFunction
            {
                Calibrations = new List<LcmsWarpCombinedRegression> { netMassRecalibration }
            };
        }
    }
}
