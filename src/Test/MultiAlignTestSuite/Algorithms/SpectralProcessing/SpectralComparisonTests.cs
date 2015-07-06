#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.RawData;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using LinearAxis = OxyPlot.Axes.LinearAxis;

#endregion

namespace MultiAlignTestSuite.Algorithms
{
    [TestFixture]
    public class SpectralComparisonTest: TestBase
    {
        /// <summary>
        ///     Maps the path to a group ID for reading multiple files.
        /// </summary>
        private Dictionary<string, int> m_pathMap = new Dictionary<string, int>();

        #region Spectra Retrieval

        private MSSpectra GetSpectrum(string path, int scan)
        {
            ISpectraProvider reader = new ThermoRawDataFileReader();
            reader.AddDataFile(path, 0);
            return GetSpectrum(reader, scan, 0);
        }

        private MSSpectra GetSpectrum(ISpectraProvider reader, int scan, int group, double mzTolerance = .5)
        {
            var summary = new ScanSummary();
            var peaks = reader.GetRawSpectra(scan, group, 2, out summary);
            var spectrum = new MSSpectra();
            spectrum.Peaks = peaks;

            return spectrum;
        }

        #endregion

        #region Display

        /// <summary>
        ///     Creates a plot for the given peaks list
        /// </summary>
        /// <param name="peaksX"></param>
        /// <param name="peaksY"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private PlotModel CreatePlot(List<XYData> peaksX,
            List<XYData> peaksY,
            double tolerance)
        {
            var plotModel1 = new PlotModel();
            plotModel1.LegendBorderThickness = 0;
            plotModel1.LegendOrientation = LegendOrientation.Horizontal;
            plotModel1.LegendPlacement = LegendPlacement.Outside;
            plotModel1.LegendPosition = LegendPosition.BottomCenter;
            plotModel1.Title = "MS/MS Spectra";

            var categoryAxis1 = new LinearAxis();
            categoryAxis1.MinorStep = tolerance;
            plotModel1.Axes.Add(categoryAxis1);

            var linearAxis1 = new LinearAxis();
            linearAxis1.MaximumPadding = 0.06;
            linearAxis1.MinimumPadding = 0.06;
            plotModel1.Axes.Add(linearAxis1);

            var xseries = new StemSeries();
            for (var j = 0; j < peaksY.Count; j++)
            {
                var peakX = peaksX[j];
                var peakY = peaksY[j];

                double value = 0;
                if (peakX.Y > 0 && peakY.Y > 0)
                {
                    value = 1;
                }
                xseries.Points.Add(new DataPoint(peakX.X, value));
            }
            xseries.Color = OxyColors.Green;

            //plotModel1.Series.Add(xseries);

            var series = new StemSeries();
            series.Title = "Spectra X";
            double max = 0;
            foreach (var datum in peaksX)
            {
                max = Math.Max(max, datum.Y);
            }
            foreach (var datum in peaksX)
            {
                series.Points.Add(new DataPoint(datum.X, datum.Y/max));
            }
            plotModel1.Series.Add(series);

            foreach (var datum in peaksY)
            {
                max = Math.Max(max, datum.Y);
            }
            var series2 = new StemSeries();
            series2.Title = "Spectra Y";
            foreach (var datum in peaksY)
            {
                series2.Points.Add(new DataPoint(datum.X, (datum.Y*-1)/max));
            }
            plotModel1.Series.Add(series2);


            return plotModel1;
        }

        private void DisplayComparisonPlot(MSSpectra spectrumX, MSSpectra spectrumY, double mzTolerance,
            string path = "comparison.png", string newTitle = "MS/MS Spectra")
        {
            var model = CreatePlot(spectrumX.Peaks, spectrumY.Peaks, mzTolerance);
            model.Title = newTitle;

            var plot = new Plot();
            plot.Model = model;
            var form = new Form();
            form.Size = Screen.PrimaryScreen.WorkingArea.Size;
            plot.Dock = DockStyle.Fill;
            form.Controls.Add(plot);
            form.Show();

            IO.Utilities.SleepNow(3);
            
            using (var bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, form.DisplayRectangle);
                bitmap.Save(path);
            }
        }

        #endregion

        #region Single Comparison Displays

        [Test(Description = "Compares two spectra against each other.")]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            .5,
            Ignore = false)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            7413,
            1,
            Ignore = false)]
        public void DisplayComparisonFigure(
            string pathX, int scanX, 
            string pathY, int scanY, 
            double mzTolerance)
        {
            // Convert relative paths to absolute paths
            pathX = GetPath(pathX);
            pathY = GetPath(pathY);

            var spectrumX = GetSpectrum(pathX, scanX);
            var spectrumY = GetSpectrum(pathY, scanY);

            var path = Path.GetDirectoryName(pathX);
            var pathCompareImage = Path.Combine(path, string.Format("comparison-{0}-{1}.png", scanX, scanY));

            spectrumX.Peaks = XYData.Bin(spectrumX.Peaks,
                0,
                2000,
                mzTolerance);

            spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
                0,
                2000,
                mzTolerance);

            DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, path = pathCompareImage);
        }

        [Test(Description = "Compares two spectra against each other.")]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            SpectralComparison.NormalizedDotProduct,
            .5,
            Ignore = true)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            15304,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            17614,
            SpectralComparison.NormalizedDotProduct,
            .5,
            Ignore = true)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            SpectralComparison.PeakCounts,
            SpectraFilters.TopPercent,
            .8,
            1,
            Ignore = false)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            7413,
            SpectralComparison.PeakCounts,
            SpectraFilters.TopPercent,
            .8,
            1,
            Ignore = false)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            SpectralComparison.CosineDotProduct,
            SpectraFilters.TopPercent,
            .8,
            1,
            Ignore = false)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            7413,
            SpectralComparison.CosineDotProduct,
            SpectraFilters.TopPercent,
            .8,
            1,
            Ignore = false)]
        public void TestSpectralSimilarityScore(
            string pathX, int scanX,
            string pathY, int scanY,
            SpectralComparison comparerType,
            SpectraFilters filterType,
            double percent,
            double mzTolerance)
        {
            // Convert relative paths to absolute paths
            pathX = GetPath(pathX);
            pathY = GetPath(pathY);

            var spectrumX = GetSpectrum(pathX, scanX);
            var spectrumY = GetSpectrum(pathY, scanY);
            var comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType, percent);

            var filter = SpectrumFilterFactory.CreateFilter(filterType);
            spectrumX.Peaks = filter.Threshold(spectrumX.Peaks, percent);
            spectrumY.Peaks = filter.Threshold(spectrumY.Peaks, percent);

            spectrumX.Peaks = XYData.Bin(spectrumX.Peaks,
                0,
                2000,
                mzTolerance);

            spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
                0,
                2000,
                mzTolerance);

            var value = comparer.CompareSpectra(spectrumX, spectrumY);

            var path = Path.GetDirectoryName(pathX);
            var plotTitle = string.Format("comparison-{2}-{3}-{0}-{1}_{4:0.000}", scanX, scanY, comparerType, percent,
                value);
            var pathCompareImage = Path.Combine(path, plotTitle + ".png");

            DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, pathCompareImage, plotTitle);
        }

        #endregion

        #region Peptide file reading

        /// <summary>
        ///     Gets a list of the peptide matches from the path provided.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<PeptideMatch> GetPeptideMatches(string path)
        {
            var lines = File.ReadAllLines(path).ToList();

            var matches = new List<PeptideMatch>();
            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (data.Length < 6)
                {
                    continue;
                }

                try
                {
                    var match = new PeptideMatch();
                    match.Peptide = data[5];
                    match.ScanX = Convert.ToInt32(data[1]);
                    match.ScanY = Convert.ToInt32(data[3]);
                    matches.Add(match);
                }
                catch
                {
                }
            }

            return matches;
        }

        /// <summary>
        ///     Creates a dictionary for scan to peptide match
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Dictionary<int, Dictionary<int, PeptideMatch>> CreateMatches(List<PeptideMatch> matches, int type)
        {
            var matchesX = new Dictionary<int, Dictionary<int, PeptideMatch>>();
            foreach (var match in matches)
            {
                var scanx = match.ScanX;
                var scany = match.ScanY;
                if (type == 1)
                {
                    scanx = match.ScanY;
                    scany = match.ScanX;
                }


                if (!matchesX.ContainsKey(scanx))
                {
                    matchesX.Add(scanx, new Dictionary<int, PeptideMatch>());
                }

                if (!matchesX[scanx].ContainsKey(scany))
                {
                    matchesX[scanx].Add(scany, match);
                }
            }

            return matchesX;
        }

        /// <summary>
        ///     Reads a peptide file.
        /// </summary>
        /// <param name="peptidePathX"></param>
        /// <returns></returns>
        private Dictionary<int, PeptideTest> ReadPeptideFile(string peptidePath)
        {
            var peptideMap = new Dictionary<int, PeptideTest>();
            var lines = File.ReadAllLines(peptidePath);

            var header = lines[0];
            var headerData = header.Split('\t');

            var scanIndex = 0;
            var peptideIndex = 0;
            var fdrIndex = 0;
            var scoreIndex = 0;

            var i = 0;
            foreach (var x in headerData)
            {
                switch (x.ToLower())
                {
                    case "scan":
                        scanIndex = i;
                        break;
                    case "peptide":
                        peptideIndex = i;
                        break;
                    case "fdr":
                        fdrIndex = i;
                        break;
                    case "msgfdb_specprob":
                        scoreIndex = i;
                        break;
                    default:
                        break;
                }
                i++;
            }

            // Map all of the lines.
            for (i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var data = line.Split('\t');
                var pep = data[peptideIndex];
                var scan = Convert.ToInt32(data[scanIndex]);
                var score = Convert.ToDouble(data[scoreIndex]);
                var fdr = Convert.ToDouble(data[fdrIndex]);

                var p = new PeptideTest();
                p.Sequence = pep;
                p.Score = score;
                p.FDR = fdr;

                if (!peptideMap.ContainsKey(scan))
                {
                    peptideMap.Add(scan, p);
                }
            }
            return peptideMap;
        }

        #endregion

        #region Peptide String Manipulation 

        /// <summary>
        ///     Cleans the peptide string
        /// </summary>
        /// <param name="peptide"></param>
        /// <returns></returns>
        private string CleanString(string peptide)
        {
            var peptides = peptide.Split('.');

            if (peptides.Length > 2)
            {
                return peptides[1];
            }
            return peptides[0];
        }

        #endregion

        //#region Error Distribution Construction
        //[Test(Description = "Compares two spectra against each other.")]
        //[TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 3726,
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 3541,
        //            SpectralComparison.NormalizedDotProduct,
        //            .5,
        //            Ignore = true)]
        //[TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 15304,
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 17614,
        //            SpectralComparison.NormalizedDotProduct,
        //            .5,
        //            Ignore = true)]
        //[TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
        //            SpectralComparison.CosineDotProduct,
        //            .8)]
        //public void TestConstructErrorDistributions(string pathX, string pathY, SpectralComparison comparerType, double mzTolerance)
        //{
        //    // Convert relative paths to absolute paths
        //    pathX = GetPath(pathX);
        //    pathY = GetPath(pathY);
        //
        //    double percent = .8;
        //    Console.WriteLine("{2}, Test: {0}\tcompared to\t{1}", pathX, pathY, comparerType);

        //    // Here we setup maps for similar peptide sequences
        //    Dictionary<int, string> peptideMapX = new Dictionary<int, string>();
        //    Dictionary<int, string> peptideMapY = new Dictionary<int, string>();

        //    // This guy filters the spectra, so that we only keep the N most intense ions for comparison
        //    ISpectraFilter filter   = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

        //    /// Open the providers
        //    using (ISpectraProvider readerX = new ThermoRawDataFileReader())
        //    {
        //        using(ISpectraProvider readerY = new ThermoRawDataFileReader())
        //        {
        //            readerX.AddDataFile(pathX, 0);
        //            readerY.AddDataFile(pathY, 0);

        //            Dictionary<int, ScanSummary> scanDataX = readerX.GetScanData(0);
        //            Dictionary<int, ScanSummary> scanDataY = readerY.GetScanData(0);

        //            int Nx = scanDataX.Count;
        //            int Ny = scanDataX.Count;

        //            ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType, percent: percent);

        //            List<double> scans = new List<double>();
        //            Dictionary<int, MSSpectra> spectraMap = new Dictionary<int, MSSpectra>();

        //            // Iterature through 
        //            foreach (int scanx in scanDataX.Keys)
        //            {
        //                ScanSummary xsum = scanDataX[scanx];

        //                if (scanDataX[scanx].MsLevel != 2)
        //                    continue;

        //                MSSpectra spectrumX = GetSpectrum(readerX, scanDataX[scanx].Scan, 0, mzTolerance: mzTolerance);
        //                spectrumX           = filter.Threshold(spectrumX, percent);                        
        //                spectrumX.Peaks     = XYData.Bin(spectrumX.Peaks,
        //                                                    0,
        //                                                    2000,
        //                                                    mzTolerance);

        //                foreach (int scany in scanDataY.Keys)
        //                {
        //                    // And make sure we are comparing the right levels
        //                    if (scanDataY[scany].MsLevel != 2)
        //                        continue;

        //                    // Make sure we compare the same precursors
        //                    double tol = Math.Abs(xsum.PrecursorMZ - scanDataY[scany].PrecursorMZ);
        //                    if (tol > mzTolerance)
        //                    {
        //                        continue;
        //                    }

        //                    MSSpectra spectrumY = null;
        //                    if (spectraMap.ContainsKey(scany))
        //                    {
        //                        spectrumY = spectraMap[scany];
        //                    }
        //                    else
        //                    {
        //                        spectrumY = GetSpectrum(readerY, scanDataY[scany].Scan, 0, mzTolerance: mzTolerance);
        //                        spectrumY = filter.Threshold(spectrumY, percent);
        //                        spectrumY.Peaks     = XYData.Bin(spectrumY.Peaks,
        //                                                            0,
        //                                                            2000,
        //                                                            mzTolerance);
        //                        spectraMap.Add(scany, spectrumY);
        //                    }

        //                    double value = comparer.CompareSpectra(spectrumX, spectrumY);

        //                    scans.Add(value);
        //                    Console.WriteLine("{0}\t{1}\t{2}", scanDataX[scanx].Scan, scanDataY[scany].Scan, value);
        //                }
        //            }
        //            Console.WriteLine();
        //        }
        //    }
        //}
        ///// <summary>
        ///// Thesis Test 1
        ///// </summary>
        ///// <param name="pathX"></param>
        ///// <param name="pathY"></param>
        ///// <param name="peptideMatches"></param>
        ///// <param name="comparerType"></param>
        ///// <param name="mzTolerance"></param>
        //[Test(Description = "Compares two spectra against each other.")]
        //[TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
        //            @"M:\data\proteomics\Thesis\testpaa-tr-cp-00-00\tr-cp-00-00-1.0e+00_matches.csv",
        //            SpectralComparison.NormalizedDotProduct,
        //            .5,
        //            Ignore=true)]
        //[TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
        //            @"M:\data\proteomics\Thesis\testpaa-tr-cp-00-00\tr-cp-00-00-1.0e+00_matches.csv",
        //            SpectralComparison.CosineDotProduct,
        //            .8)]
        //public void TestErrorDistributionsMatches(string pathX,            
        //                                            string pathY, 
        //                                            string peptideMatches,
        //                                            SpectralComparison comparerType,
        //                                            double mzTolerance)
        //{
        //
        //    // Convert relative paths to absolute paths
        //    pathX = GetPath(pathX);
        //    pathY = GetPath(pathY);
        //
        //    double percent = .8;
        //    Console.WriteLine("{2}, Test: {0}\tcompared to\t{1}", pathX, pathY, comparerType);

        //    Dictionary<int, string> peptideMapX = new Dictionary<int, string>();
        //    Dictionary<int, string> peptideMapY = new Dictionary<int, string>();

        //    List<PeptideMatch> matches = GetPeptideMatches(peptideMatches);

        //    // This guy filters the spectra, so that we only keep the N most intense ions for comparison
        //    ISpectraFilter filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

        //    Dictionary<int, Dictionary<int, PeptideMatch>> matchesX = CreateMatches(matches, 0);
        //    Dictionary<int, Dictionary<int, PeptideMatch>> matchesY = CreateMatches(matches, 1);

        //    // These track our matched and not matched 
        //    double step = .05;
        //    int N       = Convert.ToInt32(1.0/step);

        //    SpectralAnalysis analysis   = new SpectralAnalysis(step);
        //    analysis.NetTolerance       = 1.0;
        //    analysis.MassTolerance      = mzTolerance;

        //    using (ISpectraProvider readerX = new ThermoRawDataFileReader())
        //    {
        //        using (ISpectraProvider readerY = new ThermoRawDataFileReader())
        //        {
        //            readerX.AddDataFile(pathX, 0);
        //            readerY.AddDataFile(pathY, 0);

        //            Dictionary<int, ScanSummary> scanDataX = readerX.GetScanData(0);
        //            Dictionary<int, ScanSummary> scanDataY = readerY.GetScanData(0);

        //            // Determine the scan maxes
        //            var maxX = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
        //            var minX = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;

        //            var maxY = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
        //            var minY = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;

        //            int Nx = scanDataX.Count;
        //            int Ny = scanDataX.Count;

        //            ISpectralComparer comparer              = SpectralComparerFactory.CreateSpectraComparer(comparerType, percent: percent);
        //            List<double> scans                      = new List<double>();
        //            Dictionary<int, MSSpectra> spectraMap   = new Dictionary<int, MSSpectra>();

        //            /// Horrible way to make a histogram...but this will make it for our matches.
        //            Dictionary<int, int> netErrorHistogram = new Dictionary<int, int>();
        //            double start    = -.05;
        //            double max      = .05;
        //            double width    = max - start;
        //            double stepNet  = .005; 
        //            double netValue = start;
        //            int i = 0;
        //            while (netValue <= max)
        //            {
        //                netErrorHistogram.Add(i++, 0);
        //                netValue += stepNet;
        //            }

        //            foreach (int scanx in scanDataX.Keys)
        //            {
        //                ScanSummary xsum = scanDataX[scanx];

        //                if (xsum.MsLevel != 2)
        //                    continue;

        //                // Grab the first spectar
        //                MSSpectra spectrumX = GetSpectrum(readerX, scanDataX[scanx].Scan, 0, mzTolerance: mzTolerance);    
        //                spectrumX           = filter.Threshold(spectrumX, percent);                        
        //                spectrumX.Peaks     = XYData.Bin(spectrumX.Peaks,
        //                                                    0,
        //                                                    2000,
        //                                                    mzTolerance);

        //                foreach (int scany in scanDataY.Keys)
        //                {
        //                    ScanSummary ysum = scanDataY[scany];
        //                    if (ysum.MsLevel != 2)
        //                        continue;

        //                    if (Math.Abs(xsum.PrecursorMZ - ysum.PrecursorMZ) >= mzTolerance)
        //                    {
        //                        continue;
        //                    }

        //                    // Grab the first spectra...if we have it, great dont re-read
        //                    MSSpectra spectrumY = null;
        //                    if (spectraMap.ContainsKey(scany))
        //                    {
        //                        spectrumY = spectraMap[scany];
        //                    }
        //                    else
        //                    {
        //                        spectrumY = GetSpectrum(readerY, scanDataY[scany].Scan, 0, mzTolerance: mzTolerance);
        //                        spectrumY = filter.Threshold(spectrumY, percent);
        //                        spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
        //                                                            0,
        //                                                            2000,
        //                                                            mzTolerance);
        //                        spectraMap.Add(scany, spectrumY);
        //                    }

        //                    // compare the spectra
        //                    double value = comparer.CompareSpectra(spectrumX, spectrumY);
        //                    if (double.IsNaN(value))
        //                    {
        //                        continue;
        //                    }

        //                    bool isInList  = false;
        //                    string peptidex = "";
        //                    if (matchesX.ContainsKey(scanx))
        //                    {
        //                        if (matchesX[scanx].ContainsKey(scany))
        //                        {
        //                            isInList  = true;
        //                            peptidex   = matchesX[scanx][scany].Peptide;
        //                        }
        //                    }

        //                    string peptidey = "";
        //                    if (matchesY.ContainsKey(scanx))
        //                    {
        //                        if (matchesY[scanx].ContainsKey(scany))
        //                        {
        //                            isInList  = true;
        //                            peptidey   = matchesY[scanx][scany].Peptide;
        //                        }
        //                    }
        //                    // Here we determine what distribution the match belongs to.  We iterate over a number of NET tolerances
        //                    int index                       = Convert.ToInt32(value / step);


        //                    double netX = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
        //                    double netY = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);                            
        //                    double net                  = Convert.ToDouble(netX - netY);                            
        //                    if (analysis.NetTolerance >= net)
        //                    {
        //                        if (isInList)
        //                        {
        //                            if (value > .9)
        //                            {
        //                                DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, newTitle: string.Format("{0} - {1} - {2}", peptidex, peptidey, value));
        //                            }
        //                            analysis.AddTrueMatch(index);

        //                        }
        //                        else
        //                        {
        //                            if (value > .9)
        //                            {
        //                              //  DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, newTitle: string.Format("{0} - {1} - {2}", peptidex, peptidey, value));
        //                            }
        //                            analysis.AddFalseMatch(index);
        //                        }
        //                    }                              
        //                }                           
        //            }                    
        //        }
        //    }
        //    analysis.Write();
        //    Console.WriteLine();
        //}
        ///// <summary>
        ///// Tests distributions using the peptide match file (uniqued matches for building error distributions)
        ///// </summary>
        ///// <param name="pathX"></param>
        ///// <param name="pathY"></param>
        ///// <param name="peptideMatches"></param>
        ///// <param name="comparerType"></param>
        ///// <param name="mzTolerance"></param>
        //[Test(Description = "Compares two spectra against each other.")]        
        //[TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
        //            SpectralComparison.DotProduct,
        //            .5,
        //            .25,
        //            1,
        //            .7,
        //            .01,
        //            .5, Ignore = false)]
        //[TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
        //            SpectralComparison.PeakCounts,
        //            .5,
        //            .25,
        //            1,
        //            .7,
        //            .01,
        //            .5
        //            , Ignore = true)]
        //[TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
        //            SpectralComparison.CosineDotProduct,
        //            .5,
        //            .25,
        //            1,
        //            .7,
        //            .01,
        //            .5, Ignore=true)]
        //public void TestRawPeptideMatchesDistributions  (string pathX,
        //                                                string pathY,
        //                                                string peptidePathX,
        //                                                string peptidePathY,
        //                                                SpectralComparison comparerType,
        //                                                double mzTolerance,
        //                                                double netTolerance,
        //                                                double score,
        //                                                double matchScore,
        //                                                double fdr,
        //                                                double percent)
        //{
        //    // Convert relative paths to absolute paths
        //    pathX = GetPath(pathX);
        //    pathY = GetPath(pathY);
        //
        //    Console.WriteLine("{2}, Test: {0}\tcompared to\t{1}", pathX, pathY, comparerType);

        //    Dictionary<int, PeptideTest> peptideMapX = new Dictionary<int, PeptideTest>();
        //    Dictionary<int, PeptideTest> peptideMapY = new Dictionary<int, PeptideTest>();

        //    peptideMapX = ReadPeptideFile(peptidePathX);
        //    peptideMapY = ReadPeptideFile(peptidePathY);

        //    // This guy filters the spectra, so that we only keep the N most intense ions for comparison
        //    ISpectraFilter filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

        //    // These track our matched and not matched 
        //    double step                 = .05;
        //    int N                       = Convert.ToInt32(1.0 / step);
        //    SpectralAnalysis analysis   = new SpectralAnalysis(step);
        //    analysis.NetTolerance       = netTolerance;
        //    analysis.MassTolerance      = mzTolerance;

        //    using (ISpectraProvider readerX     = new ThermoRawDataFileReader())
        //    {
        //        using (ISpectraProvider readerY = new ThermoRawDataFileReader())
        //        {
        //            readerX.AddDataFile(pathX, 0);
        //            readerY.AddDataFile(pathY, 0);

        //            Dictionary<int, ScanSummary> scanDataX = readerX.GetScanData(0);
        //            Dictionary<int, ScanSummary> scanDataY = readerY.GetScanData(0); 

        //            // Determine the scan maxes
        //            var maxX    = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
        //            var minX    = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
        //            var maxY    = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
        //            var minY    = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
        //            int Nx      = scanDataX.Count;
        //            int Ny      = scanDataX.Count;

        //            ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType, percent: percent);
        //            List<double> scans = new List<double>();
        //            Dictionary<int, MSSpectra> spectraMap = new Dictionary<int, MSSpectra>();

        //            /// Horrible way to make a histogram...but this will make it for our matches.
        //            Dictionary<int, int> netErrorHistogram = new Dictionary<int, int>();
        //            double start    = -.05;
        //            double max      = .05;
        //            double width    = max - start;
        //            double stepNet  = .005;
        //            double netValue = start;
        //            int i           = 0;

        //            while (netValue <= max)
        //            {
        //                netErrorHistogram.Add(i++, 0);
        //                netValue += stepNet;
        //            }

        //            foreach (int scanx in scanDataX.Keys)
        //            {
        //                ScanSummary xsum = scanDataX[scanx];

        //                if (xsum.MsLevel != 2)
        //                    continue;

        //                // Grab the first spectar
        //                MSSpectra spectrumX = GetSpectrum(  readerX, 
        //                                                    scanDataX[scanx].Scan, 
        //                                                    0,
        //                                                    mzTolerance: mzTolerance);
        //                spectrumX           = filter.Threshold(spectrumX, percent);
        //                spectrumX.Peaks     = XYData.Bin(spectrumX.Peaks,
        //                                                    0,
        //                                                    2000,
        //                                                    mzTolerance);

        //                foreach (int scany in scanDataY.Keys)
        //                {
        //                    ScanSummary ysum = scanDataY[scany];
        //                    if (ysum.MsLevel != 2)
        //                        continue;

        //                    if (Math.Abs(xsum.PrecursorMZ - ysum.PrecursorMZ) >= mzTolerance)
        //                    {
        //                        continue;
        //                    }

        //                    // Grab the first spectra...if we have it, great dont re-read
        //                    MSSpectra spectrumY = null;
        //                    if (spectraMap.ContainsKey(scany))
        //                    {
        //                        spectrumY = spectraMap[scany];
        //                    }
        //                    else
        //                    {
        //                        spectrumY = GetSpectrum(    readerY, 
        //                                                    scanDataY[scany].Scan, 
        //                                                    0, 
        //                                                    mzTolerance: mzTolerance);
        //                        spectrumY = filter.Threshold(spectrumY, percent);
        //                        spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
        //                                                            0,
        //                                                            2000,
        //                                                            mzTolerance);
        //                        spectraMap.Add(scany, spectrumY);
        //                    }

        //                    // compare the spectra
        //                    double value = comparer.CompareSpectra(spectrumX, spectrumY);
        //                    if (double.IsNaN(value))
        //                    {
        //                        continue;
        //                    }

        //                    bool isMatch            = false;
        //                    PeptideTest peptidex    = null;
        //                    PeptideTest peptidey    = null;


        //                    if (peptideMapX.ContainsKey(scanx))
        //                        peptidex    = peptideMapX[scanx];                               

        //                    if (peptideMapY.ContainsKey(scany))                            
        //                        peptidey    = peptideMapY[scany];


        //                    if (peptidex != null && peptidey != null)
        //                    {
        //                        peptidex.Sequence = CleanString(peptidex.Sequence);
        //                        peptidey.Sequence = CleanString(peptidey.Sequence);

        //                        if (peptidex.Sequence.Equals(peptidey.Sequence) && !string.IsNullOrWhiteSpace(peptidey.Sequence))
        //                        {
        //                            isMatch = true;
        //                        }
        //                    }


        //                    bool passesCutoff = PassesCutoff(peptidex, peptidey, score, fdr);
        //                    if (!passesCutoff)                            
        //                        continue;

        //                    if (value < matchScore)
        //                        continue;

        //                    if (double.IsNaN(value) || double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value))
        //                        continue;

        //                    // Here we determine what distribution the match belongs to.  We iterate over a number of NET tolerances
        //                    int index = 0;

        //                    try
        //                    {
        //                        index = Convert.ToInt32(value / step);
        //                    }
        //                    catch(OverflowException ex)
        //                    {
        //                        int xx = 9;
        //                        if (xx > 10)
        //                        {
        //                        }
        //                    }

        //                    double netX = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
        //                    double netY = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);
        //                    double net  = Convert.ToDouble(netX - netY);

        //                    PeptideMatch match  = new PeptideMatch();
        //                    match.NetX          = netX;
        //                    match.NetY          = netY;
        //                    match.MzX           = xsum.PrecursorMZ;
        //                    match.MzY           = ysum.PrecursorMZ;
        //                    match.ScanX         = scanx;
        //                    match.ScanY         = scany;
        //                    match.Similarity    = value;
        //                    match.Index         = index;
        //                    match.IsMatch       = isMatch;


        //                    if (analysis.NetTolerance >= net)
        //                    {

        //                        analysis.AddMatch(match);

        //                        if (isMatch)
        //                        {
        //                            if (value > .9)
        //                            {
        //                              //  DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, newTitle: string.Format("{0} - {1} - {2}", peptidex, peptidey, value));
        //                            }
        //                            analysis.AddTrueMatch(index);
        //                        }
        //                        else
        //                        {
        //                            if (value > .9)
        //                            {
        //                                 // DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, newTitle: string.Format("{0} - {1} - {2}", peptidex, peptidey, value));
        //                            }
        //                            analysis.AddFalseMatch(index);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    analysis.Write();
        //    Console.WriteLine();
        //}

        //private bool PassesCutoff(PeptideTest peptidex, PeptideTest peptidey, double score, double fdr)
        //{         
        //    bool passes = true;

        //    if (peptidex == null || peptidey == null)
        //        return false;

        //    if (peptidey.FDR > fdr) return false;
        //    if (peptidey.FDR > fdr) return false;
        //    if (peptidex.Score > score) return false;
        //    if (peptidey.Score > score) return false;            
        //    return passes;
        //}
        //#endregion        
    }

    public class PeptideMatch
    {
        public bool IsMatch { get; set; }
        public double Similarity { get; set; }
        public string Peptide { get; set; }
        public int ScanX { get; set; }
        public int ScanY { get; set; }
        public double MzX { get; set; }
        public double NetX { get; set; }
        public double NetAlignedX { get; set; }
        public double MzY { get; set; }
        public double NetY { get; set; }
        public double NetAlignedY { get; set; }
        public MSSpectra SpectrumX { get; set; }
        public MSSpectra SpectrumY { get; set; }

        /// <summary>
        ///     Gets the NET difference between two features.
        /// </summary>
        public double NetDifference
        {
            get { return Math.Abs(NetX - NetY); }
        }

        public int Index { get; set; }
    }

    public class PeptideTest
    {
        public string Sequence { get; set; }
        public double FDR { get; set; }
        public double Score { get; set; }
    }
}