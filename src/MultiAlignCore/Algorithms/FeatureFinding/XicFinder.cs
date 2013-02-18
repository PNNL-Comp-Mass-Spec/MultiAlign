using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    public class XicFinder
    {

        /// <summary>
        /// Finds an XIC target based on the profile provided.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="targetScan"></param>
        /// <returns></returns>
        public List<PNNLOmics.Data.XYData> FindTarget(List<PNNLOmics.Data.XYData> points, int targetScan)
        {
            List<PNNLOmics.Data.XYData> target = new List<PNNLOmics.Data.XYData>();
            if (points.Count < 1)
                return target;

            //TODO:  Re-write this using LINQ
            double intensity = 0;
            bool foundPeak = false;
            int scan = 0;
            int i = 0;
            for (i = 0; i < points.Count && !foundPeak; i++)
            {
                scan = Convert.ToInt32(points[i].X);
                intensity = Convert.ToDouble(points[i].Y);

                if (scan == targetScan)
                {
                    foundPeak = true;
                }
            }

            if (foundPeak)
            {
                // Mark left and right
                if (intensity > 0)
                {
                    target = CreateXic(points, intensity, i);
                }
                else
                {
                    // Find left and right.
                    int j = i;
                    double tempIntensity = intensity;
                    while (intensity <= 0 && j > 0)
                    {
                        j--;
                        scan = Convert.ToInt32(points[j].X);
                        intensity = Convert.ToDouble(points[j].Y);
                    }
                    int leftScan = j;
                    int rightScan = i;
                    j = i;
                    while (intensity <= 0 && j < points.Count)
                    {
                        j++;
                        intensity = Convert.ToDouble(points[j].Y);
                    }
                    rightScan = j;


                    int deltaRight = Math.Abs(rightScan - i);
                    int deltaLeft = Math.Abs(leftScan - i);

                    // We are closer to the left peak.
                    if (deltaLeft < deltaRight && leftScan > 0)
                    {
                        target = CreateXic(points, 100, leftScan);
                    }
                    else
                    {
                        target = CreateXic(points, 100, rightScan);
                    }
                }
            }
            return target;
        }
        /// <summary>
        /// Creates an XIC from the points given, expecting the stop of the XIC to be zero.
        /// </summary>
        /// <param name="points">Points to create XIC from.</param>
        /// <param name="intensity">Intesity to create XIC from</param>
        /// <param name="targetScan"></param>
        /// <returns></returns>
        private List<PNNLOmics.Data.XYData> CreateXic(List<PNNLOmics.Data.XYData> points, double intensity, int targetScan)
        {
            List<PNNLOmics.Data.XYData> target = new List<PNNLOmics.Data.XYData>();

            int scan = targetScan;
            int j = targetScan;
            double tempIntensity = intensity;
            while (tempIntensity > 0 && j > 0)
            {
                j--;
                scan = Convert.ToInt32(points[j].X);
                tempIntensity = Convert.ToDouble(points[j].Y);
            }
            int leftScan = j;
            int rightScan = targetScan;
            j = targetScan;
            tempIntensity = intensity;
            while (tempIntensity > 0 && j < points.Count)
            {
                tempIntensity = Convert.ToDouble(points[j].Y);
                j++;
            }
            rightScan = j;

            for (targetScan = leftScan + 1; targetScan < rightScan; targetScan++)
            {
                target.Add(new PNNLOmics.Data.XYData(points[targetScan].X, points[targetScan].Y));

            }
            return target;
        }  
    }
}
