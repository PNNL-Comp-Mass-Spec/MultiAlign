using System;
using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using MultiAlignCore.IO.RawData;
using NUnit.Framework;

namespace MultiAlignTestSuite.Data.Features
{
    /// <summary>
    /// Test class for the UMC Clusters.
    /// </summary>
    [TestFixture]
    public sealed class UmcClusterTests
    {
        /// <summary>
        /// Calculates statistics for a null umc list clusters.
        /// </summary>
        [Test]
        public void CalculateStatisticsTestNullUmc()
        {
            var cluster      = new UMCClusterLight {UmcList = null};
            Assert.Throws<NullReferenceException>(() => cluster.CalculateStatistics(ClusterCentroidRepresentation.Median));
        }
        /// <summary>
        /// Calculates statistics for a empty UMC list.
        /// </summary>
        [Test]
        public void CalculateStatisticsTestEmptyUmc()
        {
            var cluster = new UMCClusterLight { UmcList = new List<UMCLight>() };
            Assert.Throws<Exception>(() => cluster.CalculateStatistics(ClusterCentroidRepresentation.Median));
        }
        /// <summary>
        /// Calculates statistics for a empty UMC list.
        /// </summary>
        [Test]
        [TestCase(100, 100, 50, 2, 15000, ClusterCentroidRepresentation.Median)]
        [TestCase(100, 100, 50, 2, 15000, ClusterCentroidRepresentation.Mean)]
        public void CalculateStatisticsTestSingleUmc(   double  umcMass,
                                                        double  umcNet,
                                                        float   umcDriftTime,
                                                        int     umcCharge,
                                                        int     umcAbundance,
                                                        ClusterCentroidRepresentation representation)
        {
            var cluster          = new UMCClusterLight {UmcList = new List<UMCLight>()};

            var umc = new UMCLight
            {
                MassMonoisotopicAligned = umcMass,
                Net = umcNet,
                DriftTime = umcDriftTime,
                ChargeState = umcCharge,
                Abundance = umcAbundance
            };
            cluster.UmcList.Add(umc);
            cluster.CalculateStatistics(representation);

            Assert.AreEqual(umc.MassMonoisotopicAligned, cluster.MassMonoisotopic, "Monoisotopic Mass");
            Assert.AreEqual(umc.Net,                     cluster.Net,              "NET");
            Assert.AreEqual(umc.DriftTime,               cluster.DriftTime,        "Drift Time");
            Assert.AreEqual(umc.ChargeState,             cluster.ChargeState,      "Charge State");
        }


        /// <summary>
        /// Calculates statistics for a empty UMC list.
        /// </summary>
        [Test]
        [TestCase(100, 100, 50, 2, 15000, 2, 2, ClusterCentroidRepresentation.Median)]
        [TestCase(100, 100, 50, 2, 15000, 2, 2, ClusterCentroidRepresentation.Mean)]
        [TestCase(100, 100, 50, 2, 15000, 2, 3, ClusterCentroidRepresentation.Median)]
        [TestCase(100, 100, 50, 2, 15000, 2, 3, ClusterCentroidRepresentation.Mean)]
        [TestCase(100, 100, 50, 2, 15000, 2, 4, ClusterCentroidRepresentation.Median)]
        [TestCase(100, 100, 50, 2, 15000, 2, 4, ClusterCentroidRepresentation.Mean)]
        [TestCase(100, 100, 50, 2, 15000, 2, 100, ClusterCentroidRepresentation.Median)]
        [TestCase(100, 100, 50, 2, 15000, 2, 100, ClusterCentroidRepresentation.Mean)]
        public void CalculateStatisticsTestMultipleUmCs(      double  umcMass,
                                                        double  umcNet,
                                                        float   umcDrifTime,
                                                        int     umcCharge,
                                                        int     umcAbundance,
                                                        int     multiplier,
                                                        int     numUmCs,
                                                        ClusterCentroidRepresentation representation)
        {
            var cluster  = new UMCClusterLight {UmcList = new List<UMCLight>()};

            var k                   = numUmCs / 2;
            double  medianMass       = 0;
            double  medianNet        = 0;
            double  medianDriftTime  = 0;

            for (var i = 0; i < numUmCs; i++)
            {
                var umc = new UMCLight
                {
                    MassMonoisotopicAligned = umcMass + multiplier*i,
                    Net = umcNet + multiplier*i,
                    DriftTime = umcDrifTime + multiplier*i,
                    ChargeState = umcCharge,
                    Abundance = umcAbundance + multiplier*i
                };
                cluster.UmcList.Add(umc);

                if (representation == ClusterCentroidRepresentation.Mean)
                {
                    medianMass      += umc.MassMonoisotopicAligned;
                    medianNet       += umc.Net;
                    medianDriftTime += umc.DriftTime;
                }
                // Odd
                else if (k == i && (numUmCs % 2 == 1))
                {
                    medianMass      = umc.MassMonoisotopicAligned;
                    medianNet       = umc.Net;
                    medianDriftTime = umc.DriftTime;
                }
                // Even
                else if ((numUmCs % 2) == 0)
                {
                    // When we have an even number of features
                    // We want to calculate the median as the average between
                    // the two median features (k, k + 1), where k is numUMCs / 2
                    // Remeber that we use k - 1 because i is zero indexed
                    if (k - 1 == i)
                    {
                        medianMass      = umc.MassMonoisotopicAligned;
                        medianNet       = umc.Net;
                        medianDriftTime = umc.DriftTime;
                    }
                    else if (k == i)
                    {
                        medianMass      += umc.MassMonoisotopicAligned;
                        medianNet       += umc.Net;
                        medianDriftTime += umc.DriftTime;
                        medianMass      /= 2;
                        medianNet       /= 2;
                        medianDriftTime /= 2;
                    }
                }
            }

            // We make sure that we calculate the mean correctly here.
            if (representation == ClusterCentroidRepresentation.Mean)
            {
                medianMass      /= numUmCs;
                medianNet       /= numUmCs;
                medianDriftTime /= numUmCs;
            }

            cluster.CalculateStatistics(representation);

            Assert.AreEqual(medianMass,      cluster.MassMonoisotopic, "Monoisotopic Mass");
            Assert.AreEqual(medianNet,       cluster.Net,              "NET");
            Assert.AreEqual(medianDriftTime, cluster.DriftTime,        "Drift Time");
            Assert.AreEqual(umcCharge,       cluster.ChargeState,      "Charge State");
        }
    }
}
