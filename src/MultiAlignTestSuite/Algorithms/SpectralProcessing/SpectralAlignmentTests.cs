using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PNNLOmics.Data.Features;
using System.IO;
using PNNLOmics.Algorithms.Distance;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using PNNLOmicsIO.IO;
using MultiAlignCore.IO.Features;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System.Windows.Forms;
using System.Drawing;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms;
using PNNLOmics.Algorithms.FeatureMatcher.MSnLinker;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Data;


namespace MultiAlignTestSuite.Algorithms
{
    
    

    [TestFixture]
    public class SpectralAlignmentTests
    {
        /// <summary>
        /// Maps the path to a group ID for reading multiple files.
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
            List<XYData> peaks  = reader.GetRawSpectra(scan, group, 2);            
            MSSpectra spectrum  = new MSSpectra();
            spectrum.Peaks      = peaks;

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
                
            var series   = new StemSeries();
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

            Plot plot   = new Plot();
            plot.Model  = model;            
            Form form   = new Form();
            form.Size   = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;
            plot.Dock   = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(plot);
            form.ShowDialog();

            using (Bitmap bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, form.DisplayRectangle);
                bitmap.Save(path);
            }
        }
        #endregion

        #region Single Comparison Displays
        [Test(Description = "Compares two spectra against each other.")]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 3726,
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 3541,
                    .5,
                    Ignore = false)]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 3726,
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 7413,
                    1,
                    Ignore = false)]
        public void DisplayComparisonFigure(string pathX, int scanX, string pathY, int scanY, double mzTolerance)
        {            
            MSSpectra spectrumX = GetSpectrum(pathX, scanX);
            MSSpectra spectrumY = GetSpectrum(pathY, scanY);

            string path = Path.GetDirectoryName(pathX);
            string pathCompareImage = Path.Combine(path, string.Format("comparison-{0}-{1}.png", scanX, scanY));
                        
            spectrumX.Peaks = XYData.Bin(spectrumX.Peaks,
                                                0,
                                                2000,
                                                mzTolerance);

            spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
                                                0,
                                                2000,
                                                mzTolerance);

            DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, path=pathCompareImage);
        }
        [Test(Description = "Compares two spectra against each other.")]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",   3726,
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 3541,
                    SpectralComparison.NormalizedDotProduct,
                    .5, 
                    Ignore=true)]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",   15304,
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 17614,
                    SpectralComparison.NormalizedDotProduct,
                    .5,
                    Ignore = true)]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 3726,
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 3541,
                    SpectralComparison.PeakCounts,
                    SpectraFilters.TopPercent,
                    .8,
                    1,
                    Ignore = false)]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 3726,
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 7413,
                    SpectralComparison.PeakCounts,
                    SpectraFilters.TopPercent,
                    .8,
                    1,
                    Ignore = false)]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 3726,
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 3541,
                    SpectralComparison.CosineDotProduct,
                    SpectraFilters.TopPercent,
                    .8,
                    1,
                    Ignore = false)]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW", 3726,
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW", 7413,
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
            MSSpectra spectrumX = GetSpectrum(pathX, scanX);
            MSSpectra spectrumY = GetSpectrum(pathY, scanY);            
            ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType, percent: percent);
            
            ISpectraFilter filter   = SpectrumFilterFactory.CreateFilter(filterType);
            spectrumX               = filter.Threshold(spectrumX, percent);
            spectrumY               = filter.Threshold(spectrumY, percent);


            spectrumX.Peaks = XYData.Bin(spectrumX.Peaks,
                                                0,
                                                2000,
                                                mzTolerance);

            spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
                                                0,
                                                2000,
                                                mzTolerance);
                

            double value = comparer.CompareSpectra(spectrumX, spectrumY);

            string path             = Path.GetDirectoryName(pathX);
            string plotTitle        = string.Format("comparison-{2}-{3}-{0}-{1}_{4:0.000}", scanX, scanY, comparerType, percent, value);
            string pathCompareImage = Path.Combine(path, plotTitle + ".png");

            DisplayComparisonPlot(spectrumX, spectrumY, mzTolerance, path: pathCompareImage, newTitle: plotTitle);
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
            List<string> lines = File.ReadAllLines(path).ToList();

            List<PeptideMatch> matches = new List<PeptideMatch>();
            foreach (string line in lines)
            {
                string[] data = line.Split(',');
                if (data.Length < 6)
                {
                    continue;
                }

                try
                {
                    PeptideMatch match = new PeptideMatch();
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
        /// Creates a dictionary for scan to peptide match
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Dictionary<int, Dictionary<int, PeptideMatch>> CreateMatches(List<PeptideMatch> matches, int type)
        {
            Dictionary<int, Dictionary<int, PeptideMatch>> matchesX = new Dictionary<int, Dictionary<int, PeptideMatch>>();
            foreach (PeptideMatch match in matches)
            {
                int scanx = match.ScanX;
                int scany = match.ScanY;
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
        /// Reads a peptide file.
        /// </summary>
        /// <param name="peptidePathX"></param>
        /// <returns></returns>
        private Dictionary<int, PeptideTest> ReadPeptideFile(string peptidePath)
        {
            Dictionary<int, PeptideTest> peptideMap = new Dictionary<int, PeptideTest>();
            string[] lines                      = File.ReadAllLines(peptidePath);

            string header = lines[0];
            string [] headerData = header.Split('\t');

            int scanIndex       = 0;
            int peptideIndex    = 0;
            int fdrIndex        = 0;
            int scoreIndex      = 0;

            int i = 0;
            foreach(string x in headerData)
            {
                switch(x.ToLower())
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
                string line     = lines[i];
                string [] data  = line.Split('\t');
                string pep      = data[peptideIndex];
                int scan        = Convert.ToInt32(data[scanIndex]);
                double score    = Convert.ToDouble(data[scoreIndex]);
                double fdr      = Convert.ToDouble(data[fdrIndex]);

                PeptideTest p = new PeptideTest();
                p.Sequence   = pep;
                p.Score     = score;
                p.FDR       = fdr; 

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
            string[] peptides = peptide.Split('.');

            if (peptides.Length > 2)
            {
                return peptides[1];
            }
            return peptides[0];
        }
        #endregion

        #region Filtering
        

        private bool PassesCutoff(PeptideTest peptidex, PeptideTest peptidey, double score, double fdr)
        {         
            bool passes = true;

            if (peptidex == null || peptidey == null)
                return false;

            if (peptidey.FDR > fdr) return false;
            if (peptidey.FDR > fdr) return false;
            if (peptidex.Score > score) return false;
            if (peptidey.Score > score) return false;            
            return passes;
        }
        #endregion

        private Tuple<List<UMCLight>, List<MSFeatureLight>> LoadFeatures(string path)
        {
            MultiAlignCore.Data.DatasetInformation info = new MultiAlignCore.Data.DatasetInformation();
            info.Features = new MultiAlignCore.IO.InputFiles.InputFile();
            info.Features.Path = path;

            // Load the MS Feature Data
            List<MSFeatureLight> msFeatures = MultiAlignCore.IO.Features.UMCLoaderFactory.LoadMsFeatureData(info, null);
            List<UMCLight> features = new List<UMCLight>();

            // Find the features 
            IFeatureFinder finder               = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.DeconToolsCSV);            
            LCMSFeatureFindingOptions options   = new LCMSFeatureFindingOptions();
            features                            = finder.FindFeatures(msFeatures, options);
            
            return new Tuple<List<UMCLight>,List<MSFeatureLight>>(features, msFeatures);
        }

        private void CreateMatches()
        {
        }

        #region p-value
        /// <summary>
        /// Tests distributions using the peptide match file (uniqued matches for building error distributions)
        /// </summary>
        /// <param name="pathX"></param>
        /// <param name="pathY"></param>
        /// <param name="peptideMatches"></param>
        /// <param name="comparerType"></param>
        /// <param name="mzTolerance"></param>
        [Test(Description = "Compares two spectra against each other.")]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
                    SpectralComparison.DotProduct,
                    .5,
                    .25,
                    1,
                    .7,
                    .01,
                    .5, Ignore = true)]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
                    SpectralComparison.PeakCounts,
                    .5,
                    .25,
                    1,
                    .7,
                    .01,
                    .5
                    , Ignore = true)]
        [TestCase(@"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
                    @"M:\data\proteomics\MsMsAlignment\data\Shewanella\ConstantPressure\TechReplicates-00\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
                    SpectralComparison.CosineDotProduct,
                    .5,
                    .25,
                    1,
                    .2,
                    .1,
                    .5, Ignore = false)]
        public void TestPValue(string pathX,
                                                        string pathY,
                                                        string peptidePathX,
                                                        string peptidePathY,
                                                        SpectralComparison comparerType,
                                                        double mzTolerance,
                                                        double netTolerance,
                                                        double score,
                                                        double matchScore,
                                                        double fdr,
                                                        double percent)
        {

            Console.WriteLine("{2}, Test: {0}\tcompared to\t{1}", pathX, pathY, comparerType);

            string featureFileX = Path.GetFileNameWithoutExtension(pathX);
            string featureFileY = Path.GetFileNameWithoutExtension(pathY);

            featureFileX = Path.Combine(Path.GetDirectoryName(pathX), featureFileX + "_isos.csv");
            featureFileY = Path.Combine(Path.GetDirectoryName(pathY), featureFileY + "_isos.csv");

            // Load the feature files
            Tuple<List<UMCLight>, List<MSFeatureLight>> featuresTupleX = LoadFeatures(featureFileX);
            Tuple<List<UMCLight>, List<MSFeatureLight>> featuresTupleY = LoadFeatures(featureFileY);

            // Then align the samples
            AlgorithmBuilder builder   = new AlgorithmBuilder();
            builder.BuildAligner();

            // Use the default settings for now
            AnalysisOptions options     = new AnalysisOptions();
            AlgorithmProvider provider  = builder.GetAlgorithmProvider(options);
            IFeatureAligner aligner     = provider.Aligner;
            aligner.AlignFeatures(  featuresTupleX.Item1, 
                                    featuresTupleY.Item1, 
                                    options.AlignmentOptions);

            // at this point we still have to link these data...
            
            
            Dictionary<int, PeptideTest> peptideMapX = new Dictionary<int, PeptideTest>();
            Dictionary<int, PeptideTest> peptideMapY = new Dictionary<int, PeptideTest>();

            peptideMapX = ReadPeptideFile(peptidePathX);
            peptideMapY = ReadPeptideFile(peptidePathY);

            // This guy filters the spectra, so that we only keep the N most intense ions for comparison
            ISpectraFilter filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

            // These track our matched and not matched 
            double step = .05;
            int N = Convert.ToInt32(1.0 / step);
            SpectralAnalysis analysis = new SpectralAnalysis(step);
            analysis.NetTolerance     = netTolerance;
            analysis.MassTolerance    = mzTolerance;

            // These arrays are for linking to MS features later.            
            Dictionary<int, PeptideMatch> spectraY = new Dictionary<int, PeptideMatch>();
            Dictionary<int, PeptideMatch> spectraX = new Dictionary<int, PeptideMatch>();

            using (ISpectraProvider readerX = new ThermoRawDataFileReader())
            {

                using (ISpectraProvider readerY = new ThermoRawDataFileReader())
                {
                    readerX.AddDataFile(pathX, 0);
                    readerY.AddDataFile(pathY, 0);

                    Dictionary<int, ScanSummary> scanDataX = readerX.GetScanData(0);
                    Dictionary<int, ScanSummary> scanDataY = readerY.GetScanData(0);

                    // Determine the scan maxes
                    var maxX = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
                    var minX = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
                    var maxY = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
                    var minY = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
                    int Nx   = scanDataX.Count;
                    int Ny   = scanDataX.Count;

                    ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType, percent: percent);
                    List<double> scans          = new List<double>();
                    Dictionary<int, MSSpectra> spectraMap = new Dictionary<int, MSSpectra>();

                    /// Horrible way to make a histogram...but this will make it for our matches.
                    Dictionary<int, int> netErrorHistogram = new Dictionary<int, int>();
                    double start    = -.05;
                    double max      = .05;
                    double width    = max - start;
                    double stepNet  = .005;
                    double netValue = start;
                    int i           = 0;

                    while (netValue <= max)
                    {
                        netErrorHistogram.Add(i++, 0);
                        netValue += stepNet;
                    }

                    foreach (int scanx in scanDataX.Keys)
                    {
                        ScanSummary xsum = scanDataX[scanx];

                        if (xsum.MsLevel != 2)
                            continue;

                        // Grab the first spectra
                        MSSpectra spectrumX = GetSpectra(mzTolerance, percent, filter, readerX, scanDataX, scanx);

                        // Iterate through the other analysis.  
                        foreach (int scany in scanDataY.Keys)
                        {
                            ScanSummary ysum = scanDataY[scany];
                            if (ysum.MsLevel != 2)
                                continue;

                            if (Math.Abs(xsum.PrecursorMZ - ysum.PrecursorMZ) >= mzTolerance)                            
                                continue;
                            

                            // Grab the first spectra...if we have it, great dont re-read
                            MSSpectra spectrumY = null;
                            if (spectraMap.ContainsKey(scany))
                            {
                                spectrumY = spectraMap[scany];
                            }
                            else
                            {
                                spectrumY = GetSpectra(mzTolerance, percent, filter, readerY, scanDataY, scany);
                                spectraMap.Add(scany, spectrumY);
                            }

                            // compare the spectra
                            double value = comparer.CompareSpectra(spectrumX, spectrumY);
                            if (double.IsNaN(value))
                            {
                                continue;
                            }

                            bool isMatch         = false;
                            PeptideTest peptidex = null;
                            PeptideTest peptidey = null;

                            if (peptideMapX.ContainsKey(scanx))
                                peptidex = peptideMapX[scanx];

                            if (peptideMapY.ContainsKey(scany))
                                peptidey = peptideMapY[scany];

                            if (peptidex != null && peptidey != null)
                            {
                                peptidex.Sequence = CleanString(peptidex.Sequence);
                                peptidey.Sequence = CleanString(peptidey.Sequence);

                                if (peptidex.Sequence.Equals(peptidey.Sequence) && !string.IsNullOrWhiteSpace(peptidey.Sequence))
                                {
                                    isMatch = true;
                                }
                            }


                            bool passesCutoff = PassesCutoff(peptidex, peptidey, score, fdr);
                            if (!passesCutoff)
                                continue;

                            if (value < matchScore)
                                continue;

                            if (double.IsNaN(value) || double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value))
                                continue;

                            // Here we determine what distribution the match belongs to.  We iterate over a number of NET tolerances
                            int index = 0;

                            try
                            {
                                index = Convert.ToInt32(value / step);
                            }
                            catch (OverflowException ex)
                            {
                                int xx = 9;
                                if (xx > 10)
                                {
                                }
                            }

                            double netX = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
                            double netY = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);
                            double net = Convert.ToDouble(netX - netY);

                            PeptideMatch match  = new PeptideMatch();
                            match.NetX          = netX;
                            match.NetY          = netY;
                            match.MzX           = xsum.PrecursorMZ;
                            match.MzY           = ysum.PrecursorMZ;
                            match.ScanX         = scanx;
                            match.ScanY         = scany;
                            match.Similarity    = value;
                            match.Index         = index;
                            match.IsMatch       = isMatch;
                            

                            if (analysis.NetTolerance >= net)
                            {
                                analysis.AddMatch(match);

                                if (isMatch)
                                {
                                    spectrumY.PrecursorMZ   = match.MzY;
                                    spectrumY.Scan          = match.ScanY;

                                    spectrumX.PrecursorMZ   = match.MzX;
                                    spectrumX.Scan          = match.ScanX;

                                    analysis.AddTrueMatch(index);
                                    match.SpectrumX = spectrumX;
                                    match.SpectrumY = spectrumY;

                                    // Here we track what spectra have been seen before...should only be once!
                                    bool doesKeyExist = spectraY.ContainsKey(scany);
                                    if (!doesKeyExist) 
                                        spectraY.Add(scany, match);

                                }
                                else
                                {                                 
                                    analysis.AddFalseMatch(index);
                                }
                            }
                        }
                    }
                }
            }

            int jj = 0;
            // Now we determine what features are aligned with what.
            List<MSSpectra> spectra = new List<MSSpectra>();
            List<MSSpectra> matchedY = new List<MSSpectra>();            
            foreach (int key in spectraY.Keys)
            {
                spectraY[key].SpectrumY.ID = jj;
                spectraY[key].SpectrumX.ID = jj++;

                spectra.Add(spectraY[key].SpectrumY);
                matchedY.Add(spectraY[key].SpectrumX);
            }

            // So we first link the spectra to the MS Features (mono masses)
            IMSnLinker linker       = MSnLinkerFactory.CreateLinker(MSnLinkerType.BoxMethod);
            linker.Tolerances.Mass  = .05;

            Dictionary<int, int> mappedxx =  linker.LinkMSFeaturesToMSn(featuresTupleY.Item2, spectra);
            Dictionary<int, int> mappedyy = linker.LinkMSFeaturesToMSn(featuresTupleX.Item2, matchedY);

            bool hasItworked = false;
            hasItworked = true;
            hasItworked = false;
            if (hasItworked)
            {
                featuresTupleX.Item2.ForEach(X => X.MSnSpectra.Clear());
                featuresTupleY.Item2.ForEach(X => X.MSnSpectra.Clear());
            }

            // Then we iterate through and find anything with MS/MS ...we should just directly do this above...but we are 
            // retrofitting here...
            Dictionary<int, UMCLight> mappedFeaturesX = new Dictionary<int, UMCLight>();
            Dictionary<int, UMCLight> mappedFeaturesY = new Dictionary<int, UMCLight>();

            // Baseline featur
            Dictionary<int, UMCLight> featureMap = new Dictionary<int, UMCLight>();
            foreach (PeptideMatch match in spectraY.Values)
            {
                if (match.SpectrumX.ParentFeature != null && match.SpectrumY.ParentFeature != null)
                {
                    UMCLight featurex = match.SpectrumX.ParentFeature.ParentFeature;
                    UMCLight featurey = match.SpectrumY.ParentFeature.ParentFeature;

                    analysis.PreAlignment.Add(featurex.NET - featurey.NET);
                    analysis.PostAlignment.Add(featurex.NET - featurey.NETAligned);
                }
            }

         

            analysis.Write();
            Console.WriteLine();
        }

        private MSSpectra GetSpectra(double mzTolerance,
                                    double percent,
                                    ISpectraFilter filter, 
                                    ISpectraProvider readerY, 
                                    Dictionary<int, ScanSummary> scanDataY,
                                    int scany)
        {
            MSSpectra spectrumY = GetSpectrum(readerY,
                                        scanDataY[scany].Scan,
                                        0,
                                        mzTolerance: mzTolerance);
            spectrumY = filter.Threshold(spectrumY, percent);
            spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
                                                0,
                                                2000,
                                                mzTolerance);
            return spectrumY;
        }
        #endregion
    }    
}
