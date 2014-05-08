using System;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Analysis Error Event Arguments
    /// </summary>
    public class AnalysisErrorEventArgs : EventArgs
    {
        public AnalysisErrorEventArgs(string error, Exception ex)
        {
            Exception    = ex;
            ErrorMessage = error;   
        }

        public string ErrorMessage
        {
            get;
            private set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }
    }
}