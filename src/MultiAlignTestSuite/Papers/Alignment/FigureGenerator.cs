using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiAlignCore.IO.Features;
using MultiAlignCore.MathUtilities;
using MultiAlignTestSuite.Algorithms.SpectralProcessing;
using MultiAlignTestSuite.Papers.Alignment.Data;
using MultiAlignTestSuite.Papers.Alignment.IO;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using PNNLOmicsIO.IO;

namespace MultiAlignTestSuite.Papers.Alignment.SSM
{
    /// <summary>
    ///     Creates all figures used in the alignment paper
    /// </summary>
    [TestFixture]
    public class FigureGenerator
    {
        [SetUp]
        public void TestSetup()
        {
            m_basePath = @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00";
            AlignmentAnalysisWriterFactory.BasePath = @"M:\doc\papers\paperAlignment\Data\figure1";
        }

        internal class PathCache
        {
            public string Cache { get; set; }
            public string Msgf { get; set; }
            public string Features { get; set; }
        }

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
            for (int j = 0; j < peaksY.Count; j++)
            {
                XYData peakX = peaksX[j];
                XYData peakY = peaksY[j];

                double value = 0;
                if (peakX.Y > 0 && peakY.Y > 0)
                {
                    value = 1;
                }
                xseries.Points.Add(new DataPoint(peakX.X, value));
            }
            xseries.Color = OxyColors.Green;
            xseries.Color.ChangeAlpha(100);
            //plotModel1.Series.Add(xseries);

            var series = new StemSeries();
            series.Title = "Spectra X";
            double max = 0;
            foreach (XYData datum in peaksX)
            {
                max = Math.Max(max, datum.Y);
            }
            foreach (XYData datum in peaksX)
            {
                series.Points.Add(new DataPoint(datum.X, datum.Y/max));
            }
            plotModel1.Series.Add(series);

            foreach (XYData datum in peaksY)
            {
                max = Math.Max(max, datum.Y);
            }
            var series2 = new StemSeries();
            series2.Title = "Spectra Y";
            foreach (XYData datum in peaksY)
            {
                series2.Points.Add(new DataPoint(datum.X, (datum.Y*-1)/max));
            }
            plotModel1.Series.Add(series2);


            return plotModel1;
        }

        private void DisplayComparisonPlot(MSSpectra spectrumX, MSSpectra spectrumY, double mzTolerance,
            string path = "comparison.png", string newTitle = "MS/MS Spectra")
        {
            PlotModel model = CreatePlot(spectrumX.Peaks, spectrumY.Peaks, mzTolerance);
            model.Title = newTitle;

            var plot = new Plot();
            plot.Model = model;
            var form = new Form();
            form.Size = Screen.PrimaryScreen.WorkingArea.Size;
            plot.Dock = DockStyle.Fill;
            form.Controls.Add(plot);
            form.ShowDialog();

            if (false)
            {
                using (var bitmap = new Bitmap(form.Width, form.Height))
                {
                    form.DrawToBitmap(bitmap, form.DisplayRectangle);
                    bitmap.Save(path);
                }
            }
        }

        protected string m_basePath;

        private SpectralAnalysis MatchDatasets(ISpectraProvider readerX,
            ISpectraProvider readerY,
            AlignmentDataset datasetX,
            AlignmentDataset datasetY,
            List<string> names,
            SpectralOptions options)
        {
            // This helps us compare various comparison calculation methods
            ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(options.ComparerType);

            // This guy filters the spectra, so that we only keep the N most intense ions for comparison
            ISpectraFilter filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);


            // Read data for peptides 
            ISequenceFileReader reader = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            IEnumerable<Peptide> peptidesA = reader.Read(datasetX.PeptideFile);
            IEnumerable<Peptide> peptidesB = reader.Read(datasetY.PeptideFile);

            peptidesA =
                peptidesA.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();
            peptidesB =
                peptidesB.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();


            Dictionary<int, Peptide> peptideMapX = PeptideUtility.MapWithBestScan(peptidesA);
            Dictionary<int, Peptide> peptideMapY = PeptideUtility.MapWithBestScan(peptidesB);

            // Here we find all the matches
            var finder = new SpectralAnchorPointFinder();
            IEnumerable<AnchorPointMatch> matches = null;

            matches = finder.FindAnchorPoints(readerX,
                readerY,
                comparer,
                filter,
                options);


            /// Then map the peptide sequences to identify True Positive and False Positives
            var matcher = new PeptideAnchorPointMatcher();
            matcher.Match(matches,
                peptideMapX,
                peptideMapY,
                options);


            Dictionary<int, ScanSummary> scanDataX = readerX.GetScanData(0);
            Dictionary<int, ScanSummary> scanDataY = readerY.GetScanData(0);


            foreach (AnchorPointMatch match in matches)
            {
                if (false /*&& match.IsValidMatch == AnchorMatch.PeptideFailed*/&& match.SimilarityScore > .97)
                {
                    if (match.AnchorPointX.Spectrum.Peaks == null)
                    {
                        match.AnchorPointX.Spectrum = SpectralUtilities.GetSpectra(options.MzBinSize,
                            options.TopIonPercent,
                            filter,
                            readerX,
                            match.AnchorPointX.Scan,
                            options.RequiredPeakCount);
                    }
                    if (match.AnchorPointY.Spectrum.Peaks == null)
                    {
                        match.AnchorPointY.Spectrum = SpectralUtilities.GetSpectra(options.MzBinSize,
                            options.TopIonPercent,
                            filter,
                            readerY,
                            match.AnchorPointY.Scan,
                            options.RequiredPeakCount);
                    }
                    DisplayComparisonPlot(match.AnchorPointX.Spectrum, match.AnchorPointY.Spectrum, options.MzBinSize,
                        newTitle:
                            string.Format("{2} - {0} - {1}", match.AnchorPointX.Peptide,
                                match.AnchorPointY.Peptide,
                                match.IsValidMatch));
                }
            }


            // Package the data
            var analysis = new SpectralAnalysis();
            analysis.Options = options;
            analysis.Matches = matches;
            analysis.DatasetNames = names;

            return analysis;
        }

        private void MatchPeptides(AlignmentDataset datasetX,
            AlignmentDataset datasetY,
            Dictionary<int, ScanSummary> scanDataX,
            Dictionary<int, ScanSummary> scanDataY,
            List<string> names,
            SpectralOptions options)
        {
            // Read data for peptides 
            ISequenceFileReader reader = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            IEnumerable<Peptide> peptidesA = reader.Read(datasetX.PeptideFile);
            IEnumerable<Peptide> peptidesB = reader.Read(datasetY.PeptideFile);

            peptidesA =
                peptidesA.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();
            peptidesB =
                peptidesB.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();

            Dictionary<int, Peptide> peptideMapX = PeptideUtility.MapWithBestScan(peptidesA);
            Dictionary<int, Peptide> peptideMapY = PeptideUtility.MapWithBestScan(peptidesB);

            // Determine the scan extrema
            int maxX = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            int minX = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
            int maxY = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            int minY = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;

            /// Then map the peptide sequences to identify True Positive and False Positives            
            int count = 0;
            foreach (int scanx in peptideMapX.Keys)
            {
                Peptide peptideX = peptideMapX[scanx];

                foreach (int scany in peptideMapY.Keys)
                {
                    Peptide peptideY = peptideMapY[scany];
                    double netX = Convert.ToDouble(scanx - minX)/Convert.ToDouble(maxX - minX);
                    double netY = Convert.ToDouble(scany - minY)/Convert.ToDouble(maxY - minY);
                    double net = Convert.ToDouble(netX - netY);

                    if (Math.Abs(net) < options.NetTolerance)
                    {
                        if (Math.Abs(peptideX.Mz - peptideY.Mz) < options.MzTolerance)
                        {
                            if (PeptideUtility.PassesCutoff(peptideX, options.IdScore, options.Fdr) &&
                                PeptideUtility.PassesCutoff(peptideY, options.IdScore, options.Fdr) &&
                                peptideX.Sequence.Equals(peptideY.Sequence))
                            {
                                count++;
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
            foreach (string name in names)
            {
                Console.WriteLine(name);
            }
            Console.WriteLine("Matches - {0}", count);
        }

        /// <summary>
        ///     Tests distributions using the peptide match file (uniqued matches for building error distributions)
        /// </summary>
        /// <param name="rawPathX"></param>
        /// <param name="rawPathY"></param>
        /// <param name="peptideMatches"></param>
        /// <param name="comparerType"></param>
        /// <param name="mzTolerance"></param>
        [Test(
            Description =
                "Figure 1: Compares all spectra (N x N) computing true / false positives based on peptide sequence.")]
        [TestCase(@"QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32",
            @"QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32",
            SpectralComparison.CosineDotProduct,
            .05, // m/z
            .25, // NET
            0, // Similarity Cutoff Score
            1e-15, // MSGF+ Score
            .1, // Peptide FDR
            .5)] // Ion Percent
        public void GenerateFigure1(string rawNameX,
            string rawNameY,
            SpectralComparison comparerType,
            double mzTolerance,
            double netTolerance,
            double similarityScoreCutoff,
            double peptideScore,
            double peptideFdr,
            double ionPercent)
        {
            Console.WriteLine("Creating Figure 1: {2}, Test: {0}\tcompared to\t{1}", rawNameX, rawNameY, comparerType);

            // Create alignment datasets 
            var datasetX = new AlignmentDataset(m_basePath, rawNameX);
            var datasetY = new AlignmentDataset(m_basePath, rawNameY);

            // Then the writer for creating a report
            ISpectralAnalysisWriter writer = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure1,
                "Figure1");

            // The options for the analysis 
            var options = new SpectralOptions();
            options.MzTolerance = mzTolerance;
            options.NetTolerance = netTolerance;
            options.SimilarityCutoff = 0;
            options.TopIonPercent = ionPercent;
            options.IdScore = peptideScore;
            options.ComparerType = comparerType;

            /// This helps us compare various comparison calculation methods
            ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(options.ComparerType);

            // This guy filters the spectra, so that we only keep the N most intense ions for comparison
            ISpectraFilter filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

            // Here we find all the matches
            var finder = new SpectralAnchorPointFinder();

            IEnumerable<AnchorPointMatch> matches = null;
            using (ISpectraProvider readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
            {
                readerX.AddDataFile(datasetX.RawFile, 0);
                using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                {
                    readerY.AddDataFile(datasetY.RawFile, 0);

                    matches = finder.FindAnchorPoints(readerX,
                        readerY,
                        comparer,
                        filter,
                        options);
                }
            }

            // Read data for peptides 
            ISequenceFileReader reader = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            IEnumerable<Peptide> peptidesA = reader.Read(datasetX.PeptideFile);
            IEnumerable<Peptide> peptidesB = reader.Read(datasetY.PeptideFile);
            Dictionary<int, Peptide> peptideMapX = PeptideUtility.MapWithBestScan(peptidesA);
            Dictionary<int, Peptide> peptideMapY = PeptideUtility.MapWithBestScan(peptidesB);

            /// Then map the peptide sequences to identify True Positive and False Positives
            var matcher = new PeptideAnchorPointMatcher();
            matcher.Match(matches,
                peptideMapX,
                peptideMapY,
                options);

            // Package the data
            var analysis = new SpectralAnalysis();
            analysis.Options = options;
            analysis.Matches = matches;

            // Write the results
            writer.Write(analysis);
        }

        [Test(
            Description =
                "Figure 1: Compares all spectra (N x N) computing true / false positives based on peptide sequence.")]
        [TestCase(
            // @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\test",
            @"M:\doc\papers\paperAlignment\Data\figure1\LargeScale\",
            SpectralComparison.CosineDotProduct,
            .5, // mz bin size when retrieving spectra
            1, // m/z
            .25, // NET
            0, // Similarity Cutoff Score
            1, // MSGF+ Score
            .01, // Peptide FDR
            .8, // Ion Percent                
            100, // Required peaks
            Ignore = true
            )]
        [TestCase(
            @"M:\doc\papers\paperAlignment\Data\figure1\LargeScale\requiredPeakCounts",
            SpectralComparison.CosineDotProduct,
            .5, // mz bin size when retrieving spectra
            1, // m/z
            .25, // NET
            0, // Similarity Cutoff Score
            1, // MSGF+ Score
            .01, // Peptide FDR
            .8, // Ion Percent            
            32, // Required peaks
            Ignore = false,
            Description =
                "This test case only looks at a subset of the large scale study to see if we can remove spectra with a low number of peaks"
            )]
        public void GenerateFigure1_LargeScaleStatistics(string directory,
            SpectralComparison comparerType,
            double mzBinSize,
            double mzTolerance,
            double netTolerance,
            double similarityScoreCutoff,
            double peptideScore,
            double peptideFdr,
            double ionPercent,
            int numberOfRequiredPeaks)
        {
            Console.WriteLine("Large Scale Test For {0}", directory);

            string[] cacheFiles = Directory.GetFiles(directory, "*.mscache");
            string[] msgfFiles = Directory.GetFiles(directory, "*_msgfdb_fht.txt");

            Console.WriteLine("Building data cache");
            var map = new Dictionary<string, PathCache>();
            foreach (string path in cacheFiles)
                map.Add(path.ToLower(), null);

            var data = new List<PathCache>();
            foreach (string path in msgfFiles)
            {
                string name = path.ToLower().Replace("_msgfdb_fht.txt", ".mscache");
                string newName = Path.Combine(directory, name);
                string featureName = path.ToLower().Replace("_msgfdb_fht.txt", "_isos.csv");
                string features = Path.Combine(directory, name);

                if (map.ContainsKey(newName))
                {
                    var cacheData = new PathCache();
                    cacheData.Cache = newName;
                    cacheData.Msgf = path;
                    cacheData.Features = features;
                    data.Add(cacheData);
                }
            }

            // Then the writer for creating a report
            ISpectralAnalysisWriter writer = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure1,
                "results-figure1-largeStatistics");

            // The options for the analysis 
            var options = new SpectralOptions();
            options.MzBinSize = mzBinSize;
            options.MzTolerance = mzTolerance;
            options.NetTolerance = netTolerance;
            options.SimilarityCutoff = similarityScoreCutoff;
            options.TopIonPercent = ionPercent;
            options.IdScore = peptideScore;
            options.ComparerType = comparerType;
            options.Fdr = peptideFdr;
            options.RequiredPeakCount = numberOfRequiredPeaks;

            Console.WriteLine("Data: {0}", data.Count);

            for (int i = 0; i < data.Count; i++)
            {
                PathCache cachex = data[i];
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                string rawPathX = ScanSummaryCache.ReadPath(cachex.Cache);
                var datasetX = new AlignmentDataset(rawPathX, "", cachex.Msgf);

                // create a raw file reader for the datasets
                using (ISpectraProvider readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
                {
                    // wrap it in the cached object so we can load scan meta-data
                    var cacheReaderX = new RawLoaderCache(readerX);
                    Dictionary<int, ScanSummary> cacheDataX = ScanSummaryCache.ReadCache(cachex.Cache);

                    readerX.AddDataFile(rawPathX, 0);
                    cacheReaderX.AddCache(0, cacheDataX);

                    for (int j = i + 1; j < data.Count; j++)
                    {
                        PathCache cachey = data[j];
                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        string rawPathY = ScanSummaryCache.ReadPath(cachey.Cache);
                        var datasetY = new AlignmentDataset(rawPathY, "", cachey.Msgf);

                        // create a raw file reader for the datasets
                        using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                        {
                            // wrap it in the cached object so we can load scan meta-data
                            var cacheReaderY = new RawLoaderCache(readerY);
                            Dictionary<int, ScanSummary> cacheDataY = ScanSummaryCache.ReadCache(cachey.Cache);
                            cacheReaderY.AddCache(0, cacheDataY);
                            readerY.AddDataFile(rawPathY, 0);

                            var names = new List<string>();
                            names.Add(data[i].Cache);
                            names.Add(data[j].Cache);

                            SpectralAnalysis analysis = MatchDatasets(cacheReaderX,
                                cacheReaderY,
                                datasetX,
                                datasetY,
                                names,
                                options);
                            writer.Write(analysis);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Tests distributions using the peptide match file (uniqued matches for building error distributions)
        /// </summary>
        /// <param name="rawPathX"></param>
        /// <param name="rawPathY"></param>
        /// <param name="peptideMatches"></param>
        /// <param name="comparerType"></param>
        /// <param name="mzTolerance"></param>
        [Test(
            Description =
                "Figure 1: Compares all spectra (N x N) computing true / false positives based on peptide sequence.")]
        [TestCase(
            @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\test",
            //@"M:\doc\papers\paperAlignment\Data\figure1\Test",
            //SpectralComparison.CosineDotProduct,    
            SpectralComparison.CosineDotProduct,
            .5, // mz bin size when retrieving spectra
            .5, // m/z
            .25, // NET
            0, // Similarity Cutoff Score
            1e-15, // MSGF+ Score
            .1, // Peptide FDR
            .8)] // Ion Percent
        public void GenerateFigure1_LargeScaleStatistics2(string directory,
            SpectralComparison comparerType,
            double mzBinSize,
            double mzTolerance,
            double netTolerance,
            double similarityScoreCutoff,
            double peptideScore,
            double peptideFdr,
            double ionPercent)
        {
            Console.WriteLine("Large Scale Test For {0}", directory);

            string[] cacheFiles = Directory.GetFiles(directory, "*.mscache");
            string[] msgfFiles = Directory.GetFiles(directory, "*_msgfdb_fht.txt");

            Console.WriteLine("Building data cache");
            var map = new Dictionary<string, PathCache>();
            foreach (string path in cacheFiles)
                map.Add(path.ToLower(), null);

            var data = new List<PathCache>();
            foreach (string path in msgfFiles)
            {
                string name = path.ToLower().Replace("_msgfdb_fht.txt", ".mscache");
                string newName = Path.Combine(directory, name);
                if (map.ContainsKey(newName))
                {
                    var cacheData = new PathCache();
                    cacheData.Cache = newName;
                    cacheData.Msgf = path;
                    data.Add(cacheData);
                }
            }

            // The options for the analysis 
            var options = new SpectralOptions();
            options.MzBinSize = mzBinSize;
            options.MzTolerance = mzTolerance;
            options.NetTolerance = netTolerance;
            options.SimilarityCutoff = 0;
            options.TopIonPercent = ionPercent;
            options.IdScore = peptideScore;
            options.ComparerType = comparerType;


            Console.WriteLine("Data: {0}", data.Count);

            for (int i = 0; i < data.Count; i++)
            {
                PathCache cachex = data[i];
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                string rawPathX = ScanSummaryCache.ReadPath(cachex.Cache);
                var datasetX = new AlignmentDataset(rawPathX, "", cachex.Msgf);

                // create a raw file reader for the datasets
                using (ISpectraProvider readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
                {
                    // wrap it in the cached object so we can load scan meta-data
                    var cacheReaderX = new RawLoaderCache(readerX);
                    Dictionary<int, ScanSummary> cacheDataX = ScanSummaryCache.ReadCache(cachex.Cache);
                    readerX.AddDataFile(rawPathX, 0);
                    cacheReaderX.AddCache(0, cacheDataX);

                    for (int j = i + 1; j < data.Count; j++)
                    {
                        PathCache cachey = data[j];
                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        string rawPathY = ScanSummaryCache.ReadPath(cachey.Cache);
                        var datasetY = new AlignmentDataset(rawPathY, "", cachey.Msgf);

                        // create a raw file reader for the datasets
                        using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                        {
                            // wrap it in the cached object so we can load scan meta-data
                            var cacheReaderY = new RawLoaderCache(readerY);
                            Dictionary<int, ScanSummary> cacheDataY = ScanSummaryCache.ReadCache(cachey.Cache);
                            cacheReaderY.AddCache(0, cacheDataY);
                            readerY.AddDataFile(rawPathY, 0);
                            // MatchDatasets(cacheReaderX, cacheReaderY, datasetX, datasetY, new List<string>(), options);
                        }
                    }
                }
            }
        }

        [Test(
            Description =
                "Figure 1: Compares all spectra (N x N) computing true / false positives based on peptide sequence.")]
        [TestCase(
            // @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\test",
            @"M:\doc\papers\paperAlignment\Data\figure1\Test",
            SpectralComparison.CosineDotProduct,
            .5, // mz bin size when retrieving spectra
            1, // m/z
            .25, // NET
            0, // Similarity Cutoff Score
            1, // MSGF+ Score
            .01, // Peptide FDR
            .8)] // Ion Percent
        public void GenerateFigure1_PeptideMatches(string directory,
            SpectralComparison comparerType,
            double mzBinSize,
            double mzTolerance,
            double netTolerance,
            double similarityScoreCutoff,
            double peptideScore,
            double peptideFdr,
            double ionPercent)
        {
            Console.WriteLine("Large Scale Test For {0}", directory);

            string[] cacheFiles = Directory.GetFiles(directory, "*.mscache");
            string[] msgfFiles = Directory.GetFiles(directory, "*_msgfdb_fht.txt");

            Console.WriteLine("Building data cache");
            var map = new Dictionary<string, PathCache>();
            foreach (string path in cacheFiles)
                map.Add(path.ToLower(), null);

            var data = new List<PathCache>();
            foreach (string path in msgfFiles)
            {
                string name = path.ToLower().Replace("_msgfdb_fht.txt", ".mscache");
                string newName = Path.Combine(directory, name);
                string featureName = path.ToLower().Replace("_msgfdb_fht.txt", "_isos.csv");
                string features = Path.Combine(directory, name);

                if (map.ContainsKey(newName))
                {
                    var cacheData = new PathCache();
                    cacheData.Cache = newName;
                    cacheData.Msgf = path;
                    cacheData.Features = features;
                    data.Add(cacheData);
                }
            }

            // The options for the analysis 
            var options = new SpectralOptions();
            options.MzBinSize = mzBinSize;
            options.MzTolerance = mzTolerance;
            options.NetTolerance = netTolerance;
            options.SimilarityCutoff = similarityScoreCutoff;
            options.TopIonPercent = ionPercent;
            options.IdScore = peptideScore;
            options.ComparerType = comparerType;
            options.Fdr = peptideFdr;

            Console.WriteLine("Data: {0}", data.Count);

            for (int i = 0; i < data.Count; i++)
            {
                PathCache cachex = data[i];
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                string rawPathX = ScanSummaryCache.ReadPath(cachex.Cache);
                var datasetX = new AlignmentDataset(rawPathX, "", cachex.Msgf);

                Dictionary<int, ScanSummary> scanDataX = ScanSummaryCache.ReadCache(cachex.Cache);

                for (int j = i + 1; j < data.Count; j++)
                {
                    PathCache cachey = data[j];


                    Dictionary<int, ScanSummary> scanDataY = ScanSummaryCache.ReadCache(cachey.Cache);

                    // Get the raw path stored in the cache file...
                    // then get the dataset object 
                    string rawPathY = ScanSummaryCache.ReadPath(cachey.Cache);
                    var datasetY = new AlignmentDataset(rawPathY, "", cachey.Msgf);

                    var names = new List<string>();
                    names.Add(data[i].Cache);
                    names.Add(data[j].Cache);
                    MatchPeptides(datasetX, datasetY, scanDataX, scanDataY, names, options);
                }
            }
        }

        /// <summary>
        ///     Tests distributions using the peptide match file (uniqued matches for building error distributions)
        /// </summary>
        /// <param name="rawPathX"></param>
        /// <param name="rawPathY"></param>
        /// <param name="peptideMatches"></param>
        /// <param name="comparerType"></param>
        /// <param name="mzTolerance"></param>
        [Test(Description = "Figure 2: Computes the NET / MASS Error Distributions for pre-post ")]
        [TestCase(@"QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32",
            @"QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32",
            SpectralComparison.CosineDotProduct,
            .5, // m/z
            .15, // NET          
            .6, // SSM similarity score,
            1e-15, // MSGF+ Score
            .1, // Peptide FDR
            .5)] // Ion Percent
        public void GenerateFigure2(string rawNameX,
            string rawNameY,
            SpectralComparison comparerType,
            double mzTolerance,
            double netTolerance,
            double similarityScoreCutoff,
            double peptideScore,
            double peptideFdr,
            double ionPercent)
        {
            Console.WriteLine("Creating Figure 2: {2}, Test: {0}\tCompared To\t{1}", rawNameX, rawNameY, comparerType);

            // Create alignment datasets 
            var datasetX = new AlignmentDataset(m_basePath, rawNameX);
            var datasetY = new AlignmentDataset(m_basePath, rawNameY);

            // The options for the analysis 
            var options = new SpectralOptions();
            options.MzTolerance = mzTolerance;
            options.NetTolerance = netTolerance;
            options.SimilarityCutoff = similarityScoreCutoff;
            options.TopIonPercent = ionPercent;
            options.IdScore = peptideScore;
            options.ComparerType = comparerType;

            // Then the writer for creating a report
            ISpectralAnalysisWriter writer = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure2,
                "Figure2");


            // Create an action to load and align the feature data.
            var features = new AlignedFeatureData();
            var aligner = new FeatureAligner();
            var loader = new FeatureLoader();
            Action loadingAction = delegate
            {
                features.Baseline = loader.LoadFeatures(datasetX.FeatureFile);
                features.Alignee = loader.LoadFeatures(datasetY.FeatureFile);
                features = aligner.AlignFeatures(features.Baseline,
                    features.Alignee,
                    options);
            };


            // Creates an action to load the peptide data on a separate task
            /// Find the anchor point matches based on peptide sequences
            ISequenceFileReader reader = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            IEnumerable<AnchorPointMatch> peptideMatches = null;
            Action peptideLoadingAction = delegate
            {
                IEnumerable<Peptide> peptidesX = reader.Read(datasetX.PeptideFile);
                IEnumerable<Peptide> peptidesY = reader.Read(datasetY.PeptideFile);

                peptidesX = peptidesX.Where(x => x.Score < options.IdScore);
                peptidesY = peptidesY.Where(x => x.Score < options.IdScore);

                /// Then map the peptide sequences to identify True Positive and False Positives
                var peptideFinder = new PeptideAnchorPointFinder();
                peptideMatches = peptideFinder.FindAnchorPoints(peptidesX,
                    peptidesY,
                    options);
            };

            var loadingTask = new Task(loadingAction);
            loadingTask.Start();

            var peptideLoadingTask = new Task(peptideLoadingAction);
            peptideLoadingTask.Start();


            /// Find the anchor point matches based on SSM
            ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(options.ComparerType);
            ISpectraFilter filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);
            var spectralFinder = new SpectralAnchorPointFinder();
            IEnumerable<AnchorPointMatch> spectralMatches = null;
            using (ISpectraProvider readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
            {
                using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                {
                    readerX.AddDataFile(datasetX.RawFile, 0);
                    readerY.AddDataFile(datasetY.RawFile, 0);

                    spectralMatches = spectralFinder.FindAnchorPoints(readerX,
                        readerY,
                        comparer,
                        filter,
                        options);
                }
            }


            // These should already be done by the time we get here...let's just try
            loadingTask.Wait();
            peptideLoadingTask.Wait();

            // Package the data
            var analysisSpectra = new SpectralAnalysis();
            analysisSpectra.Options = options;
            analysisSpectra.Matches = spectralMatches;

            // Package the data
            var analysisPeptides = new SpectralAnalysis();
            analysisSpectra.Options = options;
            analysisSpectra.Matches = peptideMatches;

            // Write the results
            writer.Write(analysisSpectra);
            writer.Write(analysisPeptides);
        }

        [Test(Description = "Figure 3: Creates a pre-post alignment using Lowess.")]
        [TestCase(
            // @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\test",
            @"M:\doc\papers\paperAlignment\Data\figure1\LargeScale\",
            SpectralComparison.CosineDotProduct,
            .5, // mz bin size when retrieving spectra
            1, // m/z
            .25, // NET
            0, // Similarity Cutoff Score
            1, // MSGF+ Score
            .01, // Peptide FDR
            .8, // Ion Percent                
            100, // Required peaks
            Ignore = true
            )]
        [TestCase(
            @"M:\doc\papers\paperAlignment\Data\figure1\LargeScale\",
            SpectralComparison.CosineDotProduct,
            .5,     // mz bin size when retrieving spectra
            1,      // m/z
            .25,    // NET
            .6,     // Similarity Cutoff Score            
            1,      // MSGF+ Score
            .01,    // Peptide FDR
            .8,     // Ion Percent            
            32,     // Required peaks
            Ignore = false,
            Description =
                "This test case only looks at a subset of the large scale study to see if we can remove spectra with a low number of peaks"
            )]
        public void GenerateFigure3_Matches(string directory,
            SpectralComparison comparerType,
            double mzBinSize,
            double mzTolerance,
            double netTolerance,
            double similarityScoreCutoff,
            double peptideScore,
            double peptideFdr,
            double ionPercent,
            int numberOfRequiredPeaks)
        {
            AlignmentAnalysisWriterFactory.BasePath = @"M:\doc\papers\paperAlignment\Data\figure3";

            Console.WriteLine("Post-Pre Tests For {0}", directory);

            string[] cacheFiles = Directory.GetFiles(directory, "*.mscache");
            string[] msgfFiles = Directory.GetFiles(directory, "*_msgfdb_fht.txt");

            Console.WriteLine("Building data cache");
            var map = new Dictionary<string, PathCache>();
            foreach (string path in cacheFiles)
                map.Add(path.ToLower(), null);

            var data = new List<PathCache>();
            foreach (string path in msgfFiles)
            {
                string name = path.ToLower().Replace("_msgfdb_fht.txt", ".mscache");
                string newName = Path.Combine(directory, name);
                string featureName = path.ToLower().Replace("_msgfdb_fht.txt", "_isos.csv");
                string features = Path.Combine(directory, name);

                if (map.ContainsKey(newName))
                {
                    var cacheData = new PathCache();
                    cacheData.Cache = newName;
                    cacheData.Msgf = path;
                    cacheData.Features = features;
                    data.Add(cacheData);
                }
            }

            
            // The options for the analysis 
            var options                 = new SpectralOptions();
            options.MzBinSize           = mzBinSize;
            options.MzTolerance         = mzTolerance;
            options.NetTolerance        = netTolerance;
            options.SimilarityCutoff    = similarityScoreCutoff;
            options.TopIonPercent       = ionPercent;
            options.IdScore             = peptideScore;
            options.ComparerType        = comparerType;
            options.Fdr                 = peptideFdr;
            options.RequiredPeakCount   = numberOfRequiredPeaks;

            int comparison = 0;
            for (int i = 0; i < data.Count; i++)
            {
                PathCache cachex = data[i];
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                string rawPathX = ScanSummaryCache.ReadPath(cachex.Cache);
                var datasetX = new AlignmentDataset(rawPathX, "", cachex.Msgf);

                // create a raw file reader for the datasets
                using (ISpectraProvider readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
                {
                    // wrap it in the cached object so we can load scan meta-data
                    var cacheReaderX = new RawLoaderCache(readerX);
                    Dictionary<int, ScanSummary> cacheDataX = ScanSummaryCache.ReadCache(cachex.Cache);

                    readerX.AddDataFile(rawPathX, 0);
                    cacheReaderX.AddCache(0, cacheDataX);

                    for (int j = i + 1; j < data.Count; j++)
                    {
                        // Then the writer for creating a report
                        ISpectralAnalysisWriter writer = 
                            AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure3,
                                "results-figure3-largeScale" + comparison);
                        comparison++;

                        PathCache cachey = data[j];
                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        string rawPathY = ScanSummaryCache.ReadPath(cachey.Cache);
                        var datasetY = new AlignmentDataset(rawPathY, "", cachey.Msgf);

                        // create a raw file reader for the datasets
                        using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                        {
                            // wrap it in the cached object so we can load scan meta-data
                            var cacheReaderY = new RawLoaderCache(readerY);
                            Dictionary<int, ScanSummary> cacheDataY = ScanSummaryCache.ReadCache(cachey.Cache);
                            cacheReaderY.AddCache(0, cacheDataY);
                            readerY.AddDataFile(rawPathY, 0);
                            var names = new List<string>();
                            names.Add(data[i].Cache);
                            names.Add(data[j].Cache);

                            // Write the results
                            SpectralAnalysis analysis = MatchDatasets(cacheReaderX,
                                                                        cacheReaderY,
                                                                        datasetX,
                                                                        datasetY,
                                                                        names,
                                                                        options);


                            var interpolator = new LoessInterpolator();
                            var xvalues      = new List<double>();
                            var yvalues      = new List<double>();

                            IOrderedEnumerable<AnchorPointMatch> matches =
                                analysis.Matches.OrderBy(x => x.AnchorPointX.Net);

                            // 1. Find the best matches
                            // 2. Find only matches that have been made once.
                            
                            var bestMatches = new Dictionary<int, AnchorPointMatch>();
                            foreach (AnchorPointMatch match in matches)
                            {
                                int scan = match.AnchorPointX.Scan;
                                if (bestMatches.ContainsKey(scan))
                                {
                                    if (bestMatches[scan].SimilarityScore < match.SimilarityScore)
                                    {
                                        bestMatches[scan] = match;
                                    }
                                }
                                else
                                {
                                    bestMatches.Add(scan, match);
                                }
                            } 

                            // 2. Find only those matched once
                            var all = new Dictionary<int, AnchorPointMatch>();
                            foreach (var match in bestMatches.Values)
                            {
                                int scan = match.AnchorPointY.Scan;
                                if (all.ContainsKey(scan))
                                {
                                    if (all[scan].SimilarityScore < match.SimilarityScore)
                                    {
                                        all[scan] = match;
                                    }
                                }
                                else
                                {
                                    all.Add(scan, match);
                                }                              
                            }

                            var anchorPoints =
                                all.Values.OrderBy(x => x.AnchorPointX.Net).ToList();

                            analysis.NetData.PostAlignment.Clear();
                            analysis.NetData.PreAlignment.Clear();
                            analysis.MassData.PostAlignment.Clear();
                            analysis.MassData.PreAlignment.Clear();
                            analysis.Matches = anchorPoints;
                            
                           
                            foreach (var match in anchorPoints)
                            {
                                xvalues.Add(match.AnchorPointX.Net);
                                yvalues.Add(match.AnchorPointY.Net);
                            }

                            IList<double> residualsPost = interpolator.Smooth(xvalues, yvalues,
                                                                FitFunctionFactory.Create(FitFunctionTypes.TriCubic));

                            var pre  = new List<double>();
                            var post = new List<double>();

                            var preMass  = new List<double>();
                            var postMass = new List<double>(); 

                            writer.Write(analysis);
                            writer.WriteLine("");
                            writer.WriteLine("[Data]");
                            writer.WriteLine("x\ty\tfit\tpre-diff\tpost-diff\tsim score\tvalid");
                            for (int index = 0; index < xvalues.Count; index++)
                            {
                                double x        = xvalues[index];
                                double y        = yvalues[index];
                                double value    = interpolator.Predict(x);
                                double preDiff  = x - y;
                                double postDif  = value - y;

                                writer.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", x,
                                                                                     y, 
                                                                                     residualsPost[index],
                                                                                     preDiff, 
                                                                                     postDif, 
                                                                                     anchorPoints[index].SimilarityScore,
                                                                                     anchorPoints[index].IsValidMatch));
                                pre.Add(preDiff);                                
                                post.Add(postDif);
                            }

                            writer.WriteLine("");
                            writer.WriteLine("[Error Histograms]");
                            Histogram preHist  = 
                                MatchCountHistogramBuilder.CreateResidualHistogram(-.5, .5, .01, pre);
                            Histogram postHist = 
                                MatchCountHistogramBuilder.CreateResidualHistogram(-.5, .5, .01, post);

                            writer.WriteLine("Value\t False Matches\t True Matches");
                            for (int index = 0; index < preHist.Bins.Count; index++)
                            {
                                double preValue = preHist.Bins[index];
                                writer.WriteLine(string.Format("{0}\t{1}\t{2}",
                                                                    preValue,
                                                                    preHist.Data[index],
                                                                    postHist.Data[index]));
                            }

                            writer.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     This function tests the Loess interpolation code
        /// </summary>
        /// <param name="type"></param>
        /// <param name="generatingFunction"></param>
        /// <param name="dt"></param>
        /// <param name="attenuation"></param>
        [Test]
        [TestCase(FitFunctionTypes.TriCubic,
            FitFunctionTypes.Sin,
            .4,
            .5)]
        public void TestLoess(FitFunctionTypes type,
            FitFunctionTypes generatingFunction,
            double dt,
            double attenuation)
        {
            var interpolator = new LoessInterpolator(.25, 0);

            var xValues = new List<double>();
            var yValues = new List<double>();


            double cv = 0;

            Func<double, double> fitFunction = FitFunctionFactory.Create(type);
            Func<double, double> genFunction = FitFunctionFactory.Create(generatingFunction);

            var random = new Random();

            // Take one period of the sine wave...
            while (cv < Math.PI*3)
            {
                double value = genFunction(cv) + random.NextDouble()*attenuation;

                xValues.Add(cv);
                yValues.Add(value);
                cv += dt;
            }

            IList<double> newYValues = interpolator.Smooth(xValues, yValues, fitFunction);
            for (int i = 0; i < xValues.Count; i++)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", i, xValues[i], yValues[i], newYValues[i]);
            }

            dt /= 2;
            cv = 0;

            Console.WriteLine();

            // Take one period of the sine wave...
            while (cv < Math.PI*3)
            {
                double predicted = interpolator.Predict(cv);
                Console.WriteLine("{0}\t{1}", cv, predicted);
                cv += dt;
            }
        }
    }
}