using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Alignment.LcmsWarp
{
    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Alignment;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MassTags;

    using PNNLOmics.Utilities;

    public class NewLcmsWarp :
        IFeatureAligner<IEnumerable<UMCLight>, IEnumerable<UMCLight>, AlignmentData>,
        IFeatureAligner<IEnumerable<MassTagLight>, IEnumerable<UMCLight>, AlignmentData>
    {
        private const int REQUIRED_MATCHES = 6;

        private LcmsWarpAlignmentOptions options;

        public NewLcmsWarp(LcmsWarpAlignmentOptions options)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aligneeFeatures"></param>
        /// <param name="baselineFeatures"></param>
        /// <param name="includeMassInMatchScore"></param>
        /// <returns></returns>
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

        public event EventHandler<ProgressNotifierArgs> Progress;

        public AlignmentData Align(IEnumerable<UMCLight> baseline, IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            // Throw an exception if separation dimensions in basline and alignee features do not match.
            this.CheckDimensionality(alignee, baseline);

            throw new NotImplementedException();
        }

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
        /// This method checks to see if both alignee features and
        /// reference features have the same separation dimensions.
        /// </summary>
        /// <param name="aligneeFeatures">The features that will be aligned.</param>
        /// <param name="baselineFeatures">The reference dataset/database features.</param>
        /// <exception cref="ArgumentException">
        /// Throws an argument exception if the alignee and reference datasets do not have equivalent dimensions.
        /// </exception>
        private void CheckDimensionality(IEnumerable<FeatureLight> aligneeFeatures, IEnumerable<FeatureLight> baselineFeatures)
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
