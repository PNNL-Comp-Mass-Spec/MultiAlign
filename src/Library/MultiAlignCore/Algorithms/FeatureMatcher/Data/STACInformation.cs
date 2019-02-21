using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MultiAlignCore.Algorithms.FeatureMatcher.Utilities;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.Algorithms.FeatureMatcher.Data
{
    public class STACInformation
    {
        # region Members
        private double m_mixtureProportion;
        private double m_logLikelihood;

        private DenseMatrix m_meanVectorT;
        private DenseMatrix m_covarianceMatrixT;
        private DenseMatrix m_meanVectorF;
        private DenseMatrix m_covarianceMatrixF;

        private FeatureMatcherTolerances m_refinedTolerances;

        private uint m_iteration;

        private const double EPSILON = 0.0001;
        private const int MAX_ITERATIONS = 100;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the mixture proportion, i.e. the probability of being from the correct distribution.
        /// </summary>
        public double MixtureProportion
        {
            get { return m_mixtureProportion; }
            set { m_mixtureProportion = value; }
        }

        /// <summary>
        /// Gets or sets the estimated means of the true normal distribution.
        /// </summary>
        public DenseMatrix MeanVectorT
        {
            get { return m_meanVectorT; }
            set { m_meanVectorT = value; }
        }
        /// <summary>
        /// Gets or sets the estimated covariance matrix of the true normal distribution.
        /// </summary>
        public DenseMatrix CovarianceMatrixT
        {
            get { return m_covarianceMatrixT; }
            set { m_covarianceMatrixT = value; }
        }
        /// <summary>
        /// Gets or sets the mean vector of the normal distribution used in the case of a low prior probability.
        /// </summary>
        public DenseMatrix MeanVectorF
        {
            get { return m_meanVectorF; }
            set { m_meanVectorF = value; }
        }
        /// <summary>
        /// Gets or sets the covariance matrix of the normal distribution used in the case of a low prior probability.
        /// </summary>
        public DenseMatrix CovarianceMatrixF
        {
            get { return m_covarianceMatrixF; }
            set { m_covarianceMatrixF = value; }
        }
        /// <summary>
        /// Gets or sets the the refined tolerances calculated during STAC training.
        /// </summary>
        public FeatureMatcherTolerances RefinedTolerances
        {
            get { return m_refinedTolerances; }
            set { m_refinedTolerances = value; }
        }
        # endregion

        #region Constructors
        /// <summary>
        /// Default constructor for STAC parameters.
        /// </summary>
        /// <param name="driftTime">Whether drift times will be used in the analysis.</param>
        public STACInformation(bool driftTime)
        {
            Clear(driftTime);
        }
        #endregion

        #region Public functions
        /// <summary>
        /// Resets parameters to default values.
        /// </summary>
        /// <param name="driftTime">Whether to use drift times in the analysis.</param>
        public void Clear(bool driftTime)
        {
            m_iteration = 0;
            m_mixtureProportion = 0.5;
            m_logLikelihood = 0.0;
            if (driftTime)
            {
                m_meanVectorT = new DenseMatrix(3, 1);
                m_covarianceMatrixT = new DenseMatrix(3, 3);
                m_covarianceMatrixT[0, 0] = 2.0;
                m_covarianceMatrixT[1, 1] = 0.3;
                m_covarianceMatrixT[2, 2] = 0.5;
                m_meanVectorF = m_meanVectorT;
                m_covarianceMatrixF = m_covarianceMatrixT;
            }
            else
            {
                m_meanVectorT = new DenseMatrix(2, 1);
                m_covarianceMatrixT = new DenseMatrix(2, 2);
                m_covarianceMatrixT[0, 0] = 2.0;
                m_covarianceMatrixT[1, 1] = 0.3;
                m_meanVectorF = m_meanVectorT;
                m_covarianceMatrixF = m_covarianceMatrixT;
            }
        }

        /// <summary>
        /// Trains the STAC parameters using the passed data.
        /// </summary>
        /// <typeparam name="T">Observed feature to be matched. Derived from Feature. Usually UMC or UMCCluster.</typeparam>
        /// <typeparam name="TU">Feature to be matched to. Derived from Feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to train parameters on.</param>
        /// <param name="uniformTolerances">User provided tolerances.</param>
        /// <param name="useDriftTime">Whether to train the data on the drift time dimension.</param>
        /// <returns>A boolean flag indicating whether convergence was reached.</returns>
        public bool TrainStac<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, FeatureMatcherTolerances uniformTolerances, bool useDriftTime)
            where T  : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            return TrainStac(featureMatchList, uniformTolerances, useDriftTime, usePrior:false);

        }

        private bool TrainStac<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, FeatureMatcherTolerances uniformTolerances, bool useDriftTime, bool usePrior)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            Clear();

            // Calculate the density of the uniform density given the tolerances.
            var toleranceMatrix = uniformTolerances.AsVector(useDriftTime);
            var uniformDensity = 1.0;
            for (var i = 0; i < toleranceMatrix.RowCount; i++)
            {
                uniformDensity /= (2 * toleranceMatrix[i, 0]);
            }

            // Train the EM parameters on the data.

            if (!usePrior || typeof (TU) != typeof (MassTagLight))
            {
                return TrainWithoutPrior(featureMatchList, uniformDensity, useDriftTime);
            }

            var newFeatureMatchList = ConvertFeatureMatchListToMassTagLightList(featureMatchList);

            return TrainWithPrior(newFeatureMatchList, uniformDensity, useDriftTime);

        }

        /// <summary>
        /// Function to calculate STAC score.
        /// </summary>
        /// <typeparam name="T">Observed feature to be matched. Derived from Feature. Usually UMC or UMCCluster.</typeparam>
        /// <typeparam name="TU">Feature to be matched to. Derived from Feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to calculate scores for.</param>
        /// <param name="uniformTolerances"></param>
        /// <param name="useDriftTime">Whether to use drift times in the calculations.</param>
        /// <returns>The STAC score corresponding to featureMatch.</returns>
        private void SetStacScores<T, TU>(IEnumerable<FeatureMatch<T, TU>> featureMatchList, FeatureMatcherTolerances uniformTolerances, bool useDriftTime)
            where T  : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            SetStacScores(featureMatchList, uniformTolerances, useDriftTime, usePrior:false);

        }

        /// <summary>
        /// Function to calculate STAC score.
        /// </summary>
        /// <typeparam name="T">Observed feature to be matched. Derived from Feature. Usually UMC or UMCCluster.</typeparam>
        /// <typeparam name="TU">Feature to be matched to. Derived from Feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to calculate scores for.</param>
        /// <param name="uniformTolerances"></param>
        /// <param name="useDriftTime">Whether to use drift times in the calculations.</param>
        /// <param name="usePrior">Whether to use prior probabilities in the calculation of the STAC score.</param>
        /// <returns>The STAC score corresponding to featureMatch.</returns>
        private void SetStacScores<T, TU>(IEnumerable<FeatureMatch<T, TU>> featureMatchList, FeatureMatcherTolerances uniformTolerances, bool useDriftTime, bool usePrior)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            // Calculate the density of the uniform density given the tolerances.
            var toleranceMatrix = uniformTolerances.AsVector(useDriftTime);
            var uniformDensity = 1.0;
            for (var i = 0; i < toleranceMatrix.RowCount; i++)
            {
                uniformDensity /= (2 * toleranceMatrix[i, 0]);
            }

            if (typeof (TU) != typeof (MassTagLight))
            {
                foreach (var match in featureMatchList)
                {
                    match.STACScore = ComputeStacFeature(match, uniformDensity);
                }
                return;
            }

            foreach (var match in featureMatchList)
            {
                var newMatch = match as FeatureMatch<T, MassTagLight>;

                if (newMatch != null)
                    newMatch.STACScore = ComputeStacMassTag(newMatch, uniformDensity, usePrior);

            }

        }

        /// <summary>
        /// Set the STAC Specificity scores for a list of matches.
        /// </summary>
        /// <typeparam name="T">Observed feature to be matched. Derived from Feature. Usually UMC or UMCCluster.</typeparam>
        /// <typeparam name="TU">Feature to be matched to. Derived from Feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to calculate Specificities for.</param>
        public void SetStacSpecificitiesFeature<T, TU>(List<FeatureMatch<T, TU>> featureMatchList)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            featureMatchList.Sort(FeatureMatch<T, TU>.FeatureComparison);
            var matchIndex = 0;
            var endIndex = matchIndex;
            while (matchIndex < featureMatchList.Count)
            {
                var totalCount = 1;
                while (endIndex < featureMatchList.Count && featureMatchList[endIndex].ObservedFeature.Id == featureMatchList[matchIndex].ObservedFeature.Id)
                {
                    totalCount++;
                    endIndex++;
                }
                var denominator = 0.0;
                for (var i = matchIndex; i < endIndex; i++)
                {
                    var alpha = 1.0 / totalCount;
                    denominator += BetaDensity(featureMatchList[i].STACScore, alpha) * featureMatchList[i].STACScore;
                }
                for (var i = matchIndex; i < endIndex; i++)
                {
                    var alpha = 1.0 / totalCount;
                    featureMatchList[i].STACSpecificity = featureMatchList[i].STACScore * BetaDensity(featureMatchList[i].STACScore, alpha) / denominator;
                }
                matchIndex = endIndex;
            }
        }
        /// <summary>
        /// Set the STAC Specificity scores for a list of matches.
        /// </summary>
        /// <typeparam name="T">Observed feature to be matched. Derived from Feature. Usually UMC or UMCCluster.</typeparam>
        /// <typeparam name="TU">Matched feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to calculate Specificities for.</param>
        public void SetSTACSpecificitiesMassTag<T, TU>(List<FeatureMatch<T, TU>> featureMatchList)
            where T : FeatureLight, new()
            where TU : MassTagLight, new()
        {
            featureMatchList.Sort(FeatureMatch<T, TU>.FeatureComparison);
            var matchIndex = 0;
            var endIndex = matchIndex;

            // If only 1 match, the Specificity score should be 1
            if (featureMatchList.Count == 1)
            {
                featureMatchList[0].STACSpecificity = 1;
                return;
            }

            while (matchIndex < featureMatchList.Count)
            {
                var maxCount = 0;
                while (endIndex < featureMatchList.Count && featureMatchList[endIndex].ObservedFeature.Id == featureMatchList[matchIndex].ObservedFeature.Id)
                {
                    if (featureMatchList[endIndex].TargetFeature.ObservationCount > maxCount)
                    {
                        maxCount = featureMatchList[endIndex].TargetFeature.ObservationCount;
                    }
                    endIndex++;
                }
                var denominator = 0.0;
                for (var i = matchIndex; i < endIndex; i++)
                {
                    var match = featureMatchList[i];
                    denominator += match.TargetFeature.ObservationCount * Math.Exp(Math.Log(maxCount) + (maxCount - 1) * Math.Log(match.STACScore));
                }
                for (var i = matchIndex; i < endIndex; i++)
                {
                    var match = featureMatchList[i];
                    match.STACSpecificity = (match.TargetFeature.ObservationCount * Math.Exp(Math.Log(maxCount) + (maxCount - 1) * Math.Log(match.STACScore))) / denominator;
                }
                matchIndex = endIndex;
            }
        }

        /// <summary>
        /// Function to train STAC parameters and to set STAC scores and Specificities.
        /// </summary>
        /// <typeparam name="T">Observed feature to be matched. Derived from Feature. Usually UMC or UMCCluster.</typeparam>
        /// <typeparam name="TU">Feature to be matched to. Derived from Feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to perform STAC on.</param>
        /// <param name="uniformTolerances"></param>
        /// <param name="useDriftTime">Whether to use drift times in the calculations.</param>
        public void PerformStac<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, FeatureMatcherTolerances uniformTolerances, bool useDriftTime)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            ReportMessage("Training STAC");
            TrainStac(featureMatchList, uniformTolerances, useDriftTime);

            ReportMessage("Calculating final STAC Scores");
            SetStacScores(featureMatchList, uniformTolerances, useDriftTime);

            ReportMessage("Calculating STAC_UP Scores");
            SetStacSpecificitiesFeature(featureMatchList);
        }

        /// <summary>
        /// Function to train STAC parameters and to set STAC scores and Specificities.
        /// </summary>
        /// <typeparam name="T">Observed feature to be matched. Derived from Feature. Usually UMC or UMCCluster.</typeparam>
        /// <typeparam name="TU">Feature to be matched to. Derived from Feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to perform STAC on.</param>
        /// <param name="uniformTolerances"></param>
        /// <param name="useDriftTime">Whether to use drift times in the calculations.</param>
        /// <param name="usePrior">Whether to use prior probabilities in the calculation of the STAC score.</param>
        public void PerformStac<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, FeatureMatcherTolerances uniformTolerances, bool useDriftTime, bool usePrior)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            ReportMessage("Training STAC");
            TrainStac(featureMatchList, uniformTolerances, useDriftTime, usePrior);

            ReportMessage("Calculating final STAC Scores");
            SetStacScores(featureMatchList, uniformTolerances, useDriftTime, usePrior);

            ReportMessage("Calculating STAC_UP Scores");

            if (typeof (TU) != typeof (MassTagLight))
            {
                SetStacSpecificitiesFeature(featureMatchList);
            }
            else
            {
                var newFeatureMatchList = ConvertFeatureMatchListToMassTagLightList(featureMatchList);

                SetSTACSpecificitiesMassTag(newFeatureMatchList);
            }


        }
        #endregion

        #region Private functions

        /// <summary>
        /// Overload Clear function to reset number of iterations.
        /// </summary>
        private void Clear()
        {
            m_logLikelihood = 0.0;
            m_iteration = 0;
        }

        /// <summary>
        /// Function to calculate STAC parameters using prior probabilities.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature. Derived from Feature.</typeparam>
        /// <typeparam name="TU">Matched feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to train data on.</param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <param name="useDriftTime">Whether to use drift times in the calculations.</param>
        /// <returns>A boolean flag indicating the convergence state of the algorithm.</returns>
        private bool TrainWithPrior<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, double uniformDensity, bool useDriftTime)
            where T : FeatureLight, new()
            where TU : MassTagLight, new()
        {
            Clear();
            var converges = false;

            var dataList = new List<DenseMatrix>();
            var priorList = new List<double>();
            foreach (var match in featureMatchList)
            {
                dataList.Add(match.DifferenceVector);
                priorList.Add(match.TargetFeature.PriorProbability);
            }

            var alphaList = new List<double>(dataList.Count);

            m_logLikelihood = CalculateNnuLogLikelihood(featureMatchList, uniformDensity, useDriftTime, ref alphaList);

            // Step through the EM algorithm up to m_maxIterations time.
            while (m_iteration <= MAX_ITERATIONS)
            {
                // Update the parameters in the following order: mixture parameters, mean, covariance.
                m_mixtureProportion = UpdateNNUMixtureParameter<T, TU>(alphaList);
                m_meanVectorT = ExpectationMaximization.UpdateNormalMeanVector(dataList, alphaList, priorList, false);
                m_meanVectorF = ExpectationMaximization.UpdateNormalMeanVector(dataList, alphaList, priorList, true);
                m_covarianceMatrixT = ExpectationMaximization.UpdateNormalCovarianceMatrix(dataList, m_meanVectorT, alphaList, priorList, false, false);
                m_covarianceMatrixF = ExpectationMaximization.UpdateNormalCovarianceMatrix(dataList, m_meanVectorF, alphaList, priorList, false, true);

                // Calculate the loglikelihood based on the new parameters.
                var nextLogLikelihood = CalculateNnuLogLikelihood(featureMatchList, uniformDensity, useDriftTime, ref alphaList);

                OnIterate(new ProgressNotifierArgs("."));
                PrintCurrentStatistics(nextLogLikelihood);

                // Increment the counter to show that another iteration has been completed.
                m_iteration++;

                // Set the convergence flag and exit the while loop if the convergence criteria is met.
                if (m_iteration > 10 && Math.Abs(nextLogLikelihood - m_logLikelihood) < EPSILON)
                {
                    converges = true;
                    break;
                }

                // Update the loglikelihood.
                m_logLikelihood = nextLogLikelihood;
            }

            // Find the refined tolerances
            ComputeRefinedTolerances(m_covarianceMatrixT, useDriftTime);

            // Return the convergence flag, which is still false unless changed to true above.
            return converges;
        }

        /// <summary>
        /// Train the STAC parameters without using prior probabilities.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature. Derived from Feature.</typeparam>
        /// <typeparam name="TU">The type of the target feature. Derived from Feature.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to train data on.</param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <param name="useDriftTime">Whether to use drift times in the calculations.</param>
        /// <returns>A boolean flag indicating the convergence state of the algorithm.</returns>
        private bool TrainWithoutPrior<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, double uniformDensity, bool useDriftTime)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            Clear();
            var converges = false;

            var dataList = new List<DenseMatrix>();
            foreach (var match in featureMatchList)
            {
                dataList.Add(match.DifferenceVector);
            }

            m_logLikelihood = CalculateNuLogLikelihood(featureMatchList, uniformDensity, useDriftTime);

            var alphaList = ExpectationMaximization.GetAlphaListNoPriors(dataList, m_mixtureProportion);

            // Step through the EM algorithm up to m_maxIterations time.
            while (m_iteration <= MAX_ITERATIONS)
            {
                // Update the parameters in the following order: mixture parameters, mean, covariance.
                m_mixtureProportion = UpdateNUMixtureParameter(featureMatchList, uniformDensity, ref alphaList);
                m_meanVectorT = ExpectationMaximization.UpdateNormalMeanVector(dataList, alphaList);
                m_covarianceMatrixT = ExpectationMaximization.UpdateNormalCovarianceMatrix(dataList, m_meanVectorT, alphaList, false);

                // Calculate the loglikelihood based on the new parameters.
                var nextLogLikelihood = CalculateNuLogLikelihood(featureMatchList, uniformDensity, useDriftTime);

                // Print statistics every 10 iterations
                if (m_iteration % 10 == 0)
                {
                    PrintCurrentStatistics(nextLogLikelihood);
                }

                // Increment the counter to show that another iteration has been completed.
                m_iteration++;

                // Set the convergence flag and exit the while loop if the convergence criteria is met.
                if (Math.Abs(nextLogLikelihood - m_logLikelihood) < EPSILON)
                {
                    converges = true;
                    break;
                }

                // Update the loglikelihood.
                m_logLikelihood = nextLogLikelihood;
            }

            // Find the refined tolerances
            ComputeRefinedTolerances(m_covarianceMatrixT, useDriftTime);

            // Return the convergence flag, which is still false unless changed to true above.
            return converges;
        }

        /// <summary>
        /// Calculate the loglikelihood for a match with the current parameters.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature. Derived from Feature.</typeparam>
        /// <typeparam name="TU">The type of the target feature. Derived from Feature.</typeparam>
        /// <param name="featureMatch">FeatureMatch to evaluate at.</param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <param name="stacScore"></param>
        /// <returns>The value of the loglikelihood evaluated at featureMatch.</returns>
        private double CalculateNuLogLikelihood<T, TU>(FeatureMatch<T, TU> featureMatch, double uniformDensity, ref double stacScore)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            if (featureMatch.UseDriftTimePredicted)
            {
                return ExpectationMaximization.NormalUniformLogLikelihood(featureMatch.ReducedDifferenceVector, MatrixUtilities.RemoveRow(m_meanVectorT, 3), MatrixUtilities.ReduceMatrix(m_covarianceMatrixT, 3), uniformDensity, m_mixtureProportion, ref stacScore);
            }
            return ExpectationMaximization.NormalUniformLogLikelihood(featureMatch.ReducedDifferenceVector, m_meanVectorT, m_covarianceMatrixT, uniformDensity, m_mixtureProportion, ref stacScore);
        }

        /// <summary>
        /// Calculate the loglikelihood for matches with the current parameters.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature. Derived from Feature.</typeparam>
        /// <typeparam name="TU">The type of the target feature. Derived from Feature.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to evaluate at.</param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <returns>The value of the loglikelihood evaluated over featureMatchList.</returns>
        private double CalculateNuLogLikelihood<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, double uniformDensity)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            var loglikelihood = 0.0;
            var stacScore = 0.0;
            foreach (var match in featureMatchList)
            {
                loglikelihood += CalculateNuLogLikelihood(match, uniformDensity, ref stacScore);
                match.STACScore = stacScore;
            }
            return loglikelihood;
        }
        /// <summary>
        /// Calculate the loglikelihood for matches with the current parameters.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature. Derived from Feature.</typeparam>
        /// <typeparam name="TU">The type of the target feature. Derived from Feature.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to evaluate at.</param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <param name="useDriftTime">Whether to use drift times in the calculations.</param>
        /// <returns>The value of the loglikelihood evaluated over featureMatchList.</returns>
        private double CalculateNuLogLikelihood<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, double uniformDensity, bool useDriftTime)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            double logLikelihood;
            if (useDriftTime)
            {
                logLikelihood = CalculateNuLogLikelihood(featureMatchList, uniformDensity);
            }
            else
            {
                var dataList = new List<DenseMatrix>();
                foreach (var match in featureMatchList)
                {
                    dataList.Add(match.DifferenceVector);
                }
                logLikelihood = ExpectationMaximization.NormalUniformLogLikelihood(dataList, m_meanVectorT, m_covarianceMatrixT, uniformDensity, m_mixtureProportion);
            }
            return logLikelihood;
        }

        /// <summary>
        /// Update the mixture proportion for the normal-uniform mixture model.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature. Derived from Feature.</typeparam>
        /// <typeparam name="TU">The type of the target feature. Derived from Feature.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to evaluate at.</param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <param name="alphaList">List to be filled with individual mixture values.</param>
        /// <returns>The updated mixture proportion.</returns>
        private double UpdateNUMixtureParameter<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, double uniformDensity, ref List<double> alphaList)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            var alphaSum = 0.0;
            foreach (var alpha in alphaList)
            {
                alphaSum += alpha;
            }

            return alphaSum / alphaList.Count;
        }
        /// <summary>
        /// Calculate the loglikelihood for a match with the current parameters.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature. Derived from Feature.</typeparam>
        /// <typeparam name="TU">Matched feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatch">FeatureMatch to evaluate at.</param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <param name="stacScore">STAC Score (output)</param>
        /// <returns>The value of the loglikelihood evaluated at featureMatch.</returns>
        private double CalculateNnuLogLikelihood<T, TU>(FeatureMatch<T, TU> featureMatch, double uniformDensity, ref double stacScore)
            where T : FeatureLight, new()
            where TU : MassTagLight, new()
        {
            if (featureMatch.UseDriftTimePredicted)
            {
                return ExpectationMaximization.NormalNormalUniformLogLikelihood(
                    featureMatch.ReducedDifferenceVector,
                    featureMatch.TargetFeature.PriorProbability,
                    MatrixUtilities.RemoveRow(m_meanVectorT, 3),
                    MatrixUtilities.ReduceMatrix(m_covarianceMatrixT, 3),
                    MatrixUtilities.RemoveRow(m_meanVectorF, 3),
                    MatrixUtilities.ReduceMatrix(m_covarianceMatrixF, 3),
                    uniformDensity,
                    m_mixtureProportion,
                    ref stacScore);
            }

            return ExpectationMaximization.NormalNormalUniformLogLikelihood(
                featureMatch.ReducedDifferenceVector,
                featureMatch.TargetFeature.PriorProbability,
                m_meanVectorT,
                m_covarianceMatrixT,
                m_meanVectorF,
                m_covarianceMatrixF,
                uniformDensity,
                m_mixtureProportion,
                ref stacScore);
        }

        /// <summary>
        /// Calculate the loglikelihood for matches with the current parameters.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature.  Derived from Feature.</typeparam>
        /// /// <typeparam name="TU">Matched feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to evaluate at.</param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <returns>The value of the loglikelihood evaluated over featureMatchList.</returns>
        private double CalculateNnuLogLikelihood<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, double uniformDensity)
            where T : FeatureLight, new()
            where TU : MassTagLight, new()
        {
            var loglikelihood = 0.0;
            var stacScore = 0.0;
            foreach (var match in featureMatchList)
            {
                loglikelihood += CalculateNnuLogLikelihood(match, uniformDensity, ref stacScore);
                match.STACScore = stacScore;
            }
            return loglikelihood;
        }

        /// <summary>
        /// Calculate the loglikelihood for matches with the current parameters.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature.  Derived from Feature.</typeparam>
        /// <typeparam name="TU">Matched feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatchList">List of FeatureMatches to evaluate at.</param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <param name="useDriftTime">Whether to use drift times in the calculations.</param>
        /// <param name="alphaList">List of mixture estimates corresponding to differences.</param>
        /// <returns>The value of the loglikelihood evaluated over featureMatchList.</returns>
        private double CalculateNnuLogLikelihood<T, TU>(List<FeatureMatch<T, TU>> featureMatchList, double uniformDensity, bool useDriftTime, ref List<double> alphaList)
            where T : FeatureLight, new()
            where TU : MassTagLight, new()
        {
            var dataList = new List<DenseMatrix>();
            var priorList = new List<double>();
            foreach (var match in featureMatchList)
            {
                dataList.Add(match.DifferenceVector);
                priorList.Add(match.TargetFeature.PriorProbability);
            }

            var logLikelihood = ExpectationMaximization.NormalNormalUniformLogLikelihood(dataList, priorList, m_meanVectorT, m_covarianceMatrixT, m_meanVectorF, m_covarianceMatrixF, uniformDensity, m_mixtureProportion);

            // Get new values for alpha list
            alphaList.Clear();
            alphaList = ExpectationMaximization.GetAlphaList(dataList, priorList, m_meanVectorT, m_covarianceMatrixT, m_meanVectorF, m_covarianceMatrixF, uniformDensity, m_mixtureProportion);

            return logLikelihood;
        }

        /// <summary>
        /// Use the covariance matrix to compute the refined tolerances
        /// </summary>
        /// <param name="covarianceMatrixT">Covariance matrix</param>
        /// <param name="useDriftTime"></param>
        private void ComputeRefinedTolerances(DenseMatrix covarianceMatrixT, bool useDriftTime)
        {

            var massPpmStDev = Math.Sqrt(covarianceMatrixT[0, 0]);
            var netStDev = Math.Sqrt(covarianceMatrixT[1, 1]);
            double driftTimeStDev = 0;
            if (useDriftTime)
            {
                driftTimeStDev = Math.Sqrt(covarianceMatrixT[2, 2]);
            }

            m_refinedTolerances = new FeatureMatcherTolerances((2.5 * massPpmStDev), (2.5 * netStDev), (float)(2.5 * driftTimeStDev))
            {
                Refined = true
            };
        }

        /// <summary>
        /// Update the mixture proportion for the normal-normal-uniform mixture model.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature.  Derived from Feature.</typeparam>
        /// <typeparam name="TU">Matched feature. Usually AMTTag.</typeparam>
        /// <param name="alphaList">List to be filled with individual mixture values.</param>
        /// <returns>The updated mixture proportion.</returns>
        private double UpdateNNUMixtureParameter<T, TU>(List<double> alphaList)
            where T : FeatureLight, new()
            where TU : MassTagLight, new()
        {
            var alphaSum = alphaList.Sum();

            return alphaSum / alphaList.Count;
        }

        /// <summary>
        /// Overload function to calculate STAC score for MassTag target data.
        /// </summary>
        /// <typeparam name="T">The type of the observed feature.  Derived from Feature.</typeparam>
        /// <param name="featureMatch"></param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <param name="usePrior">Whether to use prior probabilities in the calculation of the STAC score.</param>
        /// <returns>The STAC score corresponding to featureMatch.</returns>
        private double ComputeStacMassTag<T>(FeatureMatch<T, MassTagLight> featureMatch, double uniformDensity, bool usePrior) where T : FeatureLight, new()
        {
            var stacScore = 0.0;

            if (usePrior)
            {
                CalculateNnuLogLikelihood(featureMatch, uniformDensity, ref stacScore);
            }
            else
            {
                CalculateNuLogLikelihood(featureMatch, uniformDensity, ref stacScore);
            }

            return stacScore;
        }

        /// <summary>
        /// Function to calculate STAC score.
        /// </summary>
        /// <typeparam name="T">Observed feature to be matched. Derived from Feature. Usually UMC or UMCCluster.</typeparam>
        /// <typeparam name="TU">Feature to be matched to. Derived from Feature. Usually AMTTag.</typeparam>
        /// <param name="featureMatch"></param>
        /// <param name="uniformDensity">Density of uniform distribution.</param>
        /// <returns>The STAC score corresponding to featureMatch.</returns>
        private double ComputeStacFeature<T, TU>(FeatureMatch<T, TU> featureMatch, double uniformDensity)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            var stacScore = 0.0;
            CalculateNuLogLikelihood(featureMatch, uniformDensity, ref stacScore);
            return stacScore;
        }

        private List<FeatureMatch<T, MassTagLight>> ConvertFeatureMatchListToMassTagLightList<T, TU>(IEnumerable<FeatureMatch<T, TU>> featureMatchList)
            where T : FeatureLight, new()
            where TU : FeatureLight, new()
        {
            var newFeatureMatchList = new List<FeatureMatch<T, MassTagLight>>();

            foreach (var match in featureMatchList)
            {
                newFeatureMatchList.Add(match as FeatureMatch<T, MassTagLight>);
            }

            return newFeatureMatchList;
        }

        /// <summary>
        /// Compute the density of the Beta distribution as the marginal of a Dirichlet distribution for computation of the STAC Specificity.
        /// </summary>
        /// <param name="stac"></param>
        /// <param name="alpha">The first parameter of the Beta distribution.  The second parameter is assumed to be (1-alpha).</param>
        /// <returns>The density of the Beta distribution evaluated at STAC.</returns>
        private double BetaDensity(double stac, double alpha)
        {
            return Math.Sin(Math.PI * alpha) / Math.PI * Math.Pow(stac, (alpha - 1)) * Math.Pow((1 - stac), (-1 * alpha));
        }

        private void PrintCurrentStatistics(double logLikelihood)
        {
            ReportDebug( "Parameters after " + m_iteration + " iterations:" + Environment.NewLine +
                         "\tLoglikelihood = " + logLikelihood + "\tAlpha = " + m_mixtureProportion + Environment.NewLine +
                         "Epsilon = " + EPSILON);

        }

        #endregion

        #region "Events and related functions"


        public event EventHandler<ProgressNotifierArgs> IterationEvent;
        public event EventHandler<ProgressNotifierArgs> MessageEvent;
        public event EventHandler<ProgressNotifierArgs> DebugEvent;

        /// <summary>
        /// Report detailed debugging information using OnDebugEvent
        /// </summary>
        /// <param name="message"></param>
        private void ReportDebug(string message)
        {
            OnDebugEvent(new ProgressNotifierArgs(message));
        }

        /// <summary>
        /// Report a progress message using OnMessage
        /// </summary>
        /// <param name="message"></param>
        private void ReportMessage(string message)
        {
            OnMessage(new ProgressNotifierArgs(message));
        }


        private void OnIterate(ProgressNotifierArgs e)
        {
            IterationEvent?.Invoke(this, e);
        }

        private void OnMessage(ProgressNotifierArgs e)
        {
            MessageEvent?.Invoke(this, e);
        }

        private void OnDebugEvent(ProgressNotifierArgs e)
        {
            DebugEvent?.Invoke(this, e);
        }

        #endregion
    }

}
