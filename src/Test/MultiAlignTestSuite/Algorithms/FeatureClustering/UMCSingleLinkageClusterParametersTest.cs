/*////////////////////////////////////////////////////////////////////////////////////////////////////////////
 *
 * Name:    UMC Single Linkage Cluster Parameters Test
 * File:    UMCSingleLinkageClusterParameterTest.cs
 * Author:  Brian LaMarche
 * Purpose: Tests parameter method and properties.
 * Date:    9-22-2010
 * Revisions:
 ////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data.Features;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.FeatureClustering
{
    [TestFixture]
    public class UMCSingleLinkageClusterParameterTests
    {
        /// <summary>
        ///  Part of a clustering test to make sure when sending a
        ///  null list the clustering algorithm fails.
        /// </summary>
        [Test]
        [Description("Sends a null list of UMC's to the clustering algorithm.")]
        public void ClearMethodTest()
        {
            var parameters   = new FeatureClusterParameters<UMCLight>();
            parameters.CentroidRepresentation               = ClusterCentroidRepresentation.Mean;
            parameters.DistanceFunction                     = null;
            var useCharges                                 = parameters.OnlyClusterSameChargeStates;
            parameters.OnlyClusterSameChargeStates          = parameters.OnlyClusterSameChargeStates == false;
            parameters.Tolerances                           = null;
            parameters.Clear();

            Assert.AreEqual(parameters.CentroidRepresentation, ClusterCentroidRepresentation.Median);
            Assert.NotNull(parameters.Tolerances);
            Assert.AreEqual(useCharges, parameters.OnlyClusterSameChargeStates);
            Assert.NotNull(parameters.DistanceFunction);
        }
    }
}