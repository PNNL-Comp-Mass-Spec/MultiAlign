using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNNLProteomics.Data.MetaData
{
    /// <summary>
    /// Holds information about input files. 
    /// </summary>
    public class InputFile
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InputFile()
        {
            Path        = null;
            FileType    = InputFileType.NotRecognized;            
        }
        /// <summary>
        /// Gets or sets the path to the input file.
        /// </summary>
        public string Path
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the input file type.
        /// </summary>
        public InputFileType FileType
        {
            get;
            set;
        }
    }

}
