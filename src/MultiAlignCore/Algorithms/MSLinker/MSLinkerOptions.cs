using MultiAlignCore.IO.Parameters;

namespace MultiAlignCore.Algorithms.MSLinker
{
    /// <summary>
    /// Linker options class.
    /// </summary>
    public class MSLinkerOptions
    {
        private double m_mzTolerance;

        public MSLinkerOptions()
        {
            m_mzTolerance = .15;
        }

        /// <summary>
        /// Gets or sets the m/z tolerance window width for the MS Linker algorithm.
        /// </summary>
        [ParameterFileAttribute("MZTolerance", "MSnLinker")]
        public double MzTolerance
        {
            get
            {
                return m_mzTolerance;
            }
            set
            {
                m_mzTolerance = value;
            }
        }
    }
}
