using System;
using System.IO;

namespace MultiAlignWin.IO
{
    public static class AnalysisLogWriter
    {
        /// <summary>
        /// Writes the header and appends the newline constant.
        /// </summary>
        /// <param name="path">Path to write to.</param>
        /// <param name="header">Data to write.</param>
        public static void WriteHeader(string path, string header)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                writer.WriteLine(header);
            }
        }
        /// <summary>
        /// Logs the message to the path provided with the status level being the number of tabs to output.
        /// </summary>
        /// <param name="filePath">Path of log file to write to.</param>
        /// <param name="statusLevel">Number of tab indents.</param>
        /// <param name="message">Message to write.</param>
        public static void WriteMessage(string filePath , int statusLevel, string message)
        {
            string tabs = "";
            for(int i = 0; i < statusLevel; i++)
            {
                tabs += "\t";
            }

            using (TextWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(tabs + message);
            }
        }
    }
}
