#region

using System.Collections.Generic;

#endregion

namespace MultiAlignCore.Data
{
    /// <summary>
    ///     Maintains a list of all clusters in memory.
    /// </summary>
    public static class FeatureCacheManager<T>
        where T : IFeatureMap, new()
    {
        /// <summary>
        ///     Maintains a list of references to clusters based on id.
        /// </summary>
        private static Dictionary<object, T> m_featureMap;

        /// <summary>
        ///     Sets the list of clusters to be used later on.
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public static void SetFeatures(List<T> features)
        {
            if (m_featureMap == null)
            {
                m_featureMap = new Dictionary<object, T>();
            }

            m_featureMap.Clear();
            features.ForEach(x => m_featureMap.Add(x.Id, x));
        }

        /// <summary>
        ///     Finds the cluster based on it's id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T FindById(object id)
        {
            if (m_featureMap.ContainsKey(id))
            {
                return m_featureMap[id];
            }
            return default(T);
        }
    }
}