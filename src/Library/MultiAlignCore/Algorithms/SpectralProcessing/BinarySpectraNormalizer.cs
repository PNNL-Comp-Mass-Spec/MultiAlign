
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    /// <summary>
    /// Converts a spectrum into a binary one
    /// </summary>
    public class BinarySpectraNormalizer : ISpectralNormalizer
    {
        #region ISpectralNormalizer Members
        /// <summary>
        /// Converts a spectra into a binary one.
        /// </summary>
        /// <param name="spectrum"></param>
        /// <returns></returns>
        public MSSpectra Normalize(MSSpectra spectrum)
        {
            var filteredSpectrum = new MSSpectra();

            foreach (var peak in spectrum.Peaks)
            {
                var data = new XYData(peak.X, 1);
                filteredSpectrum.Peaks.Add(data);
            }

            return filteredSpectrum;
        }
        #endregion
    }
}
