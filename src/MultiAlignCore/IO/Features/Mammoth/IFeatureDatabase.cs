using System;
using System.Collections.Generic;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Mammoth
{
    public interface IFeatureDatabase<T, U>
    {
        /// <summary>
        /// Returns a list of features.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        List<T> GetFeatures(MammothDatabaseRange options);
        /// <summary>
        /// Retrieves non-clustered features from the database.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        List<T> GetNonClusteredFeatures(MammothDatabaseRange options);
        /// <summary>
        /// Returns a list of clusters.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        List<U> GetClusters(MammothDatabaseRange options);
		/// <summary>
		/// Returns the number of features at the range specified.
		/// </summary>
		/// <param name="options">Range query options.</param>
		/// <returns>Total number of features to potentially cluster.</returns>
		int GetFeatureCount(MammothDatabaseRange options);
        /// <summary>
        /// Flushes any cached objects.
        /// </summary>
        void Flush();
        /// <summary>
        /// Updates features and their clusters.
        /// </summary>
        /// <param name="features"></param>
        int UpdateFeaturesAndClusters(List<T> features);
    }
}
