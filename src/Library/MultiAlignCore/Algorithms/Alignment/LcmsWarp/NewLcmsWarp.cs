namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Algorithms.Alignment.LcmsWarp.MassCalibration;
    using MultiAlignCore.Algorithms.Alignment.LcmsWarp.NetCalibration;
    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MassTags;

    /// <summary>
    /// This class is the entry point for MultiAlign's implementation of the LcmsWarp alignment algorithm.
    /// </summary>
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
        [Obsolete("Use IProgress<ProgressData> in the Align() methods instead of this.")]
        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        /// Align a set of features to a set of baseline features.
        /// </summary>
        /// <param name="baseline">The baseline features to align to.</param>
        /// <param name="alignee">The features to align.</param>
        /// <param name="progress">The progress reporter for the alignment process.</param>
        /// <returns>Information about the alignment.</returns>
        public AlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            // Enumerate features to lists.
            // Perform a deep copy of the LCMS features so that the original features are unaffected by the warp.
            var aligneeFeatures = alignee.Select(feature => new UMCLight(feature)).ToList();
            var baselineFeatures = baseline.ToList();

            // Warp mass if NET_MASS_WARP, otherwise only warp NET.
            var netMassWarp = this.options.AlignType == LcmsWarpAlignmentType.NET_MASS_WARP;
            var alignmentData = netMassWarp ? this.WarpNetMass(aligneeFeatures, baselineFeatures)
                                            : this.WarpNet(aligneeFeatures, baselineFeatures, true);

            // TODO: Change this to return the new alignment data object.
            var aData = new AlignmentData();
            return aData;
        }

        /// <summary>
        /// Align a set of features to a set of mass tags.
        /// </summary>
        /// <param name="baseline">The baseline mass tags to align to.</param>
        /// <param name="alignee">The features to align.</param>
        /// <param name="progress">The progress reporter for the alignment process.</param>
        /// <returns>Information about the alignment.</returns>
        public AlignmentData Align(IEnumerable<MassTagLight> baseline, IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            // Convert baseline features to UMCLights.
            // A mass tag database is really just a list of features just like a baseline dataset.
            // So, we may want to consider doing this conversion outside of LCMS warp because that would cut some bloat here.
            var baselineUmcs = baseline.Select(
                baselineFeature => new UMCLight
                {
                    MassMonoisotopic = baselineFeature.MassMonoisotopic,
                    Net = baselineFeature.NetAligned
                });

            return this.Align(baselineUmcs, alignee, progress);
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
            var featureMatcher = new LcmsWarpFeatureMatcher(this.options);
            featureMatcher.GenerateCandidateMatches(aligneeFeatures, baselineFeatures);

            var warpedFeatures = new List<UMCLight>(aligneeFeatures);
            var results = new Dictionary<FeatureLight.SeparationTypes, LcmsWarpResults>();

            // Perform warp on each separation dimension.
            foreach (var separationType in this.options.SeparationTypes)
            {   
                // Get matches for given separation dimension
                var matches = featureMatcher.GetMatchesAs(separationType);

                // Calculate the alignment for the current separation dimension.
                var alignmentFunction = this.GetAlignmentFunction(
                                                                  matches,
                                                                  aligneeFeatures,
                                                                  baselineFeatures,
                                                                  includeMassInMatchScore);
                alignmentFunction.SeparationType = separationType;

                // Warp the values in the features for this separation type
                warpedFeatures = alignmentFunction.GetWarpedFeatures(warpedFeatures).ToList();
                var dimensionResults = new LcmsWarpResults
                {
                    AlignmentFunction = alignmentFunction,
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
            var netWarpedFirstPass = this.WarpNet(aligneeFeatures, baselineFeatures, false);

            // Generate matches from features by matching in both mass and NET
            var featureMatcher = new LcmsWarpFeatureMatcher(this.options);
            featureMatcher.GenerateCandidateMatches(
                            netWarpedFirstPass.AlignedFeatures,
                            baselineFeatures,
                            this.options.SeparationTypes);
            var matches = featureMatcher.Matches;

            // Calculate mass alignment
            var massCalibrator = MassCalibrationFactory.GetCalibrator(this.options);
            var massCalibrations = massCalibrator.CalculateCalibration(matches);

            // Warp features.
            var warpedFeatures = massCalibrations.GetWarpedFeatures(aligneeFeatures).ToList();
            var massAlignmentResult = new LcmsWarpResults { AlignmentFunction = massCalibrations };

            // TODO: calculate histograms/plots for mass alignment
            ////massAlignmentResult.ErrorHistogram - LcmsWarpPlotDataCreator.

            // Second pass NET warp: Perform warp that scores matches in mass AND net
            var netMassWarpedSecondPass = this.WarpNet(warpedFeatures, baselineFeatures, true);

            // Add mass alignment results to existing alignment results from the NET alignment.
            netMassWarpedSecondPass.MassAlignment = massAlignmentResult;
            return netMassWarpedSecondPass;
        }

        /// <summary>
        /// Get alignment function for a single separation for a set of alignee features.
        /// </summary>
        /// <param name="matches">The matched features.</param>
        /// <param name="aligneeFeatures">The features to warp.</param>
        /// <param name="baselineFeatures">The features to warp to.</param>
        /// <param name="includeMassInMatchScore">
        /// Should mass be considered when scoring a match between an alignee feature and baseline feature?
        /// </param>
        /// <returns>The features with all values for the given separation dimension warped.</returns>
        /// <returns></returns>
        private LcmsWarpNetAlignmentFunction GetAlignmentFunction(
                                                                  List<LcmsWarpFeatureMatch> matches,
                                                                  List<UMCLight> aligneeFeatures,
                                                                  List<UMCLight> baselineFeatures,
                                                                  bool includeMassInMatchScore)
        {
            // Calculate two dimensional statistics for mass and the current separation dimension.
            var statistics = LcmsWarpStatistics.CalculateAndGetStatistics(matches);

            // Calculate alignee sections
            var aligneeSections = new LcmsWarpSectionInfo(this.options.NumTimeSections);
            aligneeSections.InitSections(aligneeFeatures);

            // Calculate baseline sections
            var baselineSections = new LcmsWarpSectionInfo(this.options.NumTimeSections * this.options.ContractionFactor);
            baselineSections.InitSections(baselineFeatures);

            // Generate alignment function, only score sections based on NET.
            var alignmentScorer = new LcmsWarpAlignmentScorer(this.options, includeMassInMatchScore, statistics);
            var alignmentFunction = alignmentScorer.GetAlignment(aligneeSections, baselineSections, matches);

            return alignmentFunction;
        }
    }
}
