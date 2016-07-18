using System.Collections.Generic;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    public interface IClusterReprocessor<T, U>
        where T : FeatureLight, Data.Features.IChildFeature<U>, new()
        where U : FeatureLight, Data.Features.IFeatureCluster<T>, new()
    {

        /// <summary>
        /// Reprocesses clusters and returns a list of new clusters.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        List<U> ProcessClusters(List<U> clusters);
    }
}
