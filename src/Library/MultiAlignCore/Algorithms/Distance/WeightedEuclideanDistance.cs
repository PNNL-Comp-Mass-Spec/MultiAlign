using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Distance
{
    public class WeightedEuclideanDistance<T> where T: FeatureLight, new ()
    {
        

        /// <summary>
        /// Constructor
        /// </summary>
        public WeightedEuclideanDistance()
        {
            // this accounts for the scale factors between NET and Mass.
            NetWeight       = .01;
            DriftWeight     = .6;
            MassWeight      = 10; // PPM
        }


        
        public WeightedEuclideanDistance(double mass, double net, double drift)
        {
            MassWeight  = mass;
            NetWeight   = net;
            DriftWeight = drift;
        }

        public double MassWeight
        {
            get;
             set;
        }
        public double NetWeight
        {
            get;
            set;
        }
        public double DriftWeight
        {
            get;
            set;
        }

        /// <summary>
        /// Calculates the Euclidean distance based on drift time, aligned mass, and aligned NET.
        /// </summary>
        /// <param name="x">Feature x.</param>
        /// <param name="y">Feature y.</param>
        /// <returns>Distance calculated as </returns>
        public double EuclideanDistance(T x, T y)
        {
            var meanMass         = (x.MassMonoisotopicAligned + y.MassMonoisotopicAligned)  / 2;
            var massDifference   = (x.MassMonoisotopicAligned - y.MassMonoisotopicAligned) * 1e6 / meanMass; 
            //  / MassWeight;

            var netDifference    = (x.Net - y.Net) / NetWeight;
            var driftDifference  = (x.DriftTime - y.DriftTime) / DriftWeight;
            var sum              = (massDifference * massDifference) +
                                      (netDifference * netDifference) +
                                      (driftDifference * driftDifference);

            return Math.Sqrt(sum);
        }
        /// <summary>
        /// Calculates the Euclidean distance based on drift time, aligned mass, and aligned NET.
        /// </summary>
        /// <param name="x">Feature x.</param>
        /// <param name="y">Feature y.</param>
        /// <returns>Distance calculated as </returns>
        public double EuclideanDistanceDalton(T x, T y)
        {
            var massDifference   = x.MassMonoisotopicAligned - y.MassMonoisotopicAligned;
            
            var netDifference    = x.Net - y.Net;
            var driftDifference  = x.DriftTime - y.DriftTime;
            var sum              = MassWeight *   (massDifference * massDifference) +
                                      NetWeight   *   (netDifference * netDifference) +
                                      DriftWeight *   (driftDifference * driftDifference);

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Calculates the weighted Euclidean distance based on drift time, aligned mass, and aligned NET.
        /// </summary>
        /// <param name="x">Feature x.</param>
        /// <param name="y">Feature y.</param>
        /// <returns>Distance calculated as </returns>
        public double EuclideanDistance(T x, T y, double massWeight, double netWeight, double driftWeight)
        {
            var massDifference = FeatureLight.ComputeMassPPMDifference(x.MassMonoisotopicAligned, y.MassMonoisotopicAligned);
            var netDifference = x.Net - y.Net;
            var driftDifference = x.DriftTime - y.DriftTime;
            var sum = (massDifference * massDifference) * massWeight +
                                     (netDifference * netDifference) * netDifference +
                                     (driftDifference * driftDifference) * driftWeight;

            return Math.Sqrt(sum);
        }
        public double EuclideanDistance(T x, FeatureLight y)
        {
            var massDifference   = x.MassMonoisotopicAligned - y.MassMonoisotopicAligned;
            var netDifference    = x.Net - y.Net;
            var driftDifference  = x.DriftTime - y.DriftTime;
            var sum              = MassWeight * (massDifference * massDifference) +
                                      NetWeight * (netDifference * netDifference) +
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
