using System;
using System.Collections.Generic;
using System.Linq;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Distance
{
    public class WeightedEuclideanDistance<T> where T: FeatureLight, new ()
    {
        
        /// <summary>
        /// Constructor, using default weights
        /// </summary>
        public WeightedEuclideanDistance()
        {
            // this accounts for the scale factors between NET and Mass.
            NetWeight       = 0.01;
            DriftWeight     = 0.6;
            MassWeight      = 10; // PPM
        }

        /// <summary>
        /// Constructor, using custom weights
        /// </summary>
        public WeightedEuclideanDistance(double mass, double net, double drift)
        {
            MassWeight  = mass;
            NetWeight   = net;
            DriftWeight = drift;
        }

        public double MassWeight
        {
            get;
        }

        public double NetWeight
        {
            get;
        }

        public double DriftWeight
        {
            get;
        }

        /// <summary>
        /// Calculates the Euclidean distance based on drift time, delta aligned mass (in ppm), and aligned NET.
        /// </summary>
        /// <param name="x">Feature x.</param>
        /// <param name="y">Feature y.</param>
        /// <returns>Distance</returns>
        /// <remarks>Uses NetWeight and DriftWeight but not MassWeight</remarks>
        public double EuclideanDistance(T x, T y)
        {
            var meanMass         = (x.MassMonoisotopicAligned + y.MassMonoisotopicAligned)  / 2;
            var massDifference   = (x.MassMonoisotopicAligned - y.MassMonoisotopicAligned) * 1e6 / meanMass;

            var netDifference    = (x.Net - y.Net) / NetWeight;
            var driftDifference  = (x.DriftTime - y.DriftTime) / DriftWeight;
            var sum              = (massDifference  * massDifference) +
                                   (netDifference   * netDifference) +
                                   (driftDifference * driftDifference);

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Calculates the Euclidean distance based on drift time, delta aligned mass (in Da), and aligned NET.
        /// </summary>
        /// <param name="x">Feature x.</param>
        /// <param name="y">Feature y.</param>
        /// <returns>Distance</returns>
        /// <remarks>Uses MassWeight, NetWeight, and DriftWeight</remarks>
        public double EuclideanDistanceDalton(T x, T y)
        {
            var massDifference   = x.MassMonoisotopicAligned - y.MassMonoisotopicAligned;

            var netDifference    = x.Net - y.Net;
            var driftDifference  = x.DriftTime - y.DriftTime;
            var sum              = MassWeight  * (massDifference * massDifference) +
                                   NetWeight   * (netDifference * netDifference) +
                                   DriftWeight * (driftDifference * driftDifference);

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Calculates the weighted Euclidean distance based on drift time, delta aligned mass (in ppm), and aligned NET.
        /// </summary>
        /// <param name="x">Feature x.</param>
        /// <param name="y">Feature y.</param>
        /// <param name="massWeight">mass weight.</param>
        /// <param name="netWeight">NET weight.</param>
        /// <param name="driftWeight">drift time weight.</param>
        /// <returns>Distance</returns>
        /// <remarks>Uses the weight values based to this function</remarks>
        public double EuclideanDistance(T x, T y, double massWeight, double netWeight, double driftWeight)
        {
            var massDifference = FeatureLight.ComputeMassPPMDifference(x.MassMonoisotopicAligned, y.MassMonoisotopicAligned);
            var netDifference = x.Net - y.Net;
            var driftDifference = x.DriftTime - y.DriftTime;
            var sum = (massDifference  * massDifference) * massWeight +
                      (netDifference   * netDifference) * netWeight +
                      (driftDifference * driftDifference) * driftWeight;

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Calculates the weighted Euclidean distance based on drift time, aligned mass, and aligned NET.
        /// </summary>
        /// <param name="x">Feature x.</param>
        /// <param name="y">Feature y.</param>
        /// <returns>Distance</returns>
        /// <remarks>Uses the weights defined by properties MassWeight, NetWeight, and DriftWeight</remarks>
        public double EuclideanDistance(T x, FeatureLight y)
        {
            var massDifference   = x.MassMonoisotopicAligned - y.MassMonoisotopicAligned;
            var netDifference    = x.Net - y.Net;
            var driftDifference  = x.DriftTime - y.DriftTime;
            var sum              = MassWeight  * (massDifference * massDifference) +
                                   NetWeight   * (netDifference * netDifference) +
                                   DriftWeight * (driftDifference * driftDifference);

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Calculates the Euclidean distance for a list of differences.
        /// </summary>
        /// <param name="differences"></param>
        /// <returns></returns>
        public static double Distance(List<double> differences)
        {
            var squares = new List<double>();
            differences.ForEach(x => squares.Add(x * x));
            return Math.Sqrt(squares.Sum());
        }
    }
}
