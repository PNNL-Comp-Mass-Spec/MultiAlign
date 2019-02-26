using System;
using FeatureAlignment.Data;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    public class SpectralCosineComparer: ISpectralComparer
    {
        /// <summary>
        /// Constructor that keeps the top forty percent of ions in a spectra by default.
        /// </summary>
        public SpectralCosineComparer()
        {

        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="percent">Percentage of most intense ions to keep in the spectra.</param>
        public SpectralCosineComparer(double percent)
        {

        }


        #region ISpectralComparer Members
        /// <summary>
        /// Computes the dot product of two spectra.
        /// </summary>
        /// <param name="spectraX">Spectrum X</param>
        /// <param name="spectraY">Spectrum Y</param>
        /// <returns>Normalized Dot Product</returns>
        public double CompareSpectra(MSSpectra xSpectrum, MSSpectra ySpectrum)
        {
            var a = xSpectrum.Peaks;
            var b = ySpectrum.Peaks;

            double magX = 0;
            double magY = 0;

            // Then compute the magnitudes of the spectra
            double sum  = 0;
            for (var i = 0; i < xSpectrum.Peaks.Count; i++)
            {
                var x = a[i].Y;
                var y = b[i].Y;

                sum += Math.Sqrt(x * y);

                magY += y; // *y;
                magX += x; // *x;
            }

            var mag = Math.Sqrt(magX * magY);
            return sum / mag;
        }
        #endregion
    }
}
