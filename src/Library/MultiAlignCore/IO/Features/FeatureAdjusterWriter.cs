using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MultiAlignCore.Extensions;

namespace MultiAlignCore.IO.Features
{
    public class FeatureAdjusterWriter
    {
        public void WriteFeatures(string path, List<UMCLight> features, List<UMCLight> adjustedFeatures)
        {
            Dictionary<int, UMCLight> map = new Dictionary<int, UMCLight>();
            adjustedFeatures.ForEach(x => map.Add(x.ID, x));

            using (TextWriter writer = File.CreateText(path))
            {
                writer.WriteLine("Feature ID, Feature Count, Scan, Scan Adj, Delta Scan,  NET, NET Adj, Delta NET, Charge, H-Negative, H-Positive, HasMSMS");
                foreach (UMCLight feature in features)
                {
                    bool contained = map.ContainsKey(feature.ID);
                    if (contained)
                    {
                        UMCLight adjusted = map[feature.ID];

                        double hPositive = 0;
                        double hNegative = 0;

                        foreach(int charge in adjusted.HNegative.Keys)
                        {
                            hPositive = adjusted.HPositive[charge];
                            hNegative = adjusted.HNegative[charge];

                            bool hasMSMS = feature.HasMsMs();

                            int value = 0;
                            if (hasMSMS)
                                value = 1;

                            writer.WriteLine("{9}, {10},{0},{1},{2},{3},{4},{5},{6},{7},{8},{11}",
                                feature.Scan, 
                                adjusted.Scan, 
                                feature.Scan - adjusted.Scan, 
                                feature.RetentionTime, 
                                adjusted.RetentionTime, 
                                feature.RetentionTime - adjusted.RetentionTime,
                                charge,
                                hNegative,    
                                hPositive,
                                feature.ID,
                                feature.MSFeatures.Count,
                                value
                                );
                        }
                    }
                }
            }
        }
    }
}
