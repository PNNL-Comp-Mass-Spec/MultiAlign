#region Namespaces

using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MultiAlignCore.Data;

#endregion

namespace MultiAlignCore.Algorithms.Regression
{
    // TODO: Double check all calculations.  Doesn't calculate correctly.
    public sealed class MixtureModelEm : IRegressorAlgorithm<LinearRegressionResult>
    {
        private double m_normalizingSlopeX;
        private double m_normalizingSlopeY;
        private double m_normalizingInterceptX;
        private double m_normalizingInterceptY;

        private readonly double m_minPercentageChangeToContinue;
        private readonly bool m_normalizeInput;

        private DenseMatrix m_x;
        private DenseMatrix m_yRegressed;
        private DenseMatrix m_y;
        private DenseMatrix m_likelihoods;
        private DenseMatrix m_coefficients;

        private readonly List<Double> m_stdevReal;
        private readonly List<Double> m_stdevNoise;
        private readonly List<Double> m_meanNoise;
        private readonly List<Double> m_probReal;
        private readonly List<Double> m_likelihoodsList;
        private readonly List<RegressionPoint> m_regressionPoints;
        private readonly LinearRegressionResult m_regressionResult;

        private readonly short m_order;
        private const int MAX_ITERATIONS_TO_PERFORM = 250;

        private void CalculateCoefficient(short coeffNum, out double coeff)
        {
            var numPoints = m_x.RowCount;
            double numerator = 0;
            double denominator = 0;

            for (var index = 0; index < numPoints; index++)
            {
                var diff = m_y.At(index, 0) - m_yRegressed.At(index, 0);
                var missingX = m_x.At(index, coeffNum);
                var missingTerm = missingX*m_coefficients.At(coeffNum, 0);
                diff += missingTerm;

                var likelihoodReal = m_likelihoods.At(index, 0);
                var likelihoodNoise = m_likelihoods.At(index, 1);
                var probability = likelihoodReal/(likelihoodNoise + likelihoodReal);

                numerator += diff*missingX*probability;
                denominator += probability*missingX*missingX;
            }
            coeff = numerator/denominator;
        }

        private void CalculateNextPredictions()
        {
            m_yRegressed = m_x.Multiply(m_coefficients) as DenseMatrix;
        }

        private void SetupMatrices()
        {
            var numPoints = m_regressionPoints.Count;
            m_x = new DenseMatrix(numPoints, m_order + 1);
            m_y = new DenseMatrix(numPoints, 1);
            m_yRegressed = new DenseMatrix(numPoints, 1);
            m_likelihoods = new DenseMatrix(numPoints, 2);
            m_coefficients = new DenseMatrix(m_order + 1, 1);

            // set up X, Y and weights vector.
            for (var index = 0; index < numPoints; index++)
            {
                var currentPoint = m_regressionPoints[index];
                double val = 1;
                for (var coeffNum = 0; coeffNum < m_order + 1; coeffNum++)
                {
                    m_x.At(index, coeffNum, val);
                    val *= currentPoint.X;
                }
                m_y.At(index, 0, currentPoint.Y);
            }
        }

        private double CalculateLikelihoodsMatrix(int iterationNum)
        {
            var numPoints = m_x.RowCount;
            var noiseSigma = m_stdevNoise[iterationNum];
            var noiseMean = m_meanNoise[iterationNum];
            var realSigma = m_stdevReal[iterationNum];
            var probabilityReal = m_probReal[iterationNum];

            double logLikelihood = 0;
            for (var index = 0; index < numPoints; index++)
            {
                var diff = m_x.At(index, 0) - m_yRegressed.At(index, 0);
                var standardizedNoise = (diff - noiseMean)/noiseSigma;
                var standardizedReal = diff/realSigma;

                var likelihoodReal = probabilityReal*Math.Exp(-1*standardizedReal*standardizedReal)/
                                        (Math.Sqrt(2*Math.PI)*realSigma);
                var likelihoodNoise = (1 - probabilityReal)*Math.Exp(-1*standardizedNoise*standardizedNoise)/
                                         ((Math.Sqrt(2*Math.PI))*noiseSigma);

                m_likelihoods.At(index, 0, likelihoodReal);
                m_likelihoods.At(index, 1, likelihoodNoise);
                logLikelihood += Math.Log(likelihoodReal + likelihoodNoise);
            }
            m_likelihoodsList.Add(logLikelihood);
            return logLikelihood;
        }

        private void CalculateRealStdev(out double stdev)
        {
            var numPoints = m_x.RowCount;

            double sumSigmaSqW = 0;
            double sumW = 0;

            for (var index = 0; index < numPoints; index++)
            {
                var diff = m_y.At(index, 0) - m_yRegressed.At(index, 0);
                var likelihoodReal = m_likelihoods.At(index, 0);
                var likelihoodNoise = m_likelihoods.At(index, 1);
                var probability = likelihoodReal/(likelihoodNoise + likelihoodReal);
                sumSigmaSqW += probability*diff*diff;
                sumW += probability;
            }
            stdev = Math.Sqrt(sumSigmaSqW/sumW);
        }

        private void CalculateNoiseStdevAndMean(int iterationNum, out double stdev, out double mean)
        {
            var numPoints = m_x.RowCount;
            double sumSigmaSqW = 0;
            double sumW = 0;
            double sumWNoise = 0;
            var noiseMean = m_meanNoise[iterationNum];

            for (var index = 0; index < numPoints; index++)
            {
                var y = m_y.At(index, 0);
                var likelihoodReal = m_likelihoods.At(index, 0);
                var likelihoodNoise = m_likelihoods.At(index, 1);
                var probability = likelihoodNoise/(likelihoodNoise + likelihoodReal);

                sumSigmaSqW += probability*(y - noiseMean)*(y - noiseMean);
                sumWNoise += y*probability;
                sumW += probability;
            }
            mean = sumWNoise/sumW;
            stdev = Math.Sqrt(sumSigmaSqW/sumW);
        }

        private void CalculateRealProbability(out double probability)
        {
            var numPoints = m_x.RowCount;
            double normalProbSum = 0;

            for (var index = 0; index < numPoints; index++)
            {
                var likelihoodReal = m_likelihoods.At(index, 0);
                var likelihoodNoise = m_likelihoods.At(index, 1);
                normalProbSum += likelihoodReal/(likelihoodNoise + likelihoodReal);
            }
            normalProbSum /= numPoints;
            probability = normalProbSum;
        }

        private void NormalizeXAndYs()
        {
            var numPoints = m_regressionPoints.Count;
            if (numPoints == 0)
                throw new InvalidOperationException("Zero elements passed to NormalizeXAndYs");

            var vectXs = new List<double>();
            var vectYs = new List<double>();
            vectXs.Capacity = numPoints;
            vectYs.Capacity = numPoints;

            for (var index = 0; index < numPoints; index++)
            {
                var currentPoint = m_regressionPoints[index];
                vectXs.Add(currentPoint.X);
                vectYs.Add(currentPoint.Y);
            }

            vectXs.Sort();
            vectYs.Sort();

            int medianIndex;
            if (numPoints%2 == 0)
            {
                medianIndex = (numPoints - 1)/2;
            }
            else
            {
                medianIndex = numPoints/2;
            }

            var xMedian = vectXs[medianIndex];
            var yMedian = vectYs[medianIndex];
            var xMin = vectXs[0];
            var xMax = vectXs[numPoints - 1];
            var yMin = vectYs[0];
            var yMax = vectYs[numPoints - 1];

            m_normalizingInterceptX = -xMedian/(xMax - xMin);
            m_normalizingSlopeX = 1/(xMax - xMin);

            m_normalizingInterceptY = -yMedian/(yMax - yMin);
            m_normalizingSlopeY = 1/(yMax - yMin);

            for (var index = 0; index < numPoints; index++)
            {
                m_regressionPoints[index].X = m_regressionPoints[index].X*m_normalizingSlopeX +
                                                  m_normalizingInterceptX;
                m_regressionPoints[index].Y = m_regressionPoints[index].Y*m_normalizingSlopeY +
                                                  m_normalizingInterceptY;
            }
        }


        /// <summary>
        ///
        /// </summary>
        public MixtureModelEm()
        {
            m_regressionPoints = new List<RegressionPoint>();
            m_regressionResult = new LinearRegressionResult();

            m_stdevReal = new List<double>();
            m_stdevNoise = new List<double>();
            m_meanNoise = new List<double>();
            m_probReal = new List<double>();
            m_likelihoodsList = new List<double>();

            m_x = null;
            m_y = null;
            m_coefficients = null;
            m_likelihoods = null;
            m_yRegressed = null;

            m_order = 1;
            m_normalizeInput = true;
            m_minPercentageChangeToContinue = 0.0001;
        }

        public LinearRegressionResult CalculateRegression(IEnumerable<double> observed, IEnumerable<double> predicted)
        {
            SetPoints(observed as List<double>, predicted as List<double>);

            m_regressionPoints.Sort();

            if (m_normalizeInput)
                NormalizeXAndYs();

            SetupMatrices();
            var numPts = m_regressionPoints.Count;

            // CALCULATE FIRST SET OF COEFFICIENTS HERE ..
            // Remember that the coefficients should be:
            // (XX')-1X
            var xTranspose = m_x.Transpose() as DenseMatrix;
            if (xTranspose != null)
            {
                var xprimeX = xTranspose.Multiply(m_x) as DenseMatrix;

                if (xprimeX == null)
                {
                    throw new InvalidCastException(
                        "Unable to multiply mobj_X with XTranspose in clsMixtureModelRegressionEM::CalculateRegressionFunction");
                }

                var invXprimeX = xprimeX.Inverse() as DenseMatrix;

                if (invXprimeX == null)
                {
                    throw new InvalidCastException(
                        "Unable to invert xprime_x in clsMixtureModelRegressionEM::CalculateRegressionFunction");
                }

                var invXprimeXxTrans = invXprimeX.Multiply(xTranspose) as DenseMatrix;

                if (invXprimeXxTrans == null)
                {
                    throw new InvalidCastException(
                        "to multiple inv_xprime_wx and mobj_X in clsMixtureModelRegressionEM::CalculateRegressionFunction");
                }

                m_coefficients = invXprimeXxTrans.Multiply(m_y) as DenseMatrix;
            }

            if (m_coefficients == null)
            {
                throw new InvalidOperationException(
                    "Unable to multiple inv_xprime_x and mobj_X in clsMixtureModelRegressionEM::CalculateRegressionFunction");
            }

            var nextCoefficients = new DenseMatrix(m_order + 1, 1);
            var bestCoefficients = new DenseMatrix(m_order + 1, 1);

            m_likelihoodsList.Clear();
            m_meanNoise.Clear();
            m_probReal.Clear();
            m_stdevNoise.Clear();
            m_stdevReal.Clear();

            double sumY = 0;
            double sumYy = 0;
            for (var ptNum = 0; ptNum < numPts; ptNum++)
            {
                var y = m_regressionPoints[ptNum].Y;
                sumY += y;
                sumYy += y*y;
            }
            var noiseMean = sumY/numPts;
            var noiseStdev = Math.Sqrt((sumYy - sumY*sumY/numPts)/numPts);

            m_meanNoise.Add(noiseMean);
            m_stdevNoise.Add(noiseStdev);
            m_stdevReal.Add(noiseStdev/10);
            m_probReal.Add(0.5);

            var maxLikelihood = -1*double.MaxValue;
            for (var iterationNum = 0; iterationNum < MAX_ITERATIONS_TO_PERFORM; iterationNum++)
            {
                CalculateNextPredictions();
                var likelihood = CalculateLikelihoodsMatrix(iterationNum);
                if (likelihood > maxLikelihood)
                {
                    maxLikelihood = likelihood;
                    // set the best set of coefficients.
                    for (short coefficientNum = 0; coefficientNum < m_order + 1; coefficientNum++)
                        bestCoefficients.At(coefficientNum, 0, m_coefficients.At(coefficientNum, 0));
                }

                double probabilityReal;
                CalculateRealProbability(out probabilityReal);
                m_probReal.Add(probabilityReal);

                double stdevReal;
                CalculateRealStdev(out stdevReal);
                m_stdevReal.Add(stdevReal);

                double meanNoise, stdevNoise;
                CalculateNoiseStdevAndMean(iterationNum, out stdevNoise, out meanNoise);
                m_stdevNoise.Add(stdevNoise);
                m_meanNoise.Add(meanNoise);

                for (short coefficientNum = 0; coefficientNum < m_order + 1; coefficientNum++)
                {
                    double coefficient;
                    CalculateCoefficient(coefficientNum, out coefficient);
                    nextCoefficients.At(coefficientNum, 0, coefficient);
                }

                // set the next set of coefficient.
                for (short coefficientNum = 0; coefficientNum < m_order + 1; coefficientNum++)
                    m_coefficients.At(coefficientNum, 0, nextCoefficients.At(coefficientNum, 0));
                if (iterationNum <= 0)
                    continue;

                var likelihoodChange =
                    Math.Abs(m_likelihoodsList[iterationNum] - m_likelihoodsList[iterationNum - 1]);
                var minLikelihoodChange =
                    Math.Abs(m_minPercentageChangeToContinue*m_likelihoodsList[iterationNum - 1]);
                if (likelihoodChange < minLikelihoodChange)
                    break;
            }

            for (short coefficientNum = 0; coefficientNum < m_order + 1; coefficientNum++)
                m_coefficients.At(coefficientNum, 0, bestCoefficients.At(coefficientNum, 0));

            // Calculate Slope
            m_regressionResult.Slope = m_coefficients.At(1, 0);
            if (m_normalizeInput)
            {
                m_regressionResult.Slope = m_regressionResult.Slope*m_normalizingSlopeX/m_normalizingSlopeY;
            }

            // Calculate Intercept
            m_regressionResult.Intercept = m_coefficients.At(0, 0);
            if (!m_normalizeInput) return m_regressionResult;
            var slope = m_coefficients.At(1, 0);
            m_regressionResult.Intercept = (m_regressionResult.Intercept - m_normalizingInterceptY +
                                            slope*m_normalizingInterceptX)/m_normalizingSlopeY;
            return m_regressionResult;
        }

        private void SetPoints(List<double> x, List<double> y)
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

            const double percentToIgnore = 0.4;
            var lowerX = minScan + (maxScan - minScan) * (percentToIgnore / 2.0);
            var upperX = maxScan - (maxScan - minScan) * (percentToIgnore / 2.0);

            for (var ptNum = 0; ptNum < x.Count; ptNum++)
            {
                var pt = new RegressionPoint(x[ptNum], y[ptNum]);

                if (pt.X > lowerX && pt.X < upperX)
                    m_regressionPoints.Add(pt);
            }
        }

        public double Transform(LinearRegressionResult regressionFunction, double observed)
        {
            return (regressionFunction.Slope * observed) + regressionFunction.Intercept;
        }

    }
}