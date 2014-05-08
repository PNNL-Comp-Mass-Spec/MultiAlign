using System;

namespace MultiAlign.Data
{
    public class OpenAnalysisArgs : EventArgs
    {
        public OpenAnalysisArgs(RecentAnalysis analysis)
        {
            AnalysisData = analysis;
        }

        public RecentAnalysis AnalysisData { get; private set; }
    }
}