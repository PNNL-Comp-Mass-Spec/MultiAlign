namespace MultiAlignCore.IO.DatasetLoaders
{
    using System;
    using System.Collections.Generic;

    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Algorithms;
    using MultiAlignCore.Algorithms.Clustering;
    using MultiAlignCore.Data.Features;
    using MultiAlignCore.Data.MetaData;

    public class DeconToolsLoader : IDatasetLoader
    {
        private readonly DeconToolsFilter filter;

        public DeconToolsLoader(DeconToolsFilter filter = null)
        {
            this.filter = filter ?? new DeconToolsFilter();
        }

        public MsFeatureClusteringAlgorithmType ClustererType { get; set; }

        public List<UMCLight> Load(DatasetInformation dataset, IProgress<ProgressData> progress = null)
        {
            // Read MSFeatureFile
            var msFeatures = new List<MSFeatureLight>();

            // Filter features
            var filteredMsFeatures = this.filter.FilterFeatures(msFeatures);

            // Cluster features
            var clusterer = ClusterFactory.Create(this.ClustererType);
            var lcmsFeatures = clusterer.Cluster(filteredMsFeatures, progress);

            // Further filtering of LCMS features.
            var filteredUmcs = this.filter.FilterLcmsFeature(lcmsFeatures);

            return filteredUmcs;
        }
    }
}
