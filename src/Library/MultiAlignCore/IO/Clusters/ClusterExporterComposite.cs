﻿namespace MultiAlignCore.IO.Clusters
{
    /// <summary>
    /// Class that tracks the paths for cross tabs cluster MSMS and cross tab abundance.
    /// </summary>
    public sealed class ClusterExporterComposite
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ClusterExporterComposite()
        {
            ClusterScanPath = null;
            CrossTabPath = null;
            ClusterMSMSPath = null;
            CrossTabAbundance = null;
        }

        /// <summary>
        /// Gets or sets the abundance cross tab name.
        /// </summary>
        public string CrossTabAbundance { get; set; }

        /// <summary>
        /// Gets or sets the cluster scan path.
        /// </summary>
        public string ClusterScanPath { get; set; }

        /// <summary>
        /// Gets or sets the cross tab path
        /// </summary>
        public string CrossTabPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the cluster MSMS table cross tab.
        /// </summary>
        public string ClusterMSMSPath { get; set; }
    }
}