using System;
using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Regression
{
    /// <summary>
    /// Class to hold onto the uniform distribution data for expectation maximization
    /// </summary>
    public class NormalUniformEm
    {
        private double m_mean;
        private double m_var;
        private double m_normFraction;
        private int m_numIterations;
        private List<double> m_unifProb;

        private const double MinVar = 0.0001;

        /// <summary>
        /// Public method to get the standard dev; square root of the variance
        /// </summary>
        public double StandDev
        {
            get { return Math.Sqrt(m_var); }
        }

        /// <summary>
        /// Property for the mean of the Norm Unif
        /// </summary>
        public double Mean
        {
            get { return m_mean; }
            set { m_mean = value; }
        }

        /// <summary>
        /// Property for getting the normFraction
        /// </summary>
        public double NormProb
        {
            get { return m_normFraction; }
            set { m_normFraction = value; }
        }

        /// <summary>
        /// Constructor which initializes everything to a set value and
        /// allocates data space for the probabilities
        /// </summary>
        public NormalUniformEm()
        {
            m_mean = 0.0;
            m_var = 10.0;
            m_normFraction = 0.5;
            m_numIterations = 16;
            m_unifProb = new List<double>();
        }

        /// <summary>
        /// Method to reset the object to initial values
        /// </summary>
        public void Reset()
        {
            m_mean          = 0.0;
            m_var           = 10.0;
            m_normFraction  = 0.5;
            m_numIterations = 16;
            m_unifProb      = new List<double>();
        }

        /// <summary>
        /// Takes the values passed in and calculates the probability and distribution
        /// values for them and stores it.
        /// </summary>
        /// <param name="listVals"></param>
        public void CalculateDistributions(List<double> listVals)
        {
            Reset();
            if (listVals.Count == 0)
            {
                m_mean = 0;
                m_var = 0.1;
                m_normFraction = 0;
                return;
            }

            var minVal = double.MaxValue;
            var maxVal = double.MinValue;
            foreach (var val in listVals)
            {
                minVal = Math.Min(val, minVal);
                maxVal = Math.Max(val, maxVal);
            }
            if (Math.Abs(minVal - maxVal) < double.Epsilon)
            {
                m_mean = maxVal;
                m_var = 0.1;
                m_normFraction = 0;
                return;
            }
            var u = 1 / (maxVal - minVal);

            var numPts = listVals.Count;

            m_unifProb.Clear();
            m_unifProb.Capacity = numPts;

            for (var iteration = 0; iteration < m_numIterations; iteration++)
            {
                double meanNext = 0;
                double varNext = 0;
                double normFractionNext = 0;
                for (var pointNum = 0; pointNum < numPts; pointNum++)
                {
                    var val = listVals[pointNum];
                    var diff = val - m_mean;
                    var normProb = Math.Exp(-(0.5 * diff * diff) / m_var) / (Math.Sqrt(2 * Math.PI) * Math.Sqrt(m_var));
                    var postNormProb = (normProb * m_normFraction) / (normProb * m_normFraction + (1 - m_normFraction) * u);
                    m_unifProb.Add(postNormProb);

                    normFractionNext += postNormProb;
                    meanNext += postNormProb * val;
                    varNext += postNormProb * (val - m_mean) * (val - m_mean);
                }
                m_normFraction = normFractionNext / numPts;
                m_mean = meanNext / normFractionNext;
                m_var = varNext / normFractionNext;
                if (m_var < MinVar)
                {
                    break;
                }
            }
        }
    }
}
