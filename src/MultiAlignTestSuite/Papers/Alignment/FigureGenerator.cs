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

namespace MultiAlignTestSuite.Papers.Alignment.SSM
{

    /// <summary>
    /// Creates all figures used in the alignment paper
    /// </summary>
    [TestFixture]
    public class FigureGenerator 
    {
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
            public string Cache;
            public string Msgf;
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
        [TestCase(@"M:\doc\papers\paperAlignment\Data\figure1\Test",
                    //SpectralComparison.CosineDotProduct,    
                    SpectralComparison.NormalizedDotProduct,
                    .5,     // mz bin size when retrieving spectra
                    .05,    // m/z
                    .25,    // NET
                    0,      // Similarity Cutoff Score
                    1e-3,  // MSGF+ Score
                    .1,     // Peptide FDR
                    .5)]    // Ion Percent
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
                            MatchDatasets(cacheReaderX, cacheReaderY, datasetX, datasetY, options);
                        }
                    }
                }
            }
        }

        private void MatchDatasets( ISpectraProvider readerX,
                                    ISpectraProvider readerY, 
                                    AlignmentDataset datasetX, 
                                    AlignmentDataset datasetY, 
                                    SpectralOptions options)
        {            
            // Then the writer for creating a report
            ISpectralAnalysisWriter writer = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure1, "results-figure1-largeStatistics");

            // This helps us compare various comparison calculation methods
            ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(options.ComparerType);

            // This guy filters the spectra, so that we only keep the N most intense ions for comparison
            ISpectraFilter filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

            // Here we find all the matches
            SpectralAnchorPointFinder finder        = new SpectralAnchorPointFinder();
            IEnumerable<AnchorPointMatch> matches   = null;
                               
            matches = finder.FindAnchorPoints(  readerX,
                                                readerY,
                                                comparer,
                                                filter,
                                                options);

            // Read data for peptides 
            ISequenceFileReader reader           = PeptideReaderFactory.CreateReader(SequenceFileType.MSGF);
            IEnumerable<Peptide> peptidesA       = reader.Read(datasetX.PeptideFile);
            IEnumerable<Peptide> peptidesB       = reader.Read(datasetY.PeptideFile);
            Dictionary<int, Peptide> peptideMapX = PeptideUtility.MapWithBestScan(peptidesA);
            Dictionary<int, Peptide> peptideMapY = PeptideUtility.MapWithBestScan(peptidesB);

            /// Then map the peptide sequences to identify True Positive and False Positives
            PeptideAnchorPointMatcher matcher = new PeptideAnchorPointMatcher();
            matcher.Match(matches,
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
