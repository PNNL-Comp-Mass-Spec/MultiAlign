using MultiAlignCore.Algorithms.Clustering;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Distance;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Algorithms.Options
{
    public class LcmsClusteringOptions
    {
        public LcmsClusteringOptions(FeatureTolerances instrumentTolerances)
        {
            InstrumentTolerances = instrumentTolerances;
        }

        public bool                          ShouldSeparateCharge { get; set; }
        public DistanceMetric                DistanceFunction { get; set; }
        public ClusteringAlgorithmType       ClusteringAlgorithm { get; set; }
        public ClusterCentroidRepresentation ClusterCentroidRepresentation { get; set; }
        public FeatureTolerances             InstrumentTolerances { get; set; }

        public static FeatureClusterParameters<UMCLight> ConvertToOmics(LcmsClusteringOptions options)
        {                        
            FeatureClusterParameters<UMCLight> parameters       = new FeatureClusterParameters<UMCLight>();                 
            parameters.Tolerances                               = options.InstrumentTolerances;            
            parameters.OnlyClusterSameChargeStates              = (options.ShouldSeparateCharge == false);
            parameters.CentroidRepresentation                   = options.ClusterCentroidRepresentation;              
            return parameters;
        }
    }
}