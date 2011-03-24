using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNNLProteomics.IO
{    
    /// <summary>
    /// Input data for the console application.
    /// </summary>
    public class InputAnalysisInfo
    {
        public InputAnalysisInfo()
        {
            FilePaths               = new List<string>();
            BaselineFileIndex       = -1;
            MassTagDatabaseServer   = null;
            MassTagDatabase         = null;
        }
        /// <summary>
        /// Gets or sets the list of file paths.
        /// </summary>
        public List<string> FilePaths
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets what file index is the baseline.
        /// </summary>
        public int BaselineFileIndex
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public string MassTagDatabase
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public string MassTagDatabaseServer
        {
            get;
            set;
        }
    }
}
