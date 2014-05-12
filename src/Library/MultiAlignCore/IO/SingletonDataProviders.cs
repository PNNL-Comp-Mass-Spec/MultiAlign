#region

using System.Collections.Generic;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.IO.Features;

#endregion

namespace MultiAlign.IO
{
    public static class SingletonDataProviders
    {
        private static FeatureDataAccessProviders m_providers;
        private static Dictionary<int, DatasetInformation> m_datasets = new Dictionary<int, DatasetInformation>();

        public static FeatureDataAccessProviders Providers
        {
            get { return m_providers; }
            set
            {
                m_providers = value;
                UpdateDatasets();
            }
        }

        private static void UpdateDatasets()
        {
            m_datasets = new Dictionary<int, DatasetInformation>();
            if (m_providers != null)
            {
                foreach (var info in m_providers.DatasetCache.FindAll())
                {
                    m_datasets.Add(info.DatasetId, info);
                }
            }
        }

        /// <summary>
        ///     Retrieves a list of the datasets loaded.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DatasetInformation> GetAllInformation()
        {
            return m_datasets.Values;
        }

        /// <summary>
        ///     Finds the dataset information if a group id can be found.  Null otherwise.
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static DatasetInformation GetDatasetInformation(int groupID)
        {
            if (!m_datasets.ContainsKey(groupID))
                return null;

            return m_datasets[groupID];
        }
    }
}