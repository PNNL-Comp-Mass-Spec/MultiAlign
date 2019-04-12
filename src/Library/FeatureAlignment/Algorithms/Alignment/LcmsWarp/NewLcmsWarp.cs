using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Algorithms.Alignment.LcmsWarp.MassCalibration;
using FeatureAlignment.Algorithms.Alignment.LcmsWarp.NetCalibration;
using FeatureAlignment.Data.Alignment;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;

namespace FeatureAlignment.Algorithms.Alignment.LcmsWarp
{
    /// <summary>
    /// This class was intended to be the entry point for MultiAlign's implementation of the LcmsWarp alignment algorithm.
    /// </summary>
    [Obsolete("Unused development code")]
    public class NewLcmsWarp :
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>,       // Align features to baseline dataset
        IFeatureAligner<IEnumerable<MassTagLight>, IEnumerable<UMCLight>, AlignmentData>    // Align features to AMT tag database
    {
        /// <summary>
        /// The alignment configuration options.
        /// </summary>
        private readonly LcmsWarpAlignmentOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewLcmsWarp"/> class.
        /// </summary>
        /// <param name="options">The alignment configuration options.</param>
        public NewLcmsWarp(LcmsWarpAlignmentOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// An event for updating the progress of LcmsWarp.
        /// </summary>
        [Obsolete("Use IProgress<PRISM.ProgressData> in the Align() methods instead of this.")]
        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        /// Align a set of features to a set of baseline features.
        /// </summary>
        /// <param name="baseline">The baseline features to align to.</param>
        /// <param name="alignee">The features to align.</param>
        /// <param name="progress">The progress reporter for the alignment process.</param>
        /// <returns>Information about the alignment.</returns>
        public AlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee, IProgress<PRISM.ProgressData> progress = null)
        {
            // Enumerate features to lists.
            // Perform a deep copy of the LCMS features so that the original features are unaffected by the warp.
            var aligneeFeatures = alignee.Select(feature => new UMCLight(feature)).ToList();
            var baselineFeatures = baseline.ToList();

            // Warp mass if NET_MASS_WARP, otherwise only warp NET.
            var netMassWarp = options.AlignType == LcmsWarpAlignmentType.NET_MASS_WARP;
            var alignmentData = netMassWarp ? WarpNetMass(aligneeFeatures, baselineFeatures)
                                            : WarpNet(aligneeFeatures, baselineFeatures, true);

            // TODO: Change this to return the new alignment data object.
            var aData = new AlignmentData
            {
                AlignmentFunctions = new Dictionary<SeparationTypes, LcmsWarpResults>()
            };
            return aData;
        }

        /// <summary>
        /// Align a set of features to a set of mass tags.
        /// </summary>
        /// <param name="baseline">The baseline mass tags to align to.</param>
        /// <param name="alignee">The features to align.</param>
        /// <param name="progress">The progress reporter for the alignment process.</param>
        /// <returns>Information about the alignment.</returns>
        public AlignmentData Align(IEnumerable<MassTagLight> baseline, IEnumerable<UMCLight> alignee, IProgress<PRISM.ProgressData> progress = null)
        {
            // Convert baseline features to UMCLights.
            // A mass tag database is really just a list of features just like a baseline dataset.
            // So, we may want to consider doing this conversion outside of LCMS warp because that would cut some bloat here.
            var baselineUMCs = baseline.Select(
                baselineFeature => new UMCLight
                {
                    MassMonoisotopic = baselineFeature.MassMonoisotopic,
                    Net = baselineFeature.NetAligned
                });

            return Align(baselineUMCs, alignee, progress);
        }

        /// <summary>
        /// Generates alignment functions for alignment between features in each separation dimension.
        /// Warps the elution value for each feature based on the alignment function.
        /// </summary>
        /// <param name="aligneeFeatures">The features to warp.</param>
        /// <param name="baselineFeatures">The features to warp to.</param>
        /// <param name="includeMassInMatchScore">
        /// Should mass be considered when scoring a match between an alignee feature and baseline feature?
        /// </param>
        /// <returns>The list of alignment results for each separation dimension.</returns>
        public NewAlignmentData WarpNet(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures, bool includeMassInMatchScore)
        {
            // Generate candidate matches: Match alignee features -> baseline features by mass only
            var featureMatcher = new LcmsWarpFeatureMatcher(options);
            featureMatcher.GenerateCandidateMatches(aligneeFeatures, baselineFeatures);

            var warpedFeatures = new List<UMCLight>(aligneeFeatures);
            var results = new Dictionary<SeparationTypes, LcmsWarpResults>();

            // Perform warp on each separation dimension.
            foreach (var separationType in options.SeparationTypes)
            {
                // Get matches for given separation dimension
                var matches = featureMatcher.GetMatchesAs(separationType);

                // Calculate two dimensional statistics for mass and the current separation dimension.
                var statistics = LcmsWarpStatistics.CalculateAndGetStatistics(matches);

                // Calculate alignee sections
                var aligneeSections = new LcmsWarpSectionInfo(options.NumTimeSections);
                aligneeSections.InitSections(aligneeFeatures);

                // Calculate baseline sections
                var baselineSections = new LcmsWarpSectionInfo(options.NumTimeSections * options.ContractionFactor);
                baselineSections.InitSections(baselineFeatures);

                // Generate alignment function, only score sections based on NET.
                var alignmentScorer = new LcmsWarpAlignmentScorer(options, includeMassInMatchScore, statistics);
                var alignmentFunction = alignmentScorer.GetAlignment(aligneeSections, baselineSections, matches);
                alignmentFunction.SeparationType = separationType;

                // Create alignment score HeatMap
                var alignmentScoreHeatMap = LcmsWarpPlotDataCreator.GetAlignmentHeatMap(
                        alignmentScorer.AlignmentScoreMatrix,
                        true,
                        options.NumTimeSections,
                        options.NumBaselineSections,
                        options.MaxExpansionWidth);

                // Warp the values in the features for this separation type
                warpedFeatures = alignmentFunction.GetWarpedFeatures(warpedFeatures).ToList();
                var dimensionResults = new LcmsWarpResults
                {
                    AlignmentFunction = alignmentFunction,
                    Statistics = statistics,
                    AlignmentScoreHeatMap = alignmentScoreHeatMap
                };

                results.Add(separationType, dimensionResults);
            }

            return new NewAlignmentData
            {
                SeparationAlignments = results,
                AlignedFeatures = warpedFeatures,
            };
        }

        /// <summary>
        /// Generates alignment functions for alignment between features in each separation dimension.
        /// Warps the elution value for each feature based on the alignment function.
        /// </summary>
        /// <param name="aligneeFeatures">The features to warp.</param>
        /// <param name="baselineFeatures">The features to warp to.</param>
        /// <returns>The features with all NET values warped.</returns>
        public NewAlignmentData WarpNetMass(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures)
        {
            // First pass NET warp: Perform warp by only scoring matches in NET
            var netWarpedFirstPass = WarpNet(aligneeFeatures, baselineFeatures, false);

            // Generate matches from features by matching in both mass and NET
            var featureMatcher = new LcmsWarpFeatureMatcher(options);
            featureMatcher.GenerateCandidateMatches(
                            netWarpedFirstPass.AlignedFeatures,
                            baselineFeatures,
                            options.SeparationTypes);
            var matches = featureMatcher.Matches;

            // Calculate mass alignment
            var massCalibrator = MassCalibrationFactory.GetCalibrator(options);
            var massCalibrations = massCalibrator.CalculateCalibration(matches);

            // Warp features.
            var warpedFeatures = massCalibrations.GetWarpedFeatures(aligneeFeatures).ToList();

            // Calculate histograms/plots for mass alignment
            var massErrorHistogram = LcmsWarpPlotDataCreator.GetMassErrorHistogram(matches, 10);

            // Create results object
            var massAlignmentResult = new LcmsWarpResults
            {
                AlignmentFunction = massCalibrations,
                ErrorHistogram = massErrorHistogram,
            };

            // Second pass NET warp: Perform warp that scores matches in mass AND NET
            var netMassWarpedSecondPass = WarpNet(warpedFeatures, baselineFeatures, true);

            // Add mass alignment results to existing alignment results from the NET alignment.
            netMassWarpedSecondPass.MassAlignment = massAlignmentResult;
            return netMassWarpedSecondPass;
        }
    }
}
