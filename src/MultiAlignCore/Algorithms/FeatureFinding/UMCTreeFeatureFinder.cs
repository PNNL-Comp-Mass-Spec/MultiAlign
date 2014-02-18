using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;


namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    /// Finds UMC features based on m/z and uses a tree approach
    /// </summary>
    public class UmcTreeFeatureFinder: IFeatureFinder 
    {
        /// <summary>
        /// Finds features
        /// </summary>
        /// <returns></returns>
        public List<UMCLight> FindFeatures( List<MSFeatureLight> msFeatures,
                                            LCMSFeatureFindingOptions options,
                                            ISpectraProvider provider)
        {
            var clusterer = new MsFeatureTreeClusterer<MSFeatureLight, UMCLight>
                            {
                                Tolerances =
                                    new FeatureTolerances
                                    {
                                        Mass = options.ConstraintMonoMass,
                                        RetentionTime = 50
                                    },
                                SpectraProvider = provider
                            };

            List<UMCLight>  features            = clusterer.Cluster(msFeatures);

            int minScan = int.MaxValue;
            int maxScan = int.MinValue;
            foreach (var feature in msFeatures)
            {
                minScan = Math.Min(feature.Scan, minScan);
                maxScan = Math.Max(feature.Scan, maxScan);
            }

            int id = 0;
            foreach (var feature in features)
            {
                feature.NET             = Convert.ToDouble(feature.Scan - minScan) / Convert.ToDouble(maxScan - minScan);
                feature.CalculateStatistics(ClusterCentroidRepresentation.Median);
                feature.RetentionTime   = feature.NET;
                feature.ID              = id++;
            }
            return features;
        }
    }
}
