using System;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels.Wizard
{

    public class AnalysisStatusArgs : EventArgs
    {
        public AnalysisStatusArgs(AnalysisConfig configuration)
        {
            Configuration = configuration;
        }
        public AnalysisStatusArgs(MultiAlignAnalysis analysis)
        {
            Analysis = analysis;
        }
        public MultiAlignAnalysis Analysis { get; private set; }
        public AnalysisConfig Configuration { get; private set; }
    }
}
