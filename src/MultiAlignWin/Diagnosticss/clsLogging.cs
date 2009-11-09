/*////////////////////////////////////////////////////////////////////////
 *  File  : clsLogging.cs
 *  Author: Brian LaMarche
 *  Date  : 3/18/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      static classes for logging debug and trace outputs to file.
 * 
 *  Revisions:
 *      3-18-2008
 *          Created the file.
 */
///////////////////////////////////////////////////////////////////////

	
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiAlignWin.Diagnostics
{
    /// <summary>
    /// Debug level that the clsLogging class uses for determining if the
    /// current message should be output to cut down on output logging.
    /// </summary>
    public enum DebugLevel
    {
        MAJOR,          // Major events - e.g. loading file, etc. etc.
        EXCEPTIONS,     // Any exception thrown.
        ERRORS,         // Errors that are handled but do not crash system.
        VERBOSE         // All actions.
    }

    /// <summary>
    /// Class for logging output messages to trace listeners.
    /// </summary>
    public static class clsLogging
    {
        /// <summary>
        /// Current debug level used to filter low-level messages.
        /// </summary>
        private static DebugLevel menm_currentLevel = DebugLevel.MAJOR;

        /// <summary>
        /// Gets or sets the current debug level.
        /// </summary>
        public static DebugLevel DebugLevel
        {
            get
            {
                return menm_currentLevel;
            }
            set
            {
                menm_currentLevel = value;
            }
        }

        /// <summary>
        /// Traces output to any TRACE listeners.
        /// </summary>
        /// <param name="level">Level the message is at.</param>
        /// <param name="message">Message to output.</param>
        public static void Trace(DebugLevel level, string message)
        {
            if (level <= menm_currentLevel)
            {
                System.Diagnostics.Trace.WriteLine(message);
            }
        }
    }
}
