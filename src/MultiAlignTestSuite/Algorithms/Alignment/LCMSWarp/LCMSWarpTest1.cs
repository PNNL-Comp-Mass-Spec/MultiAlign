using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using NUnit.Framework;
using PNNLOmics.Algorithms.Alignment.LcmsWarp;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using LcmsWarpFeatureAligner = MultiAlignCore.Algorithms.Alignment.LcmsWarpFeatureAligner;


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
    }
}
