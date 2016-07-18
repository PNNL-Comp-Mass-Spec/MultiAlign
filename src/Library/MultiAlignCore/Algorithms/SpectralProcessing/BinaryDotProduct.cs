using System;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    public class BinaryDotProduct : ISpectralComparer
    {
        public BinaryDotProduct(double percent)
        {
            Percent = percent;
        }

        public double Percent { get; set; }

        #region ISpectralComparer Members
        /// <summary>
        /// Compares two spectra to each other.  Assumes that the spectra are of equal dimensions.
        /// </summary>
        /// <param name="spectraX"></param>
        /// <param name="spectraY"></param>
        /// <returns></returns>
        public double CompareSpectra(MSSpectra spectraX, MSSpectra spectraY)
        {
            ISpectralNormalizer normalizer = new BinarySpectraNormalizer();
            var x = normalizer.Normalize(spectraX);
            var y = normalizer.Normalize(spectraY);

            var NM    = Convert.ToDouble(spectraX.Peaks.Count + spectraY.Peaks.Count);
            double sum   = 0;
            for (var i = 0; i < spectraX.Peaks.Count; i++)
            {
                sum += x.Peaks[i].Y * y.Peaks[i].Y;
            }

            return sum / NM;
        }

        #endregion
    }
}
