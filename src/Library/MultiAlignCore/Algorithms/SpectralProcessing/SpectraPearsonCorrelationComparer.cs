using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data;
using InformedProteomics.Backend.MathAndStats;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    using InformedProteomics.Backend.Data.Spectrometry;

    public class SpectraPearsonCorrelationComparer : ISpectralComparer
    {
        private Tolerance tolerance;

        public SpectraPearsonCorrelationComparer(Tolerance tolerance)
        {
            this.tolerance = tolerance;
        }

        public double CompareSpectra(MSSpectra spectraX, MSSpectra spectraY)
        {
            var xIntensities = this.ExpandVector(spectraY.Peaks, spectraX.Peaks).Select(xPeak => xPeak.X).ToArray();
            var yIntensities = this.ExpandVector(spectraX.Peaks, spectraY.Peaks).Select(yPeak => yPeak.Y).ToArray();

            return FitScoreCalculator.GetPearsonCorrelation(xIntensities, yIntensities, yIntensities.Length);
        }

        /// <summary>
        /// Align the two spectra so they have the same number of peaks.
        /// Any peak that isn't present (within m/z tolerance) in the other
        /// spectrum is added with 0 intensity.
        /// </summary>
        /// <param name="spectraX">The spectrum to expand to.</param>
        /// <param name="spectraY">The spectrum to expand.</param>
        private List<XYData> ExpandVector(IReadOnlyList<XYData> spectraX, IReadOnlyList<XYData> spectraY)
        {
            var yTempSpec = new List<XYData>();

            var yIt = 0;
            foreach (var xPeak in spectraX)
            {
                var toleranceTh = this.tolerance.GetToleranceAsMz(xPeak.X);

                var minMz = Math.Max(0.0, xPeak.X + toleranceTh);
                var maxMz = xPeak.X + toleranceTh;

                var yPeak = spectraY[yIt];
                while (yPeak.X < minMz && yIt < spectraY.Count - 1)
                {
                    yPeak = spectraY[++yIt];
                }

                if (yPeak.X > maxMz)
                {
                    yTempSpec.Add(new XYData(xPeak.X, 0.0));
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
