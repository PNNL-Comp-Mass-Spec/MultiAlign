using System;
using System.Collections.Generic;

using MultiAlignCore.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Algorithms.Features;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Interface for writing clusters to a file.
    /// </summary>
    public interface IFeatureClusterWriter
    {
        /// <summary>        
        /// Gets or sets the path to write to.
        /// </summary>
        string Path
        {
            get;
            set;
        }

        bool ShouldLoadClusterData { get; set; }

        /// <summary>
        /// Gets or sets how to consolidate features
        /// </summary>
        LCMSFeatureConsolidator Consolidator
        {
            get;
            set;
        }

        string Extension
        { get;  }

        /// <summary>
        /// Gets the name of the writer
        /// </summary>
        string Name { get; }
        string Description { get; }        
        /// <summary>
        /// Writes the data about the clusters to file.
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="datasets"></param>
        void WriteClusters(List<UMCClusterLight> clusters, List<DatasetInformation> datasets);
        void WriteClusters(List<UMCClusterLight> clusters,
                            Dictionary<int, List<ClusterToMassTagMap>> clusterMap,
                            List<DatasetInformation> datasets,
                            Dictionary<string, PNNLOmics.Data.MassTags.MassTagLight> tags);         
    }
}
