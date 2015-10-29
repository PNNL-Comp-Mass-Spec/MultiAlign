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
            throw new NotImplementedException();
        }

        public AlignmentData Align(IEnumerable<MassTagLight> baseline, IEnumerable<UMCLight> alignee, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }
    }
}
