using System.Collections.Generic;
using MultiAlignCore.Algorithms.Regression;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
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
            RegressionType = LcmsWarpRegressionType.Hybrid;
            _lsqFailed = false;
            _central = new LcmsWarpCentralRegression();
            _lsqReg = new LeastSquaresSplineRegressionModel();
            _cubicSpline = new LcmsNaturalCubicSplineRegression();
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
            _cubicSpline.SetOptions(numKnots);
            _lsqReg.SetOptions(numKnots);
            _central.SetOutlierZScore(outlierZscore);
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
            _central.SetOptions(numXBins, numYBins, numJumps, regZtolerance);
            RegressionType = regType;
        }

        /// <summary>
        /// Sets the regression points to the appropriate values for the regression function
        /// </summary>
        /// <param name="matches"></param>
        public void CalculateRegressionFunction(List<RegressionPoint> matches)
        {
            switch (RegressionType)
            {
                case LcmsWarpRegressionType.Central:
                    _central.CalculateRegressionFunction(matches);
                    break;
                default:
                    _central.CalculateRegressionFunction(matches);
                    _central.RemoveRegressionOutliers();
                    _lsqFailed = !_cubicSpline.CalculateLsqRegressionCoefficients(_central.Points);
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
            switch (RegressionType)
            {
                case LcmsWarpRegressionType.Central:
                    return _central.GetPredictedValue(x);

                default:
                    return !_lsqFailed ? _cubicSpline.GetPredictedValue(x) : _central.GetPredictedValue(x);
            }
        }
    }
}