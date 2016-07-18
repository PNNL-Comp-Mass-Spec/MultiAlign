using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using PNNLOmics.Utilities;

namespace MultiAlignCore.Algorithms.FeatureMatcher.Utilities
{
    public static class ExpectationMaximization
    {
        # region Members
        const double EPSILON = 0.0001;
        const int MAX_ITERATIONS = 500;
        #endregion


        # region Update normal functions
        /// <summary>
        /// Update mean vector of normal distribution.  Used for Normal-Uniform mixture.
        /// </summary>
        /// <param name="dataList">List of difference matrices.</param>
        /// <param name="alphaList">List of mixture estimates corresponding to differences.</param>
        /// <returns>Updated DenseMatrix containing mean of normal distribution.</returns>
        public static DenseMatrix UpdateNormalMeanVector(List<DenseMatrix> dataList, List<double> alphaList)
        {
            var numerator = new DenseMatrix(dataList[0].RowCount, 1);
            var denominator = 0.0;

            for (var i = 0; i < dataList.Count; i++)
            {
                numerator += alphaList[i] * dataList[i];
                denominator += alphaList[i];
            }

            return ((1 / denominator) * numerator);
        }
        /// <summary>
        /// Update mean vector of normal distribution.  Used for Normal-Normal-Uniform mixture.
        /// </summary>
        /// <param name="dataList">List of difference matrices.</param>
        /// <param name="alphaList">List of mixture estimates corresponding to differences.</param>
        /// <param name="priorList">List of prior probabilities corresponding to differences.</param>
        /// <param name="secondNormal">Whether the data is from the second of the normal distributions, i.e. incorrect in AMT database.</param>
        /// <returns>Updated DenseMatrix containing mean of normal distribution.</returns>
        public static DenseMatrix UpdateNormalMeanVector(List<DenseMatrix> dataList, List<double> alphaList, List<double> priorList, bool secondNormal)
        {
            var numerator = new DenseMatrix(dataList[0].RowCount, 1);
            var denominator = 0.0;

            var multiplier = 1.0;
            double adder = 0;

            if (secondNormal)
            {
                multiplier = -1.0;
                adder = 1.0;
            }

            for (var i = 0; i < dataList.Count; i++)
            {
                var weight = alphaList[i] * (adder + multiplier * priorList[i]);
                numerator += weight * dataList[i];
                denominator += weight;
            }

            return ((1 / denominator) * numerator);
        }
        /// <summary>
        /// Update covariance matrix of normal distribution.  Used for Normal-Uniform mixture.
        /// </summary>
        /// <param name="dataList">List of difference matrices.</param>
        /// <param name="meanVector">Matrix containing mean parameters for the normal distribution.</param>
        /// <param name="alphaList">List of mixture estimates corresponding to differences</param>
        /// <param name="independent">Whether the dimensions of the normal distribution should be considered normal.  Returns a diagonal DenseMatrix if true.  Should use false if unknown.</param>
        /// <returns>Updated DenseMatrix containing covariances of normal distribution.</returns>
        public static DenseMatrix UpdateNormalCovarianceMatrix(List<DenseMatrix> dataList, DenseMatrix meanVector, List<double> alphaList, bool independent)
        {
            var numerator = new DenseMatrix(meanVector.RowCount, meanVector.RowCount);
            var denominator = 0.0;

            for (var i = 0; i < dataList.Count; i++)
            {
                var dataT = dataList[i].Transpose() as DenseMatrix;

                numerator += alphaList[i] * dataList[i] * dataT;
                denominator += alphaList[i];
            }

            var covarianceMatrix = (1 / denominator) * numerator;

            if (independent)
            {
                var indCovarianceMatrix = new DenseMatrix(covarianceMatrix.RowCount, covarianceMatrix.ColumnCount);
                for (var i = 0; i < covarianceMatrix.ColumnCount; i++)
                {
                    indCovarianceMatrix[i, i] = covarianceMatrix[i, i];
                }
                covarianceMatrix = indCovarianceMatrix.Clone() as DenseMatrix;
            }

            for (var i = 0; i < meanVector.RowCount; i++)
            {
                if (covarianceMatrix[i, i] < 0.001)
                {
                    covarianceMatrix[i, i] = 0.001;
                }
            }

            return (covarianceMatrix);
        }
        /// <summary>
        /// Update covariance matrix of normal distribution.  Used for Normal-Normal-Uniform mixture.
        /// </summary>
        /// <param name="dataList">List of difference matrices.</param>
        /// <param name="meanVector">Matrix containing mean parameters for the normal distribution.</param>
        /// <param name="alphaList">List of mixture estimates corresponding to differences</param>
        /// <param name="priorList">List of prior probabilities corresponding to differences.</param>
        /// <param name="independent">Whether the dimensions of the normal distribution should be considered normal.  Returns a diagonal DenseMatrix if true.  Should use false if unknown.</param>
        /// <param name="secondNormal">Whether the data is from the second of the normal distributions, i.e. incorrect in AMT database.</param>
        /// <returns>Updated DenseMatrix containing covariances of normal distribution.</returns>
        public static DenseMatrix UpdateNormalCovarianceMatrix(List<DenseMatrix> dataList, DenseMatrix meanVector, List<double> alphaList, List<double> priorList, bool independent, bool secondNormal)
        {
            var numerator = new DenseMatrix(meanVector.RowCount, meanVector.RowCount);
            var denominator = 0.0;

            var multiplier = 1.0;
            double adder = 0;

            if (secondNormal)
            {
                multiplier = -1.0;
                adder = 1.0;
            }

            for (var i = 0; i < dataList.Count; i++)
            {
                var dataT = dataList[i].Transpose() as DenseMatrix;

                var weight = alphaList[i] * (adder + multiplier * priorList[i]);
                numerator += weight * dataList[i] * dataT;
                denominator += weight;
            }

            var covarianceMatrix = (1 / denominator) * numerator;

            if (independent)
            {
                var indCovarianceMatrix = new DenseMatrix(covarianceMatrix.RowCount, covarianceMatrix.ColumnCount);
                for (var i = 0; i < covarianceMatrix.ColumnCount; i++)
                {
                    indCovarianceMatrix[i, i] = covarianceMatrix[i, i];
                }
                covarianceMatrix = indCovarianceMatrix.Clone() as DenseMatrix;
            }

            for (var i = 0; i < meanVector.RowCount; i++)
            {
                if (covarianceMatrix[i, i] < 0.000001)
                {
                    covarianceMatrix[i, i] = 0.000001;
                }
            }

            return (covarianceMatrix);
        }
        #endregion

        # region Normal-Uniform mixture

        /// <summary>
        /// Performs the EM algorithm on the given data and returns the parameters.
        /// </summary>
        /// <param name="dataList">A List containing matrices of the data.  These matrices must be of the same dimension as the mean vector.</param>
        /// <param name="meanVector">An [n x 1] matrix initially containing rough estimates and returning the mean of the normal distribution.</param>
        /// <param name="covarianceMatrix">An [n x n] positive definite matrix initially containing rough estimates and returning the covariance matrix for the normal distribution.</param>
        /// <param name="uniformTolerances">An [n x 1] matrix containing the half-width of the uniform distribution in each of the n-dimensions.</param>
        /// <param name="mixtureParameter">The proportion of the List thought to be from the normal distribution.  An initial estimate is passed and a refined proportion is returned.</param>
        /// <param name="independent">true/false:  Whether or not the multivariate normal distribution should be treated as independent, i.e. a diagonal covariance matrix.</param>
        /// <returns>A boolean flag indicating whether the EM algorithm achieved convergence.</returns>
        public static bool NormalUniformMixture(
            List<DenseMatrix> dataList,
            ref DenseMatrix meanVector,
            ref DenseMatrix covarianceMatrix,
            DenseMatrix uniformTolerances,
            ref double mixtureParameter,
            bool independent)
        {
            var iteration = 0;
            var converges = false;
            // Check that the dimensions of the matrices agree.
            if (!(dataList[0].RowCount == meanVector.RowCount && dataList[0].ColumnCount == meanVector.ColumnCount && covarianceMatrix.RowCount == covarianceMatrix.ColumnCount
                   && covarianceMatrix.Rank() == covarianceMatrix.ColumnCount && covarianceMatrix.RowCount == meanVector.RowCount && uniformTolerances.RowCount == meanVector.RowCount))
            {
                throw new InvalidOperationException("Dimensions of matrices do not agree in ExpectationMaximization.NormalUniformMixture function.");
            }
            // Calculate the uniform density based on the tolerances passed.
            var uniformDensity = 1.0;
            for (var i = 0; i < uniformTolerances.RowCount; i++)
            {
                uniformDensity /= (2 * uniformTolerances[i, 0]);
            }

            // Calculate the starting loglikelihood and initialize a variable for the loglikelihood at the next iteration.
            var logLikelihood = NormalUniformLogLikelihood(dataList, meanVector, covarianceMatrix, uniformDensity, mixtureParameter);

            // Initialize the individual observation mixture estimates to the given mixture parameter and a list of priors to 1.
            List<double> priorList;

            var alphaList = GetAlphaListNoPriors(dataList, mixtureParameter, out priorList);

            // Step through the EM algorithm up to m_maxIterations time.
            while (iteration <= MAX_ITERATIONS)
            {
                // Update the parameters in the following order: mixture parameters, mean, covariance.
                mixtureParameter = UpdateNormalUniformMixtureParameter(dataList, meanVector, covarianceMatrix, mixtureParameter, uniformDensity, alphaList);
                meanVector = UpdateNormalMeanVector(dataList, alphaList, priorList, false);
                covarianceMatrix = UpdateNormalCovarianceMatrix(dataList, meanVector, alphaList, priorList, independent, false);

                // Calculate the loglikelihood based on the new parameters.
                var nextLogLikelihood = NormalUniformLogLikelihood(dataList, meanVector, covarianceMatrix, uniformDensity, mixtureParameter);

                // Increment the counter to show that another iteration has been completed.
                iteration++;

                // Set the convergence flag and exit the while loop if the convergence criteria is met.
                if (Math.Abs(nextLogLikelihood - logLikelihood) < EPSILON)
                {
                    converges = true;
                    break;
                }

                // Update the loglikelihood.
                logLikelihood = nextLogLikelihood;
            }
            // Return the convergence flag, which is still false unless changed to true above.
            return (converges);
        }
        /// <summary>
        /// Calculate the loglikelihood for a normal-uniform mixture.
        /// </summary>
        /// <param name="data">Matrix of data.</param>
        /// <param name="meanVector">Matrix containing the current means.</param>
        /// <param name="covarianceMatrix">The current covariance matrix.</param>
        /// <param name="uniformDensity">The density of the uniform distribution.</param>
        /// <param name="mixtureParameter">The current mixture parameter.</param>
        /// <returns>The value of the loglikelihood evaluated at data.</returns>
        static public double NormalUniformLogLikelihood(DenseMatrix  data, DenseMatrix meanVector, DenseMatrix covarianceMatrix, double uniformDensity, double mixtureParameter, ref double stacScore)
        {
            var normalDensity = MathUtilities.MultivariateNormalDensity(data, meanVector, covarianceMatrix);
            if (normalDensity > 0)
            {
                var posteriorReal = mixtureParameter * normalDensity;
                var posteriorFalse = (1 - mixtureParameter) * uniformDensity;

                stacScore = (posteriorReal) / (posteriorReal + posteriorFalse);

                return Math.Log(posteriorReal + posteriorFalse);
            }
            return 0.0;
        }
        /// <summary>
        /// Calculate the loglikelihood for a normal-uniform mixture.
        /// </summary>
        /// <param name="dataList">List of Matrices of data.</param>
        /// <param name="meanVector">Matrix containing the current means.</param>
        /// <param name="covarianceMatrix">The current covariance matrix.</param>
        /// <param name="uniformDensity">The density of the uniform distribution.</param>
        /// <param name="mixtureParameter">The current mixture parameter.</param>
        /// <returns>The value of the loglikelihood evaluated over dataList.</returns>
        static public double NormalUniformLogLikelihood(List<DenseMatrix> dataList, DenseMatrix meanVector, DenseMatrix covarianceMatrix, double uniformDensity, double mixtureParameter)
        {
            var loglikelihood = 0.0;
            var stacScore = 0.0;
            foreach (var data in dataList)
            {
                loglikelihood += NormalUniformLogLikelihood(data, meanVector, covarianceMatrix, uniformDensity, mixtureParameter, ref stacScore);
            }
            return loglikelihood;
        }
        /// <summary>
        /// Update the mixture parameter for the normal-uniform mixture model.
        /// </summary>
        /// <param name="dataList">List of matrices containing the data.</param>
        /// <param name="meanVector">Matrix containing the current means.</param>
        /// <param name="covarianceMatrix">The current covariance matrix.</param>
        /// <param name="mixtureParameter">The current mixture parameter.</param>
        /// <param name="uniformDensity">The density of the uniform distribution.</param>
        /// <param name="alphaList">A List of observation-wise mixture proportion estimates to be updated and returned.</param>
        /// <returns>The updated mixture parameter.</returns>
        static public double UpdateNormalUniformMixtureParameter(
            List<DenseMatrix> dataList,
            DenseMatrix meanVector,
            DenseMatrix covarianceMatrix,
            double mixtureParameter,
            double uniformDensity,
            List<double> alphaList)
        {
            var nextMixtureParameter = 0.0;
            var stacScore = 0.0;

            for (var i = 0; i < dataList.Count; i++)
            {
                alphaList[i] = Math.Exp(NormalUniformLogLikelihood(dataList[i], meanVector, covarianceMatrix, uniformDensity, mixtureParameter, ref stacScore));
                nextMixtureParameter += alphaList[i];
            }

            return (nextMixtureParameter / alphaList.Count);
        }
        # endregion

        #region Normal-Normal-Uniform mixture
        /// <summary>
        /// Calculate the loglikelihood for a normal-normal-uniform mixture.
        /// </summary>
        /// <param name="data">Matrix of data.</param>
        /// <param name="prior">The prior probability of being correct, i.e. the probability of being from the normal distribution with parameters meanVectorT and covarianceMatrixT.</param>
        /// <param name="meanVectorT">Matrix containing the current means for the true normal distribution.</param>
        /// <param name="covarianceMatrixT">The current covariance matrix for the true normal distribution.</param>
        /// <param name="meanVectorF">Matrix containing the current means for the false normal distribution.</param>
        /// <param name="covarianceMatrixF">The current covariance matrix for the false normal distribution.</param>
        /// <param name="uniformDensity">The density of the uniform distribution.</param>
        /// <param name="mixtureParameter">The current mixture parameter.</param>
        /// <returns>The value of the loglikelihood evaluated at data.</returns>
        static public double NormalNormalUniformLogLikelihood(DenseMatrix  data, double prior, DenseMatrix meanVectorT, DenseMatrix covarianceMatrixT, DenseMatrix meanVectorF, DenseMatrix covarianceMatrixF, double uniformDensity, double mixtureParameter, ref double stacScore)
        {
            var normalDensityT = MathUtilities.MultivariateNormalDensity(data, meanVectorT, covarianceMatrixT);
            var normalDensityF = MathUtilities.MultivariateNormalDensity(data, meanVectorF, covarianceMatrixF);
            if (normalDensityT > 0)
            {
                var posteriorReal = mixtureParameter * prior * normalDensityT;
                var posteriorIReal = mixtureParameter * (1 - prior) * normalDensityF;
                var posteriorFalse = (1 - mixtureParameter) * uniformDensity;

                stacScore = (posteriorReal) / (posteriorReal + posteriorIReal + posteriorFalse);

                return Math.Log(posteriorReal + posteriorIReal + posteriorFalse);
            }

            stacScore = 0.0;
            return 0.0;
        }

        /// <summary>
        /// Compute the alpha value
        /// </summary>
        /// <param name="data"></param>
        /// <param name="prior"></param>
        /// <param name="meanVectorT"></param>
        /// <param name="covarianceMatrixT"></param>
        /// <param name="meanVectorF"></param>
        /// <param name="covarianceMatrixF"></param>
        /// <param name="uniformDensity"></param>
        /// <param name="mixtureParameter"></param>
        /// <returns></returns>
        static public double GetAlpha(DenseMatrix data, double prior, DenseMatrix meanVectorT, DenseMatrix covarianceMatrixT, DenseMatrix meanVectorF, DenseMatrix covarianceMatrixF, double uniformDensity, double mixtureParameter)
        {
            var normalDensityT = MathUtilities.MultivariateNormalDensity(data, meanVectorT, covarianceMatrixT);
            var normalDensityF = MathUtilities.MultivariateNormalDensity(data, meanVectorF, covarianceMatrixF);
            if (!(normalDensityT > 0))
            {
                return 0.0;
            }

            var posteriorReal = mixtureParameter * prior * normalDensityT;
            var posteriorIReal = mixtureParameter * (1 - prior) * normalDensityF;
            var posteriorFalse = (1 - mixtureParameter) * uniformDensity;

            return (posteriorReal + posteriorIReal) / (posteriorReal + posteriorIReal + posteriorFalse);
        }

        /// <summary>
        /// Calculate the loglikelihood for a normal-normal-uniform mixture.
        /// </summary>
        /// <param name="dataList">List of Matrices of data.</param>
        /// <param name="prior">List of prior probabilities of being correct, i.e. the probability of being from the normal distribution with parameters meanVectorT and covarianceMatrixT.</param>
        /// <param name="meanVectorT">Matrix containing the current means for the true normal distribution.</param>
        /// <param name="covarianceMatrixT">The current covariance matrix for the true normal distribution.</param>
        /// <param name="meanVectorF">Matrix containing the current means for the false normal distribution.</param>
        /// <param name="covarianceMatrixF">The current covariance matrix for the false normal distribution.</param>
        /// <param name="uniformDensity">The density of the uniform distribution.</param>
        /// <param name="mixtureParameter">The current mixture parameter.</param>
        /// <returns>The value of the loglikelihood evaluated over data.</returns>
        static public double NormalNormalUniformLogLikelihood(List<DenseMatrix> dataList, List<double> prior, DenseMatrix meanVectorT, DenseMatrix covarianceMatrixT, DenseMatrix meanVectorF, DenseMatrix covarianceMatrixF, double uniformDensity, double mixtureParameter)
        {
            var logLikelihood = 0.0;
            var stacScore = 0.0;
            for (var i = 0; i < dataList.Count; i++)
            {
                logLikelihood += NormalNormalUniformLogLikelihood(dataList[i], prior[i], meanVectorT, covarianceMatrixT, meanVectorF, covarianceMatrixF, uniformDensity, mixtureParameter, ref stacScore);
            }
            return logLikelihood;
        }

        /// <summary>
        /// Get the alpha list for the given dataList and priors
        /// </summary>
        /// <param name="dataList">List of Matrices of data.</param>
        /// <param name="prior"></param>
        /// <param name="meanVectorT"></param>
        /// <param name="covarianceMatrixT"></param>
        /// <param name="meanVectorF"></param>
        /// <param name="covarianceMatrixF"></param>
        /// <param name="uniformDensity"></param>
        /// <param name="mixtureParameter">The proportion of the List thought to be from the normal distribution.  An initial estimate is passed and a refined proportion is returned.</param>
        /// <returns></returns>
        static public List<double> GetAlphaList(List<DenseMatrix> dataList, List<double> prior, DenseMatrix meanVectorT, DenseMatrix covarianceMatrixT, DenseMatrix meanVectorF, DenseMatrix covarianceMatrixF, double uniformDensity, double mixtureParameter)
        {
            var alphaList = new List<double>(dataList.Count);
            for (var i = 0; i < dataList.Count; i++)
            {
                alphaList.Add(GetAlpha(dataList[i], prior[i], meanVectorT, covarianceMatrixT, meanVectorF, covarianceMatrixF, uniformDensity, mixtureParameter));
            }
            return alphaList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataList">List of Matrices of data.</param>
        /// <param name="mixtureParameter">The proportion of the List thought to be from the normal distribution.  An initial estimate is passed and a refined proportion is returned.</param>
        /// <returns></returns>
        public static List<double> GetAlphaListNoPriors(List<DenseMatrix> dataList, double mixtureParameter)
        {
            List<double> priorList;
            return GetAlphaListNoPriors(dataList, mixtureParameter, out priorList);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataList">List of Matrices of data.</param>
        /// <param name="mixtureParameter">The proportion of the List thought to be from the normal distribution.  An initial estimate is passed and a refined proportion is returned.</param>
        /// <param name="priorList">List of prior probabilities corresponding to differences (output)</param>
        /// <returns></returns>
        public static List<double> GetAlphaListNoPriors(List<DenseMatrix> dataList, double mixtureParameter, out List<double> priorList)
        {
            // Initialize the individual observation mixture estimates to the given mixture parameter and a list of priors to 1.
            var alphaList = new List<double>(dataList.Count);
            priorList = new List<double>(dataList.Count);

            for (var i = 0; i < dataList.Count; i++)
            {
                alphaList.Add(mixtureParameter);
                priorList.Add(1.0);
            }

            return alphaList;
        }

        /// <summary>
        /// Update the mixture parameter for the normal-normal-uniform mixture model.
        /// </summary>
        /// <param name="dataList">List of matrices containing the data.</param>
        /// <param name="prior">The prior probability of being correct, i.e. the probability of being from the normal distribution with parameters meanVectorT and covarianceMatrixT.</param>
        /// <param name="meanVectorT">Matrix containing the current means for the true normal distribution.</param>
        /// <param name="covarianceMatrixT">The current covariance matrix for the true normal distribution.</param>
        /// <param name="meanVectorF">Matrix containing the current means for the false normal distribution.</param>
        /// <param name="covarianceMatrixF">The current covariance matrix for the false normal distribution.</param>
        /// <param name="mixtureParameter">The current mixture parameter.</param>
        /// <param name="uniformDensity">The density of the uniform distribution.</param>
        /// <param name="alphaList">A List of observation-wise mixture proportion estimates to be updated and returned.</param>
        /// <returns>The updated mixture parameter.</returns>
        static public double UpdateNormalNormalUniformMixtureParameter(List<DenseMatrix> dataList, List<double> priorList, DenseMatrix meanVectorT, DenseMatrix covarianceMatrixT, DenseMatrix meanVectorF, DenseMatrix covarianceMatrixF, double mixtureParameter, double uniformDensity, ref List<double> alphaList)
        {
            var nextMixtureParameter = 0.0;
            var numerator = 0.0;
            for (var i = 0; i < dataList.Count; i++)
            {
                numerator = priorList[i] * MathUtilities.MultivariateNormalDensity(dataList[i], meanVectorT, covarianceMatrixT) + (1 - priorList[i]) * MathUtilities.MultivariateNormalDensity(dataList[i], meanVectorF, covarianceMatrixF);
                alphaList[i] = (numerator * mixtureParameter) / (numerator * mixtureParameter + (1 - mixtureParameter) * uniformDensity);
                nextMixtureParameter += alphaList[i];
            }
            return (nextMixtureParameter / alphaList.Count);
        }
        #endregion
    }
}