using System;
using System.Collections.Generic;
using FeatureAlignment.Data.Features;

namespace FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    public class LcmsWarpMassAlignmentFunction : IAlignmentFunction
    {
        public List<LcmsWarpCombinedRegression> Calibrations { get; set; }

        public double WarpValue(double value)
        {
            throw new NotImplementedException();
        }

        public UMCLight GetWarpedFeature(UMCLight feature)
        {
            var calibratedFeature = new UMCLight(feature);
            foreach (var calibration in this.Calibrations)
            {
                calibration.CalibrateFeature(calibratedFeature);
            }

            return calibratedFeature;
        }

        public IEnumerable<UMCLight> GetWarpedFeatures(IEnumerable<UMCLight> features)
        {
            var calibratedFeatures = new List<UMCLight>();
            foreach (var calibration in this.Calibrations)
            {
                foreach (var feature in features)
                {
                    calibratedFeatures.Add(this.GetWarpedFeature(feature));
                }
            }

            return features;
        }
    }
}
