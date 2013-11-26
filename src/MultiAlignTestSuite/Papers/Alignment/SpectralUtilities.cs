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
        public static MSSpectra GetSpectrum(ISpectraProvider reader, int scan, int group, double mzTolerance = .5)
        {
            List<XYData> peaks  = reader.GetRawSpectra(scan, group, 2);
            MSSpectra spectrum  = new MSSpectra();
            spectrum.Peaks      = peaks;

            return spectrum;
        }

        public static MSSpectra GetSpectra(double mzTolerance,
                                   double percent,
                                   ISpectraFilter filter,
                                   ISpectraProvider readerY,
                                   Dictionary<int, ScanSummary> scanDataY,
                                   int scany)
        {
            MSSpectra spectrumY = SpectralUtilities.GetSpectrum(readerY,
                                        scanDataY[scany].Scan,
                                        0,
                                        mzTolerance: mzTolerance);
            spectrumY           = filter.Threshold(spectrumY, percent);
            spectrumY.Peaks     = XYData.Bin(spectrumY.Peaks,
                                                0,
                                                2000,
                                                mzTolerance);
            return spectrumY;
        }
    }
}
