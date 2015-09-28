#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Alignment.LcmsWarp;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.Drawing;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.Algorithms.Alignment.LCMSWarp
{
    [TestFixture]
    public sealed class LcmsWarpTests: TestBase 
    {
        [Test(Description = "This tests the new LCMSWarp port between two database search results converted to UmcLights")]
        [TestCase(
            @"C:\UnitTestFolder\dataset1.txt",
            @"C:\UnitTestFolder\dataset2.txt",
            "asdf",
            "asdfasdf",
            Ignore = true
            )]
        [TestCase(
           @"Data\QC_Shew\QC_Shew_13_04_1b_6Oct13_Cougar_13-06-14.features",
           @"Data\QC_Shew\QC_Shew_13_04_1b_18Sep13_Cougar_13-06-14.features",
           @"testResults\Alignment\QC-Shew-Annotated3\",
           @"qc_shew_13_04_1b",
           Ignore = false
           )]
        [TestCase(
           @"Data\chronicFatigue\data\169091_Schutzer_CF_10937_18Jan10_Owl_09-08-18.features",
           @"Data\chronicFatigue\data\169114_Schutzer_CF_10818_18Jan10_Owl_09-08-18.features",
           @"testResults\Alignment\Schutzer-CF\",
           @"Schutzer_CF_18_01_10",
           Ignore = false
           )]
        public void TestLcmsWarpPort(string relativeBaselinePath, string relativeAligneePath, string relativeOutput, string name)
        {
            var baselinePath    = GetPath(relativeBaselinePath);
            var aligneePath     = GetPath(relativeAligneePath);
            var options = new LcmsWarpAlignmentOptions
            {
                AlignType = LcmsWarpAlignmentType.NET_MASS_WARP,
                CalibrationType = LcmsWarpCalibrationType.Both
            };
            var aligner         = new LcmsWarpFeatureAligner(options); 
           
            var rawBaselineData = File.ReadAllLines(baselinePath);
            var rawFeaturesData = File.ReadAllLines(aligneePath);
            var outputPath      = GetOutputPath(relativeOutput);
            var delimiter       = new[] {TextDelimiter};

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var baseline        = (from line in rawBaselineData
                where line != ""
                select line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                into parsed
                select new UMCLight
                {
                    Net                     = Convert.ToDouble(parsed[0]),
                    ChargeState             = Convert.ToInt32(parsed[1]),
                    Mz                      = Convert.ToDouble(parsed[2]),
                    Scan                    = Convert.ToInt32(parsed[3]),
                    MassMonoisotopic        = Convert.ToDouble(parsed[4]),
                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                    Id                      = Convert.ToInt32(parsed[6]),
                    ScanStart               = Convert.ToInt32(parsed[7]),
                    ScanEnd                 = Convert.ToInt32(parsed[8]),
                    ScanAligned             = Convert.ToInt32(parsed[9])
                }).ToList();

            var features = (from line in rawFeaturesData
                where line != ""
                            select line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                into parsed
                select new UMCLight
                {
                    Net                     = Convert.ToDouble(parsed[0]),
                    ChargeState             = Convert.ToInt32(parsed[1]),
                    Mz                      = Convert.ToDouble(parsed[2]),
                    Scan                    = Convert.ToInt32(parsed[3]),
                    MassMonoisotopic        = Convert.ToDouble(parsed[4]),
                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                    Id                      = Convert.ToInt32(parsed[6]),
                    ScanStart               = Convert.ToInt32(parsed[7]),
                    ScanEnd                 = Convert.ToInt32(parsed[8]),
                    ScanAligned             = Convert.ToInt32(parsed[9])
                }).ToList();


            var outputData  = aligner.Align(baseline, features);
            var residuals   = outputData.ResidualData;
            
            var heatmap        = HeatmapFactory.CreateAlignedHeatmap(outputData.HeatScores, false);
            var netHistogram   = HistogramFactory.CreateHistogram(outputData.NetErrorHistogram, "NET Error", "NET Error");
            var massHistogram  = HistogramFactory.CreateHistogram(outputData.MassErrorHistogram, "Mass Error", "Mass Error (ppm)");

            var netResidual         = ScatterPlotFactory.CreateResidualPlot(residuals.Net, residuals.LinearCustomNet,
               residuals.LinearNet, "NET Residuals", "Scans", "NET");
            var massMzResidual      = ScatterPlotFactory.CreateResidualPlot(residuals.Mz, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "m/z", "Mass Errors");
            var massScanResidual    = ScatterPlotFactory.CreateResidualPlot(residuals.Net, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "Scan", "Mass Errors");

            var directory   = Path.Combine(outputPath, name);

            var encoder     = new SvgEncoder();
            PlotImageUtility.SaveImage(heatmap,             directory + "_heatmap.svg",             encoder);            
            PlotImageUtility.SaveImage(netResidual,         directory + "_netResidual.svg",         encoder);
            PlotImageUtility.SaveImage(massMzResidual,      directory + "_massMzResidual.svg",      encoder);
            PlotImageUtility.SaveImage(massScanResidual,    directory + "_massScanResidual.svg",    encoder);
            PlotImageUtility.SaveImage(netHistogram,       directory + "_netHistogram.svg",       encoder);
            PlotImageUtility.SaveImage(massHistogram,      directory + "_massHistogram.svg",      encoder);
        }

        [Test(Description = "This tests the new LCMSWarp port between two database search results converted to UmcLights")]
        [TestCase(
            @"C:\UnitTestFolder\dataset1.txt",
            @"C:\UnitTestFolder\dataset2.txt",
            "asdf",
            "asdfasdf",
            Ignore = true
            )]
        [TestCase(
           @"Data\QC_Shew\QC_Shew_13_04_1b_6Oct13_Cougar_13-06-14.features",
           @"Data\QC_Shew\QC_Shew_13_04_1b_18Sep13_Cougar_13-06-14.features",
           @"testResults\Alignment\QC-Shew-Annotated3-CPP\",
           @"qc_shew_13_04_1b",
           Ignore = false
           )]
        [TestCase(
           @"Data\chronicFatigue\data\169091_Schutzer_CF_10937_18Jan10_Owl_09-08-18.features",
           @"Data\chronicFatigue\data\169114_Schutzer_CF_10818_18Jan10_Owl_09-08-18.features",
           @"testResults\Alignment\Schutzer-CF-CPP\",
           @"Schutzer_CF_18_01_10",
           Ignore = false
           )]
        public void TestLcmsWarpPortCpp(string relativeBaselinePath, string relativeAligneePath, string relativeOutput, string name)
        {
            var baselinePath    = GetPath(relativeBaselinePath);
            var aligneePath     = GetPath(relativeAligneePath);
            var lcmsWarpOptions = new LcmsWarpAlignmentOptions
            {
                AlignType = LcmsWarpAlignmentType.NET_MASS_WARP
            };
            var aligner         = new LcmsWarpFeatureAligner(lcmsWarpOptions);
            var rawBaselineData = File.ReadAllLines(baselinePath);
            var rawFeaturesData = File.ReadAllLines(aligneePath);
            var outputPath      = GetOutputPath(relativeOutput);
            var delimiter       = new[] {TextDelimiter};

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var baseline        = (from line in rawBaselineData
                where line != ""
                select line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                into parsed
                select new UMCLight
                {
                    Net                     = Convert.ToDouble(parsed[0]),
                    ChargeState             = Convert.ToInt32(parsed[1]),
                    Mz                      = Convert.ToDouble(parsed[2]),
                    Scan                    = Convert.ToInt32(parsed[3]),
                    MassMonoisotopic        = Convert.ToDouble(parsed[4]),
                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                    Id                      = Convert.ToInt32(parsed[6]),
                    ScanStart               = Convert.ToInt32(parsed[7]),
                    ScanEnd                 = Convert.ToInt32(parsed[8]),
                    ScanAligned             = Convert.ToInt32(parsed[9])
                }).ToList();

            var features = (from line in rawFeaturesData
                where line != ""
                            select line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                into parsed
                select new UMCLight
                {
                    Net                     = Convert.ToDouble(parsed[0]),
                    ChargeState             = Convert.ToInt32(parsed[1]),
                    Mz                      = Convert.ToDouble(parsed[2]),
                    Scan                    = Convert.ToInt32(parsed[3]),
                    MassMonoisotopic        = Convert.ToDouble(parsed[4]),
                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                    Id                      = Convert.ToInt32(parsed[6]),
                    ScanStart               = Convert.ToInt32(parsed[7]),
                    ScanEnd                 = Convert.ToInt32(parsed[8]),
                    ScanAligned             = Convert.ToInt32(parsed[9])
                }).ToList();

            var maxd = features.Max(x => x.Net);
            var mind = features.Min(x => x.Net);
            if (maxd - mind < double.Epsilon)
                throw new Exception("There is something wrong with the features NET values");


            var outputData      = aligner.Align(baseline, features);
            var residuals       = outputData.ResidualData;

            var heatmap         = HeatmapFactory.CreateAlignedHeatmap(outputData.HeatScores, false);
            var netHistogram    = HistogramFactory.CreateHistogram(outputData.NetErrorHistogram, "NET Error Histogram", "NET Error");
            var massHistogram   = HistogramFactory.CreateHistogram(outputData.MassErrorHistogram, "Mass Error Histogram", "Mass Error (ppm)");

            var netResidual         = ScatterPlotFactory.CreateResidualPlot(residuals.Net, residuals.LinearCustomNet,
               residuals.LinearNet, "NET Residuals", "Scans", "NET");

            var massMzResidual      = ScatterPlotFactory.CreateResidualPlot(residuals.Mz, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "m/z", "Mass Errors");

            var massScanResidual    = ScatterPlotFactory.CreateResidualPlot(residuals.Net, residuals.MzMassError,
                residuals.MzMassErrorCorrected, "Mass Residuals", "Scan", "Mass Errors");

            var directory   = Path.Combine(outputPath, name);

            var encoder     = new SvgEncoder();
            PlotImageUtility.SaveImage(heatmap,             directory + "_heatmap.svg",             encoder);            
            PlotImageUtility.SaveImage(netResidual,         directory + "_netResidual.svg",         encoder);
            PlotImageUtility.SaveImage(massMzResidual,      directory + "_massMzResidual.svg",      encoder);
            PlotImageUtility.SaveImage(massScanResidual,    directory + "_massScanResidual.svg",    encoder);
            PlotImageUtility.SaveImage(netHistogram,       directory + "_netHistogram.svg",       encoder);
            PlotImageUtility.SaveImage(massHistogram,      directory + "_massHistogram.svg",      encoder);
        }


        [Test(Description = "Tests the old C++ version, to make sure it sets the reference features correctly.")]
        [TestCase(
            @"C:\UnitTestFolder\dataset1.txt",
            Ignore=true)]
        public void TestCppSetReferenceFeatures(string baselinePath)
        {
            Console.WriteLine(@"I'm Testing!");

            var rawBaselineData = File.ReadAllLines(baselinePath);

            var baseline = new List<UMCLight>();

            foreach (var line in rawBaselineData)
            {
                if (line != "")
                {
                    var parsed = line.Split(',');
                    var data = new UMCLight
                    {
                        Net = Convert.ToDouble(parsed[0]),
                        ChargeState = Convert.ToInt32(parsed[1]),
                        Mz = Convert.ToDouble(parsed[2]),
                        Scan = Convert.ToInt32(parsed[3]),
                        MassMonoisotopic = Convert.ToDouble(parsed[4]),
                        MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                        Id = Convert.ToInt32(parsed[6])
                    };
                    baseline.Add(data);
                }
            }

            var oldStyle = new MultiAlignEngine.Alignment.clsAlignmentProcessor();
            var oldBaseline = baseline.Select(baseData => new MultiAlignEngine.Features.clsUMC
            {
                Net = baseData.Net,
                MZForCharge = baseData.Mz,
                Scan = baseData.Scan,
                Mass = baseData.MassMonoisotopic,
                MassCalibrated = baseData.MassMonoisotopicAligned,
                Id = baseData.Id
            }).ToList();

            oldStyle.SetReferenceDatasetFeatures(oldBaseline);
            Console.WriteLine(@"Done testing");
        }

        [Test(Description = "Tests the old C++ version, to make sure it sets the alignee features correctly.")]
        [TestCase(
            @"C:\UnitTestFolder\dataset2.txt",
            Ignore=true)]
        public void TestSetAligneeFeatures(string aligneePath)
        {
            Console.WriteLine(@"I'm Testing!");
            var rawFeaturesData = File.ReadAllLines(aligneePath);

            var features = (from line in rawFeaturesData
                where line != ""
                select line.Split(',')
                into parsed
                select new UMCLight
                {
                    Net = Convert.ToDouble(parsed[0]),
                    ChargeState = Convert.ToInt32(parsed[1]),
                    Mz = Convert.ToDouble(parsed[2]),
                    Scan = Convert.ToInt32(parsed[3]),
                    MassMonoisotopic = Convert.ToDouble(parsed[4]),
                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                    Id = Convert.ToInt32(parsed[6])
                }).ToList();

            var oldStyle = new MultiAlignEngine.Alignment.clsAlignmentProcessor();
            var oldFeatures = features.Select(baseData => new MultiAlignEngine.Features.clsUMC
            {
                Net = baseData.Net,
                MZForCharge = baseData.Mz,
                Scan = baseData.Scan,
                Mass = baseData.MassMonoisotopic,
                MassCalibrated = baseData.MassMonoisotopicAligned,
                Id = baseData.Id
            }).ToList();

            var oldMzBound = new MultiAlignEngine.Alignment.classAlignmentMZBoundary(0, 9999999.0);
            oldStyle.SetAligneeDatasetFeatures(oldFeatures, oldMzBound);

            Console.WriteLine(@"Done testing");
        }

        [Test(Description = "Tests the old C++ version, to make sure it sets the reference features correctly.")]
        [TestCase(
            @"C:\UnitTestFolder\dataset1.txt",
            @"C:\UnitTestFolder\dataset2.txt",
            Ignore = true)]
        public void TestPerformNetAlignment(string baselinePath, string aligneePath)
        {
            Console.WriteLine(@"I'm Testing!");

            var rawBaselineData = File.ReadAllLines(baselinePath);
            var rawFeaturesData = File.ReadAllLines(aligneePath);

            var baseline = (from line in rawBaselineData
                where line != ""
                select line.Split(',')
                into parsed
                select new UMCLight
                {

                    Net = Convert.ToDouble(parsed[0]),
                    ChargeState = Convert.ToInt32(parsed[1]),
                    Mz = Convert.ToDouble(parsed[2]),
                    ScanStart = Convert.ToInt32(parsed[3]),
                    Scan = Convert.ToInt32(parsed[3]),
                    MassMonoisotopic = Convert.ToDouble(parsed[4]),
                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                    Id = Convert.ToInt32(parsed[6])
                }).ToList();

            var features = (from line in rawFeaturesData
                where line != ""
                select line.Split(',')
                into parsed
                select new UMCLight
                {
                    Net = Convert.ToDouble(parsed[0]),
                    ChargeState = Convert.ToInt32(parsed[1]),
                    Mz = Convert.ToDouble(parsed[2]),
                    ScanStart = Convert.ToInt32(parsed[3]),
                    Scan = Convert.ToInt32(parsed[3]),
                    MassMonoisotopic = Convert.ToDouble(parsed[4]),
                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                    Id = Convert.ToInt32(parsed[6])
                }).ToList();

            var oldStyle = new LcmsWarpFeatureAligner(new LcmsWarpAlignmentOptions());

            var oldOutputData = oldStyle.Align(baseline, features);

            Console.WriteLine(@"Done testing");
        }


        [Test(Description = "Testing the grid app being developed (will be moved to appropriate folder)")]
        [TestCase(@"C:\UnitTestFolder\csvFolder\mz.csv", Ignore = true)]
        public void TestLamarcheGridApp(string csvPath)
        {
            //Read a csv file, put the data into a new UMCLight for each one
            var csvFileText = File.ReadAllLines(csvPath);

            var csvDataList = new List<UMCLight> {Capacity = csvFileText.Length};

            foreach (var line in csvFileText)
            {
                var parsedLine = line.Split(',');

                var umcDataMember = new UMCLight();
                //put the data from the parsed line into the umcDataMember in the appropriate fashion

                csvDataList.Add(umcDataMember);
            }

            //Create clusters from the data read in from the file

            UMCClusterLight cluster = null;

            var filteredClusters = new List<UMCClusterLight>();

            if (!Filter(cluster))
            {
                //Save the cluster
                filteredClusters.Add(cluster);
            }


            //Read a mtdb file using MACore or sqliteDb
            var databasePath = @"C:\UnitTestFolder\MSGFPlus\blah.db";
                //Either read from console, or entered at program execution
            // Console.ReadLine(databasePath) or databasePath = args[2]
            var database = ReadDatabase(databasePath);

            var stacAdapter = new STACAdapter<UMCClusterLight>();

            var matchList = stacAdapter.PerformPeakMatching(filteredClusters, database);
            string writePath = null;
            // As with databasePath, could either be read from console, or entered at program execution
            // Console.ReadLine(writePath) or, if(args.Length >= 4){ writePath = args[3]; }
            // If writePath isn't entered, then it writes to a default file, defined inside the WriteData method
            WriteData(matchList, writePath);
        }

        private static bool Filter(UMCClusterLight cluster) // might be different cluster object
        {
            //Perform filtering as necesary, returns true if the cluster does not pass the filter
            return true;
        }

        private static MassTagDatabase ReadDatabase(string databasePath)
        {
            MassTagDatabase database = null;

            //Perform the reading from the binary file, saving data into database
            var fileBytes = File.ReadAllBytes(databasePath);

            var sb = new StringBuilder();

            foreach (var b in fileBytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            //Now it's a string of data
            var data = sb.ToString();

            return database;
        }

        private static void WriteData(List<FeatureMatchLight<UMCClusterLight, MassTagLight>> matchList,
            string writePath)
        {
            if (writePath == null)
            {
                writePath = @"C:\DataGoesHere.csv";
            }

            // Open the file, or create it if it didn't exist, for write access
            using (var writeFile = new StreamWriter(writePath))
            {
                foreach (var match in matchList)
                {
                    //Write the data into the file!
                    writeFile.Write(match.Observed.DriftTime);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.MassMonoisotopicAligned);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.NetAligned);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.DatasetMemberCount);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.Features.Count);
                    writeFile.Write('\n');
                }
            }
        }
    }
}