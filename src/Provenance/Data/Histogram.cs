using System;
using System.Collections.Generic;

using PNNLOmics.Data.Features;

namespace FOX.Data
{
    public class Histogram<T> where T: FeatureLight
    {
        /// <summary>
        /// Holds frequency of bins.
        /// </summary>
        private int[]    m_counts;
        /// <summary>
        /// Holds the bin values.
        /// </summary>
        private double[] m_bins;
        /// <summary>
        /// Holds the bin size.
        /// </summary>
        private double   m_binSize;        

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="binSize"></param>
        /// <param name="numberOfBins"></param>
        public Histogram(int numberOfBins)
        {
            if (numberOfBins < 1)
            {
                throw new Exception("The number of bins must be greater than zero.");
            }
            
            m_bins      = new double[numberOfBins];
            m_counts    = new int[numberOfBins];
        }
        /// <summary>
        /// Gets the array of counts for the data in each bin.
        /// </summary>
        public int[] Counts
        {
            get
            {
                return m_counts;
            }
        }
        /// <summary>
        /// Gets the array of bin values.
        /// </summary>
        public double[] Bins
        {
            get
            {
                return m_bins;
            }
        }
        /// <summary>
        /// Gets the size of each bin.
        /// </summary>
        public double BinSize
        {
            get
            {
                return m_binSize;
            }
        }
        /// <summary>
        /// Calculates a histogram of 
        /// </summary>
        /// <param name="map">Distance map</param>
        public void CalculateHistogram(DistanceMap<T> map)
        {
            
            double min = map.DistancePairs[0].Distance;
            double max = map.DistancePairs[map.DistancePairs.Count - 1].Distance;
            m_binSize  = (max - min) / m_bins.Length;

            if (m_binSize == 0)
            {
                throw new Exception("There is no data spanning this range to calculate a histogram over.");
            }

            // Initialize the bin array size.
            for (int i = 0; i < m_bins.Length; i++)
            {
                m_bins[i] = min + i * m_binSize;
            }

            // Bin the data.
            foreach (DistancePair<T> pair in map.DistancePairs)
            {
                double distance = pair.Distance;
                int binNumber   = Convert.ToInt32((distance - min) / Convert.ToDouble(m_binSize));
                binNumber       = Math.Max(0, Math.Min(m_bins.Length - 1, binNumber));

                m_counts[binNumber]++;
            }
        }
    }
}
