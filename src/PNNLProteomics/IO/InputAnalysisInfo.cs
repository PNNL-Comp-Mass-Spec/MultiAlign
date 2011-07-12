using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLProteomics.Data.MetaData;

namespace PNNLProteomics.IO
{    
    /// <summary>
    /// Input data for the console application.
    /// </summary>
    public class InputAnalysisInfo
    {
        public InputAnalysisInfo()
        {
            Files                   = new List<InputFile>();
            BaselineFile            = null;
            MassTagDatabaseServer   = null;
            MassTagDatabase         = null;
        }
        /// <summary>
        /// Gets or sets the list of file paths.
        /// </summary>
        public List<InputFile> Files
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the baseline file..
        /// </summary>
        public InputFile BaselineFile
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
