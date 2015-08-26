using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlignCore.Algorithms.SpectralProcessing;
using MultiAlignCore.Data;

namespace MultiAlignCore.Algorithms.Alignment.SpectralMatching
{

    /// <summary>
    /// Finds Anchor Points using Spectral Comparisons
    /// </summary>
    public class SpectralAnchorPointFinder
    {
        ///// <summary>
        ///// Links anchor points use the raw spectra provided.
        ///// </summary>
        //public IEnumerable<SpectralAnchorPointMatch> FindAnchorPoints2( ISpectraProvider            readerX,
        //                                                                ISpectraProvider           readerY,
        //                                                                ISpectralComparer          comparer,
        //                                                                ISpectraFilter             filter,
        //                                                                SpectralOptions            options,
        //                                                                bool skipComparison        = true)
        //{
        //    var matches = new List<SpectralAnchorPointMatch>();                    
        //    var scanDataX  = readerX.GetScanData(0);
        //    var scanDataY  = readerY.GetScanData(0);

        //    // Determine the scan extrema
        //    var maxX = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
        //    var minX = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
        //    var maxY = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
        //    var minY = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;

        //    // Create a spectral comparer
        //    var ySpectraCache = new Dictionary<int, MSSpectra>();

        //    // Here we sort the summary spectra....so that we can improve run time efficiency
        //    // and minimize as much memory as possible.
        //    var ySpectraSummary = scanDataY.Values.Where(summary => summary.MsLevel == 2).ToList();
        //    var xSpectraSummary = scanDataX.Values.Where(summary => summary.MsLevel == 2).ToList();

        //    ySpectraSummary.Sort((x, y) => x.PrecursorMZ.CompareTo(y.PrecursorMZ));
        //    xSpectraSummary.Sort((x, y) => x.PrecursorMZ.CompareTo(y.PrecursorMZ));
            
        //    double mzTolerance = options.MzTolerance;

        //    foreach (var xsum in xSpectraSummary)
        //    {
        //        int scanx = xsum.Scan;              

        //        // Grab the first spectra
        //        var spectrumX     = SpectralUtilities.GetSpectra(options.MzBinSize, 
        //                                                            options.TopIonPercent, 
        //                                                            filter, 
        //                                                            readerX, 
        //                                                            scanx,
        //                                                            options.RequiredPeakCount);

        //        spectrumX.PrecursorMZ   = xsum.PrecursorMZ;


        //        // Here we make sure that we are efficiently using the cache...we want to clear any 
        //        // cached spectra that we arent using.  We know that the summaries are sorted by m/z
        //        // so if the xsum m/z is greater than anything in the cache, dump the spectra...
        //        double currentMz = xsum.PrecursorMZ;
        //        // Use linq?
        //        var toRemove = new List<int>();
        //        foreach (int scan in ySpectraCache.Keys)
        //        {
        //            MSSpectra yscan     = ySpectraCache[scan];
        //            double difference   = currentMz - yscan.PrecursorMZ;
        //            // We only need to care about smaller m/z's
        //            if (difference >= mzTolerance)
        //            {
        //                toRemove.Add(scan);
        //            }
        //            else
        //            {
        //                // Because if we are here, we are within range...AND! 
        //                // ...the m/z of i + 1 > i...because they are sorted...
        //                // so if the m/z comes within range (positive) then 
        //                // that means we need to evaluate the tolerance.
        //                break;
        //            }
        //        }

        //        // Then we clean up...since spectra can be large...we'll take the performance hit here...
        //        // and minimize memory impacts!
        //        if (toRemove.Count > 0)
        //        {
        //            toRemove.ForEach(x => ySpectraCache.Remove(x));
        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //        }

        //        // Iterate through the other analysis.  
        //        foreach (var ysum in ySpectraSummary)
        //        {
        //            int scany = ysum.Scan;

        //            // We know that we are out of range here....
        //            if (Math.Abs(xsum.PrecursorMZ - ysum.PrecursorMZ) >= mzTolerance)
        //                continue;

        //            double netX = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
        //            double netY = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);
        //            double net  = Convert.ToDouble(netX - netY);

        //            // Has to pass the NET tolerance
        //            if (options.NetTolerance < Math.Abs(net)) continue;


        //            // Grab the first spectra...if we have it, great dont re-read
        //            MSSpectra spectrumY = null;
        //            if (ySpectraCache.ContainsKey(scany))
        //            {
        //                if (!skipComparison)
        //                    spectrumY = ySpectraCache[scany];
        //            }
        //            else
        //            {
        //                if (!skipComparison)
        //                {
        //                    spectrumY = SpectralUtilities.GetSpectra(options.MzBinSize,
        //                                                            options.TopIonPercent,
        //                                                            filter,
        //                                                            readerY,
        //                                                            scany,
        //                                                            options.RequiredPeakCount);
        //                    spectrumY.PrecursorMZ = ysum.PrecursorMZ;
        //                    ySpectraCache.Add(scany, spectrumY);
        //                }
        //            }

        //            // compare the spectra
        //            double spectralSimilarity = 0;


        //            if (!skipComparison)
        //                spectralSimilarity = comparer.CompareSpectra(spectrumX, spectrumY);

        //            if (double.IsNaN(spectralSimilarity) || double.IsNegativeInfinity(spectralSimilarity) || double.IsPositiveInfinity(spectralSimilarity))
        //                continue;

        //            if (spectralSimilarity < options.SimilarityCutoff)
        //                continue;
                                      
        //            var pointX      = new SpectralAnchorPoint
        //            {
        //                Net = netX,
        //                Mass = 0,
        //                Mz = xsum.PrecursorMZ,
        //                Scan = scanx,
        //                Spectrum = spectrumX
        //            };

        //            var pointY = new SpectralAnchorPoint
        //            {
        //                Net = netX,
        //                Mass = 0,
        //                Mz = ysum.PrecursorMZ,
        //                Scan = scany,
        //                Spectrum = spectrumY
        //            };

        //            var match = new SpectralAnchorPointMatch
        //            {
        //                AnchorPointX    = pointX,
        //                AnchorPointY    = pointY,
        //                SimilarityScore = spectralSimilarity,
        //                IsValidMatch    = AnchorPointMatchType.FalseMatch
        //            };

        //            matches.Add(match);                    
        //        }                    
        //    }

        //    return matches;
        //}

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
        public IEnumerable<SpectralAnchorPointMatch> FindAnchorPoints(  ISpectraProvider   readerX,
                                                                        ISpectraProvider   readerY,
                                                                        ISpectralComparer  comparer,
                                                                        ISpectraFilter     filter,
                                                                        SpectralOptions    options,
                                                                        bool               skipComparison = false)
        {
            var matches = new List<SpectralAnchorPointMatch>();
            var scanDataX  = readerX.GetScanData(0);
            var scanDataY  = readerY.GetScanData(0);

            // Determine the scan extrema
            var maxX = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            var minX = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
            var maxY = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            var minY = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;

            // Here we sort the summary spectra....so that we can improve run time efficiency
            // and minimize as much memory as possible.
            var ySpectraSummary = scanDataY.Values.Where(summary => summary.MsLevel == 2).ToList();
            var xSpectraSummary = scanDataX.Values.Where(summary => summary.MsLevel == 2).ToList();


            ySpectraSummary.Sort((x, y) => x.PrecursorMz.CompareTo(y.PrecursorMz));
            xSpectraSummary.Sort((x, y) => x.PrecursorMz.CompareTo(y.PrecursorMz));

            var netTolerance = options.NetTolerance;
            var mzTolerance  = options.MzTolerance;
            var j               = 0;
            var i               = 0;
            var yTotal          = ySpectraSummary.Count;
            var xTotal          = xSpectraSummary.Count;

            var similarities = new List<double>();

            var cache   = new Dictionary<int, MSSpectra>();
            var pointsY = new Dictionary<int, SpectralAnchorPoint>();

            while (i < xTotal && j < yTotal)
            {
                var xsum    = xSpectraSummary[i];
                var scanx           = xsum.Scan;
                var precursorX   = xsum.PrecursorMz;
                MSSpectra spectrumX = null;

                while (j < yTotal && ySpectraSummary[j].PrecursorMz < (precursorX - mzTolerance))
                {
                    // Here we make sure we arent caching something 
                    var scany = ySpectraSummary[j].Scan;
                    if (cache.ContainsKey(scany))
                    {
                        cache.Remove(scany);
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


                var k = 0;
                var points = new List<SpectralAnchorPoint>();

                while ((j + k) < yTotal && Math.Abs(ySpectraSummary[j + k].PrecursorMz - precursorX) < mzTolerance)
                {
                    var ysum = ySpectraSummary[j + k];
                    k++;
                    var scany       = ysum.Scan;
                    var netX     = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
                    var netY     = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);
                    var net      = Convert.ToDouble(netX - netY);

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
                                    spectrumX.PrecursorMz = xsum.PrecursorMz;
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
                                    spectrumY.PrecursorMz = ysum.PrecursorMz;
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

                        // similarities.Add(spectralSimilarity);
                        File.AppendAllText(@"c:\data\proteomics\test.txt", string.Format("{0}\t{1}\t{2}\n", spectrumX.PrecursorMz, spectrumY.PrecursorMz, spectralSimilarity));

                        if (double.IsNaN(spectralSimilarity) || double.IsInfinity(spectralSimilarity))
                            continue;



                        if (spectralSimilarity < options.SimilarityCutoff)
                            continue;

                        var pointX = new SpectralAnchorPoint
                        {
                            Net = netX,
                            Mass = 0,
                            Mz = xsum.PrecursorMz,
                            Scan = scanx,
                            Spectrum = spectrumX
                        };

                        var pointY = new SpectralAnchorPoint
                        {
                            Net = netY,
                            Mass = 0,
                            Mz = ysum.PrecursorMz,
                            Scan = scany,
                            Spectrum = spectrumY
                        };

                        var match = new SpectralAnchorPointMatch();
                        match.AnchorPointX      = pointX;
                        match.AnchorPointY      = pointY;
                        match.SimilarityScore   = spectralSimilarity;
                        match.IsValidMatch      = AnchorPointMatchType.FalseMatch;
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
