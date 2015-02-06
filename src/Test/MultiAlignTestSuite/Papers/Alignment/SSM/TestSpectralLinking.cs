#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Extensions;
using NUnit.Framework;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Algorithms.Statistics;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment.SSM
{
    [TestFixture]
    public class TestSpectralLinking : BaseSpectralAlignmentTest
    {
        [TestFixtureSetUp]
        public void SetupTests()
        {
            RootDataPath = @"M:\data\proteomics\TestData\QC-Shew";
        }

        [Test]
        [TestCase(@"226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05")]
        public void TestLinking(string datasetName)
        {
            var featureFile = GetTestPath(datasetName + "_isos.csv");
            var rawFile = GetTestPath(datasetName + ".raw");

            var features = FindFeatures(rawFile, featureFile);

            var count = 0;
            var doubleCount = 0;
            foreach (var feature in features)
            {
                var singleCount = 0;
                foreach (var msFeature in feature.MsFeatures)
                {
                    if (msFeature.MSnSpectra.Count > 0)
                    {
                        count++;
                        singleCount++;
                    }
                }
                doubleCount += (singleCount > 0) ? 1 : 0;
            }

            Console.WriteLine("{0} Features Have {1} MS/MS spectra - {2} have more than one", features.Count, count,
                doubleCount);
        }

        [Test]
        [TestCase(@"226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05",
            @"226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27")]
        public void TestAlignment(string datasetNameX, string datasetNameY)
        {
            var featureFileX = GetTestPath(datasetNameX + "_isos.csv");
            var rawFileX = GetTestPath(datasetNameX + ".raw");

            var featureFileY = GetTestPath(datasetNameY + "_isos.csv");
            var rawFileY = GetTestPath(datasetNameY + ".raw");

            Print("Detecting Features");
            var featuresX = FindFeatures(rawFileX, featureFileX);
            var featuresY = FindFeatures(rawFileY, featureFileY);

            PrintFeatureMsMsData(featuresX);
            PrintFeatureMsMsData(featuresY);

            Print("Aligning Features");
            // Align the features
            var aligner = new LcmsWarpFeatureAligner();
            var alignmentOptions = new AlignmentOptions();
            aligner.Options = alignmentOptions.LCMSWarpOptions;
            aligner.Align(featuresX, featuresY);

            Print("");
            Print("NET, NETAligned");
            foreach (var feature in featuresY)
            {
                if (feature.HasMsMs())
                {
                    Print(string.Format("{0}", feature.Net - feature.NetAligned));
                }
            }
        }

        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew",
            "226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05",
            "226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27",
            .6,
            Ignore = true)]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
            "QC_Shew_11_06-pt5_1_24Feb12_Falcon_12-02-32",
            "QC_Shew_11_06-pt5_7_24Feb12_Falcon_12-02-34",
            .6,
            Ignore = false)]
        [TestCase(@"M:\data\proteomics\Thesis\mci\rumen",
            "Bioreacter_Rumen5_R2_31Jul11_Jaguar_11-07-18",
            "Bioreacter_Rumen4_R2_31Jul11_Jaguar_11-07-19",
            .7,
            Ignore = true)]
        public void TestSpectralAlignment(string basePath, string baselineName, string aligneeName,
            double comparisonCutoff)
        {
            RootDataPath = basePath;


            var featureFileX = GetTestPath(baselineName + "_isos.csv");
            var rawFileX = GetTestPath(baselineName + ".raw");

            var featureFileY = GetTestPath(aligneeName + "_isos.csv");
            var rawFileY = GetTestPath(aligneeName + ".raw");

            Print("Detecting Features");
            var baselineFeatures = FindFeatures(rawFileX, featureFileX);
            var aligneeFeatures = FindFeatures(rawFileY, featureFileY);

            Print("Aligning Features");
            // Align the features
            var aligner = new LcmsWarpFeatureAligner();
            var alignmentOptions = new AlignmentOptions();
            aligner.Options = alignmentOptions.LCMSWarpOptions;
            aligner.Align(baselineFeatures, aligneeFeatures);

            PrintFeatureMsMsData(baselineFeatures);
            PrintFeatureMsMsData(aligneeFeatures);

            var matches = GetSpectralMatches(
                baselineFeatures, aligneeFeatures, comparisonCutoff);
            Print(string.Format("Found {0} spectral matches", matches.Count));
            Print("Similarity, Pre-Alignment, Post-Alignment");

            var counts = new Dictionary<double, int>();
            counts.Add(.9, 0);
            counts.Add(.8, 0);
            counts.Add(.7, 0);
            counts.Add(.6, 0);
            counts.Add(.5, 0);

            var preDist = new List<double>();
            var postDist = new List<double>();

            foreach (var match in matches)
            {
                var baselineFeature = match.Baseline.ParentFeature.ParentFeature;
                var aligneeFeature = match.Alignee.ParentFeature.ParentFeature;

                var preAlignment = baselineFeature.Net - aligneeFeature.Net;
                var postAlignment = baselineFeature.Net - aligneeFeature.NetAligned;

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
            foreach (var key in counts.Keys)
            {
                Print(string.Format("{0},{1}", key, counts[key]));
            }

            var test = new MannWhitneyTest();
            var data = test.Test(preDist, postDist);
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

            var preDist = new List<double>();
            var postDist = new List<double>();

            var lines = File.ReadAllLines(Path.Combine(basePath, name));
            var header = true;
            foreach (var line in lines)
            {
                if (!header)
                {
                    var stringData = line.Split(',');
                    if (stringData.Length > 2)
                    {
                        var preValue = Convert.ToDouble(stringData[1]);
                        var postValue = Convert.ToDouble(stringData[2]);

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

            var test = HypothesisTestingFactory.CreateTests(HypothesisTests.MannWhitneyU);
            var data = test.Test(preDist, postDist);
            Print(string.Format("Two Tail - {0} ", data.TwoTail));
            Print(string.Format("Left Tail - {0} ", data.LeftTail));
            Print(string.Format("Right Tail - {0} ", data.RightTail));
        }


        [Test]
        [TestCase(@"M:\data\proteomics\Applications\NCRR-falcon",
            "dist1-lcmsWarp.csv",
            NormalityTests.JacqueBera,
            Ignore = false)]
        public void TestNormality(string basePath, string name, NormalityTests testType)
        {
            RootDataPath = basePath;

            var preDist = new List<double>();
            var postDist = new List<double>();

            var lines = File.ReadAllLines(Path.Combine(basePath, name));
            var header = true;

            Print(string.Format("TEST: {1} - {0}", name, testType));
            Print("");

            foreach (var line in lines)
            {
                if (!header)
                {
                    var stringData = line.Split(',');
                    if (stringData.Length > 2)
                    {
                        var preValue = Convert.ToDouble(stringData[1]);
                        var postValue = Convert.ToDouble(stringData[2]);

                        preDist.Add(preValue);
                        postDist.Add(postValue);
                    }
                }
                header = false;
            }

            var test = NormalityTestingFactory.CreateTests(testType);
            var preTestData = test.Test(preDist);
            var postTestData = test.Test(postDist);

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
            Ignore = true)]
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

            var preDist = new List<double>();
            var postDist = new List<double>();

            var lines = File.ReadAllLines(Path.Combine(basePath, name));
            var header = true;

            Print(string.Format("TEST: {1} - {0}", name, testType));
            Print("");

            foreach (var line in lines)
            {
                if (!header)
                {
                    var stringData = line.Split(',');
                    if (stringData.Length > 2)
                    {
                        var preValue = Convert.ToDouble(stringData[1]);
                        var postValue = Convert.ToDouble(stringData[2]);

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
        ///     Calculates the p-value until statistically insignificant or past a number of iterations (100);
        /// </summary>
        /// <param name="preDist"></param>
        /// <param name="postDist"></param>
        /// <param name="testType"></param>
        private void TestAlignmentPValueToFailure(List<double> preDist, List<double> postDist, HypothesisTests testType,
            double step)
        {
            double pValue = 0;
            var nIterations = 0;
            double shift = 0;
            Print("Mean-Pre, Mean Post, Shift Amount, Mean Diff, p-Value two, left, right");
            var meanPost = postDist.Average();


            var postHistogram = new Histogram(.002, -.05, .05, "post-alignment");
            postHistogram.AddData(postDist);
            var histograms = new List<Histogram>();
            histograms.Add(postHistogram);
            while (pValue < .05 && nIterations < 100)
            {
                var newPre = new List<double>();
                preDist.ForEach(x => newPre.Add(x + shift));
                var mean = newPre.Average();

                var test = HypothesisTestingFactory.CreateTests(testType);
                var data = test.Test(newPre, postDist);
                Print(string.Format("{0},{1},{2},{3},{4},{5},{5}", mean, meanPost, shift, Math.Abs(mean - meanPost),
                    data.TwoTail, data.LeftTail, data.RightTail));

                var preHistogram = new Histogram(.002, -.05, .05, string.Format("{0:.0000}", mean));
                preHistogram.AddData(newPre);
                histograms.Add(preHistogram);

                pValue = data.TwoTail;
                nIterations++;
                shift += step;
            }

            var originalPreHistogram = new Histogram(.002, -.05, .05, "pre-histogram");
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
        ///     Gets the spectral matches
        /// </summary>
        /// <param name="baselineFeatures"></param>
        /// <param name="aligneeFeatures"></param>
        /// <returns></returns>
        private List<SpectralMatch> GetSpectralMatches(List<UMCLight> baselineFeatures,
            List<UMCLight> aligneeFeatures,
            double comparisonCutoff)
        {
            var matches = new List<SpectralMatch>();
            var baselineSpectra = GetSpectra(baselineFeatures);
            var aligneeSpectra = GetSpectra(aligneeFeatures);

            // Optimizes the loading of a spectra...
            var map = new Dictionary<int, MSSpectra>();
            var filter = SpectrumFilterFactory.CreateFilter(SpectraFilters.TopPercent);
            var comparer = SpectralComparerFactory.CreateSpectraComparer(SpectralComparison.CosineDotProduct);

            var percent = .2;
            var mzTolerance = .5;
            double maxScanDiff = 1500;


            foreach (var baselineSpectrum in baselineSpectra)
            {
                baselineSpectrum.Peaks = filter.Threshold(baselineSpectrum.Peaks, percent);
                baselineSpectrum.Peaks = XYData.Bin(baselineSpectrum.Peaks, 0, 2000, mzTolerance);

                foreach (var aligneeSpectrum in aligneeSpectra)
                {
                    // Only consider spectra that are near each other in mass.
                    var diff = Math.Abs(baselineSpectrum.PrecursorMz - aligneeSpectrum.PrecursorMz);
                    if (diff >= mzTolerance)
                        continue;

                    // Only consider spectra that are within some range of another.
                    var scanDiff = Math.Abs(aligneeSpectrum.Scan - baselineSpectrum.Scan);
                    if (scanDiff > maxScanDiff)
                        continue;

                    // Bin and threshold the spectra
                    aligneeSpectrum.Peaks = filter.Threshold(aligneeSpectrum.Peaks, percent);
                    aligneeSpectrum.Peaks = XYData.Bin(aligneeSpectrum.Peaks, 0, 2000, mzTolerance);

                    // Compare the spectra
                    var value = comparer.CompareSpectra(baselineSpectrum, aligneeSpectrum);
                    if (double.IsNaN(value))
                        continue;

                    if (value > comparisonCutoff)
                    {
                        var match = new SpectralMatch();
                        match.Alignee = aligneeSpectrum;
                        match.Baseline = baselineSpectrum;
                        match.Similarity = value;
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
        private readonly double m_low;
        private double m_high;

        public Histogram(double spacing, double low, double high, string name)
        {
            Bins = new List<double>();
            Data = new List<double>();
            Spacing = spacing;

            var temp = low;
            while (temp <= high)
            {
                temp += spacing;
                Bins.Add(temp);
                Data.Add(0);
            }

            m_low = low;
            m_high = high;

            Name = name;
        }

        public Histogram(double spacing, string name, List<double> bins, List<double> values)
        {
            Bins = bins;
            Data = values;
            Spacing = spacing;
            Name = name;
        }


        public string Name { get; set; }
        public double Spacing { get; private set; }
        public List<double> Bins { get; private set; }
        public List<double> Data { get; private set; }

        public void AddData(double value)
        {
            var i = Convert.ToInt32(Math.Abs(value - m_low)/Spacing);
            i = Math.Max(0, Math.Min(Data.Count - 1, i));
            Data[i]++;
        }

        public void AddData(IEnumerable<double> data)
        {
            foreach (var datum in data)
            {
                AddData(datum);
            }
        }
    }
}