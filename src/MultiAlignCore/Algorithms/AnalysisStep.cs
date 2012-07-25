using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.Algorithms
{
    /// <summary>
    /// Determines what step to start the analysis at.
    /// </summary>
    public enum AnalysisStep
    {
        /// <summary>
        /// Invalid, initial, or final state.
        /// </summary>
        None = -1,
        LoadMTDB = 0,
        /// <summary>
        /// Loads the MSMS scan data into the database.
        /// </summary>
        LoadMSMSScanData,
        /// <summary>
        /// Indicates that feature data should be loaded.
        /// </summary>
        FindFeatures,
        /// <summary>
        /// Features must exist in the database.
        /// </summary>
        Traceback,
        /// <summary>
        /// MS/MS spectra must already be tied to features.  
        /// </summary>
        SpectralClustering,
        /// <summary>
        /// Features must already be found.
        /// </summary>
        Alignment,
        /// <summary>
        /// Features must already be aligned.
        /// </summary>
        Clustering,         
        /// <summary>
        /// Compute the QC for a cluster
        /// </summary>
        ClusterQC,
        /// <summary>
        /// Features must already be clustered.
        /// </summary>
        PeakMatching
    }
}
