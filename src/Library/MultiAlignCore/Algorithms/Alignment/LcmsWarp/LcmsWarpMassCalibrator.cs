using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;

    public class LcmsWarpMassCalibrator
    {
        private LcmsWarpAlignmentOptions options;

        public LcmsWarpMassCalibrator(LcmsWarpAlignmentOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Performs Mass calibration, depending on calibration type, utilizing MZ
        /// regression, scan regression, or both (with the MZ regression preceeding
        /// the scan regression)
        /// </summary>
        public List<LcmsWarpCombinedRegression> GetMassCalibrations(List<LcmsWarpFeatureMatch> matches)
        {
            var combinedRegressions = new List<LcmsWarpCombinedRegression>();
            switch (this.options.CalibrationType)
            {
                case LcmsWarpCalibrationType.MzRegression:
                    combinedRegressions.Add(this.GetMzMassErrorRegression(matches));
                    break;
                case LcmsWarpCalibrationType.NetRegression:
                    combinedRegressions.Add(this.GetNetMassErrorRegression(matches));
                    break;
                case LcmsWarpCalibrationType.Both:
                    combinedRegressions.Add(this.GetMzMassErrorRegression(matches));
                    combinedRegressions.Add(this.GetNetMassErrorRegression(matches));
                    break;
            }

            return combinedRegressions;
        }

        /// <summary>
        /// Performs Mass error regression based on MZ of the match.
        /// </summary>
        /// <param name="matches">The matches to calibrate.</param>
        private LcmsWarpCombinedRegression GetMzMassErrorRegression(List<LcmsWarpFeatureMatch> matches)
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

            mzRecalibration.CalculateRegressionFunction(calibrations);

            return mzRecalibration;
        }

        /// <summary>
        /// Performs Mass error regression based on NET of the match
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        private LcmsWarpCombinedRegression GetNetMassErrorRegression(List<LcmsWarpFeatureMatch> matches)
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

            return netMassRecalibration;
        }
    }
}
