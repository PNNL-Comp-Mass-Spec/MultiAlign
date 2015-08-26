using System;
using System.Collections.Generic;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    public class SpectralNormalizedDotProductComparer: ISpectralComparer
    {
        /// <summary>
        /// Constructor that keeps the top forty percent of ions in a spectra by default.
        /// </summary>
        public SpectralNormalizedDotProductComparer()
        {
            TopPercent = .4;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="percent">Percentage of most intense ions to keep in the spectra.</param>
        public SpectralNormalizedDotProductComparer(double percent)
        {
            TopPercent = percent;
        }

        /// <summary>
        /// Gets or sets the top spectra values to keep.
        /// </summary>
        public double TopPercent
        {
            get;
            set;
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
            var x = xSpectrum.Peaks;
            var y = ySpectrum.Peaks;

            double magX = 0;
            double magY = 0;
            var N       = x.Count;

            // Compute magnitudes of x y spectra
            var xIons = new List<double>(N);
            var yIons = new List<double>(N);
            var xTotalNonZero  = 0;
            var yTotalNonZero  = 0;

            for (var i = 0; i < x.Count; i++)
            {
                if (x[i].Y > 0)
                {
                    xTotalNonZero++;
                }
                if (y[i].Y > 0)
                {
                    yTotalNonZero++;
                }

                xIons.Add(x[i].Y);
                yIons.Add(y[i].Y);
            }
            // Find the top ions to keep.
            var xTopIons = new List<double>(N);
            var yTopIons = new List<double>(N);
            xTopIons.AddRange(xIons);
            yTopIons.AddRange(yIons);

            xTopIons.Sort();
            yTopIons.Sort();
            
            var xTop = Math.Max(0, xTopIons.Count - System.Convert.ToInt32(System.Convert.ToDouble(xTotalNonZero) * TopPercent));
            var yTop = Math.Max(0, yTopIons.Count - System.Convert.ToInt32(System.Convert.ToDouble(yTotalNonZero) * TopPercent));
            
            xTop = Math.Min(xTopIons.Count - 1, xTop);
            yTop = Math.Min(yTopIons.Count - 1, yTop);

            var xThreshold = xTopIons[xTop];
            var yThreshold = yTopIons[yTop];

            // Then compute the magnitudes of the spectra
            for (var i = 0; i < x.Count; i++)
            {
                var xIon = xIons[i];
                var yIon = yIons[i];

                if (xIon < xThreshold)
                    xIon = 0;

                if (yIon <= yThreshold)
                    yIon = 0;

                magY += yIon * yIon;
                magX += xIon * xIon;
            }
            magY = Math.Sqrt(magY);
            magX = Math.Sqrt(magX);


            // Normalize each component and calculate the dot product.
            double sum = 0;
            for (var i = 0; i < x.Count; i++)
            {
                var xIon = xIons[i];
                var yIon = yIons[i];

                if (xIon < xThreshold)
                    xIon = 0;

                if (yIon <= yThreshold)
                    yIon = 0;

                xIon /= magX;
                yIon /= magY;
                
                sum += (xIon * yIon);
            }
            
            return sum;
        }
        #endregion
    }
}
