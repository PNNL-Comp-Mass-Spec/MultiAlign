using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{

    /// <summary>
    /// Holds data about the mean and stdev of a Gaussian distribution.
    /// </summary>
    internal class DistributionData
    {
        public DistributionData(double mean, double stdev, double variance, int size)
        {
            Mean                = mean;
            StandardDeviation   = stdev;
            Variance            = variance;
            N                   = size;
        }
        /// <summary>
        /// Gets the sample mean.
        /// </summary>
        public double Mean
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the sample standard deviation
        /// </summary>
        public double StandardDeviation
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the sample variance
        /// </summary>
        public double Variance
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the sample size.
        /// </summary>
        public double N
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Splits a cluster by testing to see if it has two distributions of features.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class MedianSplitReprocessor<T, U>: IClusterReprocessor<T, U>
        where T : FeatureLight, Data.Features.IChildFeature<U>, new()
        where U : FeatureLight, Data.Features.IFeatureCluster<T>, new()
    {

        /// <summary>
        /// Calculates the distribution data over a data set between two vertices.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private DistributionData CalculuteDistributionData(List<double> data, int i, int j)
        {
            double mean     = 0;
            double sum      = 0;
            double stdev    = 0;
            var N           = Math.Abs(j - i);

            if (N == 0)
            {
                return null;
            }
            
            // Get the mean
            while (i < j && i < data.Count)
            {
                sum += data[i++];                
            }

            // Get the stdev
            mean = sum / N;
            sum  = 0;
            for (i = 0; i < j; i++)
            {
                var diff = (data[i] - mean);
                sum += (diff * diff);
            }

            // Calculate stdev and variance...
            stdev       = Math.Sqrt(sum / N);
            var var  = sum / (N - 1); 

            var distribution = new DistributionData(mean, stdev, var, N);
            return distribution;
        }
        
        /// <summary>
        /// Calculates the means of potentially doubled groups
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Positive data item 1, negative dataitem 2</returns>
        private Tuple<DistributionData, DistributionData, DistributionData> CalculateAllDistributions(List<double> data)
        {
            if (data == null)
                return null;

            // Get negative distribution
            var i = 0;
            while (i < data.Count && data[i] < 0)  i++;
            
            var N = data.Count;

            var positive = CalculuteDistributionData(data, 0, i);
            var negative = CalculuteDistributionData(data, i, N);
            var all      = CalculuteDistributionData(data, 0, N);

            var means 
                            = new Tuple<DistributionData, DistributionData, DistributionData>(positive, negative, all);
            return means;
        }


        private double CalculateZScore(DistributionData sampleA, DistributionData sampleB)
        {
            double z    = 0;

            var stdA = sampleA.StandardDeviation / Math.Sqrt(Convert.ToDouble(sampleA.N));
            stdA *= stdA;

            var stdB = sampleB.StandardDeviation / Math.Sqrt(Convert.ToDouble(sampleB.N));
            stdB *= stdB;
            
            z = (sampleA.Mean - sampleB.Mean) / Math.Sqrt(stdA + stdB);
            return z;
        }

        private void DisplayDistance(List<double> differences)
        {
            foreach (var diff in differences)
            {
                Console.WriteLine("{0}", diff);
            }
        }


        public List<U> ProcessClusters(List<U> clusters)
        {
            var newClusters = new List<U>();

            //Sort the clusters
            // Look for merged clusters that need to be split...
            foreach (var cluster in clusters)
            {
                var medianNet = cluster.Net;
                var medianMass = cluster.MassMonoisotopic;
                var medianDrift = cluster.DriftTime;

                var massDistributions = new Dictionary<T, double>();
                var netDistributions = new Dictionary<T, double>();
                var driftDistributions = new Dictionary<T, double>();

                var massDistances = new List<double>();
                var netDistances = new List<double>();
                var driftDistances = new List<double>();

                // Build distributions
                foreach (var feature in cluster.Features)
                {
                    var mass = FeatureLight.ComputeMassPPMDifference(feature.MassMonoisotopicAligned, medianMass);
                    var net = feature.Net - medianNet;
                    var drift = feature.DriftTime - medianDrift;

                    massDistributions.Add(feature, mass);
                    netDistributions.Add(feature, drift);
                    driftDistributions.Add(feature, net);

                    massDistances.Add(mass);
                    driftDistances.Add(drift);
                    netDistances.Add(net);
                }

                massDistances.Sort();
                netDistances.Sort();
                driftDistances.Sort();

                // Calculates the sample means for positive and negative sides of the median.
                var massDistribution    = CalculateAllDistributions(massDistances);
                var netDistribution     = CalculateAllDistributions(netDistances);
                var driftDistribution   = CalculateAllDistributions(driftDistances);

                var massZScore   = CalculateZScore(massDistribution.Item1, massDistribution.Item2);
                var netZScore    = CalculateZScore(netDistribution.Item1, netDistribution.Item2);
                var driftZScore  = CalculateZScore(driftDistribution.Item1, driftDistribution.Item2);

                // Now that we have data we can test the distributions to see if they are similar or not...
                Console.WriteLine("   Neg to Pos ");
                Console.WriteLine("Mass z-score \t{0}",  massZScore);
                Console.WriteLine("Net z-score  \t{0}",   netZScore);
                Console.WriteLine("Drift z-score\t{0}", driftZScore);
                Console.WriteLine();

                massZScore = CalculateZScore(massDistribution.Item1, massDistribution.Item3);
                netZScore = CalculateZScore(netDistribution.Item1, netDistribution.Item3);
                driftZScore = CalculateZScore(driftDistribution.Item1, driftDistribution.Item3);
                Console.WriteLine("   Negative ");
                Console.WriteLine("Mass z-score \t{0}", massZScore);
                Console.WriteLine("Net z-score  \t{0}", netZScore);
                Console.WriteLine("Drift z-score\t{0}", driftZScore);
                Console.WriteLine();
                
                Console.WriteLine("   Positive ");
                massZScore = CalculateZScore(massDistribution.Item2, massDistribution.Item3);
                netZScore = CalculateZScore(netDistribution.Item2, netDistribution.Item3);
                driftZScore = CalculateZScore(driftDistribution.Item2, driftDistribution.Item3);                                
                Console.WriteLine("Mass z-score \t{0}", massZScore);
                Console.WriteLine("Net z-score  \t{0}", netZScore);
                Console.WriteLine("Drift z-score\t{0}", driftZScore);

                //Console.WriteLine();
                //Console.WriteLine("Mass Difference");
                //DisplayDistance(massDistances);

                //Console.WriteLine();
                //Console.WriteLine("NET Difference");
                //DisplayDistance(netDistances);

                //Console.WriteLine();
                //Console.WriteLine("Drift Time Difference");
                //DisplayDistance(driftDistances);
            }

            return newClusters;
        }
    }
}
