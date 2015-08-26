using System;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    public class SpectralPeakCountComparer: ISpectralComparer
    {
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
                                               
            // Then compute the magnitudes of the spectra
            double sum  = 0;
            var xc = 0;
            var yc = 0;


            for (var i = 0; i < xSpectrum.Peaks.Count; i++)
            {
                var x = a[i].Y;
                var y = b[i].Y;

                if (x > 0)
                {
                    xc++;
                }

                if (y > 0)
                {
                    yc++;
                }

                if (x > 0 && y > 0)
                {
                    sum++;
                }
            }
            return Convert.ToDouble(sum) / Convert.ToDouble(xc + yc);
        }
        #endregion
    }
}
