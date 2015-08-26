using System;
using System.Collections.Generic;
using MultiAlignCore.Data;

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
            var results = new List<XYData>();
            foreach (var point in data)
            {
                results.Add(new XYData(point.X, point.Y));
            }
            return results;
        }
        
        /// <summary>
        /// Converts an XY Data List into a dictionary mapping the scan to the intensity. (X, Y)
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static Dictionary<int, long> CreateScanMap(this List<XYData> profile)
        {
            var profileScanMap = new Dictionary<int, long>();

            // Converts the point structure into a map.
            foreach (var point in profile)
            {
                var scan        = Convert.ToInt32(point.X);
                var intensity  = Convert.ToInt64(point.Y);

                // Takes the max intensity for duplicate scans...
                if (profileScanMap.ContainsKey(scan))
                {
                    profileScanMap[scan] = Math.Max(intensity, profileScanMap[scan]);
                    continue;
                }
                profileScanMap.Add(scan, intensity);
            }

            return profileScanMap;
        }
    }
}