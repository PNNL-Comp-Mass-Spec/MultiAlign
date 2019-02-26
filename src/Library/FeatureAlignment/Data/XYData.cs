using System;
using System.Collections.Generic;

namespace FeatureAlignment.Data
{
    public class XYData
    {
        public XYData(double newX, double newY)
        {
            X = newX;
            Y = newY;
        }

        public double X
        {
            get;
            set;
        }
        public double Y
        {
            get;
            set;
        }
        public static List<XYData> Bin(List<XYData> data, double binSize)
        {
            var lowMass       = data[0].X;
            var highMass      = data[data.Count - 1].X;
            return Bin(data, lowMass, highMass, binSize);
        }

        public  static List<XYData> Bin(List<XYData> data, double lowMass, double highMass, double binSize)
        {
            var newData = new List<XYData>();
            var total            = Convert.ToInt32((highMass - lowMass)/binSize);

            for (var i = 0; i < total; i++)
            {
                var part = new XYData(lowMass + (Convert.ToDouble(i) * binSize), 0.0);
                newData.Add(part);
            }

            for (var i = 0; i < data.Count; i++)
            {
                var intensity = data[i].Y;
                var bin = Math.Min(total - 1, Convert.ToInt32((data[i].X - lowMass) / binSize));
                try
                {
                    newData[bin].Y += intensity;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return newData;
        }


        /// <summary>
        /// Convert XYData to arrays to interact with other functions more easily.
        /// </summary>
        /// <param name="xyList">List of XYData values to be converted.</param>
        /// <param name="xArray">Array to be populated with X values.</param>
        /// <param name="yArray">Array to be populated with Y values.</param>
        public static void XYDataListToArrays(List<XYData> xyList, double[] xArray, double[] yArray)
        {
            if (xArray.Length == xyList.Count && yArray.Length == xyList.Count)
            {
                for (var i = 0; i < xyList.Count; i++)
                {
                    xArray[i] = xyList[i].X;
                    yArray[i] = xyList[i].Y;
                }
            }
            else
            {
                throw new InvalidOperationException("X and Y arrays must be same length as XYData list in function XYDataListToArrays.");
            }
        }
    }
}