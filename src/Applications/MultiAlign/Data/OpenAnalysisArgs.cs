using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlign.Data
{
    public class OpenAnalysisArgs : EventArgs
    {
        public OpenAnalysisArgs(RecentAnalysis analysis)
        {
            AnalysisData = analysis;
        }

        public RecentAnalysis AnalysisData
        {
            get;
            private set;
        }
    }
}
