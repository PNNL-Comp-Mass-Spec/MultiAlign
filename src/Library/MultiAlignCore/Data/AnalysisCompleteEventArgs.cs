#region

using System;

#endregion

namespace MultiAlignCore.Data
{
    public class AnalysisCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="analysis"></param>
        public AnalysisCompleteEventArgs(MultiAlignAnalysis analysis)
        {
            Analysis = analysis;
        }

        /// <summary>
        /// Gets or sets the analysis.
        /// </summary>
        public MultiAlignAnalysis Analysis { get; private set; }
    }
}