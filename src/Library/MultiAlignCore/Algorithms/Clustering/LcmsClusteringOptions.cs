using MultiAlignCore.Algorithms.Distance;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Clustering
{
    public class LcmsClusteringOptions : ISettingsContainer
    {
        public LcmsClusteringOptions(FeatureTolerances instrumentTolerances)
        {
            InstrumentTolerances = instrumentTolerances;
        }

        public LcmsClusteringOptions()
        {
            InstrumentTolerances = new FeatureTolerances();
        }

        public bool                          ShouldSeparateCharge { get; set; }
        public DistanceMetric                DistanceFunction { get; set; }
        public ClusteringAlgorithmTypes       LcmsFeatureClusteringAlgorithm { get; set; }
        public ClusterCentroidRepresentation ClusterCentroidRepresentation { get; set; }
        public FeatureTolerances InstrumentTolerances { get; set; }

        public static FeatureClusterParameters<UMCLight> ConvertToOmics(LcmsClusteringOptions options)
        {                        
            var parameters       = new FeatureClusterParameters<UMCLight>
            {
                Tolerances                  = options.InstrumentTolerances,
                OnlyClusterSameChargeStates = (options.ShouldSeparateCharge == false),
                CentroidRepresentation      = options.ClusterCentroidRepresentation
            };

            parameters.DistanceFunction = Distance.DistanceFactory<UMCLight>.CreateDistanceFunction(options.DistanceFunction);
            return parameters;
        }

        public void RestoreDefaults()
        {
            this.ShouldSeparateCharge = true;
            this.DistanceFunction = DistanceMetric.Euclidean;
            this.LcmsFeatureClusteringAlgorithm = ClusteringAlgorithmTypes.AverageLinkage;
            this.ClusterCentroidRepresentation = ClusterCentroidRepresentation.Apex;
            this.InstrumentTolerances = new FeatureTolerances();
        }
    }
}