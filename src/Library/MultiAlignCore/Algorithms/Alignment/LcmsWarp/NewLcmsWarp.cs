namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MassTags;

    /// <summary>
    /// This class is the entry point for MultiAlign's implementation of the LcmsWarp
    /// alignment algorithm
    /// </summary>
    public class NewLcmsWarp :
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>,
        IFeatureAligner<IEnumerable<MassTagLight>, IEnumerable<UMCLight>, AlignmentData>
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
        /// <exception cref="ArgumentException">
        /// Throws an argument exception if the alignee and reference datasets do not have equivalent dimensionality.
        /// </exception>
        public AlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            // Enumerate features to lists.
            var aligneeFeatures = alignee.ToList();
            var baselineFeatures = baseline.ToList();

            // Throw an exception if separation dimensions in basline and alignee features do not match.
            this.CheckDimensionality(aligneeFeatures, baselineFeatures);

            // Warp mass if NET_MASS_WARP, otherwise only warp NET.
            var netMassWarp = this.options.AlignType == LcmsWarpAlignmentType.NET_MASS_WARP;
            var warpedFeatures = netMassWarp ? this.WarpNetMass(aligneeFeatures, baselineFeatures)
                                             : this.WarpNet(aligneeFeatures, baselineFeatures, true);

            // TODO: Create Plots


            // TODO: Populate alignment data.
            var alignmentData = new AlignmentData
            {
                AlignmentFunctions = new Dictionary<FeatureLight.SeparationTypes, LcmsWarpResults>()
            };
            //var netAlignmentFunction = GetAlignmentFunction()
            //alignmentData.AlignmentFunctions.Add();

            return alignmentData;
        }

        /// <summary>
        /// Align a set of features to a set of mass tags.
        /// </summary>
        /// <param name="baseline">The baseline mass tags to align to.</param>
        /// <param name="alignee">The features to align.</param>
        /// <param name="progress">The progress reporter for the alignment process.</param>
        /// <returns>Information about the alignment.</returns>
        /// <exception cref="ArgumentException">
        /// Throws an argument exception if the alignee and reference datasets do not have equivalent dimensionality.
        /// </exception>
        public AlignmentData Align(IEnumerable<MassTagLight> baseline, IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            // Convert baseline features to UMCLights.
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
        /// <returns>The features with all NET values warped.</returns>
        public List<UMCLight> WarpNet(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures, bool includeMassInMatchScore)
        {
            // Generate candidate matches: Match alignee features -> baseline features by mass only
            var featureMatcher = new LcmsWarpFeatureMatcher(this.options);
            featureMatcher.GenerateCandidateMatches(aligneeFeatures, baselineFeatures);

            var warpedFeatures = new List<UMCLight>();
            foreach (var separationType in this.options.SeparationTypes)
            {   
                // Get matches for given separation dimension
                var matches = featureMatcher.GetMatchesAs(separationType);

                // Warp features for each separation dimension.
                var alignmentFunction = this.GetAlignmentFunction(
                                                                  matches,
                                                                  aligneeFeatures,
                                                                  baselineFeatures,
                                                                  includeMassInMatchScore);
                alignmentFunction.SeparationType = separationType;

                // Warp the values in the features for this separation type
                warpedFeatures = alignmentFunction.GetWarpedFeatures(aligneeFeatures).ToList();
            }

            return warpedFeatures;
        }

        /// <summary>
        /// Generates alignment functions for alignment between features in each separation dimension.
        /// Warps the elution value for each feature based on the alignment function.
        /// </summary>
        /// <param name="aligneeFeatures">The features to warp.</param>
        /// <param name="baselineFeatures">The features to warp to.</param>
        /// <returns>The features with all NET values warped.</returns>
        public List<UMCLight> WarpNetMass(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures)
        {
            // First pass NET warp: Perform warp by only scoring matches in NET
            var netWarpedFeatures = this.WarpNet(aligneeFeatures, baselineFeatures, false);

            // Generate matches from features by matching in both mass and NET
            var featureMatcher = new LcmsWarpFeatureMatcher(this.options);
            featureMatcher.GenerateCandidateMatches(
                            aligneeFeatures,
                            baselineFeatures,
                            this.options.SeparationTypes);
            var matches = featureMatcher.Matches;

            // Warp mass
            var massCalibrator = new LcmsWarpMassCalibrator(this.options);
            var massCalibrations = massCalibrator.GetMassCalibrations(matches);
            foreach (var calibration in massCalibrations)
            {
                calibration.CalibrateFeatures(aligneeFeatures);
            }

            // Second pass NET warp: Perform warp that scores matches in mass AND net
            var netMassWarpedFeatures = this.WarpNet(netWarpedFeatures, baselineFeatures, true);

            return netMassWarpedFeatures;
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

        private AlignmentData GetAlignmentData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method checks to see if both alignee features and
        /// reference features have the same separation dimensions.
        /// </summary>
        /// <param name="aligneeFeatures">The features that will be aligned.</param>
        /// <param name="baselineFeatures">The reference dataset/database features.</param>
        /// <exception cref="ArgumentException">
        /// Throws an argument exception if the alignee and reference datasets do not have equivalent dimensionality.
        /// </exception>
        private void CheckDimensionality(List<UMCLight> aligneeFeatures, List<UMCLight> baselineFeatures)
        {
            foreach (var aligneeFeature in aligneeFeatures)
            {
                if (baselineFeatures.Any(baselineFeature => !aligneeFeature.CheckDimensionality(baselineFeature)))
                {
                    throw new ArgumentException("The dimensions of the alignee dataset and the reference dataset are not the same.");
                }
            }
        }
    }
}
