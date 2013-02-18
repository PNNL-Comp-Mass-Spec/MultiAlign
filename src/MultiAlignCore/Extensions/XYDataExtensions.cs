using System;
using PNNLOmics.Data.MassTags;
using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data;
using MultiAlignCore.Data;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Extensions
{
    public static class XYDataExtensions
    {
        /// <summary>
        /// Converts a list of XYZ data points to XY.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<XYData> ToXYData(this List<XYZData> data)
        {
            List<XYData> results = new List<XYData>();
            foreach ( XYZData point in data)
            {
                results.Add(new XYData(point.X, point.Y));               
            }
            return results;
        }      
    }
}
