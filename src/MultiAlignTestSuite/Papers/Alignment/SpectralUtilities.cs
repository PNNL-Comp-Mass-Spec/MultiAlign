using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Algorithms.SpectralProcessing;

namespace MultiAlignTestSuite.Papers.Alignment
{
    public static class SpectralUtilities
    {
         static SpectralUtilities()
        {
            ShouldLogScale = false;
        }

        public static bool ShouldLogScale { get; set; }

        public static MSSpectra GetSpectrum(ISpectraProvider reader, int scan, int group, double mzTolerance = .5)
        {

            var summary = new ScanSummary();
            List<XYData> peaks  = reader.GetRawSpectra(scan, group, 2, out summary);

            if (ShouldLogScale)
            {
                foreach (var peak in peaks)
                {
                    peak.Y = Math.Log(peak.Y, 2);
                }
            }

            MSSpectra spectrum  = new MSSpectra();
            spectrum.Peaks      = peaks;

            return spectrum;
        }

        public static MSSpectra GetSpectra(double mzTolerance,
                                           double percent,
                                           ISpectraFilter filter,
                                           ISpectraProvider readerY,                                           
                                           int scany,
                                           int numberRequiredPeaks)
        {
            MSSpectra spectrumY = SpectralUtilities.GetSpectrum(readerY,
                                                                scany,
                                                                0,
                                                                mzTolerance: mzTolerance);

            if (spectrumY.Peaks.Count < numberRequiredPeaks)
                return null;

            spectrumY           = filter.Threshold(spectrumY, percent);
            spectrumY.Peaks     = XYData.Bin(spectrumY.Peaks,
                                                0,
                                                2000,
                                                mzTolerance);
            return spectrumY;
        }
    }
}
