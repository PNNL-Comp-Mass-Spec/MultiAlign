using System;
using System.IO;
using System.Collections.Generic;

namespace PNNLProteomics.MultiAlign
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
    }
}
