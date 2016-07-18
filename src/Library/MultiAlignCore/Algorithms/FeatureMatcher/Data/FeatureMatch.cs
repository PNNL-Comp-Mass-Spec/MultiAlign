using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MultiAlignCore.Algorithms.FeatureMatcher.Utilities;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.FeatureMatcher.Data
{
    public sealed class FeatureMatch<TObserved, TTarget>
        where TObserved : FeatureLight, new()
        where TTarget : FeatureLight, new()
    {
        #region Members
        private bool m_useDriftTime;
        private bool m_useDriftTimePredicted;
        private bool m_withinRefinedRegion;
        private bool m_shiftedMatch;

        private double m_stacScore;
        private double m_stacSpecificity;
        private double m_slicScore;
        private double m_delSliC;

        private DenseMatrix m_differenceVector;
        private DenseMatrix m_reducedDifferenceVector;

        private TObserved m_observedFeature;
        private TTarget m_targetFeature;
        #endregion

        #region Properties
        /// <summary>
        /// Gets whether the drift time provided was predicted.
        /// </summary>
        public bool UseDriftTimePredicted
        {
            get { return m_useDriftTimePredicted; }
        }
        /// <summary>
        /// Gets or sets whether the match was within the refined region.
        /// </summary>
        public bool WithinRefinedRegion
        {
            get { return m_withinRefinedRegion; }
            set { m_withinRefinedRegion = value; }
        }
        /// <summary>
        /// Gets or sets whether the match is a shifted match.
        /// </summary>
        public bool ShiftedMatch
        {
            get { return m_shiftedMatch; }
            set { m_shiftedMatch = value; }
        }

        /// <summary>
        /// Gets or sets the STAC score for the match.
        /// </summary>
        public double STACScore
        {
            get { return m_stacScore; }
            set { m_stacScore = value; }
        }
        /// <summary>
        /// Gets or sets the STAC Specificity of the match.
        /// </summary>
        public double STACSpecificity
        {
            get { return m_stacSpecificity; }
            set { m_stacSpecificity = value; }
        }
        /// <summary>
        /// Gets or sets the SLiC score for the match.
        /// </summary>
        public double SLiCScore
        {
            get { return m_slicScore; }
            set { m_slicScore = value; }
        }
        /// <summary>
        /// Gets or sets the delSLiC for the match.
        /// </summary>
        public double DelSLiC
        {
            get { return m_delSliC; }
            set { m_delSliC = value; }
        }

        /// <summary>
        /// Gets the difference vector between the matched features.  This includes both observed and predicted drift times where appropriate.
        /// </summary>
        public DenseMatrix DifferenceVector
        {
            get { return m_differenceVector; }
        }
        /// <summary>
        /// Gets the distance matrix with only applicable dimensions.
        /// </summary>
        public DenseMatrix ReducedDifferenceVector
        {
            get { return m_reducedDifferenceVector; }
        }

        /// <summary>
        /// Gets the observed feature, i.e. the feature seen in the analysis.
        /// </summary>
        public TObserved ObservedFeature
        {
            get { return m_observedFeature; }
        }
        /// <summary>
        /// Gets the target feature, i.e. the feature that was matched to.
        /// </summary>
        public TTarget TargetFeature
        {
            get { return m_targetFeature; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Parameterless constructor.  Must use AddFeatures function before attempting to use match.
        /// </summary>
        public FeatureMatch()
        {

            m_observedFeature = new TObserved();
            m_targetFeature = new TTarget();
            m_delSliC = 0;
            m_slicScore = 0;
            m_stacScore = 0;
            m_stacSpecificity = 0;
            m_differenceVector = new DenseMatrix(2, 1);
            m_reducedDifferenceVector = m_differenceVector;
            m_useDriftTimePredicted = false;
            m_withinRefinedRegion = false;
            m_shiftedMatch = false;
        }
        /// <summary>
        /// Constructor that takes in all necessary information.
        /// </summary>
        /// <param name="observedFeature">Feature observed in experiment.  Typically a UMC or UMCCluster.</param>
        /// <param name="targetFeature">Feature to match to.  Typically an AMTTag.</param>
        /// <param name="useDriftTime">Whether to use the drift time in distance vectors.</param>
        /// <param name="shiftedMatch">Whether the match is the result of a fixed shift.</param>
        public FeatureMatch(TObserved observedFeature, TTarget targetFeature, bool useDriftTime, bool shiftedMatch)
        {

            m_observedFeature = new TObserved();
            m_targetFeature = new TTarget();
            m_delSliC = 0;
            m_slicScore = 0;
            m_stacScore = 0;
            m_stacSpecificity = 0;
            m_differenceVector = new DenseMatrix(2, 1);
            m_reducedDifferenceVector = m_differenceVector;
            m_useDriftTimePredicted = false;
            m_withinRefinedRegion = false;
            m_shiftedMatch = false;

            AddFeatures(observedFeature, targetFeature, useDriftTime, shiftedMatch);
        }
        #endregion

        #region Comparisons
        /// <summary>
        /// Comparison function for sorting by feature ID.
        /// </summary>
        public static Comparison<FeatureMatch<TObserved, TTarget>> FeatureComparison = delegate(FeatureMatch<TObserved, TTarget> featureMatch1, FeatureMatch<TObserved, TTarget> featureMatch2)
        {
            return featureMatch1.m_observedFeature.Id.CompareTo(featureMatch2.ObservedFeature.Id);
        };
        /// <summary>
        /// Comparison function for sorting by STAC score.
        /// </summary>
        public static Comparison<FeatureMatch<TObserved, TTarget>> STACComparison = delegate(FeatureMatch<TObserved, TTarget> featureMatch1, FeatureMatch<TObserved, TTarget> featureMatch2)
        {
            return featureMatch1.m_stacScore.CompareTo(featureMatch2.STACScore);
        };
        /// <summary>
        /// Comparison function for sorting by STAC score descending.
        /// </summary>
        public static Comparison<FeatureMatch<TObserved, TTarget>> STACComparisonDescending = delegate(FeatureMatch<TObserved, TTarget> featureMatch1, FeatureMatch<TObserved, TTarget> featureMatch2)
        {
            return featureMatch2.m_stacScore.CompareTo(featureMatch1.STACScore);
        };
        #endregion

        #region Private functions
        /// <summary>
        /// Sets internal flag as to whether drift time or predicted drift time is used.
        /// </summary>
        /// <param name="useDriftTime"></param>
        private void SetFlags(bool useDriftTime)
        {
            if (useDriftTime)
            {
                if (m_targetFeature.DriftTime == 0)
                {
                    m_useDriftTimePredicted = true;
                }
            }
        }
        #endregion

        #region Public functions

        /// <summary>
        /// Add (or replace) features in a match.
        /// </summary>
        /// <param name="observedFeature">Feature observed in experiment.  Typically a UMC or UMCCluster.</param>
        /// <param name="targetFeature">Feature to match to.  Typically an AMTTag.</param>
        /// <param name="useDriftTime">Whether to use the drift time in distance vectors.</param>
        /// <param name="shiftedMatch">Whether the match is the result of a fixed shift.</param>
        public void AddFeatures(TObserved observedFeature, TTarget targetFeature, bool useDriftTime, bool shiftedMatch)
        {

            m_observedFeature = observedFeature;
            m_targetFeature = targetFeature;
            m_useDriftTime = useDriftTime;
            m_shiftedMatch = shiftedMatch;
            SetDifferenceMatrices();
        }
        /// <summary>
        /// Sets the internal flag as to whether the match is within the given tolerances.
        /// </summary>
        /// <param name="tolerances">Tolerances to use for matching.</param>
        /// <param name="useElllipsoid">Whether to use ellipsoidal region for matching.</param>
        /// <returns></returns>
        public bool InRegion(FeatureMatcherTolerances tolerances, bool useElllipsoid)
        {
            if (m_targetFeature == new TTarget())
            {
                throw new InvalidOperationException("Match must be populated before using functions involving the match.");
            }
            var toleranceMatrix = tolerances.AsVector(true);
            if (m_reducedDifferenceVector != new DenseMatrix(2, 1))
            {
                var dimensions = m_reducedDifferenceVector.RowCount;

                if (useElllipsoid)
                {
                    double distance = 0;
                    for (var i = 0; i < dimensions; i++)
                    {
                        distance += m_reducedDifferenceVector[i, 0] * m_reducedDifferenceVector[i, 0] / toleranceMatrix[i, 0] / toleranceMatrix[i, 0];
                    }
                    m_withinRefinedRegion = (distance <= 1);
                }
                else
                {
                    var truthValue = true;
                    for (var i = 0; i < dimensions; i++)
                    {
                        truthValue = (truthValue && Math.Abs(m_reducedDifferenceVector[i, 0]) <= toleranceMatrix[i, 0]);
                    }
                    m_withinRefinedRegion = truthValue;
                }
            }
            else
            {
                if (useElllipsoid)
                {
                    double distance = 0;
                    var massDiff = m_observedFeature.MassMonoisotopicAligned - m_targetFeature.MassMonoisotopicAligned;
                    var netDiff = m_observedFeature.NetAligned - m_targetFeature.NetAligned;
                    distance += massDiff * massDiff / toleranceMatrix[0, 0] / toleranceMatrix[0, 0];
                    distance += netDiff * netDiff / toleranceMatrix[1, 0] / toleranceMatrix[1, 0];
                    // TODO: Add drift time difference.
                    m_withinRefinedRegion = (distance <= 1);
                }
            }
            return m_withinRefinedRegion;
        }

        private void SetDifferenceMatrices()
        {
            m_reducedDifferenceVector = MatrixUtilities.Differences(m_observedFeature, m_targetFeature, m_useDriftTime);
            m_differenceVector = MatrixUtilities.Differences(m_observedFeature, m_targetFeature, m_useDriftTime);
            SetFlags(m_useDriftTime);
        }
        #endregion
    }
}
