using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using MultiAlignCore.Algorithms.SpectralProcessing;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.RawData;

    class Ms2ComparisonScorer : IFeatureComparisonScorer
    {
        private readonly ISpectralComparer comparer;

        public Ms2ComparisonScorer(ISpectralComparer comparer = null)
        {
            this.comparer = comparer ?? new SpectralDotProductComprarer();
        }

        public double ScoreComparison(FeatureLight feature1, FeatureLight feature2)
        {
            double score = 0.0;
            for (int i = 0; i < feature1.MSnSpectra.Count; i++)
            {
                var leftSpectrum = feature1.MSnSpectra[i];

                for (int j = 0; j < feature2.MSnSpectra.Count; j++)
                {
                    var rightSpectrum = feature1.MSnSpectra[j];
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
