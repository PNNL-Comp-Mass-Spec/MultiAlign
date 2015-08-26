using System;
using MultiAlignCore.Algorithms.Distance;
using MultiAlignCore.Data.Features;
using NUnit.Framework;

namespace MultiAlignTestSuite.Algorithms.Distance
{
	/// <summary>
	/// Test class for methods located in MahalanobisDistanceCalculator that contains various methods for calculating the mahalanobis distance.
	/// </summary>
	[TestFixture]
    public sealed class WeightedDistanceCalculatorTests
	{

        private UMCClusterLight CreateCluster(double mass, double net, double drift)
        {
            var cluster  = new UMCClusterLight();
            cluster.MassMonoisotopic = mass;
            cluster.Net              = net;
            cluster.Net    = net;
            cluster.DriftTime        = drift;
            return cluster;
        }

		[Test]
		public void TestDistances()
		{
            var dist = new WeightedEuclideanDistance<UMCClusterLight>();
            

            var clusterA = CreateCluster(500, .2, 27);
            var clusterB = CreateCluster(500, .2, 27);

            var N                = 50;
            var stepMass      = .5;
            var stepNET       = .001;
            var stepDrift     = .01;
            

            Console.WriteLine("Walk in drift time");            
            for (var i = 0; i < N; i++)
            {
                clusterB.DriftTime += stepDrift; 
                var distance    = dist.EuclideanDistance(clusterA, clusterB);
                Console.WriteLine("{0}, {1}, {3}, {2}", clusterB.DriftTime, clusterB.DriftTime, distance, clusterB.DriftTime - clusterA.DriftTime);
            }

            Console.WriteLine();
            Console.WriteLine("Walk in net ");
            clusterB.DriftTime = 27;

            for (var i = 0; i < N; i++)
            {
                clusterB.Net += stepNET;
                var distance = dist.EuclideanDistance(clusterA, clusterB);
                Console.WriteLine("{0}, {1}, {3}, {2}", clusterB.Net, clusterB.Net, distance, clusterB.Net - clusterA.Net);                
            }


            Console.WriteLine();
            Console.WriteLine("Walk in mass ");
            clusterB.Net = .2;
            for (var i = 0; i < N; i++)
            {

                var d = FeatureLight.ComputeDaDifferenceFromPPM(clusterA.MassMonoisotopic, stepMass * i);
                clusterB.MassMonoisotopic = d;
                var distance = dist.EuclideanDistance(clusterA, clusterB);
                Console.WriteLine("{0}, {1}, {3}, {2}", clusterB.MassMonoisotopic, 
                                                        clusterB.MassMonoisotopic, 
                                                        distance, 
                                                        FeatureLight.ComputeMassPPMDifference(clusterA.MassMonoisotopic, clusterB.MassMonoisotopic));
            }     
		}	
	}
}
