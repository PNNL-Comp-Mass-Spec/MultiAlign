#region

using System;
using System.Collections.Generic;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using NUnit.Framework;

#endregion

namespace MultiAlignCore.Algorithms
{
    [TestFixture]
    public class LargeScaleClusterTests
    {
        public DistanceFunction<UMCLight> DistanceFunction { get; set; }

        /// <summary>
        ///     Compares the masses of two light features.
        /// </summary>
        protected Comparison<UMCLight> m_massComparer;

        public LargeScaleClusterTests()
        {
            m_massComparer = FeatureLight.MassAlignedComparison;
            DistanceFunction = DistanceFactory<UMCLight>.CreateDistanceFunction(DistanceMetric.Euclidean);
        }

        protected virtual bool AreClustersWithinTolerance(UMCLight clusterX,
            UMCLight clusterY,
            double massTolerance,
            double netTolerance,
            double driftTolerance)
        {
            // Calculate differences
            var massDiff =
                Math.Abs(FeatureLight.ComputeMassPPMDifference(clusterX.MassMonoisotopicAligned,
                    clusterY.MassMonoisotopicAligned));
            var netDiff = Math.Abs(clusterX.Net - clusterY.Net);
            var driftDiff = Math.Abs(clusterX.DriftTime - clusterY.DriftTime);

            // Return true only if all differences are within tolerance
            if (massDiff <= massTolerance && netDiff <= netTolerance && driftDiff <= driftTolerance)
            {
                return true;
            }
            return false;
        }

        private List<PairwiseDistance<UMCLight>> CalculatePairwiseDistances(List<UMCLight> data,
            int start,
            int stop,
            double massTolerance,
            double netTolerance,
            double driftTolerance)
        {
            var distances = new List<PairwiseDistance<UMCLight>>();
            for (var i = start; i < stop; i++)
            {
                var featureX = data[i];
                var driftTimeX = featureX.DriftTime;
                var netAlignedX = featureX.Net;
                var massAlignedX = featureX.MassMonoisotopicAligned;
                var chargeStateX = featureX.ChargeState;

                for (var j = i + 1; j <= stop; j++)
                {
                    // Don't calculate distance to self.                    
                    var featureY = data[j];

                    // Calculate the distances here (using a cube).  We dont care if we are going to re-compute
                    // these again later, because here we want to fall within the cube, the distance function used
                    // later is more related to determining a scalar value instead.
                    var withinRange = AreClustersWithinTolerance(featureX,
                        featureY,
                        massTolerance,
                        netTolerance,
                        driftTolerance); //Parameters.RangeFunction(featureX, featureY);

                    // Make sure we fall within the distance range before computing...
                    if (withinRange)
                    {
                        // If IMS or equivalent only cluster similar charge states                        

                        // Make sure it's the same charge state
                        if (chargeStateX == featureY.ChargeState)
                        {
                            // Calculate the pairwise distance
                            var pairwiseDistance = new PairwiseDistance<UMCLight>();
                            pairwiseDistance.FeatureX = featureX;
                            pairwiseDistance.FeatureY = featureY;
                            pairwiseDistance.Distance = DistanceFunction(featureX, featureY);
                            distances.Add(pairwiseDistance);
                        }
                    }
                }
            }
            return distances;
        }

        [Test]
        [TestCase(@"M:\data\proteomics\OHSU\FullStudy\results-boneLossNoMeds\results-bonelossNoMeds.db3",
            5,
            16,
            .014,
            .3)]
        public void MassPartitionTest(string databasePath,
            int chargeState,
            double massTolerance,
            double netTolerance,
            double driftTolerance)
        {
            var database = new UmcAdoDAO();
            database.DatabasePath = databasePath;

            Logger.PrintMessage(string.Format("Extracting Features"), true);
            var data = database.FindByCharge(chargeState);

            // Make sure there is no null UMC data in the input list.
            var nullIndex = data.FindIndex(delegate(UMCLight x) { return x == null; });
            if (nullIndex > 0)
            {
                throw new NullReferenceException("The feature at index " + nullIndex +
                                                 " was null.  Cannot process this data.");
            }

            // The first thing we do is to sort the features based on mass since we know that has the least variability in the data across runs.
            data.Sort(m_massComparer);

            // This is the index of first feature of a given mass partition.
            var startUMCIndex = 0;
            var totalFeatures = data.Count;
            var tenPercent = Convert.ToInt32(totalFeatures*.1);
            var singletons = 0;
            var sizes = new List<int>();
            var times = new List<double>();

            for (var i = 0; i < totalFeatures - 1; i++)
            {
                // Here we compute the ppm mass difference between consecutive features (based on mass).
                // This will determine if we cluster a block of data or not.                
                var umcX = data[i];
                var umcY = data[i + 1];
                var ppm =
                    Math.Abs(FeatureLight.ComputeMassPPMDifference(umcX.MassMonoisotopicAligned,
                        umcY.MassMonoisotopicAligned));

                // If the difference is greater than the tolerance then we cluster 
                //  - we dont check the sign of the ppm because the data should be sorted based on mass.
                if (ppm > massTolerance)
                {
                    // If start UMC Index is equal to one, then that means the feature at startUMCIndex
                    // could not find any other features near it within the mass tolerance specified.
                    if (startUMCIndex == i)
                    {
                        singletons++;
                    }
                    else
                    {
                        var starttime = DateTime.Now;
                        var distances = CalculatePairwiseDistances(data,
                            startUMCIndex,
                            i,
                            massTolerance,
                            netTolerance,
                            driftTolerance);
                        var endTime = DateTime.Now;
                        sizes.Add(i - startUMCIndex + 1);
                        times.Add(endTime.Subtract(starttime).TotalMilliseconds);
                    }
                    startUMCIndex = i + 1;
                }
            }

            var xxx = 0;
            xxx++;
            if (xxx > 1)
            {
                sizes.Add(0);
            }
            Console.WriteLine("{0}", singletons);
            Console.WriteLine();

            for (var i = 0; i < sizes.Count; i++)
            {
                Console.WriteLine("{0}\t{1}", sizes[i], times[i]);
            }
        }
    }
}