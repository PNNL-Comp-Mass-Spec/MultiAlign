using System;
using System.Collections.Generic;

using MultiAlignCore.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Interface for writing clusters to a file.
    /// </summary>
    public interface IFeatureClusterWriter
    {
        /// <summary>
        /// Writes the data about the clusters to file.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="datasets"></param>
        void WriteClusters(List<UMCClusterLight> clusters, List<DatasetInformation> datasets);
        void WriteClusters(List<UMCClusterLight> clusters,
                                    Dictionary<int, ClusterToMassTagMap> clusterMap,
                                    List<DatasetInformation> datasets,
                                    Dictionary<string, MassTagLight> tags);              
    }
}
