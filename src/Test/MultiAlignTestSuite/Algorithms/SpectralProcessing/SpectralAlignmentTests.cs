namespace MultiAlignTestSuite.Algorithms.SpectralProcessing
{
//    [TestFixture]
//    public class SpectralAlignmentTests: BaseSpectralAlignmentTest
//    {
//        /// <summary>
//        /// Maps the path to a group ID for reading multiple files.
//        /// </summary>
//        private Dictionary<string, int> m_pathMap = new Dictionary<string, int>();
//        private Tuple<List<UMCLight>, List<MSFeatureLight>> LoadFeatures(string path)
//        {
//            MultiAlignCore.Data.DatasetInformation info = new MultiAlignCore.Data.DatasetInformation();
//            info.Features = new MultiAlignCore.IO.InputFiles.InputFile();
//            info.Features.Path = path;

//            // Load the MS Feature Data
//            List<MSFeatureLight> msFeatures = MultiAlignCore.IO.Features.UMCLoaderFactory.LoadMsFeatureData(info, null);
//            List<UMCLight> features = new List<UMCLight>();

//            // Find the features
//            IFeatureFinder finder               = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.DeconToolsCSV);
//            LCMSFeatureFindingOptions options   = new LCMSFeatureFindingOptions();
//            features                            = finder.FindFeatures(msFeatures, options);

//            return new Tuple<List<UMCLight>,List<MSFeatureLight>>(features, msFeatures);
//        }

//        #region p-value
//        /// <summary>
//        /// Tests distributions using the peptide match file (uniqued matches for building error distributions)
//        /// </summary>
//        /// <param name="pathX"></param>
//        /// <param name="pathY"></param>
//        /// <param name="peptideMatches"></param>
//        /// <param name="comparerType"></param>
//        /// <param name="mzTolerance"></param>
//        [Test(Description = "Compares two spectra against each other.")]
//        [TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
//                    @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
//                    @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
//                    @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
//                    SpectralComparison.DotProduct,
//                    .5,
//                    .25,
//                    1,
//                    .7,
//                    .01,
//                    .5, Ignore = "true")]
//        [TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
//                    @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
//                    @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
//                    @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
//                    SpectralComparison.PeakCounts,
//                    .5,
//                    .25,
//                    1,
//                    .7,
//                    .01,
//                    .5
//                    , Ignore = "true")]
//        [TestCase(@"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32.RAW",
//                    @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32.RAW",
//                    @"Data\QC_SHEW\QC_Shew_11_06-pt5_1_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
//                    @"Data\QC_SHEW\QC_Shew_11_06-pt5_5_11Jun12_Falcon_12-03-32_msgfdb_fht.txt",
//                    SpectralComparison.CosineDotProduct,
//                    .5,
//                    .25,
//                    1,
//                    .2,
//                    .1,
//                    .5, Ignore = false)]
//        public void TestPValue(string pathX,
//                                                        string pathY,
//                                                        string peptidePathX,
//                                                        string peptidePathY,
//                                                        SpectralComparison comparerType,
//                                                        double mzTolerance,
//                                                        double netTolerance,
//                                                        double score,
//                                                        double matchScore,
//                                                        double fdr,
//                                                        double percent)
//        {

//            Console.WriteLine("{2}, Test: {0}\tcompared to\t{1}", pathX, pathY, comparerType);

//            string featureFileX = Path.GetFileNameWithoutExtension(pathX);
//            string featureFileY = Path.GetFileNameWithoutExtension(pathY);

//            featureFileX = Path.Combine(Path.GetDirectoryName(pathX), featureFileX + "_isos.csv");
//            featureFileY = Path.Combine(Path.GetDirectoryName(pathY), featureFileY + "_isos.csv");

//            // Load the feature files
//            Tuple<List<UMCLight>, List<MSFeatureLight>> featuresTupleX = LoadFeatures(featureFileX);
//            Tuple<List<UMCLight>, List<MSFeatureLight>> featuresTupleY = LoadFeatures(featureFileY);

//            // Then align the samples
//            AlgorithmBuilder builder   = new AlgorithmBuilder();
//            builder.BuildAligner();

//            // Use the default settings for now
//            AnalysisOptions options     = new AnalysisOptions();
//            AlgorithmProvider provider  = builder.GetAlgorithmProvider(options);
//            IFeatureAligner aligner     = provider.Aligner;
//            aligner.AlignFeatures(  featuresTupleX.Item1,
//                                    featuresTupleY.Item1,
//                                    options.AlignmentOptions);

//            // at this point we still have to link these data...


//            Dictionary<int, PeptideTest> peptideMapX = new Dictionary<int, PeptideTest>();
//            Dictionary<int, PeptideTest> peptideMapY = new Dictionary<int, PeptideTest>();

//            peptideMapX = ReadPeptideFile(peptidePathX);
//            peptideMapY = ReadPeptideFile(peptidePathY);

//            // This guy filters the spectra, so that we only keep the N most intense ions for comparison
//            ISpectraFilter filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);

//            // These track our matched and not matched
//            double step = .05;
//            int N = Convert.ToInt32(1.0 / step);
//            SpectralAnalysis analysis = new SpectralAnalysis();
//            analysis.NetTolerance     = netTolerance;
//            analysis.MassTolerance    = mzTolerance;

//            // These arrays are for linking to MS features later.
//            Dictionary<int, PeptideMatch> spectraY = new Dictionary<int, PeptideMatch>();
//            Dictionary<int, PeptideMatch> spectraX = new Dictionary<int, PeptideMatch>();

//            using (ISpectraProvider readerX = new ThermoRawDataFileReader())
//            {

//                using (ISpectraProvider readerY = new ThermoRawDataFileReader())
//                {
//                    readerX.AddDataFile(pathX, 0);
//                    readerY.AddDataFile(pathY, 0);

//                    Dictionary<int, ScanSummary> scanDataX = readerX.GetScanData(0);
//                    Dictionary<int, ScanSummary> scanDataY = readerY.GetScanData(0);

//                    // Determine the scan maxes
//                    var maxX = scanDataX.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
//                    var minX = scanDataX.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
//                    var maxY = scanDataY.Aggregate((l, r) => l.Value.Scan > r.Value.Scan ? l : r).Key;
//                    var minY = scanDataY.Aggregate((l, r) => l.Value.Scan < r.Value.Scan ? l : r).Key;
//                    int Nx   = scanDataX.Count;
//                    int Ny   = scanDataX.Count;

//                    ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType, percent: percent);
//                    List<double> scans          = new List<double>();
//                    Dictionary<int, MSSpectra> spectraMap = new Dictionary<int, MSSpectra>();

//                    /// Horrible way to make a histogram...but this will make it for our matches.
//                    Dictionary<int, int> netErrorHistogram = new Dictionary<int, int>();
//                    double start    = -.05;
//                    double max      = .05;
//                    double width    = max - start;
//                    double stepNet  = .005;
//                    double netValue = start;
//                    int i           = 0;

//                    while (netValue <= max)
//                    {
//                        netErrorHistogram.Add(i++, 0);
//                        netValue += stepNet;
//                    }

//                    foreach (int scanx in scanDataX.Keys)
//                    {
//                        ScanSummary xsum = scanDataX[scanx];

//                        if (xsum.MsLevel != 2)
//                            continue;

//                        // Grab the first spectra
//                        MSSpectra spectrumX = GetSpectra(mzTolerance, percent, filter, readerX, scanDataX, scanx);

//                        // Iterate through the other analysis.
//                        foreach (int scany in scanDataY.Keys)
//                        {
//                            ScanSummary ysum = scanDataY[scany];
//                            if (ysum.MsLevel != 2)
//                                continue;

//                            if (Math.Abs(xsum.PrecursorMZ - ysum.PrecursorMZ) >= mzTolerance)
//                                continue;


//                            // Grab the first spectra...if we have it, great dont re-read
//                            MSSpectra spectrumY = null;
//                            if (spectraMap.ContainsKey(scany))
//                            {
//                                spectrumY = spectraMap[scany];
//                            }
//                            else
//                            {
//                                spectrumY = GetSpectra(mzTolerance, percent, filter, readerY, scanDataY, scany);
//                                spectraMap.Add(scany, spectrumY);
//                            }

//                            // compare the spectra
//                            double value = comparer.CompareSpectra(spectrumX, spectrumY);
//                            if (double.IsNaN(value))
//                            {
//                                continue;
//                            }

//                            bool isMatch         = false;
//                            PeptideTest peptidex = null;
//                            PeptideTest peptidey = null;

//                            if (peptideMapX.ContainsKey(scanx))
//                                peptidex = peptideMapX[scanx];

//                            if (peptideMapY.ContainsKey(scany))
//                                peptidey = peptideMapY[scany];

//                            if (peptidex != null && peptidey != null)
//                            {
//                                peptidex.Sequence = PeptideUtility.CleanString(peptidex.Sequence);
//                                peptidey.Sequence = PeptideUtility.CleanString(peptidey.Sequence);

//                                if (peptidex.Sequence.Equals(peptidey.Sequence) && !string.IsNullOrWhiteSpace(peptidey.Sequence))
//                                {
//                                    isMatch = true;
//                                }
//                            }

//                            bool passesCutoff = PeptideUtility.PassesCutoff(peptidex, peptidey, score, fdr);
//                            if (!passesCutoff)
//                                continue;

//                            if (value < matchScore)
//                                continue;

//                            if (double.IsNaN(value) || double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value))
//                                continue;

//                            // Here we determine what distribution the match belongs to.  We iterate over a number of NET tolerances
//                            int index = 0;

//                            try
//                            {
//                                index = Convert.ToInt32(value / step);
//                            }
//                            catch (OverflowException ex)
//                            {
//                                int xx = 9;
//                                if (xx > 10)
//                                {
//                                }
//                            }

//                            double netX = Convert.ToDouble(scanx - minX) / Convert.ToDouble(maxX - minX);
//                            double netY = Convert.ToDouble(scany - minY) / Convert.ToDouble(maxY - minY);
//                            double net = Convert.ToDouble(netX - netY);

//                            PeptideMatch match  = new PeptideMatch();
//                            match.NetX          = netX;
//                            match.NetY          = netY;
//                            match.MzX           = xsum.PrecursorMZ;
//                            match.MzY           = ysum.PrecursorMZ;
//                            match.ScanX         = scanx;
//                            match.ScanY         = scany;
//                            match.Similarity    = value;
//                            match.Index         = index;
//                            match.IsMatch       = isMatch;


//                            if (analysis.NetTolerance >= net)
//                            {
//                                analysis.AddMatch(match);

//                                if (isMatch)
//                                {
//                                    spectrumY.PrecursorMZ   = match.MzY;
//                                    spectrumY.Scan          = match.ScanY;

//                                    spectrumX.PrecursorMZ   = match.MzX;
//                                    spectrumX.Scan          = match.ScanX;

//                                    analysis.AddTrueMatch(index);
//                                    match.SpectrumX = spectrumX;
//                                    match.SpectrumY = spectrumY;

//                                    // Here we track what spectra have been seen before...should only be once!
//                                    bool doesKeyExist = spectraY.ContainsKey(scany);
//                                    if (!doesKeyExist)
//                                        spectraY.Add(scany, match);

//                                }
//                                else
//                                {
//                                    analysis.AddFalseMatch(index);
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            int jj = 0;
//            // Now we determine what features are aligned with what.
//            List<MSSpectra> spectra = new List<MSSpectra>();
//            List<MSSpectra> matchedY = new List<MSSpectra>();
//            foreach (int key in spectraY.Keys)
//            {
//                spectraY[key].SpectrumY.ID = jj;
//                spectraY[key].SpectrumX.ID = jj++;

//                spectra.Add(spectraY[key].SpectrumY);
//                matchedY.Add(spectraY[key].SpectrumX);
//            }

//            // So we first link the spectra to the MS Features (mono masses)
//            IMSnLinker linker       = MSnLinkerFactory.CreateLinker(MSnLinkerType.BoxMethod);
//            linker.Tolerances.Mass  = .05;

//            Dictionary<int, int> mappedxx =  linker.LinkMSFeaturesToMSn(featuresTupleY.Item2, spectra);
//            Dictionary<int, int> mappedyy = linker.LinkMSFeaturesToMSn(featuresTupleX.Item2, matchedY);

//            bool hasItworked = false;
//            hasItworked = true;
//            hasItworked = false;
//            if (hasItworked)
//            {
//                featuresTupleX.Item2.ForEach(X => X.MSnSpectra.Clear());
//                featuresTupleY.Item2.ForEach(X => X.MSnSpectra.Clear());
//            }

//            // Then we iterate through and find anything with MS/MS ...we should just directly do this above...but we are
//            // retrofitting here...
//            Dictionary<int, UMCLight> mappedFeaturesX = new Dictionary<int, UMCLight>();
//            Dictionary<int, UMCLight> mappedFeaturesY = new Dictionary<int, UMCLight>();

//            // Baseline featur
//            Dictionary<int, UMCLight> featureMap = new Dictionary<int, UMCLight>();
//            foreach (PeptideMatch match in spectraY.Values)
//            {
//                if (match.SpectrumX.ParentFeature != null && match.SpectrumY.ParentFeature != null)
//                {
//                    UMCLight featurex = match.SpectrumX.ParentFeature.ParentFeature;
//                    UMCLight featurey = match.SpectrumY.ParentFeature.ParentFeature;

//                    analysis.PreAlignment.Add(featurex.NET - featurey.NET);
//                    analysis.PostAlignment.Add(featurex.NET - featurey.NETAligned);
//                }
//            }


//            analysis.Write();
//            Console.WriteLine();
//        }

//        private MSSpectra GetSpectra(double mzTolerance,
//                                    double percent,
//                                    ISpectraFilter filter,
//                                    ISpectraProvider readerY,
//                                    Dictionary<int, ScanSummary> scanDataY,
//                                    int scany)
//        {
//            MSSpectra spectrumY = GetSpectrum(readerY,
//                                        scanDataY[scany].Scan,
//                                        0,
//                                        mzTolerance: mzTolerance);
//            spectrumY = filter.Threshold(spectrumY, percent);
//            spectrumY.Peaks = XYData.Bin(spectrumY.Peaks,
//                                                0,
//                                                2000,
//                                                mzTolerance);
//            return spectrumY;
//        }
//        #endregion
//    }
}