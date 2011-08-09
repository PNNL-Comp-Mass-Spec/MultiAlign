
namespace MultiAlignCore.IO.InputFiles
{
    /// <summary>
    /// Input file type.
    /// </summary>
    public enum InputFileType
    {
        /// <summary>
        /// Raw data
        /// </summary>
        Raw,
        /// <summary>
        /// Holds information about each scan for provenance tracking.
        /// </summary>
        Scans,
        /// <summary>
        /// Holds information about MS features (decon2ls output or LCMS Feature Finder).
        /// </summary>
        Features,
        /// <summary>
        /// The input File format could not be recognized.
        /// </summary>
        NotRecognized
    }
}
