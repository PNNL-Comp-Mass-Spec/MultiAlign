using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.SpectralProcessing
{
    /// <summary>
    /// Filters spectra based on a perctange value.
    /// </summary>
    public class TopPercentSpectralFilter : ISpectraFilter
    {
        #region ISpectraFilter Members
        public List<XYData> Threshold(List<XYData> peaks, double threshold)
        {

            // Find the non-zero peaks and order them based on intensity.
            var nonZeroPeaks = (from peak in peaks
                                where peak.Y > 0
                                orderby peak.Y ascending
                                select peak).ToList();

            // If the spectrum is empty, so will be the filtered version.
            if (nonZeroPeaks.Count < 1)
            {
                return peaks;
            }

            var N               = nonZeroPeaks.Count;                                   // make sure
            var Npercent        = Convert.ToInt32(Convert.ToDouble(N) * threshold);     // Find the percent of peaks
            Npercent            = Math.Max(0, Math.Min(N - 1, Npercent));               // then make sure it fits in the bounds
            var peakHeight   = nonZeroPeaks[Npercent].Y;


            var filteredPeaks   = (from peak in nonZeroPeaks
                                 where peak.Y > peakHeight
                                 orderby peak.X ascending
                                 select peak).ToList();

            return filteredPeaks;
        }
        #endregion
    }
}
