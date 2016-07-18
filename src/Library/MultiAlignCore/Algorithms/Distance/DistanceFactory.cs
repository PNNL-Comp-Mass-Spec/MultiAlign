using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Distance
{
    public static class DistanceFactory<T> where T: FeatureLight, new()
    {
        public static DistanceFunction<T> CreateDistanceFunction(DistanceMetric metric)
        {
            DistanceFunction<T> function = null;
            switch (metric)
            {
                case DistanceMetric.Euclidean:
                    var metricFunction = new EuclideanDistanceMetric<T>();
                    function = metricFunction.EuclideanDistance;
                    break;
                case DistanceMetric.WeightedEuclidean:
                    var weighted = new WeightedEuclideanDistance<T>();
                    function = weighted.EuclideanDistance;
                    break;
                default:
                    break;
            }
            return function;
        }
    }

}
