using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using MultiAlignCore.Data.Features;

    using PNNLOmics.Utilities;

    public class NewLcmsWarp
    {
        private const int REQUIRED_MATCHES = 6;

        private LcmsWarpAlignmentOptions options;

        public NewLcmsWarp(LcmsWarpAlignmentOptions options)
        {
            
        }

        public List<UMCLight> WarpNet(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures, bool includeMassInMatchScore)
        {
            // Generate candidate matches: Match alignee features -> baseline features by mass only
            var featureMatcher = new LcmsWarpFeatureMatcher(this.options);
            featureMatcher.GenerateCandidateMatches(aligneeFeatures, baselineFeatures);

            var warpedFeatures = new List<UMCLight>();
            foreach (var separationType in this.options.SeparationTypes)
            {
                // Get matches for current separation type
                var matches = featureMatcher.GetMatchesAs(separationType);

                // Calculate two dimensional statistics for mass and the current separation dimension.
                var statistics = this.CalculateStandardDeviations(matches, separationType);

                // Calculate alignee sections
                var aligneeSections = new LcmsWarpSectionInfo(this.options.NumTimeSections);
                aligneeSections.InitSections(aligneeFeatures);

                // Calculate baseline sections
                var baselineSections = new LcmsWarpSectionInfo(this.options.NumTimeSections * this.options.ContractionFactor);
                baselineSections.InitSections(baselineFeatures);

                // Generate alignment function, only score sections based on NET.
                var alignmentScorer = new LcmsWarpAlignmentScorer(this.options, includeMassInMatchScore, statistics);
                var alignmentFunction = alignmentScorer.GetAlignment(aligneeSections, baselineSections, matches);

                // Warp the values in the features for this separation type
                warpedFeatures = this.WarpFeatures(aligneeFeatures, alignmentFunction, separationType);
            }

            // Calculate actual feature matches in mass and all separation dimensions

            return warpedFeatures;
        }

        public List<UMCLight> WarpNetMass(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures)
        {
            // First pass NET warp: Perform warp by only scoring matches in Nnet
            var netWarpedFeatures = this.WarpNet(aligneeFeatures, baselineFeatures, false);

            // Warp mass

            // Second pass NET warp: Perform warp that scores matches in mass AND net
            var netMassWarpedFeatures = this.WarpNet(netWarpedFeatures, baselineFeatures, true);

            return netMassWarpedFeatures;
        }

        /// <summary>
        /// Calculates the Standard deviations of the matches.
        /// Note: method requires more than 6 matches to produce meaningful
        /// results.
        /// </summary>
        private LcmsWarpStatistics CalculateStandardDeviations(List<LcmsWarpFeatureMatch> matches, FeatureLight.SeparationTypes separationType)
        {
            if (matches.Count <= REQUIRED_MATCHES)
            {
                throw new ArgumentException(string.Format("This requires at least {0} matches to produce meaningful results", REQUIRED_MATCHES));
            }

            var massDeltas = new List<double>(matches.Count);
            var netDeltas = new List<double>(matches.Count);
            for (var matchNum = 0; matchNum < matches.Count; matchNum++)
            {
                var match = matches[matchNum];
                var feature = match.AligneeFeature;
                var baselineFeature = match.BaselineFeature;
                massDeltas.Add(((baselineFeature.MassMonoisotopic - feature.MassMonoisotopic) * 1000000) /
                                       feature.MassMonoisotopic);
                netDeltas.Add(baselineFeature.GetSeparationValue(separationType) - feature.NetAligned);
            }

            double normalProb, u, muMass, muNet, massStd, netStd;

            MathUtilities.TwoDem(massDeltas, netDeltas, out normalProb, out u,
                out muMass, out muNet, out massStd, out netStd);

            return new LcmsWarpStatistics
            {
                MassStdDev = massStd,
                NetStdDev = netStd,
                Mu = u,
                NormalProbability = normalProb
            };
        }

        private List<UMCLight> WarpFeatures(List<UMCLight> features, LcmsWarpNetAlignmentFunction alignmentFunction, FeatureLight.SeparationTypes separationType)
        {
            var warpedFeatures = new List<UMCLight> { Capacity = features.Count };
            foreach (var feature in features)
            {
                var warpedFeature = new UMCLight(feature);
                var separationValue = feature.GetSeparationValue(separationType);
                var warpedValue = alignmentFunction.WarpNet(separationValue);
                warpedFeature.SetSeparationValue(separationType, warpedValue);
            }

            return warpedFeatures;
        }
    }
}
