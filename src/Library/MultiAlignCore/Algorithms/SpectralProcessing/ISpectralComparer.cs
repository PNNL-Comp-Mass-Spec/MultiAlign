
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    /// <summary>
    /// Interface for spectral comparison algorithms
    /// </summary>
    public interface ISpectralComparer
    {
        /// <summary>
        /// Compares two spectra together.
        /// </summary>
        /// <param name="spectraX"></param>
        /// <param name="spectraY"></param>
        /// <returns>Score based on how similar they are.</returns>
        double CompareSpectra(MSSpectra spectraX, MSSpectra spectraY);
    }
}
