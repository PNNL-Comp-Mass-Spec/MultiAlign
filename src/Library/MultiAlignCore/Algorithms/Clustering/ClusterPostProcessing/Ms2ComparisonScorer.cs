using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using InformedProteomics.Backend.Data.Spectrometry;

    using MultiAlignCore.Algorithms.SpectralProcessing;
    using MultiAlignCore.Data;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.RawData;

    public class Ms2ComparisonScorer : IFeatureComparisonScorer
    {
        private readonly ISpectralComparer comparer;

        private readonly ScanSummaryProviderCache spectraProvider;

        public Ms2ComparisonScorer(ScanSummaryProviderCache spectraProvider, ISpectralComparer comparer = null)
        {
            this.spectraProvider = spectraProvider;
            this.comparer = comparer ?? new SpectraPearsonCorrelationComparer(new Tolerance(5, ToleranceUnit.Ppm));
        }

        public double ScoreComparison(FeatureLight feature1, FeatureLight feature2)
        {
            double score = 0.0;

            var leftSpectraProvider = this.spectraProvider.GetScanSummaryProvider(feature1.GroupId) as ISpectraProvider;
            var rightSpectraProvider = this.spectraProvider.GetScanSummaryProvider(feature2.GroupId) as ISpectraProvider;

            if (leftSpectraProvider == null || rightSpectraProvider == null)
            {
                throw new Exception("Do not have spectra data available for dataset.");
            }

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
