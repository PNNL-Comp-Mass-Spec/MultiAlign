using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using PNNLOmics.Data;
using MultiAlignCore.IO.Features;
using MultiAlignCore.Data;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.Algorithms.FeatureFinding;
using PNNLOmics.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlignCore.Algorithms;
using PNNLOmics.Algorithms.FeatureMatcher.MSnLinker;
using MultiAlignCore.Algorithms.Alignment;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Algorithms.Statistics;
using PNNLOmics.Algorithms.Statistics.Distributions;
using MultiAlignTestSuite.Papers.Alignment.SSM;

namespace MultiAlignTestSuite.Algorithms.SpectralProcessing
{
    [TestFixture]
    public class TestSpectralLinking: BaseSpectralAlignmentTest
    {        
        [TestFixtureSetUp]
        public void SetupTests( )
        {
            RootDataPath = @"M:\data\proteomics\TestData\QC-Shew";
        }

        [Test]
        [TestCase(@"226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05")]
        public void TestLinking(string datasetName)
        {
            string featureFile = GetTestPath(datasetName + "_isos.csv");
            string rawFile     = GetTestPath(datasetName + ".raw");

            List<UMCLight> features = FindFeatures(rawFile, featureFile);

            int count = 0;
            int doubleCount = 0;
            foreach (var feature in features)
            {
                int singleCount = 0;
                foreach (var msFeature in feature.MSFeatures)
                {
                    if (msFeature.MSnSpectra.Count > 0)
                    {
                        count++;
                        singleCount++;
                    }
                }
                doubleCount += (singleCount > 0) ? 1 : 0;
            }

            Console.WriteLine("{0} Features Have {1} MS/MS spectra - {2} have more than one", features.Count, count, doubleCount);
        
        }
        
        [Test]
        [TestCase(  @"226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05", 
                    @"226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27")]
        public void TestAlignment(string datasetNameX, string datasetNameY)
        {
            string featureFileX = GetTestPath(datasetNameX + "_isos.csv");
            string rawFileX     = GetTestPath(datasetNameX + ".raw");

            string featureFileY = GetTestPath(datasetNameY + "_isos.csv");
            string rawFileY     = GetTestPath(datasetNameY + ".raw");

            Print("Detecting Features");
            List<UMCLight> featuresX = FindFeatures(rawFileX, featureFileX);
            List<UMCLight> featuresY = FindFeatures(rawFileY, featureFileY);

            PrintFeatureMsMsData(featuresX);
            PrintFeatureMsMsData(featuresY);

            Print("Aligning Features");
            // Align the features
            IFeatureAligner aligner  = new LCMSWarpFeatureAligner();
            AlignmentOptions options = new AlignmentOptions();
            aligner.AlignFeatures(featuresX, featuresY, options);

            Print("");
            Print("NET, NETAligned");
            foreach (UMCLight feature in featuresY)
            {
                if (feature.HasMsMs())
                {
                    Print(string.Format("{0}", feature.NET - feature.NETAligned));
                }
            }
        }        
        [Test]
        [TestCase(  @"M:\data\proteomics\TestData\QC-Shew",
                    "226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05", 
                    "226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27",
                    .6,
                    Ignore=true)]
        [TestCase( @"M:\data\proteomics\Applications\NCRR-falcon",
                    "QC_Shew_11_06-pt5_1_24Feb12_Falcon_12-02-32",
                    "QC_Shew_11_06-pt5_7_24Feb12_Falcon_12-02-34",
                    .6,
                    Ignore = false)]
        [TestCase(@"M:\data\proteomics\Thesis\mci\rumen",
                "Bioreacter_Rumen5_R2_31Jul11_Jaguar_11-07-18",
                "Bioreacter_Rumen4_R2_31Jul11_Jaguar_11-07-19",
                .7,
                    Ignore=true)]
        public void TestSpectralAlignment(string basePath, string baselineName, string aligneeName, double comparisonCutoff)
        {
            RootDataPath = basePath;

            

            string featureFileX = GetTestPath(baselineName + "_isos.csv");
            string rawFileX     = GetTestPath(baselineName + ".raw");

            string featureFileY = GetTestPath(aligneeName + "_isos.csv");
            string rawFileY     = GetTestPath(aligneeName + ".raw");

            Print("Detecting Features");
            List<UMCLight> baselineFeatures = FindFeatures(rawFileX, featureFileX);
            List<UMCLight> aligneeFeatures  = FindFeatures(rawFileY, featureFileY);
            
            Print("Aligning Features");
            // Align the features
            IFeatureAligner aligner  = new LCMSWarpFeatureAligner();
            AlignmentOptions options = new AlignmentOptions();
            aligner.AlignFeatures(baselineFeatures, aligneeFeatures, options);

            PrintFeatureMsMsData(baselineFeatures);
            PrintFeatureMsMsData(aligneeFeatures);

            List<SpectralMatch> matches = GetSpectralMatches(
                                                    baselineFeatures, aligneeFeatures, comparisonCutoff);
            Print(string.Format("Found {0} spectral matches", matches.Count));
            Print("Similarity, Pre-Alignment, Post-Alignment");

            Dictionary<double, int> counts = new Dictionary<double, int>();
            counts.Add(.9, 0);
            counts.Add(.8, 0);
            counts.Add(.7, 0);
            counts.Add(.6, 0);
            counts.Add(.5, 0);

            List<double> preDist  = new List<double>();
            List<double> postDist = new List<double>();

            foreach(SpectralMatch match in matches)
            {
                UMCLight baselineFeature = match.Baseline.ParentFeature.ParentFeature;
                UMCLight aligneeFeature  = match.Alignee.ParentFeature.ParentFeature;
                
                double preAlignment     = baselineFeature.NET - aligneeFeature.NET;
                double postAlignment    = baselineFeature.NET - aligneeFeature.NETAligned;

                postDist.Add(postAlignment);
                preDist.Add(preAlignment);

                Print(string.Format("{0},{1},{2}", match.Similarity, preAlignment, postAlignment));

                if (match.Similarity > .9)
                {
                    counts[.9]++;
                }
                else if (match.Similarity > .8)
                {
                    counts[.8]++;
                }
                else if (match.Similarity > .7)
                {
                    counts[.7]++;
                }
                else if (match.Similarity > .6)
                {
                    counts[.6]++;
                }
            }
            Print("");
            Print("Counts");
            Print("");
            foreach (double key in counts.Keys)
            {
                Print(string.Format("{0},{1}", key, counts[key]));
            }

            MannWhitneyTest test = new MannWhitneyTest();
            HypothesisTestingData data = test.Test(preDist, postDist);
            Print(string.Format("Two Tail - {0} ", data.TwoTail));
            Print(string.Format("Left Tail - {0} ", data.LeftTail));
            Print(string.Format("Right Tail - {0} ", data.RightTail));

        }

        [Test]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
                    "dist1-lcmsWarp.csv")]
        public void TestAlignmentPValue(string basePath, string name)            
        {
            RootDataPath = basePath;

            List<double> preDist = new List<double>();
            List<double> postDist = new List<double>();

            string[] lines = File.ReadAllLines(Path.Combine(basePath, name));
            bool header = true;
            foreach (string line in lines)
            {
                if (!header)
                {
                    string[] stringData = line.Split(',');
                    if (stringData.Length > 2)
                    {
                        double preValue     = Convert.ToDouble(stringData[1]);
                        double postValue    = Convert.ToDouble(stringData[2]);

                        if (Math.Abs(preValue) < .05)
                        {
                            preDist.Add(preValue);
                        }
                        if (Math.Abs(postValue) < .05)
                        {
                            postDist.Add(postValue);
                        }
                    }
                }
                header = false;
            }

            IHypothesisTestingTwoSample test = HypothesisTestingFactory.CreateTests(HypothesisTests.MannWhitneyU);
            HypothesisTestingData data            = test.Test(preDist, postDist);
            Print(string.Format("Two Tail - {0} ", data.TwoTail));
            Print(string.Format("Left Tail - {0} ", data.LeftTail));
            Print(string.Format("Right Tail - {0} ", data.RightTail));
        }

        [Test]
        [TestCase(
              1000,
              -.02,      // mu
              .009,     // sigma
              .003,     // sigma post
              @"M:\data\proteomics\Applications\NCRR-falcon\synthetic_gaussian_1000-left.csv")]
        [TestCase(
              1000,
              .02,      // mu
              .009,     // sigma
              .003,     // sigma post
              @"M:\data\proteomics\Applications\NCRR-falcon\synthetic_gaussian_1000-right.csv")]
        public void CreateGaussianDistributions(int samples,  double mu, double sigma, double sigmaPost, string path)
        {
            List<double> pre    = Distributions.CreateNormalDistribution(samples, mu, sigma);
            List<double> post   = Distributions.CreateNormalDistribution(samples, 0,  sigmaPost);

            using (TextWriter writer = File.CreateText(path))
            {
                writer.WriteLine("empty, pre, post");
                for(int i = 0; i < pre.Count; i++)
                {
                    writer.WriteLine("0,{0},{1}", pre[i], post[i]);
                }
            }
        }


        [Test]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
                    "dist1-lcmsWarp.csv",                    
                    NormalityTests.JacqueBera,
                    Ignore = false)]
        public void TestNormality(string basePath, string name, NormalityTests testType)
        {
            RootDataPath = basePath;

            List<double> preDist = new List<double>();
            List<double> postDist = new List<double>();

            string[] lines = File.ReadAllLines(Path.Combine(basePath, name));
            bool header = true;

            Print(string.Format("TEST: {1} - {0}", name, testType));
            Print("");

            foreach (string line in lines)
            {
                if (!header)
                {
                    string[] stringData = line.Split(',');
                    if (stringData.Length > 2)
                    {
                        double preValue = Convert.ToDouble(stringData[1]);
                        double postValue = Convert.ToDouble(stringData[2]);

                        preDist.Add(preValue);
                        postDist.Add(postValue);
                     }
                }
                header = false;
            }

            INormalityTest test = NormalityTestingFactory.CreateTests(testType);
            HypothesisTestingData preTestData  = test.Test(preDist);
            HypothesisTestingData postTestData = test.Test(postDist);
            
            Print(string.Format("Pre  {0} ", preTestData.PValue));
            Print(string.Format("Post {0} ", postTestData.PValue));
        }

        [Test]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
                    "dist1-lcmsWarp.csv",
                    .0005,
                    HypothesisTests.Wilcoxon,
                    Ignore = false)]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
                    "dist1-lcmsWarp.csv",
                    .0005,
                    HypothesisTests.KolmogorovSmirnov,
                    Ignore = false)]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
                    "synthetic_gaussian_1000-right.csv",
                    -.0005,
                    HypothesisTests.MannWhitneyU,
                    Ignore = true)]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
                    "synthetic_gaussian_1000-left.csv",
                    .0005,
                    HypothesisTests.MannWhitneyU,
                    Ignore = true)]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
                    "dist1-lcmsWarp.csv",
                    .0005,
                    HypothesisTests.TTest,
                    Ignore=true)]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
                    "synthetic_gaussian_1000-right.csv",
                    -.0005,
                    HypothesisTests.TTest,
                    Ignore = true)]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
                    "synthetic_gaussian_1000-left.csv",
                    .0005,
                    HypothesisTests.TTest,
                    Ignore = true)]
        public void TestAlignmentPValueToFailure(string basePath, string name, double step, HypothesisTests testType)
        {
            RootDataPath = basePath;

            List<double> preDist  = new List<double>();
            List<double> postDist = new List<double>();

            string[] lines  = File.ReadAllLines(Path.Combine(basePath, name));
            bool header     = true;

            Print(string.Format("TEST: {1} - {0}", name, testType));
            Print("");

            foreach (string line in lines)
            {
                if (!header)
                {
                    string[] stringData = line.Split(',');
                    if (stringData.Length > 2)
                    {
                        double preValue = Convert.ToDouble(stringData[1]);
                        double postValue = Convert.ToDouble(stringData[2]);

                        //if (Math.Abs(preValue) < .05)
                        {                         
                            preDist.Add(preValue);
                        }

                        //if (Math.Abs(postValue ) < .05)
                        {
                            postDist.Add(postValue); 
                        }                       
                    }
                }
                header = false;
            }



            TestAlignmentPValueToFailure(preDist, postDist, testType, step);
        }
        /// <summary>
        /// Calculates the p-value until statistically insignificant or past a number of iterations (100);
        /// </summary>
        /// <param name="preDist"></param>
        /// <param name="postDist"></param>
        /// <param name="testType"></param>
        private void TestAlignmentPValueToFailure(List<double> preDist, List<double> postDist, HypothesisTests testType, double step)
        {
            double pValue   = 0;
            int nIterations = 0;
            double shift    = 0;
            Print("Mean-Pre, Mean Post, Shift Amount, Mean Diff, p-Value two, left, right");
            double meanPost = postDist.Average();




            
            Histogram postHistogram = new Histogram(.002, -.05, .05, "post-alignment");     
            postHistogram.AddData(postDist);
            List<Histogram> histograms  =  new List<Histogram>();
            histograms.Add(postHistogram);
            while(pValue < .05 && nIterations < 100)
            {
                List<double> newPre = new List<double>();
                preDist.ForEach(x => newPre.Add(x + shift));
                double mean = newPre.Average();                   
             
                IHypothesisTestingTwoSample test        = HypothesisTestingFactory.CreateTests(testType);
                HypothesisTestingData data              = test.Test(newPre, postDist);
                Print(string.Format("{0},{1},{2},{3},{4},{5},{5}", mean, meanPost, shift, Math.Abs(mean - meanPost), data.TwoTail, data.LeftTail, data.RightTail));

                Histogram preHistogram = new Histogram(.002, -.05, .05, string.Format("{0:.0000}", mean));
                preHistogram.AddData(newPre);
                histograms.Add(preHistogram);

                pValue = data.TwoTail;
                nIterations++;
                shift += step;
            }
            
            Histogram originalPreHistogram  = new Histogram(.002, -.05, .05, "pre-histogram");
            originalPreHistogram.AddData(preDist);
            Print("");
            PrintHistogram(originalPreHistogram);
            Print("");
            PrintHistogram(postHistogram);
            Print("");
            PrintHistogram("pre-post", histograms);
            Print("");                                            
        }

        /// <summary>
        /// Gets the spectral matches
        /// </summary>
        /// <param name="baselineFeatures"></param>
        /// <param name="aligneeFeatures"></param>
        /// <returns></returns>
        private List<SpectralMatch> GetSpectralMatches(List<UMCLight> baselineFeatures, 
                                                    List<UMCLight> aligneeFeatures,
                                                    double comparisonCutoff)
        {
            List<SpectralMatch> matches     = new List<SpectralMatch>();
            List<MSSpectra> baselineSpectra = GetSpectra(baselineFeatures);
            List<MSSpectra> aligneeSpectra  = GetSpectra(aligneeFeatures);

            // Optimizes the loading of a spectra...
            Dictionary<int, MSSpectra> map  = new Dictionary<int, MSSpectra>();
            ISpectraFilter filter           = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);
            ISpectralComparer comparer      = SpectralComparerFactory.CreateSpectraComparer(SpectralComparison.CosineDotProduct);

            double percent          = .2;
            double mzTolerance      = .5;
            double maxScanDiff      = 1500;

            
            foreach (var baselineSpectrum in baselineSpectra)
            {                    
                MSSpectra tempBaseline             = filter.Threshold(baselineSpectrum, percent);
                tempBaseline.Peaks       = XYData.Bin(tempBaseline.Peaks, 0, 2000, mzTolerance);

                foreach (var aligneeSpectrum in aligneeSpectra)
                {
                    // Only consider spectra that are near each other in mass.
                    double diff = Math.Abs(baselineSpectrum.PrecursorMZ - aligneeSpectrum.PrecursorMZ);
                    if (diff >= mzTolerance)
                        continue;

                    // Only consider spectra that are within some range of another.
                    int scanDiff = Math.Abs(aligneeSpectrum.Scan - baselineSpectrum.Scan);
                    if (scanDiff > maxScanDiff)
                        continue;

                    // Bin and threshold the spectra
                    MSSpectra tempAlignee   = filter.Threshold(aligneeSpectrum, percent);
                    tempAlignee.Peaks       = XYData.Bin(tempAlignee.Peaks, 0, 2000, mzTolerance);

                    // Compare the spectra
                    double value = comparer.CompareSpectra(tempBaseline, tempAlignee);
                    if (double.IsNaN(value))
                        continue;

                    if (value > comparisonCutoff)
                    {
                        SpectralMatch match = new SpectralMatch();
                        match.Alignee       = aligneeSpectrum;
                        match.Baseline      = baselineSpectrum;
                        match.Similarity    = value;
                        matches.Add(match);
                    }
                }

            }
            
            return matches;            
        }
    }

    public class SpectralMatch
    {
        public double Similarity { get; set; }
        public MSSpectra Baseline { get; set; }
        public MSSpectra Alignee { get; set; }        
    }

    public class Histogram
    {
        private double m_low;
        private double m_high;

        public Histogram(double spacing, double low, double high, string name)
        {       
            Bins    = new List<double>();
            Data    = new List<double>();
            Spacing = spacing;

            double temp = low;
            while (temp <= high)
            {
                temp += spacing;
                Bins.Add(temp);
                Data.Add(0);
            }

            m_low  = low;
            m_high = high;

            Name = name;
        }

        public Histogram(double spacing, string name, List<double> bins, List<double> values)
        {
            Bins     = bins;
            Data     = values;
            Spacing  = spacing;            
            Name     = name;
        }


        public string Name { get; set; }
        public double Spacing { get; private  set; }
        public List<double> Bins { get; private set; }
        public List<double> Data { get; private set; }

        public void AddData(double value)
        {
            int i   = Convert.ToInt32(Math.Abs(value - m_low) / Spacing);
            i       = Math.Max(0, Math.Min(Data.Count - 1, i)); 
            Data[i]++;
        }

        public void AddData(IEnumerable<double> data)
        {
            foreach(double datum in data)
            {                
                AddData(datum);
            }
        }
    }

}
