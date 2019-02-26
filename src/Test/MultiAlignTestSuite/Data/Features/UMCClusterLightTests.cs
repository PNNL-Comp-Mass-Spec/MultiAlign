using System;
using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using NUnit.Framework;

namespace MultiAlignTestSuite.Data.Features
{
    /// <summary>
    /// Test class for the UMC Clusters.
    /// </summary>
    [TestFixture]
    public class UMCClusterLightTests
    {
        /// <summary>
        /// Calculates statistics for a null umc list clusters.
        /// </summary>
        [Test]
        public void CalculateStatisticsTestNullUMC()
        {
            var cluster = new UMCClusterLight();
            cluster.UmcList         = null;
            Assert.Throws<NullReferenceException>(() => cluster.CalculateStatistics(ClusterCentroidRepresentation.Median));
        }
        /// <summary>
        /// Calculates statistics for a empty UMC list.
        /// </summary>
        [Test]
        public void CalculateStatisticsTestEmptyUMC()
        {
            var cluster = new UMCClusterLight();
            cluster.UmcList         = new List<UMCLight>();
            Assert.Throws<Exception>(() => cluster.CalculateStatistics(ClusterCentroidRepresentation.Median));
        }
        /// <summary>
        /// Calculates statistics for a empty UMC list.
        /// </summary>
        [Test]
        [TestCase(100, 100, 50, 2, 15000, ClusterCentroidRepresentation.Median)]
        [TestCase(100, 100, 50, 2, 15000, ClusterCentroidRepresentation.Mean)]
        public void CalculateStatisticsTestSingleUMC(   double  umcMass,
                                                        double  umcNET,
                                                        float   umcDrifTime,
                                                        int     umcCharge,
                                                        int     umcAbundance,
                                                        ClusterCentroidRepresentation representation)
        {
            var cluster     = new UMCClusterLight();
            cluster.UmcList             = new List<UMCLight>();

            var umc                    = new UMCLight();
            umc.MassMonoisotopicAligned            = umcMass;
            umc.Net               = umcNET;
            umc.DriftTime                   = umcDrifTime;
            umc.ChargeState                 = umcCharge;
            umc.Abundance                   = umcAbundance;
            cluster.UmcList.Add(umc);
            cluster.CalculateStatistics(representation);

            Assert.AreEqual(umc.MassMonoisotopicAligned, cluster.MassMonoisotopic, "Monoisotopic Mass");
            Assert.AreEqual(umc.Net,                     cluster.Net,              "NET");
            Assert.AreEqual(umc.DriftTime,               cluster.DriftTime,        "Drift Time");
            Assert.AreEqual(umc.ChargeState,             cluster.ChargeState,      "Charge State");
            Assert.AreEqual(0,                           cluster.Score,            "Score");
        }
        /// <summary>
        /// Calculates statistics for a empty UMC list.
        /// </summary>
        [Test]
        [TestCase(ClusterCentroidRepresentation.Median)]
        [TestCase(ClusterCentroidRepresentation.Mean)]
        public void CalculateStatisticsSame(ClusterCentroidRepresentation representation)
        {
            var cluster = new UMCClusterLight();
            cluster.UmcList         = new List<UMCLight>();

            var umc            = new UMCLight();
            umc.MassMonoisotopicAligned = 100;
            umc.Net       = 100;
            umc.DriftTime           = 100;
            umc.ChargeState         = 2;
            umc.Abundance           = 100;
            cluster.UmcList.Add(umc);
            cluster.UmcList.Add(umc);

            cluster.CalculateStatistics(representation);
            Assert.AreEqual(0, cluster.Score);
        }
        /// <summary>
        /// Calculates statistics for a empty UMC list.
        /// </summary>
        [Test]
        [TestCase(ClusterCentroidRepresentation.Median)]
        [TestCase(ClusterCentroidRepresentation.Mean)]
        public void CalculateStatisticsMultipleMass(ClusterCentroidRepresentation representation)
        {
            var cluster = new UMCClusterLight();
            cluster.UmcList = new List<UMCLight>();

            var umc = new UMCLight();
            umc.MassMonoisotopicAligned = 100;
            umc.Net = 100;
            umc.DriftTime = 100;
            umc.ChargeState = 2;
            umc.Abundance = 100;
            cluster.UmcList.Add(umc);

            var umc2 = new UMCLight();
            umc2.MassMonoisotopicAligned = 200;
            umc2.Net = 100;
            umc2.DriftTime = 100;
            umc2.ChargeState = 2;
            umc2.Abundance = 100;
            cluster.UmcList.Add(umc2);

            cluster.CalculateStatistics(representation);
            Assert.AreEqual(150, cluster.MassMonoisotopic);
        }

        /// <summary>
        /// Calculates statistics for a empty UMC list.
        /// </summary>
        [Test]
        [TestCase(ClusterCentroidRepresentation.Median)]
        [TestCase(ClusterCentroidRepresentation.Mean)]
        public void CalculateStatisticsMultipleNet(ClusterCentroidRepresentation representation)
        {
            var cluster = new UMCClusterLight();
            cluster.UmcList         = new List<UMCLight>();

            var umc            = new UMCLight();
            umc.MassMonoisotopicAligned = 100;
            umc.Net       = 100;
            umc.DriftTime           = 100;
            umc.ChargeState         = 2;
            umc.Abundance           = 100;
            cluster.UmcList.Add(umc);

            var umc2 = new UMCLight();
            umc2.MassMonoisotopicAligned = 100;
            umc2.Net = 200;
            umc2.DriftTime = 100;
            umc2.ChargeState = 2;
            umc2.Abundance = 100;
            cluster.UmcList.Add(umc2);

            cluster.CalculateStatistics(representation);
            Assert.AreEqual(150, cluster.Net);
        }
    }
}
