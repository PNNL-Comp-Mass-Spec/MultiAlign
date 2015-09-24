using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignCore.Algorithms.Statistics
{
    public class Histogram
    {
        public static Dictionary<double, int> CreateHistogram(List<double> inputValues, double valStep)
        {
            var histData = new Dictionary<double, int>();
            var numPts = inputValues.Count;

            // Tried to pass an empty list to the histogram creator
            if (numPts == 0)
            {
                return histData;
            }

            var minVal = inputValues[0];
            var maxVal = inputValues[0];

            foreach (var val in inputValues)
            {
                if (val < minVal)
                {
                    minVal = val;
                }
                if (val > maxVal)
                {
                    maxVal = val;
                }
            }

            //Only one unique value in the input values
            if (Math.Abs(minVal - maxVal) < double.Epsilon)
            {
                histData.Add(minVal, 1);
                return histData;
            }

            var numBins = Math.Max((int)Math.Floor((maxVal - minVal) / valStep), 1);
            var frequencies = Enumerable.Range(0, numBins).Select(x => 0).ToList();

            for (var i = 0; i < numPts; i++)
            {
                var binIndex = (int)Math.Floor((inputValues[i] - minVal) / valStep);
                if (binIndex >= numBins)
                {
                    binIndex = numBins - 1;
                }
                frequencies[binIndex]++;
            }

            var binVal = minVal;
            for (var i = 0; i < numBins; i++)
            {
                histData.Add(binVal, frequencies[i]);
                binVal += valStep;
            }
            return histData;
        }
    }
}
