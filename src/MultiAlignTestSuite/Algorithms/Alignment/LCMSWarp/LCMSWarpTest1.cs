using MTDBFramework.Data;
using MTDBFramework.IO;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Features;
using NUnit.Framework;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignTestSuite.Algorithms.Alignment.LCMSWarp
{
    [TestFixture]
    public class LcmsWarpTest1
    {
        [Test (Description = "This tests the new LCMSWarp port between two database search results converted to UmcLights")]
        [TestCase(
             @"C:\UnitTestFolder\LCMSWarpTesting\QC_Shew_14-02-01\QC_Shew_13_02_2a_03Mar14_Leopard_14-02-01_msgfdb_syn.txt",
             @"C:\UnitTestFolder\LCMSWarpTesting\QC_Shew_14_02_02\QC_Shew_13_02_2b_03Mar14_Leopard_14-02-02_msgfdb_syn.txt"
            )]
        public void TestLcmsWarpPort(string path1, string path2)
        {
            Console.Write(@"I'm Testing!\n");
            
            var options = new Options();
            var msgfReader = new MsgfPlusPhrpReader(options);
            var datasets = new List<LcmsDataSet>();
            
            var dataset = msgfReader.Read(path1);
            datasets.Add(dataset);
            var baseline = dataset.Evidences.Select(evidence => new UMCLight
            {
                NET = evidence.ObservedNet,
                ChargeState = evidence.Charge,
                Mz = evidence.Mz,
                Scan = evidence.Scan,
                MassMonoisotopic = evidence.ObservedMonoisotopicMass,
                MassMonoisotopicAligned = evidence.MonoisotopicMass,
                ID = evidence.AnalysisId
            }).ToList();

            dataset = msgfReader.Read(path2);
            datasets.Add(dataset);
            var features = dataset.Evidences.Select(evidence => new UMCLight
            {
                NET = evidence.ObservedNet,
                ChargeState = evidence.Charge,
                Mz = evidence.Mz,
                Scan = evidence.Scan,
                MassMonoisotopic = evidence.ObservedMonoisotopicMass,
                MassMonoisotopicAligned = evidence.MonoisotopicMass,
                ID = evidence.AnalysisId
            }).ToList();
                                    
            var oldBaseline = new List<clsUMC>();
            foreach (var baseData in baseline)
            {
                var oldData = new clsUMC
                {
                    Net = baseData.NET,
                    MZForCharge = baseData.Mz,
                    Scan = baseData.Scan,
                    Mass = baseData.MassMonoisotopic,
                    MassCalibrated = baseData.MassMonoisotopicAligned,
                    Id = baseData.ID
                };
                oldBaseline.Add(oldData);
            }

            //oldStyle.SetReferenceDatasetFeatures(oldBaseline);

            var oldFeatures = new List<clsUMC>();
            foreach (var baseData in features)
            {
                var oldData = new clsUMC
                {
                    Net = baseData.NET,
                    MZForCharge = baseData.Mz,
                    Scan = baseData.Scan,
                    Mass = baseData.MassMonoisotopic,
                    MassCalibrated = baseData.MassMonoisotopicAligned,
                    Id = baseData.ID
                };
                oldFeatures.Add(oldData);
            }

            //classAlignmentMZBoundary oldMZBound = new classAlignmentMZBoundary(0, 9999999.0);

            //oldStyle.SetAligneeDatasetFeatures(oldFeatures, oldMZBound);


            Console.Write(@"Done testing");
        }

        [Test(Description = "Tests the old C++ version, to make sure it sets the reference features correctly.")]
        [TestCase(
             @"C:\UnitTestFolder\LCMSWarpTesting\QC_Shew_14-02-01\QC_Shew_13_02_2a_03Mar14_Leopard_14-02-01_msgfdb_syn.txt",
             @"C:\UnitTestFolder\LCMSWarpTesting\QC_Shew_14_02_02\QC_Shew_13_02_2b_03Mar14_Leopard_14-02-02_msgfdb_syn.txt"
            )]
        public void TestCppSetReferenceFeatures(string path1, string path2)
        {
            Console.Write(@"I'm Testing!");
            
            var options     = new Options();
            var msgfReader  = new MsgfPlusPhrpReader(options);
            var datasets    = new List<LcmsDataSet>();
            var dataset     = msgfReader.Read(path1);
            datasets.Add(dataset);
            var baseline = dataset.Evidences.Select(evidence => new UMCLight
            {
                NET = evidence.ObservedNet,
                ChargeState = evidence.Charge,
                Mz = evidence.Mz,
                Scan = evidence.Scan,
                MassMonoisotopic = evidence.ObservedMonoisotopicMass,
                MassMonoisotopicAligned = evidence.MonoisotopicMass,
                ID = evidence.AnalysisId
            }).ToList();

            dataset = msgfReader.Read(path2);
            datasets.Add(dataset);
            
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
            Console.Write(@"Done testing");
        }

        [Test(Description = "Tests the old C++ version, to make sure it sets the alignee features correctly.")]
        [TestCase(
             @"C:\UnitTestFolder\LCMSWarpTesting\QC_Shew_14-02-01\QC_Shew_13_02_2a_03Mar14_Leopard_14-02-01_msgfdb_syn.txt",
             @"C:\UnitTestFolder\LCMSWarpTesting\QC_Shew_14_02_02\QC_Shew_13_02_2b_03Mar14_Leopard_14-02-02_msgfdb_syn.txt"
            )]
        public void TestSetAligneeFeatures(string path1, string path2)
        {
            Console.Write(@"I'm Testing!");            
            var options     = new Options();
            var msgfReader  = new MsgfPlusPhrpReader(options);
            var datasets    = new List<LcmsDataSet>();
            
            var dataset = msgfReader.Read(path1);
            datasets.Add(dataset);
            
            dataset = msgfReader.Read(path2);
            datasets.Add(dataset);
            var features = dataset.Evidences.Select(evidence => new UMCLight
            {
                NET = evidence.ObservedNet,
                ChargeState = evidence.Charge,
                Mz = evidence.Mz,
                Scan = evidence.Scan,
                MassMonoisotopic = evidence.ObservedMonoisotopicMass,
                MassMonoisotopicAligned = evidence.MonoisotopicMass,
                ID = evidence.AnalysisId
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

            Console.Write(@"Done testing");
        }

        [Test(Description = "Tests the old C++ version, to make sure it sets the reference features correctly.")]
        [TestCase(
             @"C:\UnitTestFolder\dataset1.txt",
             @"C:\UnitTestFolder\dataset2.txt"
            )]
        public void TestPerformNetAlignment(string baselinePath, string aligneePath)
        {
            Console.Write(@"I'm Testing!");            
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
            
            var oldStyle = new clsAlignmentProcessor();

            var oldBaseline = new List<clsUMC>();
            foreach (var baseData in baseline)
            {
                var oldData = new clsUMC
                {
                    Net = baseData.NET,
                    MZForCharge = baseData.Mz,
                    Scan = baseData.Scan,
                    Mass = baseData.MassMonoisotopic,
                    MassCalibrated = baseData.MassMonoisotopicAligned,
                    Id = baseData.ID
                };
                oldBaseline.Add(oldData);
            }

            oldStyle.SetReferenceDatasetFeatures(oldBaseline);

            var oldFeatures = features.Select(baseData => new clsUMC
            {
                Net = baseData.NET,
                MZForCharge = baseData.Mz, 
                Scan = baseData.Scan, 
                Mass = baseData.MassMonoisotopic,
                MassCalibrated = baseData.MassMonoisotopicAligned,
                Id = baseData.ID
            }).ToList();

            var oldMzBound = new classAlignmentMZBoundary(505.7, 999999999.0);

            oldStyle.SetAligneeDatasetFeatures(oldFeatures, oldMzBound);
            oldStyle.ApplyNETMassFunctionToAligneeDatasetFeatures(ref oldFeatures);
                        
            double[,] mass = null, net = null, drift = null;
            oldStyle.GetErrorHistograms(0.3, 0.1, 0.1, ref mass, ref net, ref drift);
            
            Console.Write(@"Done testing");
        }
    }
}
