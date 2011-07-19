using System;
using System.IO;
using System.Collections.Generic;

namespace MultiAlignCore.Data
{

    /// <summary>
    /// Class used for analysis naming and general path handling utilities.
    /// </summary>
    public static class AnalysisPathUtils
    {
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
            DateTime now = DateTime.Now;
            string logPath = string.Format("{6}-log_{1}-{0}-{2}-{3}-{4}-{5}.txt",
                                        now.Day,
                                        now.Month,
                                        now.Year,
                                        now.Hour,
                                        now.Minute,
                                        now.Second,
                                        name);
            return Path.Combine(path, logPath);
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
