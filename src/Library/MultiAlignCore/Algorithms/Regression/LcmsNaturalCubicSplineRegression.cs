using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Regression
{
    /// <summary>
    /// Object which contains all the information necesary for a natural cubic spline regression
    /// for LCMS Warp
    /// </summary>
    public class LcmsNaturalCubicSplineRegression
    {
        readonly List<RegressionPoint> m_pts;
        readonly List<double> m_intervalStart;

        readonly double[] m_coeffs = new double[512];

        int m_numKnots;
        double m_minX;
        double m_maxX;

        /// <summary>
        /// Default constructor for Cubic Spline regression, initializing the number of knots
        /// to 2 and then allocating initial memory for the points and intervals
        /// </summary>
        public LcmsNaturalCubicSplineRegression()
        {
            m_numKnots = 2;
            m_pts = new List<RegressionPoint>();
            m_intervalStart = new List<double>();
        }


        private void Clear()
        {
            m_pts.Clear();
            m_intervalStart.Clear();
        }

        /// <summary>
        /// Sets the parameter for the number of knots for the cubic spline regression
        /// </summary>
        /// <param name="numKnots"></param>
        public void SetOptions(int numKnots)
        {
            m_numKnots = numKnots;
        }

        private void PreprocessCopyData(IEnumerable<RegressionPoint> points)
        {
            m_minX = double.MaxValue;
            m_maxX = double.MinValue;
            foreach (var point in points)
            {
                if (point.X < m_minX)
                {
                    m_minX = point.X;
                }
                if (point.X > m_maxX)
                {
                    m_maxX = point.X;
                }
                m_pts.Add(point);
            }

            for (var i = 0; i <= m_numKnots; i++)
            {
                var val = (i * (m_maxX - m_minX)) / (m_numKnots + 1) + m_minX;
                m_intervalStart.Add(val);
            }
        }

        /// <summary>
        /// input points are [x, y], order specifies order of the regression line
        /// Returns false if the number of knots is less than 2, if there are no points
        /// to perform regression on or if the matrix of regression points can't be inverted
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool CalculateLsqRegressionCoefficients(List<RegressionPoint> points)
        {
            Clear();
            if (m_numKnots < 2)
            {
                // Needs at least two knots for a natural cubic spline
                return false;
            }

            if (points.Count == 0)
            {
                // Needs at least a single point for the coefficient
                return false;
            }

            PreprocessCopyData(points);

            var numPts = m_pts.Count;

            var a = new DenseMatrix(numPts, m_numKnots);
            var b = new DenseMatrix(numPts, 1);

            var intervalWidth = (m_maxX - m_minX) / (m_numKnots + 1);

            for (var pointNum = 0; pointNum < numPts; pointNum++)
            {
                var point = m_pts[pointNum];
                a[pointNum, 0] = 1.0;
                a[pointNum, 1] = point.X;

                double kMinus1 = 0;
                if (point.X > m_intervalStart[m_numKnots - 1])
                {
                    kMinus1 = Math.Pow(point.X - m_intervalStart[m_numKnots], 3);
                    if (point.X > m_intervalStart[m_numKnots])
                    {
                        kMinus1 = kMinus1 - Math.Pow(point.X - m_intervalStart[m_numKnots], 3);
                    }
                    kMinus1 = kMinus1 / intervalWidth;
                }

                for (var k = 1; k <= m_numKnots - 2; k++)
                {
                    double kminus1 = 0;

                    if (point.X > m_intervalStart[k])
                    {
                        kminus1 = Math.Pow(point.X - m_intervalStart[k], 3);
                        if (point.X > m_intervalStart[m_numKnots])
                        {
                            kminus1 = kminus1 - Math.Pow(point.X - m_intervalStart[m_numKnots], 3);
                        }
                        kminus1 = kminus1 / intervalWidth;
                    }

                    a[pointNum, k + 1] = kminus1 - kMinus1;
                }

                b[pointNum, 0] = point.MassError;
            }

            var aTrans = (DenseMatrix)a.Transpose();
            var aTransA = (DenseMatrix)a.Multiply(aTrans);

            // Can't invert a matrix with a determinant of 0.
            if (Math.Abs(aTransA.Determinant()) < double.Epsilon)
            {
                return false;
            }

            var invATransA = (DenseMatrix)aTrans.Inverse();
            var invATransAaTrans = (DenseMatrix)invATransA.Multiply(aTrans);

            var c = (DenseMatrix)invATransAaTrans.Multiply(b);

            for (var colNum = 0; colNum < m_numKnots; colNum++)
            {
                m_coeffs[colNum] = c[colNum, 0];
            }

            return true;
        }

        /// <summary>
        /// Given a value "x", returns the predicted y value that corresponds to it on the regression line
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetPredictedValue(double x)
        {
            if (m_pts.Count == 0)
            {
                return 0;
            }

            if (x <= m_minX)
            {
                return m_coeffs[0] + m_coeffs[1] * m_minX;
            }

            if (x >= m_maxX)
            {
                x = m_maxX;
            }

            var intervalWidth = (m_maxX - m_minX) / (m_numKnots + 1);

            var val = m_coeffs[0] + m_coeffs[1] * x;

            double kMinus1 = 0;
            if (x > m_intervalStart[m_numKnots - 1])
            {
                kMinus1 = Math.Pow(x - m_intervalStart[m_numKnots - 1], 3);
                if (x > m_intervalStart[m_numKnots])
                {
                    kMinus1 = kMinus1 - Math.Pow(x - m_intervalStart[m_numKnots], 3);
                }
                kMinus1 = kMinus1 / intervalWidth;
            }

            for (var k = 1; k <= m_numKnots - 2; k++)
            {
                double kminus1 = 0;
                if (x > m_intervalStart[k])
                {
                    kminus1 = Math.Pow(x - m_intervalStart[k], 3);
                    if (x > m_intervalStart[m_numKnots])
                    {
                        kminus1 = kminus1 - Math.Pow(x - m_intervalStart[m_numKnots], 3);
                    }
                }
                val = val + (kminus1 - kMinus1) * m_coeffs[k + 1];
            }

            return val;
        }
    }
}
