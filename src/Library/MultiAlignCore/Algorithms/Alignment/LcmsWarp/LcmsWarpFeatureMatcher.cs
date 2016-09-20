using System;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using System.Collections.Generic;
    using System.Linq;
    using MultiAlignCore.Data.Features;

    /// <summary>
    /// This class matches alignee features to baseline features.
    /// </summary>
    public class LcmsWarpFeatureMatcher
    {
        /// <summary>
        /// Options for the LCMSWarp alignment algorithm.
        /// </summary>
        private readonly LcmsWarpAlignmentOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="LcmsWarpFeatureMatcher" /> class.
        /// </summary>
        /// <param name="options">Options for the LCMSWarp alignment algorithm.</param>
        public LcmsWarpFeatureMatcher(LcmsWarpAlignmentOptions options)
        {
            this.options = options;
            this.Matches = new List<LcmsWarpFeatureMatch>();
        }

        /// <summary>
        /// Gets the list of matches between alignee and baseline features.
        /// </summary>
        public List<LcmsWarpFeatureMatch> Matches { get; private set; }

        /// <summary>
        /// Gets the matches where the NET value is the given separation type.
        /// </summary>
        /// <param name="separationType">The separation type to get matches as.</param>
        /// <returns>List of feature matches.</returns>
        public List<LcmsWarpFeatureMatch> GetMatchesAs(FeatureLight.SeparationTypes separationType)
        {
            return this.Matches.Select(
                    match =>
                    new LcmsWarpFeatureMatch
                        {
                            AligneeFeature = match.AligneeFeature,
                            BaselineFeature = match.BaselineFeature,
                            Net = match.AligneeFeature.GetSeparationValue(separationType),
                            BaselineNet = match.AligneeFeature.GetSeparationValue(separationType)
                        }).ToList();
        }

        /// <summary>
        /// Generates candidate matches between alignee features and baseline features.
        /// It does so by finding all alignee-baseline feature pairs that match within a provided
        /// mass tolerance window.
        /// This method matches in mass only.
        /// </summary>
        /// <param name="aligneeFeatures"></param>
        /// <param name="baselineFeatures"></param>
        /// <param name="separationTypes">Separation types to include in matching.</param>
        public void GenerateCandidateMatches(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures, IEnumerable<FeatureLight.SeparationTypes> separationTypes = null)
        {
            // Sort features by mass
            var massComparer = new UMCLight.UmcMassComparer();
            aligneeFeatures.Sort(massComparer);
            baselineFeatures.Sort(massComparer);

            // Go through each MassTimeFeature and see if the next baseline MassTimeFeature matches it
            var baselineFeatureIndex = 0;

            var featureMatches = new List<LcmsWarpFeatureMatch>();

            foreach (var aligneeFeature in aligneeFeatures)
            {
                // Convert tolerance from ppm to Dalton
                var massToleranceDa = aligneeFeature.MassMonoisotopic * this.options.MassTolerance / 1000000;

                // Backtrack baselineFeatureIndex while the baseline feature's mass is greater than the candidate feature's mass minus massToleranceDa
                while (baselineFeatureIndex == baselineFeatures.Count || baselineFeatureIndex >= 0 &&
                       (baselineFeatures[baselineFeatureIndex].MassMonoisotopic > aligneeFeature.MassMonoisotopic - massToleranceDa))
                {
                    baselineFeatureIndex--;
                }
                baselineFeatureIndex++;

                // Add candidate matches
                while (baselineFeatureIndex < baselineFeatures.Count &&
                       (baselineFeatures[baselineFeatureIndex].MassMonoisotopic <
                        (aligneeFeature.MassMonoisotopic + massToleranceDa)))
                {
                    var baselineFeature = baselineFeatures[baselineFeatureIndex];
                    if (baselineFeature.MassMonoisotopic >
                        (aligneeFeature.MassMonoisotopic - massToleranceDa))
                    {   // Feature is within mass tolerance, add the match.
                        var matchToAdd = new LcmsWarpFeatureMatch
                        {
                            AligneeFeature = aligneeFeature,
                            BaselineFeature = baselineFeature,
                            Net = aligneeFeature.Net,
                            BaselineNet = baselineFeature.Net
                        };

                        featureMatches.Add(matchToAdd);
                    }
                    baselineFeatureIndex++;
                }
            }

            // Filter out ambiguous matches
            featureMatches = this.RemovePromiscuousMatches(featureMatches);

            this.Matches = featureMatches;
        }

        /// <summary>
        /// Generates matches between alignee and baseline features in ALL dimensions.
        /// </summary>
        /// <param name="aligneeFeatures"></param>
        /// <param name="baselineFeatures"></param>
        /// <param name="netStdDev"></param>
        /// <param name="massStdDev"></param>
        [Obsolete("This function is superseded by CalculateAlignmentMatches in MultiAlignCore.Algorithms.Alignment.LcmsWarp")]
        public List<LcmsWarpFeatureMatch> CalculateAlignmentMatches(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures, double netStdDev, double massStdDev)
        {
            // Sort features by mass
            var massComparer = new UMCLight.UmcMassComparer();
            aligneeFeatures.Sort(massComparer);
            baselineFeatures.Sort(massComparer);

            var baselineFeatureIndex = 0;

            var featureMatches = new List<LcmsWarpFeatureMatch>();

            var minMatchScore = -0.5 * (this.options.MassTolerance * this.options.MassTolerance) / (massStdDev * massStdDev);
            minMatchScore -= 0.5 * (this.options.NetTolerance * this.options.NetTolerance) / (netStdDev * netStdDev);

            foreach (var aligneeFeature in aligneeFeatures)
            {
                // Convert tolerance from ppm to Dalton
                var massTolerance = aligneeFeature.MassMonoisotopic * this.options.MassTolerance / 1000000;

                while (baselineFeatureIndex == baselineFeatures.Count || baselineFeatureIndex >= 0 &&
                       baselineFeatures[baselineFeatureIndex].MassMonoisotopic >
                       aligneeFeature.MassMonoisotopic - massTolerance)
                {
                    baselineFeatureIndex--;
                }
                baselineFeatureIndex++;

                LcmsWarpFeatureMatch bestMatchFeature = null;
                var bestMatchScore = minMatchScore;
                while (baselineFeatureIndex < baselineFeatures.Count &&
                       baselineFeatures[baselineFeatureIndex].MassMonoisotopic <
                       aligneeFeature.MassMonoisotopic + massTolerance)
                {
                    var baselineFeature = baselineFeatures[baselineFeatureIndex];
                    if (baselineFeature.MassMonoisotopic >
                        aligneeFeature.MassMonoisotopic - massTolerance)
                    {
                        // Calculate the mass and net errors
                        // Compute as observedValue - expectedValue
                        var netDiff = aligneeFeature.NetAligned - baselineFeature.Net;
                        var driftDiff = aligneeFeature.DriftTime - baselineFeature.DriftTime;
                        var massDiff = aligneeFeature.MassMonoisotopic - baselineFeature.MassMonoisotopic;
                        var massDiffPpm = massDiff * 1000000.0 / baselineFeature.MassMonoisotopic;

                        var massDiffOriginal = aligneeFeature.MassMonoisotopicOriginal - baselineFeature.MassMonoisotopic;
                        var originalMassDiffPpm = massDiffOriginal * 1000000.0 / baselineFeature.MassMonoisotopic;

                        // Calculate the match score.
                        var matchScore = -0.5 * (netDiff * netDiff) / (netStdDev * netStdDev);
                        matchScore -= 0.5 * (massDiffPpm * massDiffPpm) / (massStdDev * massStdDev);

                        // If the match score is greater than the best match score, update the holding item.
                        if (matchScore > bestMatchScore)
                        {
                            bestMatchScore = matchScore;
                            bestMatchFeature = new LcmsWarpFeatureMatch
                            {
                                AligneeFeature = aligneeFeature,
                                BaselineFeature = baselineFeature,
                                Net = aligneeFeature.Net,
                                NetError = netDiff,
                                MassError = massDiff,
                                PpmMassError = massDiffPpm,
                                PpmMassErrorOriginal = originalMassDiffPpm,
                                DriftError = driftDiff,
                                BaselineNet = baselineFeatures[baselineFeatureIndex].Net
                            };
                        }
                    }
                    baselineFeatureIndex++;
                }

                // If we found a match, add it to the list of matches.
                if (bestMatchFeature != null)
                {
                    featureMatches.Add(bestMatchFeature);
                }
            }

            return featureMatches;
        }

        /// <summary>
        /// Determine which matches are promiscuous.
        /// A promiscuous match is a match such that more than N (N = options.MaxPromiscuity)
        /// alignee features map to a single baseline feature.
        ///
        /// N points per promiscuous match are kept if options.UsePromiscuous points is set to true.
        /// Otherwise, the promiscuous matches are removed completely.
        /// </summary>
        /// <param name="featureMatches">Features to remove promiscuous matches from.</param>
        /// <returns>List with promiscuous matches removed.</returns>
        private List<LcmsWarpFeatureMatch> RemovePromiscuousMatches(List<LcmsWarpFeatureMatch> featureMatches)
        {
            // Map baseline feature indices to the matches they are a part of
            var baselineFeatureToMatches = new Dictionary<int, List<LcmsWarpFeatureMatch>>();
            foreach (var match in featureMatches)
            {
                if (!baselineFeatureToMatches.ContainsKey(match.BaselineFeatureIndex))
                {
                    baselineFeatureToMatches.Add(match.BaselineFeatureIndex, new List<LcmsWarpFeatureMatch>());
                }

                baselineFeatureToMatches[match.BaselineFeatureIndex].Add(match);
            }

            // Now go through each of the baseline features matched and for each one keep at
            // most MaxPromiscuousUmcMatches (or none if KeepPromiscuousMatches is false)
            // keeping only the first MaxPromiscuousUmcMatches by scan
            var matchesToUse = new List<LcmsWarpFeatureMatch> { Capacity = featureMatches.Count };

            foreach (var baselineMatch in baselineFeatureToMatches)
            {
                var baselineIndex = baselineMatch.Key;
                if (baselineFeatureToMatches[baselineIndex].Count <= this.options.MaxPromiscuity)
                {   // Match isn't ambiguous enough to be filtered.
                    matchesToUse.AddRange(baselineMatch.Value);
                }
                else if (this.options.UsePromiscuousPoints)
                {   // Match is ambiguous. Group the matches by NET, keep the first MaxPromiscuity matches at each time point.
                    var netToMatches = new Dictionary<double, List<LcmsWarpFeatureMatch>>();
                    foreach (var match in baselineMatch.Value)
                    {   // TODO: (By Chris) Seems like NETs should probably be rounded here since they are being used as a dictionary key.
                        if (!netToMatches.ContainsKey(match.Net))
                        {   // First time this NET has been seen
                            netToMatches.Add(match.Net, new List<LcmsWarpFeatureMatch>());
                        }

                        // Keep only the first MaxPromiscuity matches at each time point.
                        if (netToMatches[match.Net].Count < this.options.MaxPromiscuity)
                        {
                            netToMatches[match.Net].Add(match);
                            matchesToUse.Add(match);
                        }
                    }
                }
            }

            return matchesToUse;
        }
    }
}
