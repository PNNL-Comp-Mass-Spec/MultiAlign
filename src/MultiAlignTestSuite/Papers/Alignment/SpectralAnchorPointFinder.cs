using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Algorithms.SpectralProcessing;

namespace MultiAlignTestSuite.Papers.Alignment.SSM
{

    /// <summary>
    /// Finds Anchor Points using Spectral Comparisons
    /// </summary>
    public class SpectralAnchorPointFinder
    {
        /// <summary>
        /// Links anchor points use the raw spectra provided.
        /// </summary>
        /// <param name="readerX"></param>
        /// <param name="readerY"></param>
        /// <param name="comparer"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<AnchorPointMatch> FindAnchorPoints(  ISpectraProvider           readerX,
                                                                ISpectraProvider           readerY,
                                                                ISpectralComparer          comparer,
                                                                ISpectraFilter             filter,
                                                                SpectralOptions            options)
        {
            List<AnchorPointMatch> matches          = new List<AnchorPointMatch>();                    
            Dictionary<int, ScanSummary> scanDataX  = readerX.GetScanData(0);
            Dictionary<int, ScanSummary> scanDataY  = readerY.GetScanData(0);

            // Determine the scan extrema
            var maxX = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            var minX = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
            var maxY = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            var minY = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
            int Nx = scanDataX.Count;
            int Ny = scanDataX.Count;

            // Create a spectral comparer
            List<double> scans = new List<double>();
            Dictionary<int, MSSpectra> spectraMap = new Dictionary<int, MSSpectra>();

            foreach (int scanx in scanDataX.Keys)
            {
                ScanSummary xsum = scanDataX[scanx];

                if (xsum.MsLevel != 2)
                    continue;

                // Grab the first spectra
                MSSpectra spectrumX = SpectralUtilities.GetSpectra( options.MzTolerance, 
                                                                    options.TopIonPercent, 
                                                                    filter, 
                                                                    readerX, 
                                                                    scanDataX,
                                                                    scanx);

                // Iterate through the other analysis.  
                foreach (int scany in scanDataY.Keys)
                {
                    ScanSummary ysum = scanDataY[scany];
                    if (ysum.MsLevel != 2)
                        continue;

                    if (Math.Abs(xsum.PrecursorMZ - ysum.PrecursorMZ) >= options.MzTolerance)
                        continue;

                    // Grab the first spectra...if we have it, great dont re-read
                    MSSpectra spectrumY = null;
                    if (spectraMap.ContainsKey(scany))
                    {
                        spectrumY = spectraMap[scany];
                    }
                    else
                    {
                        spectrumY = SpectralUtilities.GetSpectra(   options.MzTolerance, 
                                                                    options.TopIonPercent, 
                                                                    filter, 
                                                                    readerY,
                                                                    scanDataY, 
                                                                    scany);
                        spectraMap.Add(scany, spectrumY);
                    }

                    // compare the spectra
                    double spectralSimilarity = comparer.CompareSpectra(spectrumX, spectrumY);
                    if (double.IsNaN(spectralSimilarity))
                    {
                        continue;
                    }

                    if (spectralSimilarity < options.SimilarityCutoff)
                        continue;

                    bool isMatch = false;
                    

                    if (double.IsNaN(spectralSimilarity) || double.IsNegativeInfinity(spectralSimilarity) || double.IsPositiveInfinity(spectralSimilarity))
                        continue;

                    double netX = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
                    double netY = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);
                    double net  = Convert.ToDouble(netX - netY);

                    // Has to pass the NET tolerance
                    if (options.NetTolerance < net) continue;

                    AnchorPointMatch match = new AnchorPointMatch();
                    AnchorPoint pointX  = new AnchorPoint();
                    pointX.Net          = netX;
                    pointX.Mass         = 0;
                    pointX.Mz           = xsum.PrecursorMZ;
                    pointX.Scan         = scanx;
                    pointX.Spectrum     = spectrumX;
                    
                    AnchorPoint pointY  = new AnchorPoint();
                    pointY.Net          = netX;
                    pointY.Mass         = 0;
                    pointY.Mz           = ysum.PrecursorMZ;
                    pointY.Scan         = scany;
                    pointY.Spectrum     = spectrumY;

                    match.AnchorPointX      = pointX;
                    match.AnchorPointY      = pointY;
                    match.SimilarityScore   = spectralSimilarity;
                    match.IsValidMatch      = isMatch;

                    matches.Add(match);                    
                }                    
            }

            return matches;
        }
    }
}
