
using FeatureAlignment.Data;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    /// <summary>
    /// Converts a spectra
    /// </summary>
    public interface ISpectralNormalizer
    {
        /// <summary>
        /// Normalizes a spectrum
        /// </summary>
        /// <param name="spectrum"></param>
        /// <returns></returns>
        MSSpectra Normalize(MSSpectra spectrum);
    }



}
