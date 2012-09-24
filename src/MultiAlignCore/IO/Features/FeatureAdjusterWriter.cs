using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using System.IO;

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
                writer.WriteLine("Scan, Scan Adj, Delta Scan,  NET, NET Adj, Delta NET");
                foreach (UMCLight feature in features)
                {
                    bool contained = map.ContainsKey(feature.ID);
                    if (contained)
                    {
                        UMCLight adjusted = map[feature.ID];
                        writer.WriteLine("{0},{1},{2},{3},{4},{5}", feature.Scan, adjusted.Scan, feature.Scan - adjusted.Scan, feature.RetentionTime, adjusted.RetentionTime, feature.RetentionTime - adjusted.RetentionTime);
                    }
                }
            }
        }
    }
}
