#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data;
using MathNet.Numerics.LinearAlgebra.Double;

#endregion

namespace FeatureAlignment.Algorithms.Regression
{
    public class LinearEmModel : IRegressorAlgorithm<LinearRegressionResult>
    {
        private DenseMatrix m_x;
        private DenseMatrix m_xTranspose;
        private DenseMatrix m_y;
        private DenseMatrix m_weights;

        private double m_unifU;
        private double m_stdev;
        private const int NUM_ITERATIONS_TO_PERFORM = 20;
        private readonly List<RegressionPoint> m_regressionPoints;
        private readonly LinearRegressionResult m_regressionResult;
        private double m_percentNormal;

        public LinearEmModel()
        {
            m_regressionPoints = new List<RegressionPoint>();
            m_regressionResult = new LinearRegressionResult();
        }



        private void CalculateProbabilitiesAndWeightMatrix()
        {
            var numPoints = m_x.RowCount;
            double normalProbSum = 0;
            double sumSquareDiff = 0;
            double sumSquareDiffForRsq = 0;
            double sumSquareY = 0;
            double sumY = 0;

            for (var index = 0; index < numPoints; index++)
            {
                var diff = m_y.At(index, 0) - (m_regressionResult.Slope * m_x.At(index, 0) + m_regressionResult.Intercept);
                var exponent = Math.Exp(-0.5 * (diff / m_stdev) * (diff / m_stdev));
                var normalizer = m_stdev * Math.Sqrt(2 * Math.PI);
                var probNormalConditional = exponent / normalizer;
                var probNormalPosterior = (m_percentNormal * probNormalConditional) / (m_percentNormal * probNormalConditional + (1 - m_percentNormal) * m_unifU);
                m_weights.At(index, index, probNormalPosterior);
                normalProbSum += probNormalPosterior;
                sumSquareDiff += probNormalPosterior * diff * diff;
                sumSquareDiffForRsq += diff * diff;
                sumSquareY += (m_y.At(index, 0) * m_y.At(index, 0));
                sumY += m_y.At(index, 0);
            }

            m_percentNormal = normalProbSum / numPoints;
            m_stdev = Math.Sqrt(sumSquareDiff / (normalProbSum));
            var yVar = sumSquareY - sumY * sumY / numPoints;
            if (Math.Abs(yVar) < double.Epsilon)
            {
                m_regressionResult.RSquared = 0;
                throw new Exception("All y values are the same");
            }
            m_regressionResult.RSquared = 1 - sumSquareDiffForRsq / yVar;
        }

        private void CalculateSlopeInterceptEstimates()
        {
            var wx = m_weights.Multiply(m_x) as DenseMatrix;

            if (wx == null)
            {
                throw new InvalidOperationException();
            }
            var xprimeWx = m_xTranspose.Multiply(wx) as DenseMatrix;
            if (xprimeWx == null)
            {
                throw new InvalidOperationException();
            }
            var invXprimeWx = xprimeWx.Inverse() as DenseMatrix;
            if (invXprimeWx == null)
            {
                throw new InvalidOperationException();
            }
            var wy = m_weights.Multiply(m_y) as DenseMatrix;
            if (wy == null)
            {
                throw new InvalidOperationException();
            }
            var xprimeWy = m_xTranspose.Multiply(wy) as DenseMatrix;
            if (xprimeWy == null)
            {
                throw new InvalidOperationException();
            }
            var beta = invXprimeWx.Multiply(xprimeWy) as DenseMatrix;
            if (beta == null)
            {
                throw new InvalidOperationException();
            }


            m_regressionResult.Slope = beta.At(0, 0); // Create slope
            m_regressionResult.Intercept = beta.At(1, 0); // Create intercept
            var numPoints = m_x.RowCount;
            var maxDiff = -1 * double.MaxValue;
            var minDiff = double.MaxValue;
            var maxY = -1 * double.MaxValue;

            for (var index = 0; index < numPoints; index++)
            {
                var diff = m_y.At(index, 0) - (m_regressionResult.Slope * m_x.At(index, 0) + m_regressionResult.Intercept);
                if (diff > maxDiff)
                    maxDiff = diff;
                if (diff < minDiff)
                    minDiff = diff;
                if (m_y.At(index, 0) > maxY)
                    maxY = m_y.At(index, 0);
            }
            //_mdblUnifU = 1.0 / (maxDiff - minDiff);
            m_unifU = 1.0 / maxY;
        }

        private void CalculateInitialStdev()
        {
            var numPoints = m_x.RowCount;

            m_stdev = 0;
            for (var index = 0; index < numPoints; index++)
            {
                var diff = m_y.At(index, 0) - (m_regressionResult.Slope * m_x.At(index, 0) + m_regressionResult.Intercept);
                m_stdev += diff * diff;
            }
            m_stdev /= numPoints;
            m_stdev = Math.Sqrt(m_stdev);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetPoints(double[] x, double[] y)
        {
            m_regressionPoints.Clear();
            double minScan = 1024 * 1024 * 16;
            var maxScan = -1 * minScan;

            foreach (var val in x)
            {
                if (val < minScan)
                    minScan = val;
                if (val > maxScan)
                    maxScan = val;
            }

            const double percentToIgnore = 0.0;

            /*Old regression model*/
            // var lowerX = minScan;
            // var upperX = maxScan - (maxScan - minScan) * percentToIgnore;

            /*New regression model*/
            var lowerX = minScan + (minScan + maxScan) * (percentToIgnore / 2.0);
            var upperX = maxScan - (maxScan - minScan) * (percentToIgnore / 2.0);

            for (var ptNum = 0; ptNum < x.Length; ptNum++)
            {
//                if (Math.Abs(x[ptNum] - y[ptNum]) < 0.2)
                {
                    if (x[ptNum] >= lowerX && x[ptNum] <= upperX)
                    {
                        var pt = new RegressionPoint(x[ptNum], y[ptNum]);
                        m_regressionPoints.Add(pt);
                    }

                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="observed"></param>
        /// <param name="predicted"></param>
        /// <returns></returns>
        public LinearRegressionResult CalculateRegression(IEnumerable<double> observed, IEnumerable<double> predicted)
        {

            SetPoints(observed.ToArray(), predicted.ToArray());
            var numPoints = m_regressionPoints.Count;

            m_x = new DenseMatrix(numPoints, 2);
            m_y = new DenseMatrix(numPoints, 1);
            m_weights = new DenseMatrix(numPoints, numPoints);
            m_percentNormal = 0.5;

            // set up X, Y and weights vector.
            double sumX = 0;
            double sumXx = 0;

            for (var index = 0; index < numPoints; index++)
            {
                var currentPoint = m_regressionPoints[index];

                m_x.At(index, 0, currentPoint.X);
                sumX += currentPoint.X;
                sumXx += (currentPoint.X * currentPoint.X);
                m_x.At(index, 1, 1);

                m_y.At(index, 0, currentPoint.Y);
                for (var colNum = 0; colNum < numPoints; colNum++)
                {
                    m_weights.At(index, colNum, 0);
                }
                m_weights.At(index, index, 1);
            }

            if (Math.Abs(sumX * sumX - numPoints * sumXx) < double.Epsilon)
            {
                m_regressionResult.Intercept = 0;
                m_regressionResult.Slope = 0;
                m_regressionResult.RSquared = 0;
                throw new Exception("All X values cannot be same for linear regression");
            }

            m_xTranspose = m_x.Transpose() as DenseMatrix;
            CalculateSlopeInterceptEstimates();
            CalculateInitialStdev();
            CalculateProbabilitiesAndWeightMatrix();

            for (var iterationNum = 0; iterationNum < NUM_ITERATIONS_TO_PERFORM; iterationNum++)
            {
                CalculateSlopeInterceptEstimates();
                CalculateProbabilitiesAndWeightMatrix();
                if (m_percentNormal < 0.01)
                {
                    throw new Exception("PercentNormal < 0.01");
                }
            }

            return m_regressionResult;
        }

        /// <summary>
        /// Transforms the observed value based on the regression function's slope and intercept.
        /// </summary>
        /// <param name="regressionFunction"></param>
        /// <param name="observed"></param>
        /// <returns></returns>
        public double Transform(LinearRegressionResult regressionFunction, double observed)
        {
            return (regressionFunction.Slope * observed + regressionFunction.Intercept);
        }
    }
}
