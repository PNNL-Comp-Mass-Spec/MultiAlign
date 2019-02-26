using System;
using System.Collections.Generic;

namespace FeatureAlignment.Algorithms.Statistics
{
    public class TwoDimensionEM
    {
        /// <summary>
        /// Two dimensional expectation maximization
        /// </summary>
        /// <param name="x">Difference between alignee feature and baseline feature, X value (e.g. mass)</param>
        /// <param name="y">Difference between alignee feature and baseline feature, Y value (e.g. NET)</param>
        /// <param name="p">Probabability of belonging to the normal distribution (output)</param>
        /// <param name="u">Probability density of false hits (output)</param>
        /// <param name="muX">Mean of X values (output)</param>
        /// <param name="muY">Mean of Y values (output)</param>
        /// <param name="stdX">Standard deviation of X values (output)</param>
        /// <param name="stdY">Standard deviation of Y values (output)</param>
        /// <remarks>Used by LCMSWarp for computation of likelihood</remarks>
        public static void TwoDem(List<double> x, List<double> y, out double p, out double u, out double muX,
                           out double muY, out double stdX, out double stdY)
        {
            const int numIterations = 40;
            var numPoints = x.Count;
            var pVals = new double[2, numPoints];

            double minX = x[0], maxX = x[0];
            double minY = y[0], maxY = y[0];

            for (var pointNumber = 0; pointNumber < numPoints; pointNumber++)
            {
                if (x[pointNumber] < minX)
                {
                    minX = x[pointNumber];
                }
                if (x[pointNumber] > maxX)
                {
                    maxX = x[pointNumber];
                }
                if (y[pointNumber] < minY)
                {
                    minY = y[pointNumber];
                }
                if (y[pointNumber] > maxY)
                {
                    maxY = y[pointNumber];
                }
            }

            u = 1.0 / ((maxX - minX) * (maxY - minY));
            p = 0.5;

            CalcMeanAndStd(x, out muX, out stdX);
            stdX = stdX / 3.0;
            CalcMeanAndStd(y, out muY, out stdY);
            stdY = stdY / 3.0;

            for (var iterNum = 0; iterNum < numIterations; iterNum++)
            {
                // Calculate current probability assignments
                // (expectation step)
                for (var pointNum = 0; pointNum < numPoints; pointNum++)
                {
                    var xDiff = (x[pointNum] - muX) / stdX;
                    var yDiff = (y[pointNum] - muY) / stdY;
                    pVals[0, pointNum] = p * Math.Exp(-0.5 * (xDiff * xDiff + yDiff * yDiff)) / (2 * Math.PI * stdX * stdY);
                    pVals[1, pointNum] = (1 - p) * u;
                    var sum = pVals[0, pointNum] + pVals[1, pointNum];
                    pVals[0, pointNum] = pVals[0, pointNum] / sum;
                    pVals[1, pointNum] = pVals[1, pointNum] / sum;
                }

                // Calculate new estimates from maximization step
                // (maximization step)
                double pNumerator = 0;
                double muXNumerator = 0;
                double muYNumerator = 0;
                double sigmaXNumerator = 0;
                double sigmaYNumerator = 0;

                double pDenominator = 0;
                double denominator = 0;

                for (var pointNum = 0; pointNum < numPoints; pointNum++)
                {
                    pNumerator = pNumerator + pVals[0, pointNum];
                    pDenominator = pDenominator + (pVals[0, pointNum] + pVals[1, pointNum]);

                    var xDiff = (x[pointNum] - muX);
                    muXNumerator = muXNumerator + pVals[0, pointNum] * x[pointNum];
                    sigmaXNumerator = sigmaXNumerator + pVals[0, pointNum] * xDiff * xDiff;

                    var yDiff = (y[pointNum] - muY);
                    muYNumerator = muYNumerator + pVals[0, pointNum] * y[pointNum];
                    sigmaYNumerator = sigmaYNumerator + pVals[0, pointNum] * yDiff * yDiff;

                    denominator = denominator + pVals[0, pointNum];
                }

                muX = muXNumerator / denominator;
                muY = muYNumerator / denominator;
                stdX = Math.Sqrt(sigmaXNumerator / denominator);
                stdY = Math.Sqrt(sigmaYNumerator / denominator);
                p = pNumerator / pDenominator;
            }
        }

        public static void CalcMeanAndStd(List<double> values, out double mean, out double stdev)
        {
            var numPoints = values.Count;
            double sumSquare = 0;
            double sum = 0;
            for (var pointNum = 0; pointNum < numPoints; pointNum++)
            {
                var val = values[pointNum];
                sum = sum + val;
                sumSquare = sumSquare + (val * val);
            }
            mean = sum / numPoints;
            stdev = Math.Sqrt((numPoints * sumSquare - sum * sum)) / (Math.Sqrt(numPoints) * Math.Sqrt(numPoints - 1));
        }
    }
}
