using System;
using System.Collections.Generic;
using System.Text;

namespace MultiAlignCore.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class IOProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Message about IO progress.
        /// </summary>
        string m_message;
        /// <summary>
        /// Percentage complete.
        /// </summary>
        int m_percent;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="percent"></param>
        public IOProgressEventArgs(string message, int percent)
        {
            m_message = message;
            m_percent = percent;
        }

        /// <summary>
        /// Gets the message for the IO progress.
        /// </summary>
        public string Message
        {
            get
            {
                return m_message;
            }
        }

        /// <summary>
        /// Gets the percent (0-100) elapsed during IO process.
        /// </summary>
        public int Percent
        {
            get
            {
                return m_percent;
            }
        }
    }
}
