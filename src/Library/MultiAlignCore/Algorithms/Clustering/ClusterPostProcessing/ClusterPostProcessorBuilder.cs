using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureAlignment.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using InformedProteomics.Backend.Data.Spectrometry;

    using MultiAlignCore.Algorithms.SpectralProcessing;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.IO.Features;

    public class ClusterPostProcessorBuilder
    {
        public static ClusterPostProcessor<T, U> GetClusterPostProcessor<T, U>(ClusterPostProcessingOptions options, FeatureDataAccessProviders dataProviders)
            where T : FeatureLight, IFeatureCluster<U>, new()
            where U : FeatureLight, IChildFeature<T>, new()
        {
            IFeatureComparisonScorer scorer;
            switch (options.ComparisonType)
            {
                case ClusterPostProcessingOptions.ClusterComparisonType.MsMsIdentifications:
                    scorer = new IdComparisonScorer(dataProviders.IdentificationProviderCache);
                    break;
                default:
                    var tolerance = new Tolerance(options.MsMsComparisonTolerance, options.MsMsComparisonToleranceUnit);
                    var comparer = new SpectraPearsonCorrelationComparer(tolerance);
                    scorer = new Ms2ComparisonScorer(dataProviders.ScanSummaryProviderCache, comparer);
                    break;
            }

            return new ClusterPostProcessor<T, U>(scorer);
        }
    }
}
