#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTDBFramework.Data;
using MTDBFramework.IO;
using NUnit.Framework;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignTestSuite.Algorithms.Alignment.LCMSWarp
{
    /// <summary>
    ///     This class is used for assisting the alignment, to make sure test files are correctly read in.
    /// </summary>
    [TestFixture]
    public class MtdbFileReaderTest
    {
        [Test]
        [TestCase(@"C:\UnitTestFolder\dataset1.txt",
            @"C:\UnitTestFolder\dataset2.txt",
            @"C:\UnitTestFolder\LCMSWarpTesting\QC_Shew_14-02-01\QC_Shew_13_02_2a_03Mar14_Leopard_14-02-01_msgfdb_syn.txt",
            @"C:\UnitTestFolder\LCMSWarpTesting\QC_Shew_14_02_02\QC_Shew_13_02_2b_03Mar14_Leopard_14-02-02_msgfdb_syn.txt"
            )]
        public void MtdbVsTxt(string datasetOnePath,
            string datasetTwoPath,
            string path1,
            string path2)
        {
            var txtbaseline = new List<UMCLight>();
            var txtfeatures = new List<UMCLight>();

            var rawBaselineData = File.ReadAllLines(datasetOnePath);
            var rawFeaturesData = File.ReadAllLines(datasetTwoPath);

            foreach (var line in rawBaselineData)
            {
                if (line == "") continue;
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
                txtbaseline.Add(data);
            }

            foreach (var line in rawFeaturesData)
            {
                if (line == "") continue;

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
                txtfeatures.Add(data);
            }

            var options = new Options();
            var msgfReader = new MsgfPlusPhrpReader(options);
            var datasets = new List<LcmsDataSet>();

            var dataset = msgfReader.Read(path1);
            datasets.Add(dataset);
            var baseline = dataset.Evidences.Select(evidence => new UMCLight
            {
                Net = evidence.ObservedNet,
                ChargeState = evidence.Charge,
                Mz = evidence.Mz,
                Scan = evidence.Scan,
                MassMonoisotopic = evidence.ObservedMonoisotopicMass,
                MassMonoisotopicAligned = evidence.MonoisotopicMass,
                Id = evidence.AnalysisId
            }).ToList();

            dataset = msgfReader.Read(path2);
            datasets.Add(dataset);

            for (var i = 0; i < baseline.Count; i++)
            {
                Assert.IsTrue(Math.Abs(baseline[i].Net - txtbaseline[i].Net) < 0.0000000001);
                Assert.AreEqual(baseline[i].ChargeState, txtbaseline[i].ChargeState);
                Assert.IsTrue(Math.Abs(baseline[i].Mz - txtbaseline[i].Mz) < 0.0000000001);
                Assert.AreEqual(baseline[i].Scan, txtbaseline[i].Scan);
                Assert.IsTrue(Math.Abs(baseline[i].MassMonoisotopic - txtbaseline[i].MassMonoisotopic) < 0.0000000001);
                Assert.IsTrue(Math.Abs(baseline[i].MassMonoisotopicAligned - txtbaseline[i].MassMonoisotopicAligned) <
                              0.0000000001);
            }
        }
    }
}