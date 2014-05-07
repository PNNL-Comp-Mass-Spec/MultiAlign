using System;
using System.IO;

namespace MultiAlignCore.Data
{

    /// <summary>
    /// Class used for analysis naming and general path handling utilities.
    /// </summary>
    public static class AnalysisPathUtils
    {
        private static string m_dateSuffix;
        /// <summary>
        /// Builds an analysis name from the path and name provided.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string BuildAnalysisName(string path, string name)
        {
            return Path.Combine(path, name);
        }
        /// <summary>
        /// Builds the log path for the given analysis.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string BuildLogPath(string path, string name)
        {
            
            var dateSuffix = BuildDateSuffix();
            var logPath    = string.Format("{1}-log_{0}.txt",
                                        dateSuffix,
                                        name);
            return Path.Combine(path, logPath);
        }
        /// <summary>
        /// Builds the log path for the given analysis.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string BuildLogPath(string path, string name, string dateSuffix)
        {            
            var logPath    = string.Format("{1}-log_{0}.txt",
                                        dateSuffix,
                                        name);
            return Path.Combine(path, logPath);
        }
        /// <summary>
        /// Builds the date suffix string for logs and path locations.  A single 
        /// date suffix is built for the entire application lifecycle.
        /// </summary>
        /// <returns></returns>
        public static string BuildDateSuffix()
        {                        
            if (m_dateSuffix == null)
            {
                var now = DateTime.Now;
                m_dateSuffix = string.Format("{1:00}-{0:00}-{2:0000}-{3}-{4}-{5}",
                                            now.Day,
                                            now.Month,
                                            now.Year,
                                            now.Hour,
                                            now.Minute,
                                            now.Second);
            }
            return m_dateSuffix;
        }
        /// <summary>
        /// Builds the plot path for the given analysis.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string BuildPlotPath(string path)
        {
            return Path.Combine(path, "Plots");
        }
        /// <summary>
        /// Builds the parameter path for the given analysis.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string BuildParameterPath(string path, string name, string extension)
        {
            return Path.Combine(path, name + "_parameters" + extension);
        }
    }
}
