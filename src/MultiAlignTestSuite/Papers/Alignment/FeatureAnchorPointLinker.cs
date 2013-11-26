using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignTestSuite.Papers.Alignment.SSM;
using PNNLOmics.Data;
using PNNLOmics.Algorithms.FeatureMatcher.MSnLinker;
using PNNLOmics.Data.Features;
using MultiAlignCore.Algorithms;
using MultiAlignTestSuite.Papers.Alignment.Data;

namespace MultiAlignTestSuite.Papers.Alignment
{
    /// <summary>
    /// Links Anchor points with Features
    /// </summary>
    public class FeatureAnchorPointLinker
    {

        /// <summary>
        /// Links Anchor point matches to actual features.
        /// </summary>
        /// <param name="featureData"></param>
        /// <param name="matches"></param>
        public void LinkMatchesToFeatures(AlignedFeatureData featureData,
                                            IEnumerable<AnchorPointMatch> matches)
        {
            // Now we determine what features are aligned with what.
            int id = 0;
            List<MSSpectra> matchedSpectraX = new List<MSSpectra>();
            List<MSSpectra> matchedSpectraY = new List<MSSpectra>();

            foreach (AnchorPointMatch match in matches)
            {
                MSSpectra spectrumx = match.AnchorPointX.Spectrum;
                MSSpectra spectrumy = match.AnchorPointY.Spectrum;

                spectrumx.ID = id;
                spectrumy.ID = id++;

                matchedSpectraX.Add(spectrumx);
                matchedSpectraY.Add(spectrumy);
            }

            // So we first link the spectra to the MS Features (mono masses)
            IMSnLinker linker = MSnLinkerFactory.CreateLinker(MSnLinkerType.BoxMethod);
            linker.Tolerances.Mass = .05;
            linker.LinkMSFeaturesToMSn(featureData.Baseline.Item2, matchedSpectraX);
            linker.LinkMSFeaturesToMSn(featureData.Alignee.Item2, matchedSpectraY);

            // Then we iterate through and find anything with MS/MS ...we should just directly do this above...but we are 
            // retrofitting here...
            Dictionary<int, UMCLight> mappedFeaturesX = new Dictionary<int, UMCLight>();
            Dictionary<int, UMCLight> mappedFeaturesY = new Dictionary<int, UMCLight>();
        }
    }
}
