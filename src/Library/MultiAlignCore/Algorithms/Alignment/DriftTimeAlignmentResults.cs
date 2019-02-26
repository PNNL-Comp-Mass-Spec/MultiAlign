using System.Collections.Generic;
using FeatureAlignment.Algorithms.Regression;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.Algorithms.Regression;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Alignment
{
    /// <summary>
    /// Holds matches from drift time alignments.
    /// </summary>
    public sealed class DriftTimeAlignmentResults<TTarget, TObserved>
        where TTarget   : FeatureLight, new()
        where TObserved : FeatureLight, new()
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="alignmentFunction"></param>
        public DriftTimeAlignmentResults(List<FeatureMatch<TTarget, TObserved>> matches, LinearRegressionResult alignmentFunction)
        {
            Matches             = matches;
            AlignmentFunction   = alignmentFunction;
        }

        #region Properties.
        /// <summary>
        /// Gets the matches made by the drift time alignment algorithm.
        /// </summary>
        public List<FeatureMatch<TTarget, TObserved>> Matches
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the alignment function between the matches.
        /// </summary>
        public LinearRegressionResult AlignmentFunction
        {
            get;
            private set;
        }
        #endregion
    }
}
