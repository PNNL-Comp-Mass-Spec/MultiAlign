using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.MetaData;
using MultiAlignRogue.Clustering;

namespace MultiAlignRogue
{
    public class RogueProject
    {
        /// <summary>
        /// Gets or sets the MultiAlign analysis options.
        /// </summary>
        public MultiAlignAnalysisOptions MultiAlignAnalysisOptions { get; set; }

        /// <summary>
        /// Gets or sets the settings for the cluster viewer.
        /// </summary>
        public ClusterViewerSettings ClusterViewerSettings { get; set; }

        /// <summary>
        /// Gets or sets the list of datasets.
        /// </summary>
        public List<DatasetInformation> Datasets { get; set; }

        /// <summary>
        /// Gets or sets the path for the analysis file.
        /// </summary>
        public string AnalysisPath { get; set; }

        /// <summary>
        /// Gets or sets the path for the layout file.
        /// </summary>
        public string LayoutFilePath { get; set; }
    }
}
