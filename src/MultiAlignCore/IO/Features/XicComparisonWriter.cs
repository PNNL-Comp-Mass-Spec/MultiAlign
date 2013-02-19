using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using System.IO;
using MultiAlignCore.Extensions;
using PNNLOmics.Data;

namespace MultiAlignCore.IO.Features
{
    public class XicComparisonWriter : IXicWriter 
    {


        public void WriteXics(string path,
                              List<UMCLight> features)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                foreach (UMCLight feature in features)
                {
                    Dictionary<int, List<XYZData>> chargeMap = feature.CreateChargeSIC();

                    foreach (int charge in feature.ChargeStateChromatograms.Keys)
                    {
                        Chromatogram gram = feature.ChargeStateChromatograms[charge];
                        List<XYData> oldPoints = chargeMap[charge].ToXYData();
                        List<XYData> newPoints = gram.Points;

                        writer.WriteLine("id\t{0}\tcharge\t{1}", feature.ID, charge);
                        writer.WriteLine("mz\tscan\tintensity\told-intensity");

                        // Find the minimum and maximum scans
                        int minScan = Convert.ToInt32(oldPoints.Min(x => x.X));
                        int maxScan = Convert.ToInt32(oldPoints.Max(x => x.X));
                        minScan     = Math.Min(Convert.ToInt32(newPoints.Min(x => x.X)), minScan);
                        maxScan     = Math.Max(Convert.ToInt32(newPoints.Max(x => x.X)), maxScan);

                        // Map out the points to their respective scan numbers
                        Dictionary<int, long> oldMap = new Dictionary<int, long>();
                        Dictionary<int, long> newMap = new Dictionary<int, long>();

                        foreach (XYData x in oldPoints)
                        {
                            int scan = Convert.ToInt32(x.X);
                            if (oldMap.ContainsKey(scan))
                            {
                                oldMap[scan] = Math.Max(Convert.ToInt64(x.Y), oldMap[scan]);
                                continue;
                            }

                            oldMap.Add(scan, Convert.ToInt64(x.Y));
                        }
                        foreach (XYData x in newPoints)
                        {
                            int scan = Convert.ToInt32(x.X);
                            if (newMap.ContainsKey(scan))
                            {
                                int xx = 99;
                                xx++;
                                continue;
                            }

                            newMap.Add(scan, Convert.ToInt64(x.Y));
                        }

                        for (int i = minScan; i <= maxScan; i++)
                        {
                            string oldData = "";
                            string newData = "";

                            bool found = false;

                            if (oldMap.ContainsKey(i))
                            {
                                oldData = oldMap[i].ToString();
                                found   = true;
                            }
                            
                            if (newMap.ContainsKey(i))
                            {
                                newData = newMap[i].ToString();
                                found = true;
                            }

                            if (found)
                                writer.WriteLine("{0}\t{1}\t{2}\t{3}", gram.Mz, i, newData, oldData);
                        }
                    }
                }
            }
        }
    }
}
