using System;

namespace MultiAlignCore.Data
{
    public class AnalysisStatusEventArgs : EventArgs
    {
        private string m_message;
        private int    m_percentComplete;

        public AnalysisStatusEventArgs(string message, int percent)
        {
            m_message           = message;
            m_percentComplete   = percent;
        }

        public int PercentComplete
        {
            get
            {
                return m_percentComplete;
            }
        }
        public string StatusMessage
        {
            get
            {
                return m_message;
            }
        }
    }
}