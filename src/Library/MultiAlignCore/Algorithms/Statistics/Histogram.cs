using System;
using System.Collections.Generic;

namespace MultiAlignCore.Algorithms.Statistics
{
    public class Histogram
    {
        public static void CreateHistogram(List<double> inputValues, ref List<double> bins,
                                            ref List<int> frequency, double valStep)
        {
            bins.Clear();
            frequency.Clear();
            var numPts = inputValues.Count;

            // Tried to pass an empty list to the histogram creator
            if (numPts == 0)
            {
                return;
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
            if (System.Math.Abs(minVal - maxVal) < double.Epsilon)
            {
                bins.Add(minVal);
                frequency.Add(1);
                return;
            }

            var numBins = Math.Max((int)Math.Floor((maxVal - minVal) / valStep), 1);

            var binVal = minVal;
            for (var i = 0; i < numBins; i++)
            {
                bins.Add(binVal);
                frequency.Add(0);
                binVal += valStep;
            }

            for (var i = 0; i < numPts; i++)
            {
                var binIndex = (int)Math.Floor((inputValues[i] - minVal) / valStep);
                if (binIndex >= numBins)
                {
                    binIndex = numBins - 1;
                }
                frequency[binIndex]++;
            }
        }
    }
}
