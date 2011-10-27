using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignConsole.IO
{
    /// <summary>
    /// class used to aggregate clusters.
    /// </summary>
    internal sealed class ClusterExporterComposite
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ClusterExporterComposite()
        {
            ClusterScanPath   = null;
            CrossTabPath      = null;
            ClusterMSMSPath   = null;
            CrossTabAbundance = null;
        }
        /// <summary>
        /// Gets or sets the abundance cross tab name.
        /// </summary>
        public string CrossTabAbundance
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cluster scan path.
        /// </summary>
        public string ClusterScanPath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cross tab path
        /// </summary>
        public string CrossTabPath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the path to the cluster MSMS table cross tab.
        /// </summary>
        public string ClusterMSMSPath
        {
            get;
            set;
        }
    }    
}
