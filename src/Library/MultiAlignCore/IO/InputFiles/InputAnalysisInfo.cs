#region

using System.Collections.Generic;

#endregion

namespace MultiAlignCore.IO.InputFiles
{
    /// <summary>
    ///     Input data for the console application.
    /// </summary>
    public class InputAnalysisInfo
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public InputAnalysisInfo()
        {
            Files = new List<InputFile>();
            BaselineFile = null;
            Database = new InputDatabase();
            FactorFile = null;
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the list of file paths.
        /// </summary>
        public List<InputFile> Files { get; set; }

        /// <summary>
        ///     Gets or sets the baseline file..
        /// </summary>
        public InputFile BaselineFile { get; set; }

        /// <summary>
        ///     Get or sets the input database type.
        /// </summary>
        public InputDatabase Database { get; set; }

        /// <summary>
        ///     Gets or sets the file the factors are stored in.
        /// </summary>
        public string FactorFile { get; set; }

        #endregion
    }
}