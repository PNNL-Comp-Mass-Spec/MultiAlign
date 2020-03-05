#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FeatureAlignment.Data;
using MultiAlignCore.Algorithms.SpectralProcessing;
using MultiAlignCore.Data;
using MultiAlignCore.IO.RawData;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using LinearAxis = OxyPlot.Axes.LinearAxis;

#endregion

namespace MultiAlignTestSuite.Algorithms.SpectralProcessing
{
    [TestFixture]
    public class SpectralComparisonTests: TestBase
    {

        #region Spectra Retrieval

        private MSSpectra GetSpectrum(string path, int scan)
        {
            ISpectraProvider reader = new InformedProteomicsReader(0, path);
            return GetSpectrum(reader, scan, 0);
        }

        private MSSpectra GetSpectrum(ISpectraProvider reader, int scan, int group, double mzTolerance = .5)
        {
            var peaks = reader.GetRawSpectra(scan, 2, out _);
            var spectrum = new MSSpectra {Peaks = peaks};

            return spectrum;
        }

        #endregion

        #region Display

        /// <summary>
        /// Creates a plot for the given peaks list
        /// </summary>
        /// <param name="peaksX"></param>
        /// <param name="peaksY"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private PlotModel CreatePlot(IReadOnlyList<XYData> peaksX,
            IReadOnlyList<XYData> peaksY,
            double tolerance)
        {
            var plotModel1 = new PlotModel
            {
                LegendBorderThickness = 0,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
                Title = "MS/MS Spectra"
            };

            var categoryAxis1 = new LinearAxis {MinorStep = tolerance};
            plotModel1.Axes.Add(categoryAxis1);

            var linearAxis1 = new LinearAxis {MaximumPadding = 0.06, MinimumPadding = 0.06};
            plotModel1.Axes.Add(linearAxis1);

            var xSeries = new StemSeries();
            for (var j = 0; j < peaksY.Count; j++)
            {
                var peakX = peaksX[j];
                var peakY = peaksY[j];

                double value = 0;
                if (peakX.Y > 0 && peakY.Y > 0)
                {
                    value = 1;
                }
                xSeries.Points.Add(new DataPoint(peakX.X, value));
            }
            xSeries.Color = OxyColors.Green;

            //plotModel1.Series.Add(xSeries);

            var series = new StemSeries {Title = "Spectra X"};
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

            var series2 = new StemSeries {Title = "Spectra Y"};
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

            var plot = new PlotView {Model = model};
            var form = new Form {Size = Screen.PrimaryScreen.WorkingArea.Size};
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
            .5)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            7413,
            1)]
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
        //[TestCase(
        //    @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
        //    3726,
        //    @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
        //    3541,
        //    SpectralComparison.NormalizedDotProduct,
        //    .5,
        //    Ignore = "true")]
        //[TestCase(
        //    @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
        //    15304,
        //    @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
        //    17614,
        //    SpectralComparison.NormalizedDotProduct,
        //    .5,
        //    Ignore = "true")]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            SpectralComparison.PeakCounts,
            SpectraFilters.TopPercent,
            .8,
            1)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            7413,
            SpectralComparison.PeakCounts,
            SpectraFilters.TopPercent,
            .8,
            1)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            SpectralComparison.CosineDotProduct,
            SpectraFilters.TopPercent,
            .8,
            1)]
        [TestCase(
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            7413,
            SpectralComparison.CosineDotProduct,
            SpectraFilters.TopPercent,
            .8,
            1)]
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
        /// Gets a list of the peptide matches from the path provided.
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
                    var match = new PeptideMatch {
                        Peptide = data[5],
                        ScanX = Convert.ToInt32(data[1]),
                        ScanY = Convert.ToInt32(data[3])
                    };

                    matches.Add(match);
                }
                catch
                {
                    // Ignore errors here
                }
            }

            return matches;
        }

        /// <summary>
        /// Creates a dictionary for scan to peptide match
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Dictionary<int, Dictionary<int, PeptideMatch>> CreateMatches(IEnumerable<PeptideMatch> matches, int type)
        {
            var matchesX = new Dictionary<int, Dictionary<int, PeptideMatch>>();
            foreach (var match in matches)
            {
                var scanX = match.ScanX;
                var scanY = match.ScanY;
                if (type == 1)
                {
                    scanX = match.ScanY;
                    scanY = match.ScanX;
                }


                if (!matchesX.ContainsKey(scanX))
                {
                    matchesX.Add(scanX, new Dictionary<int, PeptideMatch>());
                }

                if (!matchesX[scanX].ContainsKey(scanY))
                {
                    matchesX[scanX].Add(scanY, match);
                }
            }

            return matchesX;
        }

        /// <summary>
        /// Reads a peptide file.
        /// </summary>
        /// <param name="peptidePath"></param>
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

                var p = new PeptideTest {
                    Sequence = pep,
                    Score = score,
                    FDR = fdr
                };

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
        /// Cleans the peptide string
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
        //            Ignore = "true")]
        //[TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 15304,
        //            @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 17614,
        //            SpectralComparison.NormalizedDotProduct,
        //            .5,
        //            Ignore = "true")]
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

        //            // Iterate through
        //            foreach (int scanX in scanDataX.Keys)
        //            {
        //                ScanSummary sumX = scanDataX[scanX];

        //                if (scanDataX[scanX].MsLevel != 2)
        //                    continue;

        //                MSSpectra spectrumX = GetSpectrum(readerX, scanDataX[scanX].Scan, 0, mzTolerance: mzTolerance);
        //                spectrumX           = filter.Threshold(spectrumX, percent);
        //                spectrumX.Peaks     = XYData.Bin(spectrumX.Peaks,
        //                                                    0,
        //                                                    2000,
        //                                                    mzTolerance);

        //                foreach (int scanY in scanDataY.Keys)
        //                {
        //                    // And make sure we are comparing the right levels
        //                    if (scanDataY[scanY].MsLevel != 2)
        //                        continue;

        //                    // Make sure we compare the same precursors
        //                    double tol = Math.Abs(sumX.PrecursorMZ - scanDataY[scanY].PrecursorMZ);
        //                    if (tol > mzTolerance)
        //                    {
        //                        continue;
        //                    }

        //                    MSSpectra spectrumY = null;
        //                    if (spectraMap.ContainsKey(scanY))
        //                    {
        //                        spectrumY = spectraMap[scanY];
        //                    }
        //                    else
        //                    {
        //                        spectrumY = GetSpectrum(readerY, scanDataY[scanY].Scan, 0, mzTolerance: mzTolerance);
        //                        spectrumY = filter.Threshold(spectrumY, percent);
        //                        spectrumY.Peaks     = XYData.Bin(spectrumY.Peaks,
        //                                                            0,
        //                                                            2000,
        //                                                            mzTolerance);
        //                        spectraMap.Add(scanY, spectrumY);
        //                    }

        //                    double value = comparer.CompareSpectra(spectrumX, spectrumY);

        //                    scans.Add(value);
        //                    Console.WriteLine("{0}\t{1}\t{2}", scanDataX[scanX].Scan, scanDataY[scanY].Scan, value);
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
        //            Ignore = "true")]
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

        //            foreach (int scanX in scanDataX.Keys)
        //            {
        //                ScanSummary sumX = scanDataX[scanX];

        //                if (sumX.MsLevel != 2)
        //                    continue;

        //                // Grab the first spectrum
        //                MSSpectra spectrumX = GetSpectrum(readerX, scanDataX[scanX].Scan, 0, mzTolerance: mzTolerance);
        //                spectrumX           = filter.Threshold(spectrumX, percent);
        //                spectrumX.Peaks     = XYData.Bin(spectrumX.Peaks,
        //                                                    0,
        //                                                    2000,
        //                                                    mzTolerance);

        //                foreach (int scanY in scanDataY.Keys)
        //                {
        //                    ScanSummary ySum = scanDataY[scanY];
        //                    if (ySum.MsLevel != 2)
        //                        continue;

        //                    if (Math.Abs(sumX.PrecursorMZ - ySum.PrecursorMZ) >= mzTolerance)
        //                    {
        //                        continue;
        //                    }

        //                    // Grab the first spectrum ... if we have it, great don't re-read
        //                    MSSpectra spectrumY = null;
        //                    if (spectraMap.ContainsKey(scanY))
        //                    {
        //                        spectrumY = spectraMap[scanY];
        //                    }
        //                    else
        //                    {
        //                        spectrumY = GetSpectrum(readerY, scanDataY[scanY].Scan, 0, mzTolerance: mzTolerance);
        //                        spectrumY = filter.Threshold(spectrumY, percent);
        //                        spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
        //                                                            0,
        //                                                            2000,
        //                                                            mzTolerance);
        //                        spectraMap.Add(scanY, spectrumY);
        //                    }

        //                    // compare the spectra
        //                    double value = comparer.CompareSpectra(spectrumX, spectrumY);
        //                    if (double.IsNaN(value))
        //                    {
        //                        continue;
        //                    }

        //                    bool isInList  = false;
        //                    string peptideX = "";
        //                    if (matchesX.ContainsKey(scanX))
        //                    {
        //                        if (matchesX[scanX].ContainsKey(scanY))
        //                        {
        //                            isInList  = true;
        //                            peptideX   = matchesX[scanX][scanY].Peptide;
        //                        }
        //                    }

        //                    string peptideY = "";
        //                    if (matchesY.ContainsKey(scanX))
        //                    {
        //                        if (matchesY[scanX].ContainsKey(scanY))
        //                        {
        //                            isInList  = true;
        //                            peptideY   = matchesY[scanX][scanY].Peptide;
        //                        }
        //                    }
        //                    // Here we determine what distribution the match belongs to.  We iterate over a number of NET tolerances
        //                    int index                       = Convert.ToInt32(value / step);


        //                    double netX = Convert.ToDouble(scanX - minX) / Convert.ToDouble(maxX - minX);
        //                    double netY = Convert.ToDouble(scanY - minY) / Convert.ToDouble(maxY - minY);
        //                    double net                  = Convert.ToDouble(netX - netY);
        //                    if (analysis.NetTolerance >= net)
        //                    {
        //                        if (isInList)
        //                        {
        //                            if (value > .9)
        //                            {
        //                                DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, newTitle: string.Format("{0} - {1} - {2}", peptideX, peptideY, value));
        //                            }
        //                            analysis.AddTrueMatch(index);

        //                        }
        //                        else
        //                        {
        //                            if (value > .9)
        //                            {
        //                              //  DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, newTitle: string.Format("{0} - {1} - {2}", peptideX, peptideY, value));
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
        ///// Tests distributions using the peptide match file (unique matches for building error distributions)
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
        //            .5)]
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
        //            , Ignore = "true")]
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
        //            .5, Ignore = "true")]
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

        //            foreach (int scanX in scanDataX.Keys)
        //            {
        //                ScanSummary sumX = scanDataX[scanX];

        //                if (sumX.MsLevel != 2)
        //                    continue;

        //                // Grab the first spectrum
        //                MSSpectra spectrumX = GetSpectrum(  readerX,
        //                                                    scanDataX[scanX].Scan,
        //                                                    0,
        //                                                    mzTolerance: mzTolerance);
        //                spectrumX           = filter.Threshold(spectrumX, percent);
        //                spectrumX.Peaks     = XYData.Bin(spectrumX.Peaks,
        //                                                    0,
        //                                                    2000,
        //                                                    mzTolerance);

        //                foreach (int scanY in scanDataY.Keys)
        //                {
        //                    ScanSummary ySum = scanDataY[scanY];
        //                    if (ySum.MsLevel != 2)
        //                        continue;

        //                    if (Math.Abs(sumX.PrecursorMZ - ySum.PrecursorMZ) >= mzTolerance)
        //                    {
        //                        continue;
        //                    }

        //                    // Grab the first spectra...if we have it, great dont re-read
        //                    MSSpectra spectrumY = null;
        //                    if (spectraMap.ContainsKey(scanY))
        //                    {
        //                        spectrumY = spectraMap[scanY];
        //                    }
        //                    else
        //                    {
        //                        spectrumY = GetSpectrum(    readerY,
        //                                                    scanDataY[scanY].Scan,
        //                                                    0,
        //                                                    mzTolerance: mzTolerance);
        //                        spectrumY = filter.Threshold(spectrumY, percent);
        //                        spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
        //                                                            0,
        //                                                            2000,
        //                                                            mzTolerance);
        //                        spectraMap.Add(scanY, spectrumY);
        //                    }

        //                    // compare the spectra
        //                    double value = comparer.CompareSpectra(spectrumX, spectrumY);
        //                    if (double.IsNaN(value))
        //                    {
        //                        continue;
        //                    }

        //                    bool isMatch            = false;
        //                    PeptideTest peptideX    = null;
        //                    PeptideTest peptideY    = null;


        //                    if (peptideMapX.ContainsKey(scanX))
        //                        peptideX    = peptideMapX[scanX];

        //                    if (peptideMapY.ContainsKey(scanY))
        //                        peptideY    = peptideMapY[scanY];


        //                    if (peptideX != null && peptideY != null)
        //                    {
        //                        peptideX.Sequence = CleanString(peptideX.Sequence);
        //                        peptideY.Sequence = CleanString(peptideY.Sequence);

        //                        if (peptideX.Sequence.Equals(peptideY.Sequence) && !string.IsNullOrWhiteSpace(peptideY.Sequence))
        //                        {
        //                            isMatch = true;
        //                        }
        //                    }


        //                    bool passesCutoff = PassesCutoff(peptideX, peptideY, score, fdr);
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

        //                    double netX = Convert.ToDouble(scanX - minX) / Convert.ToDouble(maxX - minX);
        //                    double netY = Convert.ToDouble(scanY - minY) / Convert.ToDouble(maxY - minY);
        //                    double net  = Convert.ToDouble(netX - netY);

        //                    PeptideMatch match  = new PeptideMatch();
        //                    match.NetX          = netX;
        //                    match.NetY          = netY;
        //                    match.MzX           = sumX.PrecursorMZ;
        //                    match.MzY           = ySum.PrecursorMZ;
        //                    match.ScanX         = scanX;
        //                    match.ScanY         = scanY;
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
        //                              //  DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, newTitle: string.Format("{0} - {1} - {2}", peptideX, peptideY, value));
        //                            }
        //                            analysis.AddTrueMatch(index);
        //                        }
        //                        else
        //                        {
        //                            if (value > .9)
        //                            {
        //                                 // DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, newTitle: string.Format("{0} - {1} - {2}", peptideX, peptideY, value));
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

        //private bool PassesCutoff(PeptideTest peptideX, PeptideTest peptideY, double score, double fdr)
        //{
        //    bool passes = true;

        //    if (peptideX == null || peptideY == null)
        //        return false;

        //    if (peptideY.FDR > fdr) return false;
        //    if (peptideY.FDR > fdr) return false;
        //    if (peptideX.Score > score) return false;
        //    if (peptideY.Score > score) return false;
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
        /// Gets the NET difference between two features.
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