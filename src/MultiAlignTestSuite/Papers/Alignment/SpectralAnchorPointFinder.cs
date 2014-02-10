using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Algorithms.SpectralProcessing;
using System.Diagnostics;

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
        public IEnumerable<AnchorPointMatch> FindAnchorPoints2(  ISpectraProvider           readerX,
                                                                ISpectraProvider           readerY,
                                                                ISpectralComparer          comparer,
                                                                ISpectraFilter             filter,
                                                                SpectralOptions            options,
                                                                bool skipComparison=true)
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
            Dictionary<int, MSSpectra> ySpectraCache = new Dictionary<int, MSSpectra>();

            // Here we sort the summary spectra....so that we can improve run time efficiency
            // and minimize as much memory as possible.
            List<ScanSummary> ySpectraSummary = new List<ScanSummary>();
            foreach (var summary in scanDataY.Values)
            {
                if (summary.MsLevel != 2)
                    continue;

                ySpectraSummary.Add(summary);
            }
            List<ScanSummary> xSpectraSummary = new List<ScanSummary>();
            foreach (var summary in scanDataX.Values)
            {
                if (summary.MsLevel != 2)
                    continue;
                xSpectraSummary.Add(summary);
            }
            ySpectraSummary.Sort(delegate(ScanSummary x, ScanSummary y) { return x.PrecursorMZ.CompareTo(y.PrecursorMZ); });
            xSpectraSummary.Sort(delegate(ScanSummary x, ScanSummary y) { return x.PrecursorMZ.CompareTo(y.PrecursorMZ); });
            
            double mzTolerance = options.MzTolerance;

            foreach (ScanSummary xsum in xSpectraSummary)
            {
                int scanx = xsum.Scan;              

                // Grab the first spectra
                MSSpectra spectrumX     = SpectralUtilities.GetSpectra(options.MzBinSize, 
                                                                    options.TopIonPercent, 
                                                                    filter, 
                                                                    readerX, 
                                                                    scanx,
                                                                    options.RequiredPeakCount);

                spectrumX.PrecursorMZ   = xsum.PrecursorMZ;


                // Here we make sure that we are efficiently using the cache...we want to clear any 
                // cached spectra that we arent using.  We know that the summaries are sorted by m/z
                // so if the xsum m/z is greater than anything in the cache, dump the spectra...
                double currentMz = xsum.PrecursorMZ;
                // Use linq?
                List<int> toRemove = new List<int>();
                foreach (int scan in ySpectraCache.Keys)
                {
                    MSSpectra yscan     = ySpectraCache[scan];
                    double difference   = currentMz - yscan.PrecursorMZ;
                    // We only need to care about smaller m/z's
                    if (difference >= mzTolerance)
                    {
                        toRemove.Add(scan);
                    }
                    else
                    {
                        // Because if we are here, we are within range...AND! 
                        // ...the m/z of i + 1 > i...because they are sorted...
                        // so if the m/z comes within range (positive) then 
                        // that means we need to evaluate the tolerance.
                        break;
                    }
                }

                // Then we clean up...since spectra can be large...we'll take the performance hit here...
                // and minimize memory impacts!
                if (toRemove.Count > 0)
                {
                    toRemove.ForEach(x => ySpectraCache.Remove(x));
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                // Iterate through the other analysis.  
                foreach (ScanSummary ysum in ySpectraSummary)
                {
                    int scany = ysum.Scan;

                    // We know that we are out of range here....
                    if (Math.Abs(xsum.PrecursorMZ - ysum.PrecursorMZ) >= mzTolerance)
                        continue;

                    double netX = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
                    double netY = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);
                    double net  = Convert.ToDouble(netX - netY);

                    // Has to pass the NET tolerance
                    if (options.NetTolerance < Math.Abs(net)) continue;


                    // Grab the first spectra...if we have it, great dont re-read
                    MSSpectra spectrumY = null;
                    if (ySpectraCache.ContainsKey(scany))
                    {
                        if (!skipComparison)
                            spectrumY = ySpectraCache[scany];
                    }
                    else
                    {
                        if (!skipComparison)
                        {
                            spectrumY = SpectralUtilities.GetSpectra(options.MzBinSize,
                                                                    options.TopIonPercent,
                                                                    filter,
                                                                    readerY,
                                                                    scany,
                                                                    options.RequiredPeakCount);
                            spectrumY.PrecursorMZ = ysum.PrecursorMZ;
                            ySpectraCache.Add(scany, spectrumY);
                        }
                    }

                    // compare the spectra
                    double spectralSimilarity = 0;


                    if (!skipComparison)
                        spectralSimilarity = comparer.CompareSpectra(spectrumX, spectrumY);

                    if (double.IsNaN(spectralSimilarity) || double.IsNegativeInfinity(spectralSimilarity) || double.IsPositiveInfinity(spectralSimilarity))
                        continue;

                    if (spectralSimilarity < options.SimilarityCutoff)
                        continue;
                                      
                    AnchorPointMatch match  = new AnchorPointMatch();
                    AnchorPoint pointX      = new AnchorPoint();
                    pointX.Net              = netX;
                    pointX.Mass             = 0;
                    pointX.Mz               = xsum.PrecursorMZ;
                    pointX.Scan             = scanx;
                    pointX.Spectrum         = spectrumX;
                    
                    AnchorPoint pointY      = new AnchorPoint();
                    pointY.Net              = netX;
                    pointY.Mass             = 0;
                    pointY.Mz               = ysum.PrecursorMZ;
                    pointY.Scan             = scany;
                    pointY.Spectrum         = spectrumY;

                    match.AnchorPointX      = pointX;
                    match.AnchorPointY      = pointY;
                    match.SimilarityScore   = spectralSimilarity;
                    match.IsValidMatch      = AnchorMatch.FalseMatch;

                    matches.Add(match);                    
                }                    
            }

            return matches;
        }

        /// <summary>
        /// Computes all anchor point matches between two sets of spectra.
        /// </summary>
        /// <param name="readerX"></param>
        /// <param name="readerY"></param>
        /// <param name="comparer"></param>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        /// <param name="skipComparison"></param>
        /// <returns></returns>
        public IEnumerable<AnchorPointMatch> FindAnchorPoints(ISpectraProvider readerX,
                                                                ISpectraProvider readerY,
                                                                ISpectralComparer comparer,
                                                                ISpectraFilter filter,
                                                                SpectralOptions options,
                                                                bool skipComparison = false)
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

            // Here we sort the summary spectra....so that we can improve run time efficiency
            // and minimize as much memory as possible.
            List<ScanSummary> ySpectraSummary = new List<ScanSummary>();
            foreach (var summary in scanDataY.Values)
            {
                if (summary.MsLevel != 2)
                    continue;

                ySpectraSummary.Add(summary);
            }
            List<ScanSummary> xSpectraSummary = new List<ScanSummary>();
            foreach (var summary in scanDataX.Values)
            {
                if (summary.MsLevel != 2)
                    continue;
                xSpectraSummary.Add(summary);
            }
            ySpectraSummary.Sort(delegate(ScanSummary x, ScanSummary y) { return x.PrecursorMZ.CompareTo(y.PrecursorMZ); });
            xSpectraSummary.Sort(delegate(ScanSummary x, ScanSummary y) { return x.PrecursorMZ.CompareTo(y.PrecursorMZ); });

            double netTolerance = options.NetTolerance;
            double mzTolerance  = options.MzTolerance;
            int j               = 0;
            int i               = 0;
            int yTotal          = ySpectraSummary.Count;
            int xTotal          = xSpectraSummary.Count;

            Dictionary<int, MSSpectra> cache = new Dictionary<int, MSSpectra>();

            int removalCount = 0;
            Dictionary<int, AnchorPoint> pointsY = new Dictionary<int, AnchorPoint>();

            while (i < xTotal && j < yTotal)
            {
                ScanSummary xsum    = xSpectraSummary[i];
                int scanx           = xsum.Scan;
                double precursorX   = xsum.PrecursorMZ;
                MSSpectra spectrumX = null;

                while (j < yTotal && ySpectraSummary[j].PrecursorMZ < (precursorX - mzTolerance))
                {
                    // Here we make sure we arent caching something 
                    int scany = ySpectraSummary[j].Scan;
                    if (cache.ContainsKey(scany))
                    {
                        cache.Remove(scany);
                        removalCount++;
                        if (pointsY.ContainsKey(scany))
                        {
                            if (pointsY[scany].Spectrum.Peaks != null)
                            {
                                pointsY[scany].Spectrum.Peaks.Clear();
                                pointsY[scany].Spectrum.Peaks = null;
                            }
                        }
                    }
                    j++;
                }


                int k = 0;
                var points = new List<AnchorPoint>();

                while ((j + k) < yTotal && Math.Abs(ySpectraSummary[j + k].PrecursorMZ - precursorX) < mzTolerance)
                {
                    ScanSummary ysum = ySpectraSummary[j + k];
                    k++;
                    int scany       = ysum.Scan;
                    double netX     = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
                    double netY     = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);
                    double net      = Convert.ToDouble(netX - netY);

                    // Test whether the spectra are within decent range.
                    if (Math.Abs(net) < netTolerance)
                    {
                        // We didnt pull this spectrum before, because we arent sure
                        // if it will be within tolerance....so we just delay this
                        // until we have to...after this happens, we only pull it once.
                        if (spectrumX == null)
                        {
                            if (!skipComparison)
                            {
                                // Grab the first spectra
                                spectrumX = SpectralUtilities.GetSpectra(options.MzBinSize,
                                                                        options.TopIonPercent,
                                                                        filter,
                                                                        readerX,
                                                                        scanx,
                                                                        options.RequiredPeakCount);

                                if (spectrumX != null)
                                {
                                    spectrumX.PrecursorMZ = xsum.PrecursorMZ;
                                }
                                else
                                {
                                    // This spectra does not have enough peaks or did not pass our filters, throw it away!
                                    break;
                                }
                            }
                        }
                        MSSpectra spectrumY = null;
                        if (!skipComparison)
                        {

                            if (cache.ContainsKey(scany))
                            {
                                spectrumY = cache[scany];
                            }
                            else
                            {
                                spectrumY = SpectralUtilities.GetSpectra(options.MzBinSize,
                                                                        options.TopIonPercent,
                                                                        filter,
                                                                        readerY,
                                                                        scany,
                                                                        options.RequiredPeakCount);

                                if (spectrumY != null)
                                {
                                    spectrumY.PrecursorMZ = ysum.PrecursorMZ;
                                    cache.Add(scany, spectrumY);
                                }
                                else continue; // This spectra does not have enough peaks or did not pass our filters, throw it away!
                            }                            
                        }

                        if (spectrumX == null || spectrumY == null)
                            continue;

                        // compare the spectra
                        double spectralSimilarity = 0;
                        if (!skipComparison)
                            spectralSimilarity = comparer.CompareSpectra(spectrumX, spectrumY);

                        if (double.IsNaN(spectralSimilarity) || double.IsInfinity(spectralSimilarity))
                            continue;

                        if (spectralSimilarity < options.SimilarityCutoff)
                            continue;

                        AnchorPointMatch match  = new AnchorPointMatch();
                        AnchorPoint pointX      = new AnchorPoint();
                        pointX.Net              = netX;
                        pointX.Mass             = 0;
                        pointX.Mz               = xsum.PrecursorMZ;
                        pointX.Scan             = scanx;
                        pointX.Spectrum         = spectrumX;

                        AnchorPoint pointY      = new AnchorPoint();
                        pointY.Net              = netY;
                        pointY.Mass             = 0;
                        pointY.Mz               = ysum.PrecursorMZ;
                        pointY.Scan             = scany;
                        pointY.Spectrum         = spectrumY;

                        match.AnchorPointX      = pointX;
                        match.AnchorPointY      = pointY;
                        match.SimilarityScore   = spectralSimilarity;
                        match.IsValidMatch      = AnchorMatch.FalseMatch;
                        matches.Add(match);


                        points.Add(pointX);
                        if (!pointsY.ContainsKey(scany))
                        {
                            pointsY.Add(scany, pointY);
                        }
                    }
                }
                // Move to the next spectra in the x-list
                i++;
                foreach (var p in points)
                {
                    if (p.Spectrum.Peaks != null)
                    {
                        p.Spectrum.Peaks.Clear();
                        p.Spectrum.Peaks = null;
                    }
                }
                points.Clear();
            }
            return matches;
        }
    }
}
