using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    using InformedProteomics.Backend.Data.Spectrometry;
    using InformedProteomics.Backend.Utils;

    using MultiAlignCore.Data;

    public class SpectraPearsonCorrelationComparer : ISpectralComparer
    {
        private Tolerance tolerance;

        public SpectraPearsonCorrelationComparer(Tolerance tolerance)
        {
            this.tolerance = tolerance;
        }
        
        public double CompareSpectra(MSSpectra spectraX, MSSpectra spectraY)
        {
            var xIntensities = this.ExpandVector(spectraY.Peaks, spectraX.Peaks).Select(xpeak => xpeak.X).ToArray();
            var yIntensities = this.ExpandVector(spectraX.Peaks, spectraY.Peaks).Select(ypeak => ypeak.Y).ToArray();

            return FitScoreCalculator.GetPearsonCorrelation(xIntensities, yIntensities, yIntensities.Length);
        }

        /// <summary>
        /// Align the two spectra so they have the same number of peaks.
        /// Any peak that isn't present (within m/z tolerance) in the other
        /// spectrum is added with 0 intensity.
        /// </summary>
        /// <param name="spectraX">The spectrum to expand to.</param>
        /// <param name="spectraY">The spectrum to expand.</param>
        private List<XYData> ExpandVector(List<XYData> spectraX, List<XYData> spectraY)
        {
            var yTempSpec = new List<XYData>();

            var yIt = 0;
            for (int xIt = 0; xIt < spectraX.Count; xIt++)
            {
                var xpeak = spectraX[xIt];

                var toleranceTh = this.tolerance.GetToleranceAsTh(xpeak.X);

                var minMz = Math.Max(0.0, xpeak.X + toleranceTh);
                var maxMz = xpeak.X + toleranceTh;

                var ypeak = spectraY[yIt];
                while (ypeak.X < minMz && yIt < spectraY.Count - 1)
                {
                    ypeak = spectraY[++yIt];
                }

                if (ypeak.X > maxMz)
                {
                    yTempSpec.Add(new XYData(xpeak.X, 0.0));
                }
            }

            yTempSpec.AddRange(spectraY);
            yTempSpec.Sort(new XYDataMzComparer());

            return yTempSpec;
        }

        private class XYDataMzComparer : IComparer<XYData>
        {
            public int Compare(XYData x, XYData y)
            {
                return x.X.CompareTo(y.X);
            }
        }
    }
}
