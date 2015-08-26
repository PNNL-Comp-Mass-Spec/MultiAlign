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
        private LcmsWarpRegressionType m_regressionType;
        private bool m_lsqFailed;
        readonly LcmsWarpCentralRegression m_central;
        readonly LeastSquaresSplineRegressionModel m_lsqReg;
        readonly LcmsNaturalCubicSplineRegression m_cubicSpline;

        /// <summary>
        /// Public constructor for a Hybrid regression
        /// </summary>
        public LcmsWarpCombinedRegression()
        {
            m_regressionType    = LcmsWarpRegressionType.Hybrid;
            m_lsqFailed         = false;
            m_central           = new LcmsWarpCentralRegression();
            m_lsqReg            = new LeastSquaresSplineRegressionModel();
            m_cubicSpline       = new LcmsNaturalCubicSplineRegression();
        }

        public LcmsWarpRegressionType RegressionType
        {
            get { return m_regressionType; }
            set { m_regressionType = value; }
        }
        /// <summary>
        /// Sets the options for all three regression types, setting up the number of knots for
        /// cubic spline and LSQ while setting the outlier z score for central regression
        /// </summary>
        /// <param name="numKnots"></param>
        /// <param name="outlierZscore"></param>
        public void SetLsqOptions(int numKnots, double outlierZscore)
        {
            m_cubicSpline.SetOptions(numKnots);
            m_lsqReg.SetOptions(numKnots);
            m_central.SetOutlierZScore(outlierZscore);
        }

        /// <summary>
        /// Sets all the options for a central regression type
        /// </summary>
        /// <param name="numXBins"></param>
        /// <param name="numYBins"></param>
        /// <param name="numJumps"></param>
        /// <param name="regZtolerance"></param>
        /// <param name="regType"></param>
        public void SetCentralRegressionOptions(int numXBins, int numYBins, int numJumps, double regZtolerance, LcmsWarpRegressionType regType)
        {
            m_central.SetOptions(numXBins, numYBins, numJumps, regZtolerance);
            m_regressionType = regType;
        }

        /// <summary>
        /// Sets the regression points to the appropriate values for the regression function
        /// </summary>
        /// <param name="matches"></param>
        public void CalculateRegressionFunction(ref List<RegressionPoint> matches)
        {
            switch (m_regressionType)
            {
                case LcmsWarpRegressionType.Central:
                    m_central.CalculateRegressionFunction(ref matches);                    
                    break;
                default:
                    m_central.CalculateRegressionFunction(ref matches);
                    m_central.RemoveRegressionOutliers();
                    var centralPoints = m_central.Points;
                    m_lsqFailed = !m_cubicSpline.CalculateLsqRegressionCoefficients(centralPoints);                    
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
            switch (m_regressionType)
            {
                case LcmsWarpRegressionType.Central:
                    return m_central.GetPredictedValue(x);
                
                default:
                    return !m_lsqFailed ? m_cubicSpline.GetPredictedValue(x) : m_central.GetPredictedValue(x);
            }
        }

    }
}
