using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Algorithms.Regression;
using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Algorithms.FeatureMatcher.Data;
using MultiAlignCore.Algorithms.Regression;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Alignment
{
    public static class DriftTimeAlignment<TTarget, TObserved>
        where TTarget   : FeatureLight, new()
        where TObserved : FeatureLight, new()
    {
        // Set this to an extremely high value so that drift time difference will not be considered when matching
        private const float DRIFT_TIME_TOLERANCE = 100f;

        /// <summary>
        /// Aligns features to the baseline correcting for drift time.
        /// </summary>
        /// <param name="fullObservedEnumerable">All features.</param>
        /// <param name="observedEnumerable">Filtered features to use for drift time correction.</param>
        /// <param name="targetEnumerable">Expected features that should be filtered.</param>
        /// <param name="massTolerance">PPM Mass Tolerance.</param>
        /// <param name="netTolerance">Normalized Elution Time Tolerance.</param>
        public static DriftTimeAlignmentResults<TTarget, TObserved> AlignObservedEnumerable(IEnumerable<UMCLight> fullObservedEnumerable, IEnumerable<TTarget> observedEnumerable, IEnumerable<TObserved> targetEnumerable, double massTolerance, double netTolerance)
        {

            // Setup Tolerance for Feature Matching
            var featureMatcherParameters   = new FeatureMatcherParameters();
            featureMatcherParameters.SetTolerances(massTolerance, netTolerance, DRIFT_TIME_TOLERANCE);
            featureMatcherParameters.UseDriftTime               = true;

            // Find all matches based on defined tolerances
            var featureMatcher = new FeatureMatcher.FeatureMatcher<TTarget, TObserved>(observedEnumerable.ToList(), targetEnumerable.ToList(), featureMatcherParameters);
            var matchList  = featureMatcher.FindMatches(observedEnumerable.ToList(), targetEnumerable.ToList(), featureMatcherParameters.UserTolerances, 0);

            // Create <ObservedDriftTime, TargetDriftTime> XYData List
            var xyDataList = new List<XYData>();
            foreach (var featureMatch in matchList)
            {
                var xyData = new XYData(featureMatch.ObservedFeature.DriftTime, featureMatch.TargetFeature.DriftTime);
                xyDataList.Add(xyData);
            }

            var linearRegression = new LinearRegressionModel();

            // Find the Linear Equation for the <ObservedDriftTime, TargetDriftTime> XYData List
            var linearEquation = linearRegression.CalculateRegression(xyDataList);

            // Set the Aligned Drift Time value for each of the observed Features, even if they were not found in matching
            foreach (var observedT in fullObservedEnumerable)
            {
                observedT.DriftTimeAligned = linearRegression.Transform(linearEquation, observedT.DriftTime);
            }

            var results = new DriftTimeAlignmentResults<TTarget, TObserved>(matchList, linearEquation);

            return results;
        }
        /// <summary>
        /// Does a zero mean drift time correction.
        /// </summary>
        /// <param name="observedEnumerable">All observed features to shift that should already be drift time aligned.</param>
        /// <param name="targetEnumerable">Expected features</param>
        /// <param name="massTolerance">PPM Mass Tolerance</param>
        /// <param name="netTolerance">Normalized Elution Time tolerance.</param>
        /// <param name="driftTimeTolerance">Drift time tolerance to use.</param>
        public static DriftTimeAlignmentResults<TTarget,TObserved> CorrectForOffset(IEnumerable<TTarget> observedEnumerable, IEnumerable<TObserved> targetEnumerable, double massTolerance, double netTolerance, double driftTimeTolerance)
        {
            // Setup Tolerance for Feature Matching
            var featureMatcherParameters = new FeatureMatcherParameters();
            featureMatcherParameters.SetTolerances(massTolerance, netTolerance, (float)driftTimeTolerance);
            featureMatcherParameters.UseDriftTime = true;

            // Find all matches based on defined tolerances
            var featureMatcher = new FeatureMatcher.FeatureMatcher<TTarget, TObserved>(observedEnumerable.ToList(), targetEnumerable.ToList(), featureMatcherParameters);
            var matchList = featureMatcher.FindMatches(observedEnumerable.ToList(), targetEnumerable.ToList(), featureMatcherParameters.UserTolerances, 0);

            // Create List of Drift Time differences
            var differenceList = new List<double>(matchList.Count);
            foreach (var featureMatch in matchList)
            {
                var observedFeature = featureMatch.ObservedFeature;
                var targetFeature = featureMatch.TargetFeature;

                double observedDriftTime;
                if (observedFeature.DriftTimeAligned != double.NaN && observedFeature.DriftTimeAligned > 0.0)
                {
                    observedDriftTime = observedFeature.DriftTimeAligned;
                }
                else
                {
                    observedDriftTime = observedFeature.DriftTime;
                }

                double targetDriftTime;
                if (!double.IsNaN(targetFeature.DriftTimeAligned) && targetFeature.DriftTimeAligned > 0.0)
                {
                    targetDriftTime = targetFeature.DriftTimeAligned;
                }
                else
                {
                    targetDriftTime = targetFeature.DriftTime;
                }

                differenceList.Add(observedDriftTime - targetDriftTime);
            }

            // Create bins for histogram
            var bins = new List<double>();
            for (var i = -driftTimeTolerance; i <= driftTimeTolerance; i += (driftTimeTolerance / 100.0))
            {
                bins.Add(i);
            }
            bins.Add(driftTimeTolerance);

            // Group drift time differences into the bins
            var groupings = differenceList.GroupBy(difference => bins.First(bin => bin >= difference));

            // Order the groupings by their count, so the group with the highest count will be first
            var orderGroupingsByCount = from singleGroup in groupings
                                        orderby singleGroup.Count() descending
                                        select singleGroup;

            // Grab the drift time from the group with the most counts
            var driftTimeOffset = orderGroupingsByCount.First().Key;

            // Update all of the observed features with the new drift time
            foreach (var observedFeature in observedEnumerable)
            {
                if (!double.IsNaN(observedFeature.DriftTimeAligned) && observedFeature.DriftTimeAligned > 0.0)
                {
                    observedFeature.DriftTimeAligned -= driftTimeOffset;
                }
                else
                {
                    observedFeature.DriftTime -= (float)driftTimeOffset;
                }
            }

            var linearEquation           = new LinearRegressionResult {Slope = 0, Intercept = driftTimeOffset};
            var results = new DriftTimeAlignmentResults<TTarget, TObserved>(matchList, linearEquation);

            return results;
        }
    }
}
