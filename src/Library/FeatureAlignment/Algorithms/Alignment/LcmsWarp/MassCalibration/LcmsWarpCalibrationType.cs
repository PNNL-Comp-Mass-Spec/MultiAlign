namespace FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    /// <summary>
    /// Enumeration of calibration types for LCMSWarping
    /// </summary>
    public enum LcmsWarpCalibrationType
    {
        /// <summary>
        /// Performs Regression based on MZ of the features
        /// </summary>
        MzRegression = 0,

        /// <summary>
        /// Performs Regression based on the NET of the features
        /// </summary>
        NetRegression,

        /// <summary>
        /// Performs a hybrid regression, performing MZ regression and then
        /// subsequently performing Scan regression
        /// </summary>
        Both
    };
}