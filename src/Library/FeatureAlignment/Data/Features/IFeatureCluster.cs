using System.Collections.Generic;

namespace FeatureAlignment.Data.Features
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">child features</typeparam>
    /// <typeparam name="U">parent features</typeparam>
    public interface IFeatureCluster<T>
        where T : FeatureLight
    {
        /// <summary>
        /// Adds a child feature the list of aggregate features.
        /// </summary>
        /// <param name="feature"></param>
        void AddChildFeature(T feature);
        /// <summary>
        /// Calcualte the statistics for the given cluster.
        /// </summary>
        /// <param name="centroid"></param>
        void CalculateStatistics(ClusterCentroidRepresentation centroid = ClusterCentroidRepresentation.Apex);

        /// <summary>
        /// Gets a list of child features.
        /// </summary>
        List<T> Features
        {
            get;
        }
    }
}
