#region

using System;

#endregion

namespace MultiAlignCore.Algorithms
{
    public class AnalysisGraphEventArgs : EventArgs
    {
        public AnalysisGraphEventArgs(AnalysisGraph graph)
        {
            AnalysisGraph = graph;
        }

        public AnalysisGraph AnalysisGraph { get; set; }
    }
}