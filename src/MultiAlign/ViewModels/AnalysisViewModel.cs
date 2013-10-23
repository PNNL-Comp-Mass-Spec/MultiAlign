using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data;

namespace MultiAlign.ViewModels
{
    public class AnalysisViewModel:ViewModelBase 
    {
        private MultiAlignAnalysis m_analysis;

        public MultiAlignAnalysis Analysis
        { 
            get; 
            set;
        }
    }
}
