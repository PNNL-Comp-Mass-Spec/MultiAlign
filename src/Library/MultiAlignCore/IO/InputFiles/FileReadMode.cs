namespace MultiAlignCore.IO.InputFiles
{
    /// <summary>
    ///     Determines the mode of what input files MA will read.
    /// </summary>
    public enum FileReadMode
    {
        /// <summary>
        ///     Sequence database search output.
        /// </summary>
        Sequence,

        /// <summary>
        ///     Reads isos or LCMS Feature files information.
        /// </summary>
        Files,

        /// <summary>
        ///     Reads raw file information.
        /// </summary>
        RawFiles,

        /// <summary>
        ///     Reads scan file information.
        /// </summary>
        ScanFiles,

        /// <summary>
        ///     Reads MTS database information
        /// </summary>
        Database,

        /// <summary>
        ///     Reads DeconTools Peaks files.
        /// </summary>
        Peaks,

        /// <summary>
        ///     There is a bad input definition file.
        /// </summary>
        Unknown,

        /// <summary>
        ///     No read mode has been initiated.
        /// </summary>
        None
    }
}