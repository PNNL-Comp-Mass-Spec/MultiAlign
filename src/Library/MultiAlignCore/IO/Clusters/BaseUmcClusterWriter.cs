#region

using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.Features;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Extensions;

#endregion

namespace MultiAlignCore.IO.Clusters
{
    public abstract class BaseUmcClusterWriter : IFeatureClusterWriter
    {
        private string m_name;
        private string m_description;
        private string m_path;

        public BaseUmcClusterWriter(bool shouldLoadMsData)
        {
            Consolidator = FeatureConsolidatorFactory.CreateConsolidator(AbundanceReportingType.Sum,
                AbundanceReportingType.Sum);

            ShouldLoadClusterData = false;
            ShouldLoadMsFeatureData = shouldLoadMsData;
        }

        public string Extension { get; protected set; }

        public string Path
        {
            get { return m_path; }
            set
            {
                if (value != null)
                {
                    if (!value.EndsWith(Extension))
                    {
                        m_path = value + Extension;
                    }
                }
            }
        }

        public LCMSFeatureConsolidator Consolidator { get; set; }

        public string Name
        {
            get { return m_name; }
            protected set { m_name = value; }
        }

        public string Description
        {
            get { return m_description; }
            protected set { m_description = value; }
        }

        /// <summary>
        /// Gets or sets whether cluster data needs to be loaded.  In case it was loaded from a cache.
        /// </summary>
        public bool ShouldLoadClusterData { get; set; }

        /// <summary>
        /// Gets or sets whether cluster data needs to be loaded, including the MS/MS data.
        /// </summary>
        public bool ShouldLoadMsFeatureData { get; private set; }

        protected abstract void Write(List<UMCClusterLight> clusters,
            List<DatasetInformation> datasets);

        protected abstract void Write(List<UMCClusterLight> clusters,
            Dictionary<int, List<ClusterToMassTagMap>> clusterMap,
            List<DatasetInformation> datasets,
            Dictionary<string, MassTagLight> tags);

        public void WriteClusters(List<UMCClusterLight> clusters,
            List<DatasetInformation> datasets)
        {
            WriteClusters(clusters,
                new Dictionary<int, List<ClusterToMassTagMap>>(),
                datasets,
                new Dictionary<string, MassTagLight>());
        }


        public void WriteClusters(List<UMCClusterLight> clusters,
            Dictionary<int, List<ClusterToMassTagMap>> clusterMap,
            List<DatasetInformation> datasets,
            Dictionary<string, MassTagLight> tags)
        {
            if (ShouldLoadClusterData)
                LoadClusterData(clusters);

            Write(clusters, clusterMap, datasets, tags);
        }

        protected virtual void LoadClusterData(IEnumerable<UMCClusterLight> clusters)
        {
            var shouldLoadFeatureData = ShouldLoadMsFeatureData;
            foreach (var cluster in clusters)
            {
                cluster.ReconstructUMCCluster(SingletonDataProviders.Providers, true, false, shouldLoadFeatureData,
                    shouldLoadFeatureData);
            }
        }
    }
}