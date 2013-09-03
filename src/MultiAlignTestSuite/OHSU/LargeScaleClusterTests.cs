using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using NUnit.Framework;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Distance;
namespace PNNLOmics.UnitTests.AlgorithmTests.FeatureClustering
{

    [TestFixture]
    public class LargeScaleClusterTests
    {
        public DistanceFunction<UMCLight> DistanceFunction {get;set;}

        /// <summary>
        /// Compares the masses of two light features.
        /// </summary>
        protected Comparison<UMCLight> m_massComparer;

        public LargeScaleClusterTests()
        {
            m_massComparer   = new Comparison<UMCLight>(FeatureLight.MassAlignedComparison);
            DistanceFunction = DistanceFactory<UMCLight>.CreateDistanceFunction(DistanceMetric.Euclidean);
        }

        protected virtual bool AreClustersWithinTolerance(  UMCLight clusterX, 
                                                            UMCLight clusterY,
                                                            double massTolerance  ,
                                                            double netTolerance   ,
                                                            double driftTolerance )
        {

            // Calculate differences
            double massDiff = Math.Abs(Feature.ComputeMassPPMDifference(clusterX.MassMonoisotopicAligned, clusterY.MassMonoisotopicAligned));
            double netDiff = Math.Abs(clusterX.RetentionTime - clusterY.RetentionTime);
            double driftDiff = Math.Abs(clusterX.DriftTime - clusterY.DriftTime);

            // Return true only if all differences are within tolerance
            if (massDiff <= massTolerance && netDiff <= netTolerance && driftDiff <= driftTolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<PairwiseDistance<UMCLight>> CalculatePairwiseDistances(List<UMCLight> data, 
                                                            int     start, 
                                                            int     stop,
                                                            double  massTolerance,
                                                            double  netTolerance,
                                                            double  driftTolerance)
        {
            List<PairwiseDistance<UMCLight>> distances = new List<PairwiseDistance<UMCLight>>();
            for (int i = start; i < stop; i++)
            {
                UMCLight featureX   = data[i];
                double driftTimeX   = featureX.DriftTime;
                double netAlignedX  = featureX.RetentionTime;
                double massAlignedX = featureX.MassMonoisotopicAligned;
                int chargeStateX    = featureX.ChargeState;

                for (int j = i + 1; j <= stop; j++)
                {
                    // Don't calculate distance to self.                    
                   UMCLight featureY = data[j];

                    // Calculate the distances here (using a cube).  We dont care if we are going to re-compute
                    // these again later, because here we want to fall within the cube, the distance function used
                    // later is more related to determining a scalar value instead.
                    bool withinRange = AreClustersWithinTolerance(featureX, 
                                                                  featureY ,
                                                                  massTolerance  ,
                                                                  netTolerance   ,
                                                                  driftTolerance); //Parameters.RangeFunction(featureX, featureY);

                    // Make sure we fall within the distance range before computing...
                    if (withinRange)
                    {
                        // If IMS or equivalent only cluster similar charge states                        
                        
                        // Make sure it's the same charge state
                        if (chargeStateX == featureY.ChargeState)
                        {
                            // Calculate the pairwise distance
                            PairwiseDistance<UMCLight> pairwiseDistance = new PairwiseDistance<UMCLight>();
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
        public void MassPartitionTest(  string      databasePath,
                                        int         chargeState,                                 
                                        double      massTolerance,
                                        double      netTolerance,
                                        double      driftTolerance)
        {
            UmcAdoDAO database      = new UmcAdoDAO();
            database.DatabasePath   = databasePath;
                                                    
            Logger.PrintMessage(string.Format("Extracting Features"), true);
            List<UMCLight> data     = database.FindByCharge(chargeState);    

            // Make sure there is no null UMC data in the input list.
            int nullIndex = data.FindIndex(delegate(UMCLight x) { return x == null; });
            if (nullIndex > 0)
            {
                throw new NullReferenceException("The feature at index " + nullIndex.ToString() + " was null.  Cannot process this data.");
            }
            
            // The first thing we do is to sort the features based on mass since we know that has the least variability in the data across runs.
            data.Sort(m_massComparer);

            // This is the index of first feature of a given mass partition.
            int startUMCIndex   = 0;
            int totalFeatures   = data.Count;            
            int tenPercent      = Convert.ToInt32(totalFeatures * .1);            
            int singletons      = 0;            
            List<int> sizes     = new List<int>();
            List<double> times = new List<double>();

            for (int i = 0; i < totalFeatures - 1; i++)
            {
                // Here we compute the ppm mass difference between consecutive features (based on mass).
                // This will determine if we cluster a block of data or not.                
                UMCLight umcX   = data[i];
                UMCLight umcY   = data[i + 1];
                double ppm      = Math.Abs(Feature.ComputeMassPPMDifference(umcX.MassMonoisotopicAligned, umcY.MassMonoisotopicAligned));

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
                        DateTime starttime = DateTime.Now;
                        List<PairwiseDistance<UMCLight>> distances = CalculatePairwiseDistances( data,
                                                                                                startUMCIndex,
                                                                                                i,                                                                                                
                                                                                                massTolerance,
                                                                                                netTolerance,
                                                                                                driftTolerance);
                        DateTime endTime = DateTime.Now;
                        sizes.Add(i - startUMCIndex + 1);
                        times.Add(endTime.Subtract(starttime).TotalMilliseconds);
                    }
                    startUMCIndex = i + 1;
                }
            }

            int xxx = 0;
            xxx++;
            if (xxx > 1)
            {
                sizes.Add(0);
            }
            Console.WriteLine("{0}", singletons);
            Console.WriteLine();

            for(int i = 0; i < sizes.Count; i++)
            {
                Console.WriteLine("{0}\t{1}", sizes[i], times[i]);
            }
        }
    }
}
