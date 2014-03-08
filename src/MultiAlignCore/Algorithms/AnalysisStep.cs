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
        /// Initial state.
        /// </summary>
        None,
        /// <summary>
        /// Invalid, initial, or final state.
        /// </summary>
        LoadMtdb,
        /// <summary>
        /// Indicates that feature data should be loaded.
        /// </summary>
        FindFeatures,
        /// <summary>
        /// Features must already be found.
        /// </summary>
        Alignment,
        /// <summary>
        /// Features must already be aligned.
        /// </summary>
        Clustering,   
        /// <summary>
        /// Features must already be clustered.
        /// </summary>
        PeakMatching
    }
}
