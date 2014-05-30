#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MultiAlignCore.IO.Features;
using MultiAlignTestSuite.Algorithms.SpectralProcessing;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;

#endregion

namespace MultiAlignTestSuite.Algorithms
{
    [TestFixture]
    public class SpectralComparisonTest
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
            form.ShowDialog();

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
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            .5,
            Ignore = false)]
        [TestCase(
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            7413,
            1,
            Ignore = false)]
        public void DisplayComparisonFigure(string pathX, int scanX, string pathY, int scanY, double mzTolerance)
        {
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
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            SpectralComparison.NormalizedDotProduct,
            .5,
            Ignore = true)]
        [TestCase(
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            15304,
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            17614,
            SpectralComparison.NormalizedDotProduct,
            .5,
            Ignore = true)]
        [TestCase(
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            SpectralComparison.PeakCounts,
            SpectraFilters.TopPercent,
            .8,
            1,
            Ignore = false)]
        [TestCase(
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            7413,
            SpectralComparison.PeakCounts,
            SpectraFilters.TopPercent,
            .8,
            1,
            Ignore = false)]
        [TestCase(
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            3541,
            SpectralComparison.CosineDotProduct,
            SpectraFilters.TopPercent,
            .8,
            1,
            Ignore = false)]
        [TestCase(
            @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
            3726,
            @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
            7413,
            SpectralComparison.CosineDotProduct,
            SpectraFilters.TopPercent,
            .8,
            1,
            Ignore = false)]
        public void TestSpectralSimilarityScore(string pathX,
            int scanX,
            string pathY,
            int scanY,
            SpectralComparison comparerType,
            SpectraFilters filterType,
            double percent,
            double mzTolerance)
        {
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

    }

    public class PeptideTest
    {
        public string Sequence { get; set; }
        public double FDR { get; set; }
        public double Score { get; set; }
    }
}