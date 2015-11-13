using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using MultiAlignCore.Algorithms.SpectralProcessing;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.RawData;

    class Ms2ComparisonScorer : IFeatureComparisonScorer
    {
        private readonly ISpectralComparer comparer;

        private readonly SpectraProviderCache spectraProvider;

        public Ms2ComparisonScorer(SpectraProviderCache spectraProvider, ISpectralComparer comparer = null)
        {
            this.spectraProvider = spectraProvider;
            this.comparer = comparer ?? new SpectralDotProductComprarer();
        }

        public double ScoreComparison(FeatureLight feature1, FeatureLight feature2)
        {
            double score = 0.0;

            var leftSpectraProvider = this.spectraProvider.GetSpectraProvider(feature1.GroupId);
            var rightSpectraProvider = this.spectraProvider.GetSpectraProvider(feature2.GroupId);

            var leftSpectra = leftSpectraProvider.GetMSMSSpectra(feature1.Scan, feature1.Mz, true);
            var rightSpectra = rightSpectraProvider.GetMSMSSpectra(feature2.Scan, feature2.Mz, true);

            for (int i = 0; i < leftSpectra.Count; i++)
            {
                var leftSpectrum = leftSpectra[i];

                for (int j = 0; j < rightSpectra.Count; j++)
                {
                    var rightSpectrum = rightSpectra[i];
                    var specScore = this.comparer.CompareSpectra(leftSpectrum, rightSpectrum);
                    score += this.IsScoreWithinTolerance(specScore) ? 1 : -1;
                }
            }

            return score;
        }

        private bool IsScoreWithinTolerance(double score)
        {
            return score >= 0.7;
        }
    }
}
