using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.IO.Features;
using MultiAlignTestSuite.Papers.Alignment.Data;
using MultiAlignTestSuite.Papers.Alignment.IO;
using NUnit.Framework;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using PNNLOmicsIO.IO;
using System.Threading.Tasks;
using System.IO;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.WindowsForms;
using System.Windows.Forms;
using System.Drawing;
using OxyPlot.Axes;

namespace MultiAlignTestSuite.Papers.Alignment.SSM
{

    /// <summary>
    /// Creates all figures used in the alignment paper
    /// </summary>
    [TestFixture]
    public class FigureGenerator 
    {
        #region Display
        /// <summary>
        /// Creates a plot for the given peaks list
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
                series.Points.Add(new DataPoint(datum.X, datum.Y / max));
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
                series2.Points.Add(new DataPoint(datum.X, (datum.Y * -1) / max));
            }
            plotModel1.Series.Add(series2);


            return plotModel1;
        }

        private void DisplayComparisonPlot(MSSpectra spectrumX, MSSpectra spectrumY, double mzTolerance, string path = "comparison.png", string newTitle = "MS/MS Spectra")
        {
            PlotModel model = CreatePlot(spectrumX.Peaks, spectrumY.Peaks, mzTolerance);
            model.Title = newTitle;

            Plot plot  = new Plot();
            plot.Model = model;
            Form form  = new Form();
            form.Size  = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;
            plot.Dock  = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(plot);
            form.ShowDialog();

            if (false)
            {
                using (Bitmap bitmap = new Bitmap(form.Width, form.Height))
                {
                    form.DrawToBitmap(bitmap, form.DisplayRectangle);
                    bitmap.Save(path);
                }
            }
        }
        #endregion
        protected string m_basePath;

        [SetUp]
        public void TestSetup()
        {
            m_basePath      = @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00";
            AlignmentAnalysisWriterFactory.BasePath = @"M:\doc\papers\paperAlignment\Data\figure1";
        }

        #region Figure 1
        /// <summary>
        /// Tests distributions using the peptide match file (uniqued matches for building error distributions)
        /// </summary>
        /// <param name="rawPathX"></param>
        /// <param name="rawPathY"></param>
        /// <param name="peptideMatches"></param>
        /// <param name="comparerType"></param>
        /// <param name="mzTolerance"></param>
        [Test(Description = "Figure 1: Compares all spectra (N x N) computing true / false positives based on peptide sequence.")]
        [TestCase(  @"QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32",
                    @"QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32",                    
                    SpectralComparison.CosineDotProduct,
                    .05,     // m/z
                    .25,    // NET
                    0,      // Similarity Cutoff Score
                    1e-15,  // MSGF+ Score
                    .1,     // Peptide FDR
                    .5)]    // Ion Percent
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
            AlignmentDataset datasetX       = new AlignmentDataset(m_basePath, rawNameX);
            AlignmentDataset datasetY       = new AlignmentDataset(m_basePath, rawNameY);
            
            // Then the writer for creating a report
            ISpectralAnalysisWriter writer  = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure1, "Figure1");

            // The options for the analysis 
            SpectralOptions options         = new SpectralOptions();
            options.MzTolerance             = mzTolerance;
            options.NetTolerance            = netTolerance;
            options.SimilarityCutoff        = 0;
            options.TopIonPercent           = ionPercent;
            options.IdScore                 = peptideScore;
            options.ComparerType            = comparerType;

            /// This helps us compare various comparison calculation methods
            ISpectralComparer   comparer            = SpectralComparerFactory.CreateSpectraComparer(options.ComparerType);

            // This guy filters the spectra, so that we only keep the N most intense ions for comparison
            ISpectraFilter filter                   = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

            // Here we find all the matches
            SpectralAnchorPointFinder finder        = new SpectralAnchorPointFinder();

            IEnumerable<AnchorPointMatch> matches   = null;
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
            ISequenceFileReader reader           = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            IEnumerable<Peptide> peptidesA       = reader.Read(datasetX.PeptideFile);
            IEnumerable<Peptide> peptidesB       = reader.Read(datasetY.PeptideFile);            
            Dictionary<int, Peptide> peptideMapX = PeptideUtility.MapWithBestScan(peptidesA);
            Dictionary<int, Peptide> peptideMapY = PeptideUtility.MapWithBestScan(peptidesB);

            /// Then map the peptide sequences to identify True Positive and False Positives
            PeptideAnchorPointMatcher matcher    = new PeptideAnchorPointMatcher();
            matcher.Match(  matches, 
                            peptideMapX, 
                            peptideMapY, 
                            options);

            // Package the data
            SpectralAnalysis analysis   = new SpectralAnalysis();
            analysis.Options            = options;
            analysis.Matches            = matches;

            // Write the results
            writer.Write(analysis);                     
        }
        #endregion

        #region Figure 1 - Statistics

        internal class PathCache
        {
            public string Cache { get; set; }
            public string Msgf { get; set; }
            public string Features { get; set; }
        }

        /// <summary>
        /// Tests distributions using the peptide match file (uniqued matches for building error distributions)
        /// </summary>
        /// <param name="rawPathX"></param>
        /// <param name="rawPathY"></param>
        /// <param name="peptideMatches"></param>
        /// <param name="comparerType"></param>
        /// <param name="mzTolerance"></param>
        [Test(Description = "Figure 1: Compares all spectra (N x N) computing true / false positives based on peptide sequence.")]
        [TestCase(
                    @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\test",
                    //@"M:\doc\papers\paperAlignment\Data\figure1\Test",
                    //SpectralComparison.CosineDotProduct,    
                    SpectralComparison.CosineDotProduct,
                    .5,     // mz bin size when retrieving spectra
                    .5,    // m/z
                    .25,    // NET
                    0,      // Similarity Cutoff Score
                    1e-15,  // MSGF+ Score
                    .1,     // Peptide FDR
                    .8)]    // Ion Percent
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
            string[] msgfFiles  = Directory.GetFiles(directory, "*_msgfdb_fht.txt");

            Console.WriteLine("Building data cache");
            Dictionary<string, PathCache> map = new Dictionary<string, PathCache>();
            foreach (var path in cacheFiles)            
                map.Add(path.ToLower(), null);

            List<PathCache> data = new List<PathCache>();
            foreach (var path in msgfFiles)
            {
                string name    = path.ToLower().Replace("_msgfdb_fht.txt", ".mscache");
                string newName = Path.Combine(directory, name);
                if (map.ContainsKey(newName))
                {
                    PathCache cacheData = new PathCache();
                    cacheData.Cache     = newName;
                    cacheData.Msgf      = path;
                    data.Add(cacheData);
                    
                }
            }

            // The options for the analysis 
            SpectralOptions options     = new SpectralOptions();
            options.MzBinSize           = mzBinSize;
            options.MzTolerance         = mzTolerance;
            options.NetTolerance        = netTolerance;
            options.SimilarityCutoff    = 0;
            options.TopIonPercent       = ionPercent;
            options.IdScore             = peptideScore;
            options.ComparerType        = comparerType;


            Console.WriteLine("Data: {0}", data.Count);

            for (int i = 0; i < data.Count; i++)
            {
                PathCache cachex            = data[i];            
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                string rawPathX             = ScanSummaryCache.ReadPath(cachex.Cache);                
                AlignmentDataset datasetX   = new AlignmentDataset(rawPathX, "", cachex.Msgf);         
       
                // create a raw file reader for the datasets
                using (ISpectraProvider readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
                {
                    // wrap it in the cached object so we can load scan meta-data
                    RawLoaderCache cacheReaderX             = new RawLoaderCache(readerX);
                    Dictionary<int, ScanSummary> cacheDataX = ScanSummaryCache.ReadCache(cachex.Cache);                    
                    readerX.AddDataFile(rawPathX, 0);
                    cacheReaderX.AddCache(0, cacheDataX);

                    for (int j = i + 1; j < data.Count; j++)
                    {
                        PathCache cachey            = data[j];            
                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        string rawPathY             = ScanSummaryCache.ReadPath(cachey.Cache);                                        
                        AlignmentDataset datasetY   = new AlignmentDataset(rawPathY, "", cachey.Msgf);         
       
                        // create a raw file reader for the datasets
                        using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                        {
                            // wrap it in the cached object so we can load scan meta-data
                            RawLoaderCache cacheReaderY             = new RawLoaderCache(readerY);
                            Dictionary<int, ScanSummary> cacheDataY = ScanSummaryCache.ReadCache(cachey.Cache);
                            cacheReaderY.AddCache(0, cacheDataY);
                            readerY.AddDataFile(rawPathY, 0);
                           // MatchDatasets(cacheReaderX, cacheReaderY, datasetX, datasetY, new List<string>(), options);
                        }
                    }
                }
            }
        }

        [Test(Description = "Figure 1: Compares all spectra (N x N) computing true / false positives based on peptide sequence.")]
        [TestCase(
                   // @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\test",
                    @"M:\doc\papers\paperAlignment\Data\figure1\LargeScale\",                    
                    SpectralComparison.CosineDotProduct,
                    .5,     // mz bin size when retrieving spectra
                    1,    // m/z
                    .25,    // NET
                    0,      // Similarity Cutoff Score
                    1,  // MSGF+ Score
                    .01,     // Peptide FDR
                    .8)]    // Ion Percent
        public void GenerateFigure1_LargeScaleStatistics(string directory,
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
            string[] msgfFiles  = Directory.GetFiles(directory, "*_msgfdb_fht.txt");

            Console.WriteLine("Building data cache");
            Dictionary<string, PathCache> map = new Dictionary<string, PathCache>();
            foreach (var path in cacheFiles)            
                map.Add(path.ToLower(), null);

            List<PathCache> data = new List<PathCache>();
            foreach (var path in msgfFiles)
            {
                string name         = path.ToLower().Replace("_msgfdb_fht.txt", ".mscache");
                string newName      = Path.Combine(directory, name);
                string featureName  = path.ToLower().Replace("_msgfdb_fht.txt", "_isos.csv");
                string features     = Path.Combine(directory, name);

                if (map.ContainsKey(newName))
                {
                    PathCache cacheData = new PathCache();
                    cacheData.Cache     = newName;
                    cacheData.Msgf      = path;
                    cacheData.Features  = features; 
                    data.Add(cacheData);                    
                }
            }

            // Then the writer for creating a report
            ISpectralAnalysisWriter writer = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure1, "results-figure1-largeStatistics");

            // The options for the analysis 
            SpectralOptions options     = new SpectralOptions();
            options.MzBinSize           = mzBinSize;
            options.MzTolerance         = mzTolerance;
            options.NetTolerance        = netTolerance;
            options.SimilarityCutoff    = similarityScoreCutoff;
            options.TopIonPercent       = ionPercent;
            options.IdScore             = peptideScore;
            options.ComparerType        = comparerType;
            options.Fdr                 = peptideFdr;


            Console.WriteLine("Data: {0}", data.Count);

            for (int i = 0; i < data.Count; i++)
            {
                PathCache cachex            = data[i];            
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                string rawPathX             = ScanSummaryCache.ReadPath(cachex.Cache);                
                AlignmentDataset datasetX   = new AlignmentDataset(rawPathX, "", cachex.Msgf);         
       
                // create a raw file reader for the datasets
                using (ISpectraProvider readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
                {
                    // wrap it in the cached object so we can load scan meta-data
                    RawLoaderCache cacheReaderX             = new RawLoaderCache(readerX);
                    Dictionary<int, ScanSummary> cacheDataX = ScanSummaryCache.ReadCache(cachex.Cache);                    
            
                    //TODO: Match spectra to features
                    
                    readerX.AddDataFile(rawPathX, 0);
                    cacheReaderX.AddCache(0, cacheDataX);

                    for (int j = i + 1; j < data.Count; j++)
                    {
                        PathCache cachey            = data[j];            
                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        string rawPathY             = ScanSummaryCache.ReadPath(cachey.Cache);                                        
                        AlignmentDataset datasetY   = new AlignmentDataset(rawPathY, "", cachey.Msgf);         
       
                        // create a raw file reader for the datasets
                        using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                        {
                            // wrap it in the cached object so we can load scan meta-data
                            RawLoaderCache cacheReaderY             = new RawLoaderCache(readerY);
                            Dictionary<int, ScanSummary> cacheDataY = ScanSummaryCache.ReadCache(cachey.Cache);
                            cacheReaderY.AddCache(0, cacheDataY);
                            readerY.AddDataFile(rawPathY, 0);
                            List<string> names = new List<string>();
                            names.Add(data[i].Cache);
                            names.Add(data[j].Cache);

                            MatchDatasets(  cacheReaderX, 
                                            cacheReaderY, 
                                            datasetX,
                                            datasetY,
                                            names,
                                            writer,
                                            options);
                        }
                    }
                }
            }
        }       

        private void MatchDatasets(ISpectraProvider readerX,
                                    ISpectraProvider readerY,
                                    AlignmentDataset datasetX,
                                    AlignmentDataset datasetY,
                                    List<string> names,
                                    ISpectralAnalysisWriter writer,
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

            peptidesA = peptidesA.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();
            peptidesB = peptidesB.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();


            Dictionary<int, Peptide> peptideMapX = PeptideUtility.MapWithBestScan(peptidesA);
            Dictionary<int, Peptide> peptideMapY = PeptideUtility.MapWithBestScan(peptidesB);

            // Here we find all the matches
            SpectralAnchorPointFinder finder = new SpectralAnchorPointFinder();
            IEnumerable<AnchorPointMatch> matches = null;

            matches = finder.FindAnchorPoints(readerX,
                                                readerY,
                                                comparer,
                                                filter,
                                                options);


            /// Then map the peptide sequences to identify True Positive and False Positives
            PeptideAnchorPointMatcher matcher = new PeptideAnchorPointMatcher();
            matcher.Match(matches,
                            peptideMapX,
                            peptideMapY,
                            options);


            Dictionary<int, ScanSummary> scanDataX = readerX.GetScanData(0);
            Dictionary<int, ScanSummary> scanDataY = readerY.GetScanData(0);


            foreach (var match in matches)
            {
                if (false /*&& match.IsValidMatch == AnchorMatch.PeptideFailed*/ && match.SimilarityScore > .97)
                {
                    if (match.AnchorPointX.Spectrum.Peaks == null)
                    {
                        match.AnchorPointX.Spectrum = SpectralUtilities.GetSpectra(options.MzBinSize,
                                                                        options.TopIonPercent,
                                                                        filter,
                                                                        readerX,
                                                                        scanDataX,
                                                                        match.AnchorPointX.Scan);
                    }
                    if (match.AnchorPointY.Spectrum.Peaks == null)
                    {
                        match.AnchorPointY.Spectrum = SpectralUtilities.GetSpectra(options.MzBinSize,
                                                                        options.TopIonPercent,
                                                                        filter,
                                                                         readerY,
                                                                        scanDataY,
                                                                        match.AnchorPointY.Scan);
                    }
                    DisplayComparisonPlot(match.AnchorPointX.Spectrum, match.AnchorPointY.Spectrum, options.MzBinSize,
                    newTitle:
                        string.Format("{2} - {0} - {1}", match.AnchorPointX.Peptide, 
                                                         match.AnchorPointY.Peptide,
                                                         match.IsValidMatch));

                }
            }


            // Package the data
            SpectralAnalysis analysis = new SpectralAnalysis();
            analysis.Options = options;
            analysis.Matches = matches;
            analysis.DatasetNames = names;

            // Write the results
            writer.Write(analysis);
        }
        #endregion

        #region How many matches can we make with peptides

        [Test(Description = "Figure 1: Compares all spectra (N x N) computing true / false positives based on peptide sequence.")]
        [TestCase(
                   // @"M:\data\proteomics\Papers\AlignmentPaper\data\Shewanella\ConstantPressure\TechReplicates-00\test",
                    @"M:\doc\papers\paperAlignment\Data\figure1\Test",                    
                    SpectralComparison.CosineDotProduct,
                    .5,     // mz bin size when retrieving spectra
                    1,    // m/z
                    .25,    // NET
                    0,      // Similarity Cutoff Score
                    1,  // MSGF+ Score
                    .01,     // Peptide FDR
                    .8)]    // Ion Percent
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
            string[] msgfFiles  = Directory.GetFiles(directory, "*_msgfdb_fht.txt");

            Console.WriteLine("Building data cache");
            Dictionary<string, PathCache> map = new Dictionary<string, PathCache>();
            foreach (var path in cacheFiles)            
                map.Add(path.ToLower(), null);

            List<PathCache> data = new List<PathCache>();
            foreach (var path in msgfFiles)
            {
                string name         = path.ToLower().Replace("_msgfdb_fht.txt", ".mscache");
                string newName      = Path.Combine(directory, name);
                string featureName  = path.ToLower().Replace("_msgfdb_fht.txt", "_isos.csv");
                string features     = Path.Combine(directory, name);

                if (map.ContainsKey(newName))
                {
                    PathCache cacheData = new PathCache();
                    cacheData.Cache     = newName;
                    cacheData.Msgf      = path;
                    cacheData.Features  = features; 
                    data.Add(cacheData);                    
                }
            }

            // The options for the analysis 
            SpectralOptions options     = new SpectralOptions();
            options.MzBinSize           = mzBinSize;
            options.MzTolerance         = mzTolerance;
            options.NetTolerance        = netTolerance;
            options.SimilarityCutoff    = similarityScoreCutoff;
            options.TopIonPercent       = ionPercent;
            options.IdScore             = peptideScore;
            options.ComparerType        = comparerType;
            options.Fdr                 = peptideFdr;
            
            Console.WriteLine("Data: {0}", data.Count);

            for (int i = 0; i < data.Count; i++)
            {
                PathCache cachex            = data[i];            
                // Get the raw path stored in the cache file...
                // then get the dataset object 
                string rawPathX             = ScanSummaryCache.ReadPath(cachex.Cache);                
                AlignmentDataset datasetX   = new AlignmentDataset(rawPathX, "", cachex.Msgf);         
            
                Dictionary<int, ScanSummary> scanDataX  = ScanSummaryCache.ReadCache(cachex.Cache);

                for (int j = i + 1; j < data.Count; j++)                
                {
                        PathCache cachey            = data[j];


                        Dictionary<int, ScanSummary> scanDataY = ScanSummaryCache.ReadCache(cachey.Cache);

                        // Get the raw path stored in the cache file...
                        // then get the dataset object 
                        string rawPathY             = ScanSummaryCache.ReadPath(cachey.Cache);                                        
                        AlignmentDataset datasetY   = new AlignmentDataset(rawPathY, "", cachey.Msgf);         
       
                        List<string> names = new List<string>();
                        names.Add(data[i].Cache);
                        names.Add(data[j].Cache);
                        MatchPeptides(datasetX, datasetY, scanDataX, scanDataY, names, options);              
                }
            }
        }
        private void MatchPeptides(AlignmentDataset datasetX,
                                   AlignmentDataset datasetY,
                                    Dictionary<int, ScanSummary> scanDataX  ,
                                    Dictionary<int, ScanSummary> scanDataY  ,
                                   List<string> names,
                                   SpectralOptions options)
        {
            // Read data for peptides 
            ISequenceFileReader reader     = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            IEnumerable<Peptide> peptidesA = reader.Read(datasetX.PeptideFile);
            IEnumerable<Peptide> peptidesB = reader.Read(datasetY.PeptideFile);

            peptidesA = peptidesA.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();
            peptidesB = peptidesB.ToList().Where(x => PeptideUtility.PassesCutoff(x, options.IdScore, options.Fdr)).ToList();

            Dictionary<int, Peptide> peptideMapX = PeptideUtility.MapWithBestScan(peptidesA);
            Dictionary<int, Peptide> peptideMapY = PeptideUtility.MapWithBestScan(peptidesB);

            // Determine the scan extrema
            var maxX = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            var minX = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
            var maxY = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
            var minY = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;            

            /// Then map the peptide sequences to identify True Positive and False Positives
            /// 
            int count = 0;
            foreach (var scanx in peptideMapX.Keys)
            {
                Peptide peptideX = peptideMapX[scanx];

                foreach (var scany in peptideMapY.Keys)
                {
                    Peptide peptideY = peptideMapY[scany];
                    double netX      = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
                    double netY      = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);
                    double net       = Convert.ToDouble(netX - netY);

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
            foreach (var name in names)
            {
                Console.WriteLine(name);
            }
            Console.WriteLine("Matches - {0}", count);
        }
        #endregion

        #region Figure 2
        /// <summary>
        /// Tests distributions using the peptide match file (uniqued matches for building error distributions)
        /// </summary>
        /// <param name="rawPathX"></param>
        /// <param name="rawPathY"></param>
        /// <param name="peptideMatches"></param>
        /// <param name="comparerType"></param>
        /// <param name="mzTolerance"></param>
        [Test(Description = "Figure 2: Computes the NET / MASS Error Distributions for pre-post ")]
        [TestCase(  @"QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32",
                    @"QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32",
                    SpectralComparison.CosineDotProduct,
                    .5,     // m/z
                    .15,    // NET          
                    .6,     // SSM similarity score,
                    1e-15,  // MSGF+ Score
                    .1,     // Peptide FDR
                    .5)]    // Ion Percent
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
            AlignmentDataset datasetX       = new AlignmentDataset(m_basePath, rawNameX);
            AlignmentDataset datasetY       = new AlignmentDataset(m_basePath, rawNameY);
            
            // The options for the analysis 
            SpectralOptions options         = new SpectralOptions();
            options.MzTolerance             = mzTolerance;
            options.NetTolerance            = netTolerance;
            options.SimilarityCutoff        = similarityScoreCutoff;
            options.TopIonPercent           = ionPercent;
            options.IdScore                 = peptideScore;
            options.ComparerType            = comparerType;

            // Then the writer for creating a report
            ISpectralAnalysisWriter writer = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure2, "Figure2");


            // Create an action to load and align the feature data.
            AlignedFeatureData features = new AlignedFeatureData();
            FeatureAligner aligner      = new FeatureAligner();
            FeatureLoader  loader       = new FeatureLoader();
            Action loadingAction = delegate()
                {
                             features.Baseline = loader.LoadFeatures(datasetX.FeatureFile);
                             features.Alignee  = loader.LoadFeatures(datasetY.FeatureFile);
                             features = aligner.AlignFeatures(features.Baseline,
                                                              features.Alignee,
                                                              options);              
                 };


            // Creates an action to load the peptide data on a separate task
            /// Find the anchor point matches based on peptide sequences
            ISequenceFileReader reader = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            IEnumerable<AnchorPointMatch> peptideMatches = null;
            Action peptideLoadingAction = delegate()
                    {
                        IEnumerable<Peptide> peptidesX = reader.Read(datasetX.PeptideFile);
                        IEnumerable<Peptide> peptidesY = reader.Read(datasetY.PeptideFile);

                        peptidesX = peptidesX.Where(x => x.Score < options.IdScore);
                        peptidesY = peptidesY.Where(x => x.Score < options.IdScore);

                        /// Then map the peptide sequences to identify True Positive and False Positives
                        PeptideAnchorPointFinder peptideFinder = new PeptideAnchorPointFinder();
                        peptideMatches = peptideFinder.FindAnchorPoints(peptidesX,
                                                                        peptidesY,
                                                                        options);

                    };

            Task loadingTask        = new Task(loadingAction);
            loadingTask.Start();

            Task peptideLoadingTask = new Task(peptideLoadingAction);
            peptideLoadingTask.Start();


            /// Find the anchor point matches based on SSM
            ISpectralComparer           comparer            = SpectralComparerFactory.CreateSpectraComparer(options.ComparerType);
            ISpectraFilter              filter              = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);            
            SpectralAnchorPointFinder   spectralFinder      = new SpectralAnchorPointFinder();
            IEnumerable<AnchorPointMatch> spectralMatches   = null;
            using (ISpectraProvider readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
            {
                using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                {
                    readerX.AddDataFile(datasetX.RawFile, 0);
                    readerY.AddDataFile(datasetY.RawFile, 0);

                    spectralMatches = spectralFinder.FindAnchorPoints(  readerX,
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
            SpectralAnalysis analysisSpectra    = new SpectralAnalysis();
            analysisSpectra.Options             = options;
            analysisSpectra.Matches             = spectralMatches;
                                    
            // Package the data
            SpectralAnalysis analysisPeptides   = new SpectralAnalysis();
            analysisSpectra.Options             = options;
            analysisSpectra.Matches             = peptideMatches;

            // Write the results
            writer.Write(analysisSpectra);
            writer.Write(analysisPeptides);
        }        
        #endregion            
    }    
}
