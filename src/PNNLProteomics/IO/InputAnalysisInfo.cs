﻿using System.Collections.Generic;
using PNNLProteomics.Data.MetaData;

namespace PNNLProteomics.IO
{    
    /// <summary>
    /// Input data for the console application.
    /// </summary>
    public class InputAnalysisInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InputAnalysisInfo()
        {
            Files           = new List<InputFile>();
            BaselineFile    = null;
            Database        = new InputDatabase();
        }

        #region Properties
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
        /// Get or sets the input database type.
        /// </summary>
        public InputDatabase Database
        {
            get;
            set;
        }
        #endregion        
    }
}
