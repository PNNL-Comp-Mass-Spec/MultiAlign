using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using NUnit.Framework;
using PNNLOmics.Algorithms.Alignment.LcmsWarp;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PNNLOmics.Data.MassTags;
using LcmsWarpFeatureAligner = MultiAlignCore.Algorithms.Alignment.LcmsWarpFeatureAligner;
using System.Text;

namespace MultiAlignTestSuite.Algorithms.Alignment.LCMSWarp
{
    [TestFixture]
    public class LcmsWarpTest1
    {
        [Test (Description = "This tests the new LCMSWarp port between two database search results converted to UmcLights")]
        [TestCase(
             @"C:\UnitTestFolder\dataset1.txt",
             @"C:\UnitTestFolder\dataset2.txt"
            )]
        public void TestLcmsWarpPort(string baselinePath, string aligneePath)
        {
            Console.WriteLine(@"I'm Testing!");

            var aligner = new LcmsWarpAdapter();

            var baseline = new List<UMCLight>();

            string[] rawBaselineData = System.IO.File.ReadAllLines(baselinePath);
            string[] rawFeaturesData = System.IO.File.ReadAllLines(aligneePath);

            foreach (var line in rawBaselineData)
            {
                if (line != "")
                {
                    string[] parsed = line.Split(',');
                    var data = new UMCLight
                    {
                        NET = Convert.ToDouble(parsed[0]),
                        ChargeState = Convert.ToInt32(parsed[1]),
                        Mz = Convert.ToDouble(parsed[2]),
                        Scan = Convert.ToInt32(parsed[3]),
                        MassMonoisotopic = Convert.ToDouble(parsed[4]),
                        MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                        ID = Convert.ToInt32(parsed[6])
                    };
                    baseline.Add(data);
                }
            }

            var features = (from line in rawFeaturesData
                            where line != ""
                            select line.Split(',')
                                into parsed
                                select new UMCLight
                                {
                                    NET = Convert.ToDouble(parsed[0]),
                                    ChargeState = Convert.ToInt32(parsed[1]),
                                    Mz = Convert.ToDouble(parsed[2]),
                                    Scan = Convert.ToInt32(parsed[3]),
                                    MassMonoisotopic = Convert.ToDouble(parsed[4]),
                                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                                    ID = Convert.ToInt32(parsed[6])
                                }).ToList();


            LcmsWarpAlignmentData outputData = aligner.Align(baseline, features);

            Console.WriteLine(@"Done testing");
        }

        [Test(Description = "Tests the old C++ version, to make sure it sets the reference features correctly.")]
        [TestCase(
             @"C:\UnitTestFolder\dataset1.txt"
            )]
        public void TestCppSetReferenceFeatures(string baselinePath)
        {
            Console.WriteLine(@"I'm Testing!");

            string[] rawBaselineData = System.IO.File.ReadAllLines(baselinePath);

            var baseline = new List<UMCLight>();

            foreach (var line in rawBaselineData)
            {
                if (line != "")
                {
                    string[] parsed = line.Split(',');
                    var data = new UMCLight
                    {
                        NET = Convert.ToDouble(parsed[0]),
                        ChargeState = Convert.ToInt32(parsed[1]),
                        Mz = Convert.ToDouble(parsed[2]),
                        Scan = Convert.ToInt32(parsed[3]),
                        MassMonoisotopic = Convert.ToDouble(parsed[4]),
                        MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                        ID = Convert.ToInt32(parsed[6])
                    };
                    baseline.Add(data);
                }
            }
            
            var oldStyle = new clsAlignmentProcessor();
            var oldBaseline = baseline.Select(baseData => new clsUMC
            {
                Net = baseData.NET, 
                MZForCharge = baseData.Mz,
                Scan = baseData.Scan, 
                Mass = baseData.MassMonoisotopic, 
                MassCalibrated = baseData.MassMonoisotopicAligned, 
                Id = baseData.ID
            }).ToList();

            oldStyle.SetReferenceDatasetFeatures(oldBaseline);
            Console.WriteLine(@"Done testing");
        }

        [Test(Description = "Tests the old C++ version, to make sure it sets the alignee features correctly.")]
        [TestCase(
             @"C:\UnitTestFolder\dataset2.txt"
            )]
        public void TestSetAligneeFeatures(string aligneePath)
        {
            Console.WriteLine(@"I'm Testing!");
            string[] rawFeaturesData = System.IO.File.ReadAllLines(aligneePath);

            var features = (from line in rawFeaturesData
                            where line != ""
                            select line.Split(',')
                                into parsed
                                select new UMCLight
                                {
                                    NET = Convert.ToDouble(parsed[0]),
                                    ChargeState = Convert.ToInt32(parsed[1]),
                                    Mz = Convert.ToDouble(parsed[2]),
                                    Scan = Convert.ToInt32(parsed[3]),
                                    MassMonoisotopic = Convert.ToDouble(parsed[4]),
                                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                                    ID = Convert.ToInt32(parsed[6])
                                }).ToList();
            
            var oldStyle = new clsAlignmentProcessor();            
            var oldFeatures = features.Select(baseData => new clsUMC
            {
                Net = baseData.NET,
                MZForCharge = baseData.Mz,
                Scan = baseData.Scan, 
                Mass = baseData.MassMonoisotopic,
                MassCalibrated = baseData.MassMonoisotopicAligned, 
                Id = baseData.ID
            }).ToList();

            var oldMzBound = new classAlignmentMZBoundary(0, 9999999.0);
            oldStyle.SetAligneeDatasetFeatures(oldFeatures, oldMzBound);

            Console.WriteLine(@"Done testing");
        }

        [Test(Description = "Tests the old C++ version, to make sure it sets the reference features correctly.")]
        [TestCase(
             @"C:\UnitTestFolder\dataset1.txt",
             @"C:\UnitTestFolder\dataset2.txt"
            )]
        public void TestPerformNetAlignment(string baselinePath, string aligneePath)
        {
            Console.WriteLine(@"I'm Testing!");            
            var baseline = new List<UMCLight>();

            string[] rawBaselineData = System.IO.File.ReadAllLines(baselinePath);
            string[] rawFeaturesData = System.IO.File.ReadAllLines(aligneePath);

            foreach (var line in rawBaselineData)
            {
                if (line != "")
                {
                    string[] parsed = line.Split(',');
                    var data = new UMCLight
                    {
                        NET = Convert.ToDouble(parsed[0]),
                        RetentionTime = Convert.ToDouble(parsed[0]),
                        ChargeState = Convert.ToInt32(parsed[1]),
                        Mz = Convert.ToDouble(parsed[2]),
                        ScanStart = Convert.ToInt32(parsed[3]),
                        Scan = Convert.ToInt32(parsed[3]),
                        MassMonoisotopic = Convert.ToDouble(parsed[4]),
                        MassMonoisotopicAligned = Convert.ToDouble(parsed[5]),
                        ID = Convert.ToInt32(parsed[6])
                    };
                    baseline.Add(data);
                }
            }

            var features = (from line in rawFeaturesData
                where line != ""
                select line.Split(',')
                into parsed
                select new UMCLight
                {
                    NET = Convert.ToDouble(parsed[0]),
                    RetentionTime = Convert.ToDouble(parsed[0]),
                    ChargeState = Convert.ToInt32(parsed[1]),
                    Mz = Convert.ToDouble(parsed[2]),
                    ScanStart = Convert.ToInt32(parsed[3]),
                    Scan = Convert.ToInt32(parsed[3]),
                    MassMonoisotopic = Convert.ToDouble(parsed[4]),
                    MassMonoisotopicAligned = Convert.ToDouble(parsed[5]), 
                    ID = Convert.ToInt32(parsed[6])
                }).ToList();
            
            var oldStyle = new LcmsWarpFeatureAligner();

            var oldOutputData = oldStyle.Align(baseline, features);

            Console.WriteLine(@"Done testing");
        }

        [Test(Description = "Testing the grid app being developed (will be moved to appropriate folder)")]
        [TestCase(@"C:\UnitTestFolder\csvFolder\mz.csv")]
        public void TestLamarcheGridApp(string csvPath)
        {
            //Read a csv file, put the data into a new UMCLight for each one
            string[] csvFileText = File.ReadAllLines(csvPath);

            var csvDataList = new List<UMCLight> { Capacity = csvFileText.Length };

            foreach (var line in csvFileText)
            {
                string[] parsedLine = line.Split(',');

                var umcDataMember = new UMCLight();
                //put the data from the parsed line into the umcDataMember in the appropriate fashion

                csvDataList.Add(umcDataMember);

            }

            //Create clusters from the data read in from the file

            UMCClusterLight cluster = null;

            List<UMCClusterLight> filteredClusters = new List<UMCClusterLight>();

            if (!Filter(cluster))
            {
                //Save the cluster
                filteredClusters.Add(cluster);
            }


            //Read a mtdb file using MACore or sqliteDb
            string databasePath = "C:\\UnitTestFolder\\MSGFPlus\\blah.db"; //Either read from console, or entered at program execution
            // Console.ReadLine(databasePath) or databasePath = args[2]
            MassTagDatabase database = ReadDatabase(databasePath);

            STACAdapter<UMCClusterLight> stacAdapter = new STACAdapter<UMCClusterLight>();

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
            byte[] fileBytes = File.ReadAllBytes(databasePath);

            StringBuilder sb = new StringBuilder();

            foreach (var b in fileBytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            //Now it's a string of data
            string data = sb.ToString();

            return database;
        }

        private static void WriteData(List<FeatureMatchLight<UMCClusterLight, MassTagLight>> matchList,
                                      string writePath)
        {
            if (writePath == null)
            {
                writePath = "C:\\DataGoesHere.csv";
            }

            // Open the file, or create it if it didn't exist, for write access
            using (StreamWriter writeFile = new StreamWriter(writePath))
            {
                foreach (var match in matchList)
                {
                    //Write the data into the file!
                    writeFile.Write(match.Observed.DriftTime);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.MassMonoisotopicAligned);
                    writeFile.Write(",");
                    writeFile.Write(match.Observed.NETAligned);
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
