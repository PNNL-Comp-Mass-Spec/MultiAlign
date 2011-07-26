
namespace MultiAlignCore.Algorithms.MSLinker
{
    /// <summary>
    /// Linker options class.
    /// </summary>
    public class MSLinkerOptions
    {
        public MSLinkerOptions()
        {
            MzTolerance = .15;
        }

        /// <summary>
        /// Gets or sets the m/z tolerance window width for the MS Linker algorithm.
        /// </summary>
        [MultiAlignEngine.clsParameterFile("MZTolerance", "MSnLinker")]
        public double MzTolerance
        {
            get;
            set;
        }
    }
}
