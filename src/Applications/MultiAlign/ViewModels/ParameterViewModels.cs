using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlign.ViewModels
{
    public class AnalysisParameterViewModels: ViewModelBase 
    {
        private bool m_isImsUsed;


        public AnalysisParameterViewModels()
        {
            m_isImsUsed = false;
        }

        public bool IsImsUsed
        {
            get;
            set;

        }

    }
}
