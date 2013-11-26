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
                    .5,     // m/z
                    .25,    // NET
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
            AlignmentDataset datasetY       = new AlignmentDataset(m_basePath, rawNameX);
            
            // Then the writer for creating a report
            ISpectralAnalysisWriter writer  = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure1);

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
            SpectralAnchorPointFinder finder                = new SpectralAnchorPointFinder();

            IEnumerable<AnchorPointMatch> matches   = null;
            using (ISpectraProvider readerX = RawLoaderFactory.CreateFileReader(datasetX.RawFile))
            {
                using (ISpectraProvider readerY = RawLoaderFactory.CreateFileReader(datasetY.RawFile))
                {
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
            AlignmentDataset datasetY       = new AlignmentDataset(m_basePath, rawNameX);
            
            // The options for the analysis 
            SpectralOptions options         = new SpectralOptions();
            options.MzTolerance             = mzTolerance;
            options.NetTolerance            = netTolerance;
            options.SimilarityCutoff        = similarityScoreCutoff;
            options.TopIonPercent           = ionPercent;
            options.IdScore                 = peptideScore;
            options.ComparerType            = comparerType;

            // Then the writer for creating a report
            ISpectralAnalysisWriter writer = AlignmentAnalysisWriterFactory.Create(AlignmentFigureType.Figure1);


            // Create an action to load and align the feature data.
            AlignedFeatureData features = null;
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
                        IEnumerable<Peptide> peptidesA = reader.Read(datasetX.PeptideFile);
                        IEnumerable<Peptide> peptidesB = reader.Read(datasetY.PeptideFile);

                        peptidesA = peptidesA.Where(x => x.Score < options.IdScore);
                        peptidesA = peptidesA.Where(x => x.Score < options.IdScore);

                        /// Then map the peptide sequences to identify True Positive and False Positives
                        PeptideAnchorPointFinder peptideFinder = new PeptideAnchorPointFinder();
                        peptideMatches = peptideFinder.FindAnchorPoints(peptidesA,
                                                                                                        peptidesB,
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
