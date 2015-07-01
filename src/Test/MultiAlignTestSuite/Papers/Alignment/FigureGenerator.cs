#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.RawData;
using MultiAlignTestSuite.Papers.Alignment.IO;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using PNNLOmics.Algorithms.Alignment.SpectralMatches;
using PNNLOmics.Algorithms.Alignment.SpectralMatching;
using PNNLOmics.Algorithms.Regression;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Utilities;
using PNNLOmicsIO.IO;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment
{
    /// <summary>
    ///     Creates all figures used in the alignment paper
    /// </summary>
    [TestFixture]
    public class FigureGenerator
    {
        [SetUp]
        public void SetupFigureTests()
        {
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
        private static PlotModel CreatePlot(IReadOnlyList<XYData> peaksX,
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

            var xseries = new StemSeries();
            for (var j = 0; j < peaksY.Count; j++)
            {
                var peakX = peaksX[j];
                var peakY = peaksY[j];

                double value = 0;
                if (peakX.Y > 0 && peakY.Y > 0)
                    value = 1;

                xseries.Points.Add(new DataPoint(peakX.X, value));
            }
            xseries.Color = OxyColor.FromAColor(100, OxyColors.Green);

            //plotModel1.Series.Add(xseries);

            var series = new StemSeries {Title = "Spectra X"};
            var max = peaksX.Select(datum => datum.Y).Concat(new double[] {0}).Max();
            foreach (var datum in peaksX)
            {
                series.Points.Add(new DataPoint(datum.X, datum.Y/max));
            }
            plotModel1.Series.Add(series);

            max = peaksY.Select(datum => datum.Y).Concat(new[] {max}).Max();
            var series2 = new StemSeries {Title = "Spectra Y"};
            foreach (var datum in peaksY)
            {
                series2.Points.Add(new DataPoint(datum.X, (datum.Y*-1)/max));
            }
            plotModel1.Series.Add(series2);


            return plotModel1;
        }

        private void DisplayComparisonPlot(MSSpectra spectrumX, MSSpectra spectrumY, double mzTolerance,
            string newTitle = "MS/MS Spectra")
        {
            var model = CreatePlot(spectrumX.Peaks, spectrumY.Peaks, mzTolerance);
            model.Title = newTitle;

            var plot = new Plot {Model = model, Dock = DockStyle.Fill};
            var form = new Form {Size = Screen.PrimaryScreen.WorkingArea.Size};
            form.Controls.Add(plot);
            form.Show();

            MultiAlignTestSuite.IO.Utilities.SleepNow(3);

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

            var fitFunction = FitFunctionFactory.Create(type);
            var genFunction = FitFunctionFactory.Create(generatingFunction);

            var random = new Random();

            // Take one period of the sine wave...
            while (cv < Math.PI*3)
            {
                var value = genFunction(cv) + random.NextDouble()*attenuation;

                xValues.Add(cv);
                yValues.Add(value);
                cv += dt;
            }

            var newYValues = interpolator.Smooth(xValues, yValues, fitFunction);
            for (var i = 0; i < xValues.Count; i++)
            {
                Console.WriteLine(@"{0}	{1}	{2}	{3}", i, xValues[i], yValues[i], newYValues[i]);
            }

            dt /= 2;
            cv = 0;

            Console.WriteLine();

            // Take one period of the sine wave...
            while (cv < Math.PI*3)
            {
                var predicted = interpolator.Predict(cv);
                Console.WriteLine(@"{0}	{1}", cv, predicted);
                cv += dt;
            }
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
            Console.WriteLine(@"Large Scale Test For {0}", directory);

            var cacheFiles = Directory.GetFiles(directory, "*.mscache");
            var msgfFiles = Directory.GetFiles(directory, "*_msgfdb_fht.txt");

            Console.WriteLine(@"Building data cache");
            var map = cacheFiles.ToDictionary<string, string, PathCache>(path => path.ToLower(), path => null);

            var data = (from path in msgfFiles
                let name = path.ToLower().Replace("_msgfdb_fht.txt", ".mscache")
                let newName = Path.Combine(directory, name)
                let features = Path.Combine(directory, name)
                where map.ContainsKey(newName)
                select new PathCache {Cache = newName, Msgf = path, Features = features}).ToList();

            // Then the writer for creating a report
            var writer = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure1,
                "results-figure1-largeStatistics");

            // The options for the analysis 
            var options = new SpectralOptions
            {
                MzBinSize = mzBinSize,
                MzTolerance = mzTolerance,
                NetTolerance = netTolerance,
                SimilarityCutoff = similarityScoreCutoff,
                TopIonPercent = ionPercent,
                IdScore = peptideScore,
                ComparerType = comparerType,
                Fdr = peptideFdr,
                RequiredPeakCount = numberOfRequiredPeaks
            };

            Console.WriteLine(@"Data: {0}", data.Count);

            for (var i = 0; i < data.Count; i++)
            {
                var cachex = data[i];
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                var rawPathX = ScanSummaryCache.ReadPath(cachex.Cache);
                var datasetX = new AlignmentDataset(rawPathX, "", cachex.Msgf);

                // create a raw file reader for the datasets
                using (var readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
                {
                    // wrap it in the cached object so we can load scan meta-data
                    var cacheReaderX = new RawLoaderCache(readerX);
                    var cacheDataX = ScanSummaryCache.ReadCache(cachex.Cache);

                    readerX.AddDataFile(rawPathX, 0);
                    cacheReaderX.AddCache(0, cacheDataX);

                    for (var j = i + 1; j < data.Count; j++)
                    {
                        var cachey = data[j];
                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        var rawPathY = ScanSummaryCache.ReadPath(cachey.Cache);
                        var datasetY = new AlignmentDataset(rawPathY, "", cachey.Msgf);

                        // create a raw file reader for the datasets
                        using (var readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                        {
                            // wrap it in the cached object so we can load scan meta-data
                            var cacheReaderY = new RawLoaderCache(readerY);
                            var cacheDataY = ScanSummaryCache.ReadCache(cachey.Cache);
                            cacheReaderY.AddCache(0, cacheDataY);
                            readerY.AddDataFile(rawPathY, 0);

                            var names = new List<string> {data[i].Cache, data[j].Cache};

                            var analysis = MatchDatasets(comparerType,
                                cacheReaderX,
                                cacheReaderY,
                                options,
                                datasetX,
                                datasetY,
                                names);
                            writer.Write(analysis);
                        }
                    }
                }
            }
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
            .5, // mz bin size when retrieving spectra
            1, // m/z
            .25, // NET
            .6, // Similarity Cutoff Score            
            1, // MSGF+ Score
            .01, // Peptide FDR
            .8, // Ion Percent            
            32, // Required peaks
            Ignore = true,
            Description =
                "This test case only looks at a subset of the large scale study to see if we can remove spectra with a low number of peaks"
            )]
        [TestCase(
            @"M:\doc\papers\paperAlignment\Data\figure4",
            SpectralComparison.CosineDotProduct,
            .5, // mz bin size when retrieving spectra
            1, // m/z
            .25, // NET
            .6, // Similarity Cutoff Score            
            1, // MSGF+ Score
            .01, // Peptide FDR
            .8, // Ion Percent            
            32, // Required peaks
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
            AlignmentAnalysisWriterFactory.BasePath = @"M:\doc\papers\paperAlignment\Data\figure4";

            Console.WriteLine(@"Post-Pre Tests For {0}", directory);

            var cacheFiles = Directory.GetFiles(directory, "*.mscache");
            var msgfFiles = Directory.GetFiles(directory, "*_msgfdb_fht.txt");

            Console.WriteLine(@"Building data cache");
            var map = cacheFiles.ToDictionary<string, string, PathCache>(path => path.ToLower(), path => null);

            var data = (from path in msgfFiles
                let name = path.ToLower().Replace("_msgfdb_fht.txt", ".mscache")
                let newName = Path.Combine(directory, name)
                let features = Path.Combine(directory, name)
                where map.ContainsKey(newName)
                select new PathCache {Cache = newName, Msgf = path, Features = features}).ToList();


            // The options for the analysis 
            var options = new SpectralOptions
            {
                MzBinSize = mzBinSize,
                MzTolerance = mzTolerance,
                NetTolerance = netTolerance,
                SimilarityCutoff = similarityScoreCutoff,
                TopIonPercent = ionPercent,
                IdScore = peptideScore,
                ComparerType = comparerType,
                Fdr = peptideFdr,
                RequiredPeakCount = numberOfRequiredPeaks
            };

            Console.WriteLine(@"{0}", data.Count);

            var comparison = 0;
            for (var i = 0; i < data.Count; i++)
            {
                var cachex = data[i];
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                var rawPathX = ScanSummaryCache.ReadPath(cachex.Cache);
                var datasetX = new AlignmentDataset(rawPathX, "", cachex.Msgf);

                // create a raw file reader for the datasets
                using (var readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
                {
                    // wrap it in the cached object so we can load scan meta-data
                    var cacheReaderX = new RawLoaderCache(readerX);
                    var cacheDataX = ScanSummaryCache.ReadCache(cachex.Cache);

                    readerX.AddDataFile(rawPathX, 0);
                    cacheReaderX.AddCache(0, cacheDataX);

                    for (var j = i + 1; j < data.Count; j++)
                    {
                        // Then the writer for creating a report
                        var writer =
                            AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure3,
                                "results-figure3-largeScale" + comparison);
                        comparison++;

                        var cachey = data[j];
                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        var rawPathY = ScanSummaryCache.ReadPath(cachey.Cache);
                        var datasetY = new AlignmentDataset(rawPathY, "", cachey.Msgf);

                        // create a raw file reader for the datasets
                        using (var readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                        {
                            // wrap it in the cached object so we can load scan meta-data
                            var cacheReaderY = new RawLoaderCache(readerY);
                            var cacheDataY = ScanSummaryCache.ReadCache(cachey.Cache);
                            cacheReaderY.AddCache(0, cacheDataY);
                            readerY.AddDataFile(rawPathY, 0);
                            var names = new List<string> {data[i].Cache, data[j].Cache};

                            // Write the results
                            var analysis = MatchDatasets(comparerType,
                                cacheReaderX,
                                cacheReaderY,
                                options,
                                datasetX,
                                datasetY,
                                names);

                            AlignMatches(analysis, writer);
                        }
                    }
                }
            }
        }

        [TestCase(
            @"M:\doc\papers\paperAlignment\Data\figure4\15-timepoints",
            SpectralComparison.CosineDotProduct,
            .5, // mz bin size when retrieving spectra
            1, // m/z
            .25, // NET
            .6, // Similarity Cutoff Score            
            1, // MSGF+ Score
            .01, // Peptide FDR
            .8, // Ion Percent            
            32, // Required peaks
            "results-figure4-metaMatches",
            Ignore = false,
            Description =
                "This test case only looks at a subset of the large scale study to see if we can remove spectra with a low number of peaks"
            )]
        public void GenerateFigure4_MetaMatches(string directory,
            SpectralComparison comparerType,
            double mzBinSize,
            double mzTolerance,
            double netTolerance,
            double similarityScoreCutoff,
            double peptideScore,
            double peptideFdr,
            double ionPercent,
            int numberOfRequiredPeaks,
            string name)
        {
            AlignmentAnalysisWriterFactory.BasePath = @"M:\doc\papers\paperAlignment\Data\figure4";

            Console.WriteLine(@"Post-Pre Tests For {0}", directory);

            var cacheFiles = Directory.GetFiles(directory, "*.mscache");
            Console.WriteLine(@"Building data cache");
            var data = cacheFiles.Select(path => new PathCache {Cache = path}).ToList();

            // The options for the analysis 
            var options = new SpectralOptions
            {
                MzBinSize = mzBinSize,
                MzTolerance = mzTolerance,
                NetTolerance = netTolerance,
                SimilarityCutoff = similarityScoreCutoff,
                TopIonPercent = ionPercent,
                IdScore = peptideScore,
                ComparerType = comparerType,
                Fdr = peptideFdr,
                RequiredPeakCount = numberOfRequiredPeaks
            };

            var comparison = 0;
            for (var i = 0; i < data.Count; i++)
            {
                var cachex = data[i];
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                var rawPathX = ScanSummaryCache.ReadPath(cachex.Cache);
                var datasetX = new AlignmentDataset(rawPathX, "", cachex.Msgf);

                // create a raw file reader for the datasets
                using (var readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
                {
                    // wrap it in the cached object so we can load scan meta-data
                    var cacheReaderX = new RawLoaderCache(readerX);
                    var cacheDataX = ScanSummaryCache.ReadCache(cachex.Cache);

                    readerX.AddDataFile(rawPathX, 0);
                    cacheReaderX.AddCache(0, cacheDataX);

                    for (var j = i + 1; j < data.Count; j++)
                    {
                        var cachey = data[j];
                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        var rawPathY = ScanSummaryCache.ReadPath(cachey.Cache);
                        var datasetY = new AlignmentDataset(rawPathY, "", cachey.Msgf);

                        // create a raw file reader for the datasets
                        using (var readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                        {
                            // Then the writer for creating a report
                            var writer =
                                AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure3, name + comparison);
                            comparison++;

                            // wrap it in the cached object so we can load scan meta-data
                            var cacheReaderY = new RawLoaderCache(readerY);
                            var cacheDataY = ScanSummaryCache.ReadCache(cachey.Cache);
                            cacheReaderY.AddCache(0, cacheDataY);
                            readerY.AddDataFile(rawPathY, 0);
                            var names = new List<string> {data[i].Cache, data[j].Cache};

                            var analysis = MatchDatasets(comparerType,
                                readerX,
                                readerY,
                                options,
                                datasetX,
                                datasetY,
                                names);

                            AlignMatches(analysis, writer);
                            writer.Close();
                        }
                    }
                }
            }
        }

        private static SpectralAnalysis MatchDatasets(SpectralComparison comparerType,
            ISpectraProvider readerX,
            ISpectraProvider readerY,
            SpectralOptions options,
            AlignmentDataset datasetX,
            AlignmentDataset datasetY,
            List<string> names)
        {
            var peptideReader = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            var finder = new SpectralAnchorPointFinder();
            var validator = new SpectralAnchorPointValidator();
            var comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType);
            var filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

            var matches = finder.FindAnchorPoints(readerX,
                readerY,
                comparer,
                filter,
                options);

            var peptidesX = peptideReader.Read(datasetX.PeptideFile);
            var peptidesY = peptideReader.Read(datasetY.PeptideFile);
            validator.ValidateMatches(matches,
                peptidesX,
                peptidesY,
                options);

            var analysis = new SpectralAnalysis
            {
                DatasetNames = names,
                Matches = matches,
                Options = options
            };
            return analysis;
        }


        private void MatchPeptides(AlignmentDataset datasetX,
            AlignmentDataset datasetY,
            Dictionary<int, ScanSummary> scanDataX,
            Dictionary<int, ScanSummary> scanDataY,
            IEnumerable<string> names,
            SpectralOptions options)
        {
            // Read data for peptides 
            var reader = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            var peptidesA = reader.Read(datasetX.PeptideFile);
            var peptidesB = reader.Read(datasetY.PeptideFile);

            peptidesA =
                peptidesA.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();
            peptidesB =
                peptidesB.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();

            var peptideMapX = PeptideUtility.MapWithBestScan(peptidesA);
            var peptideMapY = PeptideUtility.MapWithBestScan(peptidesB);

            // Determine the scan extrema
            var maxX = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            var minX = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
            var maxY = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            var minY = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;

            // Then map the peptide sequences to identify True Positive and False Positives            
            var count = (from scanx in peptideMapX.Keys
                let peptideX = peptideMapX[scanx]
                from scany in peptideMapY.Keys
                let peptideY = peptideMapY[scany]
                let netX = Convert.ToDouble(scanx - minX)/Convert.ToDouble(maxX - minX)
                let netY = Convert.ToDouble(scany - minY)/Convert.ToDouble(maxY - minY)
                let net = Convert.ToDouble(netX - netY)
                where Math.Abs(net) < options.NetTolerance
                where Math.Abs(peptideX.Mz - peptideY.Mz) < options.MzTolerance
                where PeptideUtility.PassesCutoff(peptideX, options.IdScore, options.Fdr)
                      && PeptideUtility.PassesCutoff(peptideY, options.IdScore, options.Fdr)
                      && peptideX.Sequence.Equals(peptideY.Sequence)
                select peptideX).Count();

            Console.WriteLine();
            foreach (var name in names)
                Console.WriteLine(name);
            Console.WriteLine(@"Matches - {0}", count);
        }


        private static void AlignMatches(SpectralAnalysis analysis, ISpectralAnalysisWriter writer)
        {
            var netXvalues = new List<double>();
            var netYvalues = new List<double>();
            var massXvalues = new List<double>();
            var massYvalues = new List<double>();

            var matches =
                analysis.Matches.OrderBy(x => x.AnchorPointX.Net);

            // 1. Find the best matches
            // 2. Find only matches that have been made once.

            var bestMatches = new Dictionary<int, SpectralAnchorPointMatch>();
            foreach (var match in matches)
            {
                var scan = match.AnchorPointX.Scan;
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
            var all = new Dictionary<int, SpectralAnchorPointMatch>();
            foreach (var match in bestMatches.Values)
            {
                var scan = match.AnchorPointY.Scan;
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
            // Write the analysis 
            writer.Write(analysis);

            // Then generate the NET Alignment using R1
            var anchorPoints =
                all.Values.OrderBy(x => x.AnchorPointX.Net).ToList();

            foreach (var match in anchorPoints)
            {
                netXvalues.Add(match.AnchorPointX.Net);
                netYvalues.Add(match.AnchorPointY.Net);
            }

            Func<double, double, double> netFunc = (x, y) => x - y;
            Func<double, double, double> massFunc = FeatureLight.ComputeMassPPMDifference;
            InterpolateDimension("NET-R1", writer, netXvalues, netYvalues, anchorPoints, netFunc);


            // Then generate the Mass Alignment using R1
            // We also have to resort the matches based on mass now too
            anchorPoints = all.Values.OrderBy(x => x.AnchorPointX.Mz).ToList();
            foreach (var match in anchorPoints)
            {
                massXvalues.Add(match.AnchorPointX.Mz);
                massYvalues.Add(match.AnchorPointY.Mz);
            }
            InterpolateDimension("Mass-R1", writer, massXvalues, massYvalues, anchorPoints, massFunc);
        }

        private static void InterpolateDimension(string name,
            ISpectralAnalysisWriter writer,
            List<double> xvalues,
            List<double> yvalues,
            List<SpectralAnchorPointMatch> anchorPoints,
            Func<double, double, double> func)
        {
            var interpolator = new LoessInterpolator();
            var pre = new List<double>();
            var post = new List<double>();

            var fit = interpolator.Smooth(xvalues, yvalues,
                FitFunctionFactory.Create(FitFunctionTypes.TriCubic));

            writer.WriteLine("");
            WriteMatches(name,
                writer,
                xvalues,
                yvalues,
                fit,
                interpolator,
                anchorPoints,
                pre,
                post,
                func);

            WriteErrorHistogram(name, pre, post, writer);
        }

        private static void WriteMatches(
            string name,
            ISpectralAnalysisWriter writer,
            List<double> xvalues,
            List<double> yvalues,
            IList<double> fit,
            LoessInterpolator interpolator,
            List<SpectralAnchorPointMatch> anchorPoints,
            List<double> preNet,
            List<double> postNet,
            Func<double, double, double> difference)
        {
            if (anchorPoints == null) throw new ArgumentNullException("anchorPoints");

            writer.WriteLine(string.Format(@"[{0}]", name));
            writer.WriteLine("x\ty\tfit\tpre-diff\tpost-diff\tsim score\tvalid");
            for (var index = 0; index < xvalues.Count; index++)
            {
                var x = xvalues[index];
                var y = yvalues[index];
                var value = interpolator.Predict(x);
                var preDiff = difference(x, y);
                var postDiff = difference(value, y);

                writer.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                    x,
                    y,
                    fit[index],
                    preDiff,
                    postDiff,
                    anchorPoints[index].SimilarityScore,
                    anchorPoints[index].IsValidMatch));

                preNet.Add(preDiff);
                postNet.Add(postDiff);
            }
        }

        private static void WriteErrorHistogram(string message, IEnumerable<double> pre, IEnumerable<double> post,
            ISpectralAnalysisWriter writer)
        {
            writer.WriteLine("");
            writer.WriteLine(message);
            var preHist =
                MatchCountHistogramBuilder.CreateResidualHistogram(-.5, .5, .01, pre);
            var postHist =
                MatchCountHistogramBuilder.CreateResidualHistogram(-.5, .5, .01, post);

            writer.WriteLine("Value\t False Matches\t True Matches");
            for (var index = 0; index < preHist.Bins.Count; index++)
            {
                var preValue = preHist.Bins[index];
                writer.WriteLine(string.Format("{0}\t{1}\t{2}",
                    preValue,
                    preHist.Data[index],
                    postHist.Data[index]));
            }
        }
    }
}