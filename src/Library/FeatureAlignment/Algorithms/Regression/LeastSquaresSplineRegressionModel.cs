using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data;
using MathNet.Numerics.LinearAlgebra.Double;

namespace FeatureAlignment.Algorithms.Regression
{
    /// <summary>
    /// Object to hold the necesary information for an LSQ regression for LCMSWarp
    /// </summary>
    public sealed class LeastSquaresSplineRegressionModel
    {
        readonly List<RegressionPoint> m_points;
        readonly double[] m_coeffs = new double[512];
        private const int MAX_ORDER = 16; // Maximum order of spline regression supported

        int m_order;
        int m_numKnots;
        double m_minX;
        double m_maxX;

        /// <summary>
        /// Cleans any remaining data from previous regression
        /// </summary>
        private void Clear()
        {
            m_points.Clear();
        }

        /// <summary>
        /// Constructor for an LSQSplineRegressor. Initializes number of knots, order and sets up
        /// new memory space for the regression points
        /// </summary>
        public LeastSquaresSplineRegressionModel()
        {
            m_numKnots = 0;
            m_order = 1;
            m_points = new List<RegressionPoint>();
        }

        /// <summary>
        /// Copies the number of knots to internal for LSQ regression
        /// </summary>
        /// <param name="numKnots"></param>
        public void SetOptions(int numKnots)
        {
            m_numKnots = numKnots;
        }

        /// <summary>
        /// Computes the Regressor coefficients based on the order of the LSQ and the points to regress
        /// </summary>
        /// <param name="order"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool CalculateLsqRegressionCoefficients(int order, ref List<RegressionPoint> points)
        {
            Clear();
            m_order = order;

            if (order > MAX_ORDER)
            {
                m_order = MAX_ORDER;
            }

            m_minX = double.MaxValue;
            m_maxX = double.MinValue;
            m_minX = points.Min(x => x.X);
            m_maxX = points.Max(x => x.X);
            m_points.AddRange(points);

            var numPoints = m_points.Count;

            var a = new DenseMatrix(numPoints, m_order + m_numKnots + 1);
            var b = new DenseMatrix(numPoints, 1);

            for (var pointNum = 0; pointNum < numPoints; pointNum++)
            {
                var calib = m_points[pointNum];
                double coeff = 1;
                a[pointNum, 0] = coeff;
                for (var colNum = 1; colNum <= m_order; colNum++)
                {
                    coeff = coeff * calib.X;
                    a[pointNum, colNum] = coeff;
                }

                if (m_numKnots > 0 && m_order > 0)
                {
                    var xInterval = (int)Math.Floor(((m_numKnots + 1) * (calib.X - m_minX)) / (m_maxX - m_minX));
                    if (xInterval >= m_numKnots + 1)
                    {
                        xInterval = m_numKnots;
                    }

                    for (var colNum = m_order + 1; colNum <= m_order + xInterval; colNum++)
                    {
                        var xIntervalStart = m_minX + ((colNum - m_order) * (m_maxX - m_minX)) / (m_numKnots + 1);
                        a[pointNum, colNum] = Math.Pow(calib.X - xIntervalStart, m_order);
                    }
                    for (var colNum = m_order + xInterval + 1; colNum <= m_order + m_numKnots; colNum++)
                    {
                        a[pointNum, colNum] = 0;
                    }
                }

                b[pointNum, 0] = calib.MassError;
            }

            var aTrans = (DenseMatrix)a.Transpose();
            var aTransA = (DenseMatrix)aTrans.Multiply(a);

            // Can't invert a matrix with a determinant of 0, if so return false
            if (Math.Abs(aTransA.Determinant()) < double.Epsilon)
            {
                return false;
            }

            var invATransA = (DenseMatrix)aTransA.Inverse();

            var c = (DenseMatrix)invATransA.Multiply(b);

            for (var colNum = 0; colNum <= m_order + m_numKnots; colNum++)
            {
                m_coeffs[colNum] = c[colNum, 0];
            }

            return true;
        }

        /// <summary>
        /// Given a value "x", returns where on the regression line the appropriate "y" would fall
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetPredictedValue(double x)
        {
            double powerN = 1;
            var val = m_coeffs[0];

            for (var power = 1; power <= m_order; power++)
            {
                powerN = powerN * x;
                val = val + m_coeffs[power] * powerN;
            }

            if (m_numKnots > 0 && m_order > 0)
            {
                var xInterval = (int)Math.Floor(((m_numKnots + 1) * (x - m_minX)) / (m_maxX - m_minX));
                if (xInterval >= m_numKnots + 1)
                {
                    xInterval = m_numKnots;
                }

                for (var colNum = m_order + 1; colNum <= m_order + xInterval; colNum++)
                {
                    var xIntervalStart = m_minX + ((colNum - m_order) * (m_maxX - m_minX)) / (m_numKnots + 1);
                    val = val + Math.Pow(x - xIntervalStart, m_order) * m_coeffs[colNum];
                }
            }

            return val;
        }
    }
}
