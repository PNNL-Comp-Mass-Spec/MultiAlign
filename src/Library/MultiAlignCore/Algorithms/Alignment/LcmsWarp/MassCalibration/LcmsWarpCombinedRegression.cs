namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp.MassCalibration
{
    using System.Collections.Generic;

    using MultiAlignCore.Algorithms.Regression;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;

    /// <summary>
    /// Object that holds instances of all three regression types, as well as providing a wrapper method for all three of the
    /// regression types that LCMS could use
    /// </summary>
    public sealed class LcmsWarpCombinedRegression
    {
        private bool _lsqFailed;
        private readonly LcmsWarpCentralRegression _central;
        private readonly LeastSquaresSplineRegressionModel _lsqReg;
        private readonly LcmsNaturalCubicSplineRegression _cubicSpline;

        /// <summary>
        /// Public constructor for a Hybrid regression
        /// </summary>
        public LcmsWarpCombinedRegression()
        {
            this.RegressionType = LcmsWarpRegressionType.Hybrid;
            this._lsqFailed = false;
            this._central = new LcmsWarpCentralRegression();
            this._lsqReg = new LeastSquaresSplineRegressionModel();
            this._cubicSpline = new LcmsNaturalCubicSplineRegression();
        }

        public LcmsWarpRegressionType RegressionType { get; set; }

        /// <summary>
        /// Sets the options for all three regression types, setting up the number of knots for
        /// cubic spline and LSQ while setting the outlier z score for central regression
        /// </summary>
        /// <param name="numKnots"></param>
        /// <param name="outlierZscore"></param>
        public void SetLsqOptions(int numKnots, double outlierZscore)
        {
            this._cubicSpline.SetOptions(numKnots);
            this._lsqReg.SetOptions(numKnots);
            this._central.SetOutlierZScore(outlierZscore);
        }

        /// <summary>
        /// Sets all the options for a central regression type
        /// </summary>
        /// <param name="numXBins"></param>
        /// <param name="numYBins"></param>
        /// <param name="numJumps"></param>
        /// <param name="regZtolerance"></param>
        /// <param name="regType"></param>
        public void SetCentralRegressionOptions(int numXBins, int numYBins, int numJumps, double regZtolerance,
            LcmsWarpRegressionType regType)
        {
            this._central.SetOptions(numXBins, numYBins, numJumps, regZtolerance);
            this.RegressionType = regType;
        }

        /// <summary>
        /// Sets the regression points to the appropriate values for the regression function
        /// </summary>
        /// <param name="matches"></param>
        public void CalculateRegressionFunction(List<RegressionPoint> matches)
        {
            switch (this.RegressionType)
            {
                case LcmsWarpRegressionType.Central:
                    this._central.CalculateRegressionFunction(matches);
                    break;
                default:
                    this._central.CalculateRegressionFunction(matches);
                    this._central.RemoveRegressionOutliers();
                    this._lsqFailed = !this._cubicSpline.CalculateLsqRegressionCoefficients(this._central.Points);
                    break;
            }
        }

        /// <summary>
        /// Given a value x, finds the appropriate y value that would correspond to it in the regression function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetPredictedValue(double x)
        {
            switch (this.RegressionType)
            {
                case LcmsWarpRegressionType.Central:
                    return this._central.GetPredictedValue(x);

                default:
                    return !this._lsqFailed ? this._cubicSpline.GetPredictedValue(x) : this._central.GetPredictedValue(x);
            }
        }

        public void CalibrateFeature(UMCLight feature)
        {
            var mass = feature.MassMonoisotopic;
            var ppmShift = this.GetPredictedValue(feature.Mz);
            var newMass = mass - (mass * ppmShift) / 1000000;
            feature.MassMonoisotopicAligned = newMass;
            feature.MassMonoisotopic = newMass;
        }

        public void CalibrateFeatures(List<UMCLight> umcs)
        {
            foreach (var feature in umcs)
            {
                this.CalibrateFeature(feature);
            }
        }
    }
}