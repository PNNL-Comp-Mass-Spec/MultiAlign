namespace FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    public class MassCalibrationFactory
    {
        public static IMassRegressionCalculator GetCalibrator(LcmsWarpAlignmentOptions options)
        {
            IMassRegressionCalculator calibrator;
            switch (options.CalibrationType)
            {
                case LcmsWarpCalibrationType.MzRegression:
                    calibrator = new LcmsWarpMzCalibrator(options);
                    break;
                case LcmsWarpCalibrationType.NetRegression:
                    calibrator = new LcmsWarpNetMassCalibrator(options);
                    break;
                default:
                    calibrator = new LcmsWarpNetMassCalibrator(options);
                    break;
            }

            return calibrator;
        }
    }
}
